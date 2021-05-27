using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Terraria.Achievements;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SteviesModAchievement.API
{
    public abstract class ModAchievement : Achievement
    {
        public Mod Mod { get; internal set; }
        
        public new virtual LocalizedText FriendlyName
        {
            get => AchievementHelper.GetAchievementField<LocalizedText>("FriendlyName", this);
            set => AchievementHelper.SetAchievementField("FriendlyName", value, this);
        }

        public new virtual LocalizedText Description
        {
            get => AchievementHelper.GetAchievementField<LocalizedText>("Description", this);
            set => AchievementHelper.SetAchievementField("Description", value, this);
        }

        public new virtual AchievementCategory Category
        {
            get => AchievementHelper.GetAchievementField<AchievementCategory>("_category", this);
            set => AchievementHelper.SetAchievementField("_category", value, this);
        }

        public virtual IAchievementTracker Tracker
        {
            get => AchievementHelper.GetAchievementField<IAchievementTracker>("_tracker", this);
            set => AchievementHelper.SetAchievementField("_tracker", value, this);
        }

        public IReadOnlyDictionary<string, AchievementCondition> Conditions =>
            AchievementHelper.GetAchievementField<Dictionary<string, AchievementCondition>>("_conditions", this);

        public virtual int CompletedCount
        {
            get => AchievementHelper.GetAchievementField<int>("_completedCount", this);
            set => AchievementHelper.SetAchievementField("_completedCount", value, this);
        }

        public new virtual bool HasTracker => base.HasTracker;

        public new virtual bool IsCompleted => base.IsCompleted;

        internal ModAchievement(string name) : base(name)
        {
        }

        public new virtual IAchievementTracker GetTracker() => base.GetTracker();

        public new virtual void ClearProgress() => base.ClearProgress();

        public new virtual void Load(Dictionary<string, JObject> conditions) => base.Load(conditions);

        public new virtual void AddCondition(AchievementCondition condition) => base.AddCondition(condition);

        public virtual void OnConditionComplete(AchievementCondition condition) =>
            AchievementHelper.InvokeAchievementMethod("OnConditionComplete", new object[] {condition}, this);

        public virtual void UseTracker(IAchievementTracker tracker) =>
            AchievementHelper.InvokeAchievementMethod("UseTracker", new object[] {tracker}, this);

        public new virtual void UseTrackerFromCondition(string conditionName) =>
            base.UseTrackerFromCondition(conditionName);

        public new virtual void UseConditionsCompletedTracker() => base.UseConditionsCompletedTracker();

        public new virtual void ClearTracker() => base.ClearTracker();

        public virtual IAchievementTracker GetConditionTracker(string name) =>
            AchievementHelper.InvokeAchievementMethod<IAchievementTracker>("GetConditionTracker", new object[] {name},
                this);

        public new virtual void AddConditions(params AchievementCondition[] conditions) =>
            base.AddConditions(conditions);

        public new virtual AchievementCondition GetCondition(string conditionName) => base.GetCondition(conditionName);

        private new void SetCategory(AchievementCategory category) => Category = category;
    }
}