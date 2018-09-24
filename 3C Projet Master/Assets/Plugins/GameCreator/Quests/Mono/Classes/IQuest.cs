#define VERBOSE
namespace GameCreator.Quests
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
	using GameCreator.Core.Hooks;
    using GameCreator.Localization;

    public abstract class IQuest : ScriptableObject
    {
        public enum Status
        {
            Inactive,
            Active,
            Complete,
            Failed,
            Abandoned
        }

		public enum ProgressType
        {
            Single,
            Incremental
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        [LocStringNoPostProcess] public LocString title = new LocString();
		[LocStringBigTextNoPostProcess] public LocString description = new LocString();
		public Sprite sprite;

		public string uniqueID = "";
		public string internalName = "";
        public Status status = Status.Inactive;

		public List<IQuest> children = new List<IQuest>();
		public IQuest parent;

		public QuestReaction reactions;

		public ProgressType type = ProgressType.Single;
        [Range(0.0f, 1.0f)] public float progress = 0.0f;

		// PUBLIC METHODS: ------------------------------------------------------------------------      

		public bool ChangeStatus(Status status)
		{
			switch (status)
			{
				case Status.Active : return this.Activate();
				case Status.Complete : return this.Complete();
				case Status.Failed : return this.Fail();
				case Status.Abandoned : return this.Abandon();
				case Status.Inactive : return this.Deactivate();
			}

			return false;
		}

		public bool Activate()
        {
            if (!this.CheckConditions())
            {
                LOG("Conditions not met");
                return false;
            }

            if (this.status != Status.Inactive)
            {
                LOG("Current status is not inactive");
                return false;
            }

            this.status = Status.Active;
			this.progress = 0.0f;

            QuestsManager.Instance.questEvents.OnActivate(this.uniqueID);
            return true;
        }      

		public bool Deactivate()
        {
            if (this.status != Status.Active)
            {
                LOG("Current status is not active");
                return false;
            }

            this.status = Status.Inactive;
			this.progress = 0.0f;

            QuestsManager.Instance.questEvents.OnDeactivate(this.uniqueID);
            return true;
        }

		public void AddProgress(float amount)
        {
            if (this.status != Status.Active)
            {
                LOG("Can't add progress to a non-active task");
                return;
            }

            if (this.type != ProgressType.Incremental)
            {
                LOG("Can't add progress to a non-incremental task");
                return;
            }

            this.progress += amount;
            this.progress = Mathf.Clamp01(this.progress);

            QuestsManager.Instance.questEvents.OnProgress(this.uniqueID);
            if (this.progress >= 1.0f) this.Complete();
        }

        public void SetProgress(float amount)
        {
            if (this.status != Status.Active)
            {
                LOG("Can't set progress to a non-active task");
                return;
            }

            if (this.type != ProgressType.Incremental)
            {
                LOG("Can't set progress to a non-incremental task");
                return;
            }

			this.progress = amount;
			this.progress = Mathf.Clamp01(this.progress);

            QuestsManager.Instance.questEvents.OnProgress(this.uniqueID);
            if (this.progress >= 1.0f) this.Complete();
        }

        public bool Complete()
        {
            if (this.status != Status.Active)
            {
                LOG("Current status is not active");
                return false;
            }

            this.status = Status.Complete;
			this.progress = 1.0f;

            QuestsManager.Instance.questEvents.OnComplete(this.uniqueID);
            this.ExecuteActionsList(QuestReaction.ActionsType.OnComplete);
            return true;
        }

		public bool Fail()
        {
            if (this.status != Status.Active)
            {
                LOG("Current status is not active");
                return false;
            }

            this.status = Status.Failed;

            QuestsManager.Instance.questEvents.OnFail(this.uniqueID);
            this.ExecuteActionsList(QuestReaction.ActionsType.OnFail);
            return true;
        }

		public bool Abandon()
        {
            if (this.status != Status.Active)
            {
                LOG("Current status is not active");
                return false;
            }

            this.status = Status.Abandoned;

            QuestsManager.Instance.questEvents.OnAbandon(this.uniqueID);
            this.ExecuteActionsList(QuestReaction.ActionsType.OnFail);
            return true;
        }

		public void Restart()
		{
			this.status = Status.Active;
			this.progress = 0.0f;

			IQuest root = this.GetRootQuest();
			if (root != null && root.uniqueID != this.uniqueID)
            {
				root.Restart();
            }

            QuestsManager.Instance.questEvents.OnRestart(this.uniqueID);
		}

		public bool CheckConditions()
		{
			if (this.reactions == null) return true;
			if (this.reactions.conditions == null) return true;
			return this.reactions.conditions.Check();
		}

		public override string ToString()
        {
            return this.internalName;
        }

		public virtual bool IsQuestRoot()
		{
			return false;
		}

		// PROTECTED METHODS: ---------------------------------------------------------------------

		protected IQuest GetRootQuest()
		{
			IQuest quest = QuestsManager.Instance.GetQuest(this.uniqueID);         
			while (quest != null)
			{
				if (quest.IsQuestRoot()) break;
				quest = QuestsManager.Instance.GetQuest(quest.parent.uniqueID);
			}

			return quest;
		}

		protected void LOG(string message)
        {
            # if VERBOSE && UNITY_EDITOR
            Debug.LogWarningFormat("<b>Quest</b> [{0}]: {1}", this.internalName, message);
            #endif
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------      

        private void ExecuteActionsList(QuestReaction.ActionsType types)
        {
            if (this.reactions == null) return;

            GameObject instance = Instantiate(reactions.gameObject, Vector3.zero, Quaternion.identity);
            QuestReaction instanceReaction = instance.GetComponent<QuestReaction>();

			GameObject invoker = HookPlayer.Instance != null ? HookPlayer.Instance.gameObject : null;
            IActionsList actionsList = null;

            switch (types)
            {
                case QuestReaction.ActionsType.OnComplete :
                    actionsList = instanceReaction.onComplete;
                    break;

                case QuestReaction.ActionsType.OnFail:
                    actionsList = instanceReaction.onFail;
                    break;
            }

            if (actionsList == null) return;
            actionsList.Execute(invoker, () =>
            {
                if (instance != null) Destroy(instance);
            });
        }
	}
}