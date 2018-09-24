namespace GameCreator.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    public class DatabaseGeneral : IDatabase
    {
        public enum GENERAL_SCREEN_SPACE
        {
            ScreenSpaceOverlay,
            ScreenSpaceCamera,
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public GENERAL_SCREEN_SPACE generalRenderMode = GENERAL_SCREEN_SPACE.ScreenSpaceOverlay;
        public GameObject prefabMessage;
        public GameObject prefabTouchstick;

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static DatabaseGeneral Load()
        {
            return IDatabase.LoadDatabase<DatabaseGeneral>();
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            IDatabase.Setup<DatabaseGeneral>();
        }

        #endif
	}
}