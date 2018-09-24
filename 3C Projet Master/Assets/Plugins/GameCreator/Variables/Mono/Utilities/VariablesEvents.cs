namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class VariablesEvents
    {
        public class VarEvent : UnityEvent<string> { }

        // PROPERTIES: ----------------------------------------------------------------------------

        private Dictionary<string, VarEvent> onVariableChange;

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariablesEvents()
        {
            this.onVariableChange = new Dictionary<string, VarEvent>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void OnChangeGlobal(string variableID)
        {
            if (!this.onVariableChange.ContainsKey(variableID)) return;
            VarEvent varEvent = this.onVariableChange[variableID];
            if (varEvent != null) varEvent.Invoke(variableID);
        }

        public void OnChangeLocal(GameObject gameObject, string variableID)
        {
            string localID = GetLocalID(gameObject, variableID);
            if (!this.onVariableChange.ContainsKey(variableID)) return;

            VarEvent varEvent = this.onVariableChange[localID];
            if (varEvent != null) varEvent.Invoke(variableID);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public void SetOnChangeGlobal(UnityAction<string> action, string variableID)
        {
            if (!this.onVariableChange.ContainsKey(variableID))
            {
                this.onVariableChange.Add(variableID, new VarEvent());
            }

            this.onVariableChange[variableID].AddListener(action);
        }

        public void SetOnChangeLocal(UnityAction<string> action, GameObject gameObject, string variableID)
        {
            string localID = GetLocalID(gameObject, variableID);
            if (!this.onVariableChange.ContainsKey(localID))
            {
                this.onVariableChange.Add(localID, new VarEvent());
            }

            this.onVariableChange[localID].AddListener(action);
        }

        // REMOVERS: ------------------------------------------------------------------------------

        public void RemoveChangeGlobal(UnityAction<string> action, string variableID)
        {
            if (!this.onVariableChange.ContainsKey(variableID)) return;
            this.onVariableChange[variableID].RemoveListener(action);
        }

        public void RemoveChangeLocal(UnityAction<string> action, GameObject gameObject, string variableID)
        {
            string localID = GetLocalID(gameObject, variableID);
            if (!this.onVariableChange.ContainsKey(localID)) return;
            this.onVariableChange[localID].RemoveListener(action);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private string GetLocalID(GameObject gameObject, string variableID)
        {
            return string.Format("{0}:{1}", gameObject.GetInstanceID(), variableID);
        }
    }
}
