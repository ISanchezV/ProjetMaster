namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class LocalVariablesUtilities
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static Variable Get(GameObject target, string name, bool inChildren)
        {
            if (target == null) return null;
            if (string.IsNullOrEmpty(name)) return null;

            LocalVariables[] localVariables = GatherLocals(target, inChildren);
            for (int i = 0; i < localVariables.Length; ++i)
            {
                Variable variable = localVariables[i].Get(name);
                if (variable != null) return variable;
            }

            return null;
        }

        public static LocalVariables[] GatherLocals(GameObject target, bool inChildren)
        {
            if (!inChildren) return target.GetComponents<LocalVariables>();
            else return target.GetComponentsInChildren<LocalVariables>();
        }
    }
}