namespace GameCreator.Dialogue
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;
    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public class DatabaseDialogue : IDatabase
	{
        private const string RESOURCE_DEFAULTSKIN_PATH = "GameCreator/DefaultDialogueSkin";

        [System.Serializable]
        public class ConfigData
        {
            public GameObject dialogueSkin;

            public bool enableTypewritterEffect = true;
            public float charactersPerSecond = 30f;
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public ConfigData defaultConfig;

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseDialogue Load()
        {
            DatabaseDialogue databaseDialogue = IDatabase.LoadDatabase<DatabaseDialogue>();
            if (databaseDialogue.defaultConfig.dialogueSkin == null)
            {
                GameObject skin = Resources.Load<GameObject>(RESOURCE_DEFAULTSKIN_PATH);
                databaseDialogue.defaultConfig.dialogueSkin = skin;
            }

            return databaseDialogue;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        protected override string GetProjectPath()
        {
            return "Assets/Plugins/GameCreatorData/Dialogue/Resources";
        }

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseDialogue>();
        }

        #endif
	}
}