namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.SceneManagement;
    using GameCreator.Core.Hooks;

	[AddComponentMenu("Game Creator/Managers/EventSystemManager", 100)]
	public class EventSystemManager : Singleton<EventSystemManager>
	{
        protected override void OnCreate ()
		{
			if (GameObject.FindObjectOfType<EventSystem>() == null)
			{
				gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
			}

			SceneManager.sceneLoaded += this.OnSceneLoad;
		}

		public void Wakeup()
		{
			return;
		}

		public void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
		{
            if (HookCamera.Instance != null)
			{
                PhysicsRaycaster   raycaster3D = HookCamera.Instance.GetComponent<PhysicsRaycaster>();
                Physics2DRaycaster raycaster2D = HookCamera.Instance.GetComponent<Physics2DRaycaster>();

                if (raycaster3D == null) HookCamera.Instance.gameObject.AddComponent<PhysicsRaycaster>();
                if (raycaster2D == null) HookCamera.Instance.gameObject.AddComponent<Physics2DRaycaster>();
			}
		}
	}
}