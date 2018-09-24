﻿namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[ExecuteInEditMode]
	[AddComponentMenu("Game Creator/Actions", 0)]
	public class Actions : MonoBehaviour
	{
		public static bool IS_BLOCKING_ACTION_RUNNING = false;

		// PROPERTIES: ----------------------------------------------------------------------------

		#if UNITY_EDITOR
		public int currentID = 0;
		public int instanceID = 0;
		#endif

		public IActionsList actionsList;

		[Tooltip("Only one foreground Actions collection can be executed at a given time.")]
		public bool runInBackground = true;
		[Tooltip("Useful for executing an Actions collection only once.")]
		public bool destroyAfterFinishing = false;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onExecute = new UnityEvent();
        public UnityEvent onFinish = new UnityEvent();

		// INITIALIZERS: --------------------------------------------------------------------------

		public void OnDestroy()
		{
			if (this.actionsList != null && this.actionsList.isExecuting && !this.runInBackground)
			{
				IS_BLOCKING_ACTION_RUNNING = false;
			}
		}

		private void Awake()
		{
			if (this.actionsList == null) this.actionsList = gameObject.AddComponent<IActionsList>();
		}

		private void OnEnable()
		{
			if (this.actionsList == null) this.actionsList = gameObject.AddComponent<IActionsList>();
		}

		#if UNITY_EDITOR
		private void OnValidate()
		{
			if (this.actionsList == null) this.actionsList = gameObject.AddComponent<IActionsList>();
		}
		#endif

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Execute(GameObject target, params object[] parameters)
		{
			if (this.actionsList.isExecuting) return;

			if (!this.runInBackground)
			{
				if (Actions.IS_BLOCKING_ACTION_RUNNING) return;
				else Actions.IS_BLOCKING_ACTION_RUNNING = true;
			}

            if (this.onExecute != null) this.onExecute.Invoke();
            this.actionsList.Execute(target, this.OnFinish, parameters);
		}

        public virtual void Execute(params object[] parameters)
        {
            this.Execute(null, parameters);
        }

        public virtual void Execute()
        {
            this.Execute(null, new object[0]);
        }

        protected virtual void OnFinish()
		{
            if (this.onFinish != null) this.onFinish.Invoke();
			if (!this.runInBackground) Actions.IS_BLOCKING_ACTION_RUNNING = false;

			if (this.destroyAfterFinishing) 
			{
				Destroy(gameObject);
			}
		}

		// GIZMO METHODS: -------------------------------------------------------------------------

		private void OnDrawGizmos()
		{
			int numActions = (this.actionsList == null || this.actionsList.actions == null 
				? 0 
				: this.actionsList.actions.Length
			);

			switch (numActions)
			{
			case 0  : Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions0", true); break;
			case 1  : Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions1", true); break;
			case 2  : Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions2", true); break;
			default : Gizmos.DrawIcon(transform.position, "GameCreator/Actions/actions3", true); break;
			}
		}
	}
}