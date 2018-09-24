namespace GameCreator.Dialogue
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    #if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    #endif

    [AddComponentMenu("Game Creator/Dialogue", 0)]
    public class Dialogue : MonoBehaviour
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public DialogueItemRoot dialogue;
        public IDialogueItem[] itemInstances;

        #if UNITY_EDITOR
        public TreeViewState dialogueTreeState = new TreeViewState();
        #endif

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public IEnumerator Run()
        {
            Stack<IDialogueItem> stackItems = new Stack<IDialogueItem>();
            stackItems.Push(this.dialogue);

            while (stackItems.Count > 0)
            {
                IDialogueItem item = stackItems.Pop();
                yield return item.Run();

                if (item.afterRun == IDialogueItem.AfterRunBehaviour.Jump && item.jumpTo != null)
                {
                    stackItems.Clear();

                    int jumpToID = item.jumpTo.GetInstanceID();
                    List<IDialogueItem> parentChildren = item.jumpTo.parent.children;
                    int parentChildrenCount = parentChildren.Count;

                    for (int i = parentChildrenCount - 1; i >= 0; --i)
                    {
                        if (parentChildren[i] == null) continue;
                        stackItems.Push(parentChildren[i]);
                        if (parentChildren[i].GetInstanceID() == jumpToID) break;
                    }
                }
                else if (item.afterRun == IDialogueItem.AfterRunBehaviour.Exit)
                {
                    stackItems.Clear();
                    yield break;
                }
                else
                {
                    IDialogueItem[] childItems = item.GetNextItem();
                    if (childItems != null)
                    {
                        int numChildItems = childItems.Length;
                        for (int i = numChildItems - 1; i >= 0; --i)
                        {
                            if (childItems[i] == null) continue;
                            stackItems.Push(childItems[i]);
                        }
                    }
                }
            }
        }
    }
}