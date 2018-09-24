namespace GameCreator.Dialogue
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Localization;

    public class IDialogueItem : MonoBehaviour
    {
        protected const float TIME_SAFE_OFFSET = 0.1f;

        public enum ExecuteBehaviour
        {
            Simultaneous,
            DialogueBeforeActions,
            ActionsBeforeDialogue
        }

        public enum AfterRunBehaviour
        {
            Continue,
            Exit,
            Jump
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public Dialogue dialogue;
        public IDialogueItem parent;
        public List<IDialogueItem> children;

        [LocStringBigText]
        public LocString content;
        public AudioClip voice;
        public bool autoPlay = false;
        public float autoPlayTime = 3.0f;

        public AfterRunBehaviour afterRun = AfterRunBehaviour.Continue;
        public IDialogueItem jumpTo;

        public ExecuteBehaviour executeBehavior = ExecuteBehaviour.Simultaneous;
        public IActionsList actionsList;
        public IConditionsList conditionsList;

        public bool overrideDefaultConfig = false;
        public DatabaseDialogue.ConfigData config = new DatabaseDialogue.ConfigData();

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual string GetContent()
        {
            return "";
        }

        public virtual IDialogueItem[] GetNextItem()
        {
            return null;
        }

        protected virtual IEnumerator RunItem()
        {
            yield break;
        }

        public virtual bool CanHaveParent(IDialogueItem parent)
        {
            return true;
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public List<int> GetChildrenIDs()
        {
            List<int> listIDs = new List<int>();
            if (this.children == null) return listIDs;

            for (int i = 0; i < this.children.Count; ++i)
            {
                listIDs.Add(this.children[i].GetInstanceID());
            }

            return listIDs;
        }

        public IEnumerator Run()
        {
            switch (this.executeBehavior)
            {
                case ExecuteBehaviour.Simultaneous:
                    if (this.actionsList != null) this.actionsList.Execute(gameObject, null);
                    yield return this.RunItem();
                    break;

                case ExecuteBehaviour.ActionsBeforeDialogue:
                    if (this.actionsList != null) yield return this.actionsList.ExecuteCoroutine(gameObject, null);
                    yield return this.RunItem();
                    break;

                case ExecuteBehaviour.DialogueBeforeActions:
                    yield return this.RunItem();
                    if (this.actionsList != null) yield return this.actionsList.ExecuteCoroutine(gameObject, null);
                    break;
            }
        }

        public bool CheckConditions()
        {
            if (this.conditionsList == null) return true;
            return this.conditionsList.Check();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected DatabaseDialogue.ConfigData GetConfigData()
        {
            DatabaseDialogue.ConfigData defaultConfig = DatabaseDialogue.Load().defaultConfig;
            DatabaseDialogue.ConfigData result = defaultConfig;
            if (this.overrideDefaultConfig)
            {
                if (this.config.dialogueSkin != null) result.dialogueSkin = this.config.dialogueSkin;
                result.enableTypewritterEffect = this.config.enableTypewritterEffect;
                result.charactersPerSecond = this.config.charactersPerSecond;
            }

            return result;
        }

        protected IEnumerator RunShowText()
        {
            DatabaseDialogue.ConfigData configData = this.GetConfigData();
            DialogueUI.ShowText(this, configData);

            if (this.voice != null) AudioManager.Instance.PlayVoice(this.voice);

            if (this.autoPlay)
            {
                WaitForSeconds waitForSeconds = new WaitForSeconds(this.autoPlayTime);
                yield return waitForSeconds;
            }
            else
            {
                WaitForSeconds waitForSeconds = new WaitForSeconds(TIME_SAFE_OFFSET);
                yield return waitForSeconds;
                yield return new WaitUntil(() => {
                    if (Input.anyKeyDown)
                    {
                        if (configData.enableTypewritterEffect && DialogueUI.IsTypeWritting())
                        {
                            DialogueUI.CompleteTypeWritting();
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return false;
                });
            }
        }
    }
}