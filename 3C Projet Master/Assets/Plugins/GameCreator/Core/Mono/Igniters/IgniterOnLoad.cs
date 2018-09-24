namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;

	[AddComponentMenu("")]
	public class IgniterOnLoad : Igniter 
	{
		#if UNITY_EDITOR
        public new static string NAME = "General/On Load";
        #endif

        private void Start()
        {
            SaveLoadManager.Instance.onLoad += this.OnLoad;
        }

        private void OnDestroy()
        {
            SaveLoadManager.Instance.onLoad -= this.OnLoad;
        }

        private void OnLoad(int profile)
		{
            this.ExecuteTrigger(HookPlayer.Instance.gameObject);
		}
	}
}