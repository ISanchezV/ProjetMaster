namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[ExecuteInEditMode]
	[AddComponentMenu("Game Creator/Event", 0)]
	public class Event : MonoBehaviour 
	{
		public Interaction[] interactions = new Interaction[0];
		public Actions defaultActions;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onInteract = new UnityEvent();

		// INTERACT METHOD: -----------------------------------------------------------------------

        public virtual void Interact()
        {
            this.Interact(null, new object[0]);
        }

        public virtual void Interact(GameObject target, params object[] parameters)
		{
            if (this.onInteract != null) this.onInteract.Invoke();
			for (int i = 0; i < this.interactions.Length; ++i)
			{
                if (this.interactions[i].CheckConditions(target, parameters))
				{
                    this.interactions[i].ExecuteActions(target, parameters);
					return;
				}
			}

			if (this.defaultActions != null) 
			{
				this.defaultActions.Execute();
			}
		}

        public virtual IEnumerator InteractCoroutine(GameObject target = null)
        {
            for (int i = 0; i < this.interactions.Length; ++i)
            {
                if (this.interactions[i].CheckConditions(target))
                {
                    Actions actions = this.interactions[i].actions;
                    if (actions != null)
                    {
                        Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                            actions.actionsList.ExecuteCoroutine(target, null)
                        );

                        yield return coroutine;
                    }

                    yield break;
                }
            }

            if (this.defaultActions != null)
            {
                Coroutine coroutine = CoroutinesManager.Instance.StartCoroutine(
                    this.defaultActions.actionsList.ExecuteCoroutine(target, null)
                );

                yield return coroutine;
                yield break;
            }
        }

		// GIZMO METHODS: -------------------------------------------------------------------------

		private void OnDrawGizmos()
		{
			int numInteractions = (this.interactions == null ? 0 : this.interactions.Length);
			switch (numInteractions)
			{
			case 0  : Gizmos.DrawIcon(transform.position, "GameCreator/Event/event0", true); break;
			case 1  : Gizmos.DrawIcon(transform.position, "GameCreator/Event/event1", true); break;
			case 2  : Gizmos.DrawIcon(transform.position, "GameCreator/Event/event2", true); break;
			default : Gizmos.DrawIcon(transform.position, "GameCreator/Event/event3", true); break;
			}
		}
	}
}