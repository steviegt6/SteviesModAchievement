using Mono.Cecil.Cil;
using MonoMod.Cil;
using SteviesModAchievement.API;
using SteviesModAchievement.API.Tests;
using SteviesModAchievement.Utilities;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace SteviesModAchievement
{
    public class SteviesModAchievement : Mod
    {
        public override void Load()
        {
            //typeof(Main).GetField("_achievements", BindingFlags.NonPublic | BindingFlags.Instance)
            //    ?.SetValue(Main.instance, new ModAchievementManager());
            //AchievementInitializer.Load();

            Logger.Debug("Applying counter-Steam patches...");

            IL.Terraria.Achievements.AchievementCondition.Complete += RemoveSteamAppCheck;
            //IL.Terraria.Achievements.AchievementManager.Save_string_bool += RemoveSteamAppCheck;
            //IL.Terraria.Achievements.AchievementManager.Load += RemoveSteamAppCheck;
            //IL.Terraria.Achievements.AchievementManager.Load_string_bool += RemoveSteamAppCheck;
            IL.Terraria.Main.DrawMenu += RemoveSteamAppCheck;

            Logger.Debug("Applied counter-Steam patches.");

            Logger.Debug("Applying achievement patches...");

            ModAchievementManager.LoadDetours();

            //On.Terraria.Achievements.AchievementManager.Register += RegisterSafeguard;
            //On.Terraria.Achievements.AchievementManager.Save_string_bool += OverrideAchievementSaving;
            //On.Terraria.Achievements.AchievementManager.Load_string_bool += OverrideAchievementLoading;

            Logger.Debug("Applied achievement patches.");

            // Register AchievementTagHandler again since tML removes it
            ChatManager.Register<AchievementTagHandler>("a", "achievement");

            ModTranslation name = CreateTranslation("TestAchievement1.FriendlyName");
            name.SetDefault("Friendly Name");
            AddTranslation(name);

            ModTranslation description = CreateTranslation("TestAchievement1.Description");
            description.SetDefault("Description, obtain a Copper Short-sword.");
            AddTranslation(description);
        }

        public override void PostSetupContent()
        {
            Main.Achievements.Register(new TestAchievement1(this));

            Main.Achievements.Load();
        }

        private static void RemoveSteamAppCheck(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (!c.TryGotoNext(x => x.MatchCall("Terraria.ModLoader.Engine.Steam", "get_IsSteamApp")))
                c.GotoNext(x => x.MatchCallvirt("Terraria.ModLoader.Engine.Steam", "get_IsSteamApp"));

            c.Index++;
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldc_I4_0);
        }

        /*private static void RegisterSafeguard(On.Terraria.Achievements.AchievementManager.orig_Register orig,
            AchievementManager self, Achievement achievement)
        {
            if ((typeof(AchievementManager).GetField("_achievements", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.GetValue(self) as Dictionary<string, Achievement>)?.ContainsKey(achievement.Name) ?? false)
                return;

            orig(self, achievement);
        }*/

        /*private static void OverrideAchievementSaving(
            On.Terraria.Achievements.AchievementManager.orig_Save_string_bool orig, AchievementManager self,
            string path, bool cloud)
        {
            object ioLock = typeof(AchievementManager).GetField("_ioLock", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null) ?? new object();

            object achievements = typeof(AchievementManager)
                .GetField("_achievements", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(self);

            lock (ioLock)
            {
                if (SocialAPI.Achievements != null)
                    SocialAPI.Achievements.StoreStats();

                using (StreamWriter writer = new StreamWriter(path))
                {
                    using (JsonWriter jWriter = new JsonTextWriter(writer))
                    {
                        new JsonSerializer { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented }
                            .Serialize(jWriter, achievements);
                    }
                }
            }
        }

        private static void OverrideAchievementLoading(
            On.Terraria.Achievements.AchievementManager.orig_Load_string_bool orig, AchievementManager self,
            string path, bool cloud)
        {
            object ioLock = typeof(AchievementManager).GetField("_ioLock", BindingFlags.Static | BindingFlags.NonPublic)
                ?.GetValue(null) ?? new object();

            Dictionary<string, Achievement> achievements = typeof(AchievementManager)
                .GetField("_achievements", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(self) as Dictionary<string, Achievement> ?? new Dictionary<string, Achievement>();

            bool needsSaving = false;
            lock (ioLock)
            {
                try
                {
                    Dictionary<string, StoredAchievement> loadedAchievements =
                        JsonConvert.DeserializeObject<Dictionary<string, StoredAchievement>>(AchievementsPath);

                    if (loadedAchievements is null)
                        return;

                    foreach (KeyValuePair<string, StoredAchievement> achievement in loadedAchievements.Where(
                        achievement =>
                            achievements.ContainsKey(achievement.Key)))
                        achievements[achievement.Key].Load(achievement.Value.Conditions);

                    if (SocialAPI.Achievements != null)
                        foreach (KeyValuePair<string, Achievement> achievement in achievements.Where(achievement =>
                            achievement.Value.IsCompleted &&
                            !SocialAPI.Achievements.IsAchievementCompleted(achievement.Key)))
                        {
                            needsSaving = true;
                            achievement.Value.ClearProgress();
                        }
                }
                catch
                {
                    return;
                }
            }

            if (needsSaving)
                self.Save();
        }*/
    }
}