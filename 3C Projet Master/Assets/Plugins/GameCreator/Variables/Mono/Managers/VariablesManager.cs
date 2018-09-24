namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Game Creator/Managers/VariablesManager", 100)]
    public static class VariablesManager
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public static VariablesEvents events = new VariablesEvents();

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static void Reset()
        {
            GlobalVariables globalVariables = GlobalVariablesUtilities.GetGlobalVariables();
            if (globalVariables != null) globalVariables.ResetData();

            foreach (KeyValuePair<string, LocalVariables> localVariable in LocalVariables.REGISTER)
            {
                if (localVariable.Value == null) continue;
                localVariable.Value.ResetData();
            }
        }

        // GETTERS: -------------------------------------------------------------------------------

        public static object GetGlobal(string name)
        {
            Variable variable = GlobalVariablesUtilities.Get(name);
            return (variable != null ? variable.Get() : null);
        }

        public static object GetLocal(GameObject target, string name, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            return (variable != null ? variable.Get() : null);
        }

        // SETTERS: -------------------------------------------------------------------------------

        public static void SetGlobal(string name, object value)
        {
            Variable variable = GlobalVariablesUtilities.Get(name);
            if (variable != null)
            {
                variable.Update(value);
                VariablesManager.events.OnChangeGlobal(name);
            }
        }

        public static void SetLocal(GameObject target, string name, object value, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            if (variable != null)
            {
                variable.Update(value);
                VariablesManager.events.OnChangeLocal(target, name);
            }
        }

        // CHECKERS: ------------------------------------------------------------------------------

        public static bool ExistsGlobal(string name)
        {
            Variable variable = GlobalVariablesUtilities.Get(name);
            return variable != null;
        }

        public static bool ExistsLocal(GameObject target, string name, bool inChildren = false)
        {
            Variable variable = LocalVariablesUtilities.Get(target, name, inChildren);
            return variable != null;
        }
    }
}