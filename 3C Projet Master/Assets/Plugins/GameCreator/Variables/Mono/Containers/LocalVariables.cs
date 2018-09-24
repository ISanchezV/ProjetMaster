namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using GameCreator.Core;

    [AddComponentMenu("Game Creator/Local Variables")]
    public class LocalVariables : MonoBehaviour, IGameSave
    {
        public static Dictionary<string, LocalVariables> REGISTER = new Dictionary<string, LocalVariables>();

        // PROPERTIES: ----------------------------------------------------------------------------

        public UniqueInstanceID uniqueID = new UniqueInstanceID();
        public MBVariable[] references = new MBVariable[0];

        private Dictionary<string, Variable> variables;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Variable Get(string name)
        {
            this.Initialize();
            if (this.variables.ContainsKey(name))
            {
                return this.variables[name];
            }

            return null;
        }

        // INITIALIZERS: --------------------------------------------------------------------------

		private void Start()
		{
            this.Initialize();
            SaveLoadManager.Instance.Initialize(this);
		}

		private void Initialize()
		{
            if (!REGISTER.ContainsKey(this.uniqueID.value))
            {
                REGISTER.Add(this.uniqueID.value, this);
            }

            this.RequireInit();
		}

        private void OnDestroy()
		{
            if (REGISTER.ContainsKey(this.uniqueID.value))
            {
                REGISTER.Remove(this.uniqueID.value);
            }

            SaveLoadManager.Instance.OnDestroyIGameSave(this);
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void RequireInit(bool force = false)
        {
            if (this.variables != null && !force) return;

            this.variables = new Dictionary<string, Variable>();
            for (int i = 0; i < this.references.Length; ++i)
            {
                Variable variable = this.references[i].variable;
                if (variable == null) continue;
                string variableName = variable.name;

                if (!this.variables.ContainsKey(variableName))
                {
                    this.variables.Add(variableName, new Variable(variable));
                }
            }
        }

		// IGAMESAVE: -----------------------------------------------------------------------------

		public string GetUniqueName()
        {
            string uniqueName = string.Format(
                "variables:local:{0}",
                this.uniqueID.value
            );

            return uniqueName;
        }

        public Type GetSaveDataType()
        {
            return typeof(DatabaseVariables.Container);
        }

        public object GetSaveData()
        {
            DatabaseVariables.Container container = new DatabaseVariables.Container();
            container.variables = new List<Variable>();
            if (this.variables == null || this.variables.Count == 0)
            {
                return container;
            }

            foreach (KeyValuePair<string, Variable> item in this.variables)
            {
                if (item.Value != null && item.Value.CanSave())
                {
                    container.variables.Add(item.Value);
                }
            }

            return container;
        }

        public void ResetData()
        {
            this.RequireInit(true);
        }

        public void OnLoad(object generic)
        {
            if (generic == null) return;

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