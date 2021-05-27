using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SteviesModAchievement.API.Tests
{
    public sealed class TestAchievement1 : ModAchievement
    {
        public override AchievementCategory Category => AchievementCategory.Challenger;

        public TestAchievement1(Mod mod) : base("TEST_ACHIEVEMENT_ONE")
        {
            Mod = mod;
            AddCondition(ItemPickupCondition.Create(ItemID.CopperShortsword));

            Description = Language.GetText("Mods.SteviesModAchievement.TestAchievement1.Description");
            FriendlyName = Language.GetText("Mods.SteviesModAchievement.TestAchievement1.FriendlyName");
        }
    }
}