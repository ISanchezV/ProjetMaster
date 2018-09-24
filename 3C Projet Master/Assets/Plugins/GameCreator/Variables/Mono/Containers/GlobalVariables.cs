namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [Serializable]
    public class GlobalVariables : ScriptableObject, IGameSave
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public SOVariable[] references = new SOVariable[0];

        private Dictionary<string, Variable> variables;
        private bool igamesaveInitialized = false;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(string name)
        {
            this.RequireInit();
            if (this.variables.ContainsKey(name))
            {
                return this.variables[name];
            }

            return null;
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void RequireInit(bool force = false)
		{
            if (force || this.variables != null) return;

            if (!this.igamesaveInitialized)
            {
                SaveLoadManager.Instance.Initialize(this);
                this.igamesaveInitialized = true;
            }

            this.variables = new Dictionary<string, Variable>();

            for (int i = 0; i < this.references.Length; ++i)
            {
                Variable variable = Instantiate(this.references[i]).variable;

                if (variable == null) continue;
                string variableName = variable.name;

                if (!this.variables.ContainsKey(variableName))
                {
                    this.variables.Add(variableName, variable);
                }
            }
		}

        // IGAMESAVE: -----------------------------------------------------------------------------

        public string GetUniqueName()
        {
            return "variables:global";
        }

        public Type GetSaveDataType()
        {
            return typeof(DatabaseVariables.Container);
        }

        public object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();

            foreach (KeyValuePair<string, Variable> item in this.variables)
            {
                if (item.Value.CanSave())
                {
                    container.variables.Add(item.Value);
                }
            }

            return container;
        }

        public void ResetData()
        {
            DatabaseVariables database = IDatabase.LoadDatabaseCopy<DatabaseVariables>();
            this.references = database.GetGlobalVariables().references;
            this.RequireInit(true);
        }

        public void OnLoad(object generic)
        {
            this.RequireInit();

            DatabaseVariables.Container container = (DatabaseVariables.Container)generic;
            int variablesContainerCount = container.variables.Count;

            for (int i = 0; i < variablesContainerCount; ++i)
            {
                Variable variablesContainerVariable = container.variables[i];
                string varName = variablesContainerVariable.name;

                if (this.variables.ContainsKey(varName) && this.variables[varName].CanSave())
                {
                    this.variables[varName] = variablesContainerVariable;
                }
            }
        }
	}
}