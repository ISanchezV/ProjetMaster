namespace GameCreator.Quests
{
	using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
	using UnityEngine.Events;

    public class QuestEvents
    {
        public class QuestEventStatus : UnityEvent<IQuest.Status> { }
        public class QuestEventString : UnityEvent<string> { }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<string, QuestEventStatus> onQuestChange;

        private QuestEventString onChange = new QuestEventString();
        private QuestEventString onDeactivate = new QuestEventString();
        private QuestEventString onActivate = new QuestEventString();
        private QuestEventString onComplete = new QuestEventString();
        private QuestEventString onProgress = new QuestEventString();
        private QuestEventString onFail = new QuestEventString();
        private QuestEventString onAbandon = new QuestEventString();
        private QuestEventString onRestart = new QuestEventString();
        private QuestEventString onTrack = new QuestEventString();

		// INITIALIZERS: --------------------------------------------------------------------------

		public QuestEvents()
		{
            this.onQuestChange = new Dictionary<string, QuestEventStatus>();
		}

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void OnChange(string questID)
        {
            if (this.onChange != null) this.onChange.Invoke(questID);
        }

        public void OnDeactivate(string questID)
        {
            this.OnChange(questID);
            if (this.onDeactivate != null) this.onDeactivate.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Inactive);
        }

        public void OnActivate(string questID)
        {
            this.OnChange(questID);
            if (this.onActivate != null) this.onActivate.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Active);
        }

        public void OnComplete(string questID)
        {
            this.OnChange(questID);
            if (this.onComplete != null) this.onComplete.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Complete);
        }

        public void OnProgress(string questID)
        {
            this.OnChange(questID);
            if (this.onProgress != null) this.onProgress.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Active);
        }

        public void OnFail(string questID)
        {
            this.OnChange(questID);
            if (this.onFail != null) this.onFail.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Failed);
        }

        public void OnAbandon(string questID)
        {
            this.OnChange(questID);
            if (this.onAbandon != null) this.onAbandon.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Abandoned);
        }

        public void OnRestart(string questID)
        {
            this.OnChange(questID);
            if (this.onRestart != null) this.onRestart.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Active);
        }

        public void OnTrack(string questID)
        {
            this.OnChange(questID);
            if (this.onTrack != null) this.onTrack.Invoke(questID);
            if (this.onQuestChange.ContainsKey(questID)) this.onQuestChange[questID].Invoke(IQuest.Status.Active);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public void SetOnChange(UnityAction<string> action)
        {
            this.onChange.AddListener(action);
        }

        public void SetOnDeactivate(UnityAction<string> action)
        {
            this.onDeactivate.AddListener(action);
        }

        public void SetOnActivate(UnityAction<string> action)
        {
            this.onActivate.AddListener(action);
        }

        public void SetOnComplete(UnityAction<string> action)
        {
            this.onComplete.AddListener(action);
        }

        public void SetOnProgress(UnityAction<string> action)
        {
            this.onProgress.AddListener(action);
        }

        public void SetOnFail(UnityAction<string> action)
        {
            this.onFail.AddListener(action);
        }

        public void SetOnAbandon(UnityAction<string> action)
        {
            this.onAbandon.AddListener(action);
        }

        public void SetOnRestart(UnityAction<string> action)
        {
            this.onRestart.AddListener(action);
        }

        public void SetOnTrack(UnityAction<string> action)
        {
            this.onTrack.AddListener(action);
        }

        public void SetOnQuestChange(string questID, UnityAction<IQuest.Status> action)
        {
            if (!this.onQuestChange.ContainsKey(questID))
            {
                QuestEventStatus questEvent = new QuestEventStatus();
                this.onQuestChange.Add(questID, questEvent);
            }

            this.onQuestChange[questID].AddListener(action);
        }

        // REMOVERS: ------------------------------------------------------------------------------

        public void RemoveOnChange(UnityAction<string> action)
        {
            this.onChange.RemoveListener(action);
        }

        public void RemoveOnDeactivate(UnityAction<string> action)
        {
            this.onDeactivate.RemoveListener(action);
        }

        public void RemoveOnActivate(UnityAction<string> action)
        {
            this.onActivate.RemoveListener(action);
        }

        public void RemoveOnComplete(UnityAction<string> action)
        {
            this.onComplete.RemoveListener(action);
        }

        public void RemoveOnProgress(UnityAction<string> action)
        {
            this.onProgress.RemoveListener(action);
        }

        public void RemoveOnFail(UnityAction<string> action)
        {
            this.onFail.RemoveListener(action);
        }

        public void RemoveOnAbandon(UnityAction<string> action)
        {
            this.onAbandon.RemoveListener(action);
        }

        public void RemoveOnRestart(UnityAction<string> action)
        {
            this.onRestart.RemoveListener(action);
        }

        public void RemoveOnTrack(UnityAction<string> action)
        {
            this.onTrack.RemoveListener(action);
        }

        public void RemoveOnQuestChange(string questID, UnityAction<IQuest.Status> action)
        {
            if (this.onQuestChange.ContainsKey(questID))
            {
                this.onQuestChange[questID].RemoveListener(action);
            }
        }
    }
}
