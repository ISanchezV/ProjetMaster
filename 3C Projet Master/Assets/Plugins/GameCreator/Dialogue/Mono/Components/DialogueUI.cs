namespace GameCreator.Dialogue
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [AddComponentMenu("")]
    public class DialogueUI : MonoBehaviour
    {
        private static DialogueUI Instance;

        private static int PREFAB_INSTANCE_ID = 0;
        private static readonly int ANIM_TIMER = Animator.StringToHash("Start");

        private const string ERR_NO_DIALOGUE_UI = "No Dialogue UI prefab specified";
        private const string ERR_NO_BUTTON = "No Button component found in Choice object";

        // PROPERTIES: ----------------------------------------------------------------------------

        [Header("Prefabs")]
        public GameObject prefabChoice;

        [Header("Wrappers")]
        public GameObject wrapMessage;
        public GameObject wrapChoices;
        public GameObject wrapTimer;

        [Header("Messages")]
        public Text textMessage;

        [Header("Choices")]
        public RectTransform choiceContainer;

        [Header("Timer")]
        public Animator animatorTimer;

        private string currentMessage = "";
        private bool isTypewritting = false;

        private bool typewritterEnabled = false;
        private float typewritterCharsPerSec = 1.0f;
        private float typewritterStartTime = 0.0f;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        private void Awake()
        {
            DialogueUI.Instance = this;
            DontDestroyOnLoad(gameObject);

            this.wrapMessage.SetActive(false);
            this.wrapChoices.SetActive(false);
            this.wrapTimer.SetActive(false);
        }

        // UPDATE METHODS: ------------------------------------------------------------------------

        private void Update()
        {
            this.UpdateTypewritterEffect();
        }

        protected void UpdateTypewritterEffect()
        {
            this.isTypewritting = false;

            if (!this.wrapMessage.activeInHierarchy) return;
            if (string.IsNullOrEmpty(this.currentMessage)) return;
            if (!this.typewritterEnabled)
            {
                this.textMessage.text = this.currentMessage;
                return;
            }

            float elapsedTime = Time.time - this.typewritterStartTime;
            int messageLength = Mathf.Min(
                Mathf.FloorToInt(elapsedTime * this.typewritterCharsPerSec),
                this.currentMessage.Length
            );

            string message = this.currentMessage.Substring(0, messageLength);
            this.textMessage.text = message;

            if (messageLength < this.currentMessage.Length) this.isTypewritting = true;
            else this.isTypewritting = false;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void ShowText(IDialogueItem item, DatabaseDialogue.ConfigData config)
        {
            DialogueUI.RequireInstance(config);

            DialogueUI.Instance.currentMessage = (item.content != null ? item.content.GetText() : "");
            DialogueUI.Instance.typewritterEnabled = config.enableTypewritterEffect;
            DialogueUI.Instance.typewritterCharsPerSec = config.charactersPerSecond;
            DialogueUI.Instance.typewritterStartTime = Time.time;

            string msg = (config.enableTypewritterEffect ? "" : DialogueUI.Instance.currentMessage);
            DialogueUI.Instance.textMessage.text = msg;
            DialogueUI.Instance.wrapMessage.SetActive(true);
        }

        public static bool ShowChoices(DialogueItemChoiceGroup item, DatabaseDialogue.ConfigData config)
        {
            DialogueUI.RequireInstance(config);

            DialogueUI.Instance.wrapChoices.SetActive(true);

            bool choicesAvailable = DialogueUI.Instance.SetupChoices(item);
            if (choicesAvailable && item.timedChoice) DialogueUI.Instance.StartTimer(item.timeout);

            return choicesAvailable;
        }

        public static void HideText()
        {
            DialogueUI.Instance.wrapMessage.SetActive(false);
        }

        public static void HideChoices()
        {
            DialogueUI.Instance.wrapChoices.SetActive(false);
            DialogueUI.Instance.wrapTimer.SetActive(false);
        }

        public static bool IsTypeWritting()
        {
            return DialogueUI.Instance.isTypewritting;
        }

        public static void CompleteTypeWritting()
        {
            DialogueUI.Instance.typewritterEnabled = false;
        }

        // PROTECTED STATIC METHODS: ----------------------------------------------------------------

        protected static void RequireInstance(DatabaseDialogue.ConfigData config)
        {
            if (config.dialogueSkin == null) Debug.LogError(ERR_NO_DIALOGUE_UI);
            if (config.dialogueSkin.GetInstanceID() == PREFAB_INSTANCE_ID) return;
            if (DialogueUI.Instance != null) Destroy(DialogueUI.Instance.gameObject);

            PREFAB_INSTANCE_ID = config.dialogueSkin.GetInstanceID();
            Instantiate<GameObject>(config.dialogueSkin, Vector3.zero, Quaternion.identity);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected void StartTimer(float timeout)
        {
            this.wrapTimer.SetActive(true);
            this.animatorTimer.SetTrigger(ANIM_TIMER);
            this.animatorTimer.speed = 1f / timeout;
        }

        protected bool SetupChoices(DialogueItemChoiceGroup item)
        {
            this.CleanChoices();
            int choicesSetup = 0;

            for (int i = 0; i < item.children.Count; ++i)
            {
                if (!item.children[i].CheckConditions()) continue;

                GameObject choiceInstance = Instantiate<GameObject>(this.prefabChoice);
                choiceInstance.GetComponent<RectTransform>().SetParent(this.choiceContainer, false);

                Text choiceText = choiceInstance.GetComponentInChildren<Text>();
                if (choiceText != null)
                {
                    choiceText.text = item.children[i].GetContent();
                }

                Button choiceButton = choiceInstance.GetComponentInChildren<Button>();
                if (choiceButton == null)
                {
                    Debug.LogError(ERR_NO_BUTTON);
                    continue;
                }

                this.AssignButtonChoice(choiceButton, i, item);
                choicesSetup++;
            }

            if (item.shuffleChoices) this.ShuffleChoices();
            return choicesSetup != 0;
        }

        protected void CleanChoices()
        {
            for (int i = this.choiceContainer.childCount - 1; i >= 0; --i)
            {
                Destroy(this.choiceContainer.GetChild(i).gameObject);
            }
        }

        protected void ShuffleChoices()
        {
            int childCount = this.choiceContainer.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                int randomIndex = Random.Range(0, childCount);
                this.choiceContainer.GetChild(i).SetSiblingIndex(randomIndex);
            }
        }

        protected void AssignButtonChoice(Button button, int index, DialogueItemChoiceGroup item)
        {
            button.onClick.AddListener(() => item.OnMakeChoice(index));
        }
    }
}