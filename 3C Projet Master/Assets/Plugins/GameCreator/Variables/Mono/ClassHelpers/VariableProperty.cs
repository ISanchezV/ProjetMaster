namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class VariableProperty
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public Variable.VarType variableType = Variable.VarType.GlobalVariable;

        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableProperty()
        {
			this.SetupVariables();
		}

        public VariableProperty(Variable.VarType variableType)
        {
			this.SetupVariables();
            this.variableType = variableType;         
        }

		private void SetupVariables()
        {
            this.global = this.global ?? new HelperGlobalVariable();
            this.local = this.local ?? new HelperLocalVariable();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public object Get(GameObject invoker = null)
        {
            switch (this.variableType)
            {
                case Variable.VarType.GlobalVariable: return this.global.Get(invoker);
                case Variable.VarType.LocalVariable: return this.local.Get(invoker);
            }

            return null;
        }

        public void Set(object value, GameObject invoker = null)
        {
            switch (this.variableType)
            {
                case Variable.VarType.GlobalVariable: this.global.Set(value, invoker); break;
                case Variable.VarType.LocalVariable: this.local.Set(value, invoker); break;
            }
        }

        public string GetVariableID()
        {
            switch (this.variableType)
            {
                case Variable.VarType.GlobalVariable: return this.global.name;
                case Variable.VarType.LocalVariable: return this.local.name;
            }

            return "";
        }

        public Variable.VarType GetVariableType()
        {
            return this.variableType;
        }

        public GameObject GetLocalVariableGameObject()
        {
            return this.local.GetGameObject(null);
        }

        // OVERRIDERS: ----------------------------------------------------------------------------

		public override string ToString()
		{
            string varName = "";
            switch (this.variableType)
            {
                case Variable.VarType.GlobalVariable : varName = this.global.ToString(); break;
                case Variable.VarType.LocalVariable: varName = this.local.ToString(); break;
            }

            return (string.IsNullOrEmpty(varName) ? "(unknown)" : varName);
		}

        public string ToStringValue(GameObject invoker)
        {
            switch (this.variableType)
            {
                case Variable.VarType.GlobalVariable: return this.global.ToStringValue(invoker);
                case Variable.VarType.LocalVariable: return this.local.ToStringValue(invoker);
            }

            return "unknown";
        }
	}
}