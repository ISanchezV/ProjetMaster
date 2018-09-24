namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[ExecuteInEditMode][AddComponentMenu("")]
	public class IActionsList : MonoBehaviour 
	{
		public class ActionCoroutine
		{
			public Coroutine coroutine {get; private set;}
			public object result {get; private set;}
			private IEnumerator target;

            public ActionCoroutine(IEnumerator target) 
			{
				this.target = target;
                this.coroutine = CoroutinesManager.Instance.StartCoroutine(Run());
			}

			private IEnumerator Run() 
			{
				while (this.target.MoveNext()) 
				{
					this.result = this.target.Current;
					yield return this.result;
				}
			}
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		public IAction[] actions  = new IAction[0];
		public int executingIndex = -1;
		public bool isExecuting   = false;

        private bool cancelExecution = false;

		// CONSTRUCTORS: --------------------------------------------------------------------------

		#if UNITY_EDITOR
		private void Awake()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		private void OnValidate()
		{
			this.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}
		#endif

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void Execute(GameObject target, System.Action callback, params object[] parameters)
		{
			this.isExecuting = true;
            CoroutinesManager.Instance.StartCoroutine(
                this.ExecuteCoroutine(target, callback, parameters)
            );
		}

        public IEnumerator ExecuteCoroutine(GameObject target, System.Action callback, params object[] parameters)
		{
            this.isExecuting = true;
            this.cancelExecution = false;

            for (int i = 0; i < this.actions.Length && !this.cancelExecution; ++i)
			{
				if (this.actions[i] == null) continue;

                this.executingIndex = i;

                if (!this.actions[i].InstantExecute(target, this.actions, i, parameters))
                {
                    ActionCoroutine actionCoroutine = new ActionCoroutine(
                        this.actions[i].Execute(target, this.actions, i, parameters)
                    );

                    yield return actionCoroutine.coroutine;

                    if (actionCoroutine == null || actionCoroutine.result == null) yield break;
                    else i += ((int)actionCoroutine.result);
                }

                if (i >= this.actions.Length) break;
                if (i < 0) i = -1;
			}

			this.executingIndex = -1;
			this.isExecuting = false;

			if (callback != null) callback();
		}

        public void Cancel()
        {
            if (!this.isExecuting) return;
            this.cancelExecution = true;
        }
	}
}