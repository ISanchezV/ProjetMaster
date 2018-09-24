namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
    using GameCreator.Core.Hooks;
    using GameCreator.Variables;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
    public class ActionVariablesAssignVector3 : IActionVariablesAssign
	{
        [VariableFilter(Variable.DataType.Vector3)]
        public VariableProperty variable;

        public Vector3 value = Vector3.zero;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override void ExecuteAssignement(GameObject target)
		{
            switch (this.valueFrom)
            {
                case ValueFrom.Invoker: this.variable.Set(target.transform.position); break;
                case ValueFrom.Player : this.variable.Set(HookPlayer.Instance.transform.position); break;
                case ValueFrom.Constant : this.variable.Set(this.value); break;
            }
		}

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Vector3";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, "Vector3", this.variable);
		}

		public override bool PaintInspectorTarget()
		{
            return true;
		}

        #endif
	}
}
