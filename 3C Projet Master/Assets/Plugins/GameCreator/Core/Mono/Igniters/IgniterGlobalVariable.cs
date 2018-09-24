namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Variables;

	[AddComponentMenu("")]
	public class IgniterGlobalVariable : Igniter 
	{
        public VariableGlobalProperty variable = new VariableGlobalProperty();

		#if UNITY_EDITOR
        public new static string NAME = "Variables/Global Variable";
        #endif

        private void Start()
        {
            VariablesManager.events.SetOnChangeGlobal(
                this.OnVariableChange, 
                this.variable.GetVariableID()
            );
        }

        private void OnVariableChange(string variableID)
        {
            this.ExecuteTrigger(gameObject);
        }

        private void OnDestroy()
        {
            VariablesManager.events.RemoveChangeGlobal(
                this.OnVariableChange,
                this.variable.GetVariableID()
            );
        }
    }
}