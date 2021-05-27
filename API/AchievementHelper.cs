using System.Reflection;
using Terraria.Achievements;

namespace SteviesModAchievement.API
{
    public static class AchievementHelper
    {
        public static int TotalAchievements => GetAchievementField<int>("_totalAchievements");

        public static T GetAchievementField<T>(string fieldName, Achievement instance = null) =>
            (T) typeof(Achievement).GetField(fieldName,
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(instance);

        public static void SetAchievementField<T>(string fieldName, T value, Achievement instance = null) =>
            typeof(Achievement).GetField(fieldName,
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                ?.SetValue(instance, value);

        public static void InvokeAchievementMethod(string methodName, object[] parameters,
            Achievement instance = null) =>
            typeof(Achievement)
                .GetMethod(methodName,
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(instance, parameters);

        public static T InvokeAchievementMethod<T>(string methodName, object[] parameters,
            Achievement instance = null) =>
            (T) typeof(Achievement)
                .GetMethod(methodName,
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(instance, parameters);
    }
}