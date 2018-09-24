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
    public class ActionVariablesAssignVector2 : IActionVariablesAssign
	{
        [VariableFilter(Variable.DataType.Vector2)]
        public VariableProperty variable;

        public Vector2 value = Vector2.zero;

		// EXECUTABLE: ----------------------------------------------------------------------------

		public override void ExecuteAssignement(GameObject target)
		{
            switch (this.valueFrom)
            {
                case ValueFrom.Invoker: this.variable.Set(this.GetVector2(target.transform.position)); break;
                case ValueFrom.Player : this.variable.Set(this.GetVector2(HookPlayer.Instance.transform.position)); break;
                case ValueFrom.Constant : this.variable.Set(this.value); break;
            }
		}

        private Vector2 GetVector2(Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

		public static new string NAME = "Variables/Variable Vector2";

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return string.Format(NODE_TITLE, "Vector2", this.variable);
		}

		public override bool PaintInspectorTarget()
		{
            return true;
		}

        #endif
	}
}
