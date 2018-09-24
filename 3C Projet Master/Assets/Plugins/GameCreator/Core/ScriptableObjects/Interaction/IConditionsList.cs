namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[ExecuteInEditMode][AddComponentMenu("")]
	public class IConditionsList : MonoBehaviour 
	{
		// PROPERTIES: ----------------------------------------------------------------------------

        public ICondition[] conditions  = new ICondition[0];

        #if UNITY_EDITOR

        #endif

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

        public bool Check(GameObject invoker = null, params object[] parameters)
        {
            if (this.conditions == null) return true;

            for (int i = 0; i < this.conditions.Length; ++i)
            {
                if (this.conditions[i] == null) continue;
                if (!this.conditions[i].Check(invoker, parameters)) return false;
            }

            return true;
        }
	}
}