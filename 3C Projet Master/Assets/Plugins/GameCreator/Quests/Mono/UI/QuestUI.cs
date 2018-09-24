namespace GameCreator.Quests.UI
{
    using System.Text;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

    [AddComponentMenu("Game Creator/Quest/Quests UI")]
	public class QuestUI : MonoBehaviour
	{
        public enum DescriptionType
        {
            CurrentQuest,
            RootQuest,
            CompleteAndActive
        }

        private const int COMPLETE_OR_ACTIVE = (
            (1 << (int)IQuest.Status.Complete) |
            (1 << (int)IQuest.Status.Active)
        );

		// PROPERTIES: ----------------------------------------------------------------------------

		private IQuest quest;

		public Text title;
        public Image image;
        public DescriptionType descriptionType = DescriptionType.CurrentQuest;
        public Text description;

		public Toggle toggleTrack;
        public Button buttonActivate;
        public Button buttonAbandon;

        public GameObject statusTracking;
        public GameObject statusInactive;
        public GameObject statusActive;
        public GameObject statusComplete;
        public GameObject statusFailed;
        public GameObject statusAbandoned;

        public QuestsGroup subQuestsGroup;

        // INITIALIZERS: --------------------------------------------------------------------------      

        private void OnDestroy()
		{
            if (this.quest == null) return;

            if (QuestsManager.Instance == null) return;
            QuestsManager.Instance.questEvents.RemoveOnChange(this.OnQuestChange);
		}

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Setup(IQuest quest)
		{
            gameObject.SetActive(true);

			this.quest = quest;
			this.UpdateQuest();

            QuestsManager.Instance.questEvents.SetOnChange(this.OnQuestChange);
		}

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnQuestChange(string questID)
        {
            this.UpdateQuest();
        }

		private void UpdateQuest()
		{
            Quest rootQuest = QuestsManager.Instance.GetRootQuest(this.quest);

            this.SetupTitle();
            this.SetupDescription();

            if (this.image != null)
            {
				this.image.gameObject.SetActive(this.quest.sprite != null);
				if (this.quest.sprite != null) this.image.sprite = this.quest.sprite;
            }

            if (this.toggleTrack != null) this.SetupToggleTrack(rootQuest);
            if (this.buttonActivate != null) this.SetupButtonActivate();
            if (this.buttonAbandon != null) this.SetupButtonAbandon();

            if (this.statusTracking != null) this.statusTracking.SetActive(rootQuest.IsTracking());
            if (this.statusInactive != null) this.statusInactive.SetActive(this.quest.status == IQuest.Status.Inactive);
            if (this.statusActive != null) this.statusActive.SetActive(this.quest.status == IQuest.Status.Active);
            if (this.statusComplete != null) this.statusComplete.SetActive(this.quest.status == IQuest.Status.Complete);
            if (this.statusFailed != null) this.statusFailed.SetActive(this.quest.status == IQuest.Status.Failed);
            if (this.statusAbandoned != null) this.statusAbandoned.SetActive(this.quest.status == IQuest.Status.Abandoned);

            if (this.subQuestsGroup != null)
            {
                IQuest[] tasks = QuestsManager
                    .Instance
                    .GetSubTasks(this.quest, this.subQuestsGroup.statusFilter)
                    .ToArray();

                this.subQuestsGroup.Setup(tasks);
            }

            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
		}

        private void SetupTitle()
        {
            if (this.title != null)
            {
                string textTitle = this.quest.title.GetText();
                if (string.IsNullOrEmpty(textTitle)) textTitle = this.quest.internalName;

                this.title.text = textTitle;
            }
        }

        private void SetupDescription()
        {
            if (this.description != null)
            {
                Quest rootQuest = QuestsManager.Instance.GetRootQuest(this.quest);
                switch (this.descriptionType)
                {
                    case DescriptionType.CurrentQuest:
                        this.description.text = this.quest.description.GetText();
                        break;

                    case DescriptionType.RootQuest:
                        this.description.text = rootQuest.description.GetText();
                        break;

                    case DescriptionType.CompleteAndActive:
                        StringBuilder builder = new StringBuilder(rootQuest.description.GetText());
                        IQuest[] quests = QuestsManager
                            .Instance
                            .GetSubTasks(rootQuest, COMPLETE_OR_ACTIVE)
                            .ToArray();

                        for (int i = 0; i < quests.Length; ++i)
                        {
                            string text = quests[i].description.GetText();
                            if (string.IsNullOrEmpty(text)) continue;
                            builder.Append("\n\n").Append(text);
                        }

                        this.description.text = builder.ToString();
                        break;
                }
            }
        }

        private void SetupToggleTrack(Quest rootQuest)
        {
            this.toggleTrack.onValueChanged.RemoveAllListeners();
            this.toggleTrack.onValueChanged.AddListener((bool status) =>
            {
                rootQuest.SetIsTracking(status);
            });

            this.toggleTrack.gameObject.SetActive(this.quest.status == IQuest.Status.Active);
            this.toggleTrack.isOn = rootQuest.IsTracking();
        }

        private void SetupButtonActivate()
        {
            this.buttonActivate.onClick.RemoveAllListeners();
            this.buttonActivate.onClick.AddListener(() => { this.quest.Activate(); });
            this.buttonActivate.gameObject.SetActive(this.quest.status == IQuest.Status.Inactive);
        }

        private void SetupButtonAbandon()
        {
            this.buttonAbandon.onClick.RemoveAllListeners();
            this.buttonAbandon.onClick.AddListener(() => { this.quest.Abandon(); });
            this.buttonAbandon.gameObject.SetActive(this.quest.status == IQuest.Status.Active);
        }
	}
}