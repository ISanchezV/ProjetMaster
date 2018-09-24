namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    public static class GlobalVariablesUtilities
    {
        private static GlobalVariables REFERENCE;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static Variable Get(string name)
        {
            GlobalVariablesUtilities.RequireGlobals();
            if (string.IsNullOrEmpty(name)) return null;

            return REFERENCE.Get(name);
        }

        public static GlobalVariables GetGlobalVariables()
        {
            GlobalVariablesUtilities.RequireGlobals();
            return REFERENCE;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static void RequireGlobals()
        {
            if (REFERENCE != null) return;

            DatabaseVariables database = IDatabase.LoadDatabaseCopy<DatabaseVariables>();
            REFERENCE = database.GetGlobalVariables();
        }
    }
}