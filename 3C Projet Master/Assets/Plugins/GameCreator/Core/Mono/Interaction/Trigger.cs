namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("Game Creator/Trigger", 0)]
	public class Trigger : MonoBehaviour
	{
        private static readonly Color COLOR_WHITE_LIGHT = new Color(256, 256f, 256, 0.5f);

		public enum Platforms
		{
			Editor,
			Mobile,
			tvOS,
			Desktop,
			PS4,
			XBoxOne,
			WiiU,
			Switch
		}

		public const int ALL_PLATFORMS_KEY = -1;

		[System.Serializable]
		public class PlatformIgniters : SerializableDictionaryBase<int, Igniter> {}

		// PROPERTIES: ----------------------------------------------------------------------------

		public PlatformIgniters igniters = new PlatformIgniters();

		public List<Event> events;
		public List<Actions> actions;

		// ADVANCED PROPERTIES: -------------------------------------------------------------------

		public bool minDistance = false;
		public float minDistanceToPlayer = 5.0f;

        // EVENTS: --------------------------------------------------------------------------------

        public UnityEvent onExecute = new UnityEvent();

		// INITIALIZE: ----------------------------------------------------------------------------

		private void Awake()
		{
			EventSystemManager.Instance.Wakeup();
			this.SetupPlatformIgniter();
		}

		private void SetupPlatformIgniter()
		{
			bool overridePlatform = false;

			#if UNITY_STANDALONE
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.Desktop);
			#endif

			#if UNITY_EDITOR
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.Editor);
			#endif

			#if UNITY_ANDROID || UNITY_IOS
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.Mobile);
			#endif

			#if UNITY_TVOS
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.tvOS);
			#endif

			#if UNITY_PS4
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.PS4);
			#endif

			#if UNITY_XBOXONE
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.XBoxOne);
			#endif

			#if UNITY_WIIU
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.WiiU);
			#endif

			#if UNITY_SWITCH
			if (!overridePlatform) overridePlatform = this.CheckPlatformIgniter(Platforms.Switch);
			#endif

			if (!overridePlatform) this.igniters[ALL_PLATFORMS_KEY].enabled = true;
		}

		private bool CheckPlatformIgniter(Platforms platform)
		{
			if (this.igniters.ContainsKey((int)platform)) 
			{
				this.igniters[(int)Platforms.Editor].enabled = true;
				return true;
			}

			return false;
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public virtual void Execute(GameObject target, params object[] parameters)
		{
			if (this.minDistance && HookPlayer.Instance != null)
			{
				float distance = Vector3.Distance(HookPlayer.Instance.transform.position, transform.position);
				if (distance > this.minDistanceToPlayer) return;
			}

            if (this.onExecute != null) this.onExecute.Invoke();
			for (int i = 0; i < this.events.Count; ++i)
			{
				if (this.events[i] != null)
				{
                    Core.Event eventReference = this.events[i];
                    if (string.IsNullOrEmpty(eventReference.gameObject.scene.name))
                    {
                        eventReference = Instantiate<Core.Event>(
                            eventReference, 
                            transform.position, 
                            transform.rotation
                        );
                    }

                    eventReference.Interact(target, parameters);
				}
			}

			for (int i = 0; i < this.actions.Count; ++i)
			{
				if (this.actions[i] != null)
				{
                    Actions actionsReference = this.actions[i];
                    if (string.IsNullOrEmpty(actionsReference.gameObject.scene.name))
                    {
                        actionsReference = Instantiate<Actions>(
                            actionsReference,
                            transform.position,
                            transform.rotation
                        );
                    }

                    actionsReference.Execute(target, parameters);
				}
			}
		}

        public virtual void Execute()
        {
            this.Execute(gameObject);
        }

		// GIZMO METHODS: -------------------------------------------------------------------------

		private void OnDrawGizmos()
		{
			int state = 0;
			state |= (this.events == null || this.events.Count == 0 ? 0 : 1);
			state |= (this.actions == null || this.actions.Count == 0 ? 0 : 2);

			switch (state)
			{
			case 0 : Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger0", true); break;
			case 1 : Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger1", true); break;
			case 2 : Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger2", true); break;
			case 3 : Gizmos.DrawIcon(transform.position, "GameCreator/Trigger/trigger3", true); break;
			}

            if (this.minDistance)
            {
                Gizmos.color = COLOR_WHITE_LIGHT;
                Gizmos.DrawWireSphere(transform.position, this.minDistanceToPlayer);
            }
		}
	}
}