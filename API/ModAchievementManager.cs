using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.Achievements;
using Terraria.ModLoader;
using Terraria.Social;

namespace SteviesModAchievement.API
{
    public static class ModAchievementManager
    {
        public static JsonSerializer Serializer => new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public sealed class HallowedAchievement
        {
            public Dictionary<string, JObject> Conditions;
        }

        public static string SavePath => Main.SavePath + Path.DirectorySeparatorChar + "achievements.dat";

        public static Dictionary<string, Dictionary<string, Achievement>> Achievements =>
            new Dictionary<string, Dictionary<string, Achievement>>();

        public static object Lock => typeof(AchievementManager).GetField("_ioLock", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

        public static void LoadDetours()
        {
            On.Terraria.Achievements.AchievementManager.Save += (orig, self) => Save();
            On.Terraria.Achievements.AchievementManager.Load += (orig, self) => Load();
            On.Terraria.Achievements.AchievementManager.CreateAchievementsList +=
                (orig, self) => CreateAchievementsList();
            On.Terraria.Achievements.AchievementManager.GetAchievement += (orig, self, name) =>
            {
                foreach (KeyValuePair<string, Dictionary<string, Achievement>> achievementCollection in Achievements)
                    if (achievementCollection.Value.TryGetValue(name, out Achievement value))
                        return value;

                return null;
            };
            On.Terraria.Achievements.AchievementManager.GetCondition += (orig, self, name, conditionName) =>
            {
                foreach (KeyValuePair<string, Dictionary<string, Achievement>> achievementCollection in Achievements)
                    if (achievementCollection.Value.TryGetValue(name, out Achievement value))
                        return value.GetCondition(conditionName);

                return null;
            };
        }

        public static void Save()
        {
            lock (Lock)
            {
                if (SocialAPI.Achievements != null)
                    SocialAPI.Achievements.StoreStats();

                using (StreamWriter sWriter = new StreamWriter(SavePath))
                {
                    using (JsonWriter jWriter = new JsonTextWriter(sWriter))
                    {
                        Serializer.Serialize(jWriter, Achievements);
                    }
                }
            }
        }

        public static List<Achievement> CreateAchievementsList()
        {
            List<Achievement> achievements = new List<Achievement>();

            foreach (Dictionary<string, Achievement> achievementList in Achievements.Values)
                achievements.AddRange(achievementList.Values);

            return achievements;
        }

        public static void Load()
        {
            bool shouldSave = false;
            lock (Lock)
            {
                Dictionary<string, Dictionary<string, HallowedAchievement>> achievements;

                try
                {
                    achievements =
                        JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, HallowedAchievement>>>(
                            SavePath);
                }
                catch
                {
                    return;
                }

                if (achievements is null)
                    return;

                foreach (KeyValuePair<string, Dictionary<string, HallowedAchievement>> achievementCollection in
                    achievements)
                {
                    foreach (KeyValuePair<string, HallowedAchievement> achievementList in
                        achievementCollection.Value.Where(achievementList =>
                            Achievements.ContainsKey(achievementCollection.Key) &&
                            Achievements[achievementCollection.Key].ContainsKey(achievementList.Key)))
                    {
                        Achievements[achievementCollection.Key][achievementList.Key]
                            .Load(achievementList.Value.Conditions);
                    }
                }

                if (!Achievements.ContainsKey("Terraria"))
                    Achievements.Add("Terraria",
                        typeof(AchievementManager)
                            .GetField("_achievements", BindingFlags.Instance | BindingFlags.NonPublic)
                            ?.GetValue(Main.Achievements) as Dictionary<string, Achievement>);

                if (SocialAPI.Achievements != null)
                    foreach (KeyValuePair<string, Achievement> achievementList in Achievements.SelectMany(
                        achievementCollection => achievementCollection.Value.Where(achievementList =>
                            achievementList.Value.IsCompleted &&
                            !SocialAPI.Achievements.IsAchievementCompleted(achievementList.Key))))
                    {
                        shouldSave = true;
                        achievementList.Value.ClearProgress();
                    }
            }

            if (shouldSave)
                Save();
        }
    }
}