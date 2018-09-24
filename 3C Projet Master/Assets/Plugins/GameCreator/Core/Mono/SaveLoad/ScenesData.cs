namespace GameCreator.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class ScenesData : IGameSave
    {
        public string mainScene;
        public List<string> additiveScenes;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        public ScenesData(string mainScene)
        {
            this.mainScene = mainScene;
            this.additiveScenes = new List<string>();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Add(string name, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                this.mainScene = name;
                this.additiveScenes = new List<string>();
            }
            else if (mode == LoadSceneMode.Additive)
            {
                this.additiveScenes.Add(name);
            }
        }

        public void Remove(string name)
        {
            this.additiveScenes.Remove(name);
        }

        // IGAMESAVE INTERFACE: -------------------------------------------------------------------

        public object GetSaveData()
        {
            return this;
        }

        public Type GetSaveDataType()
        {
            return typeof(ScenesData);
        }

        public string GetUniqueName()
        {
            return "scenes-data";
        }

        public void OnLoad(object generic)
        {
            ScenesData data = (ScenesData)generic;

            this.mainScene = data.mainScene;
            this.additiveScenes = data.additiveScenes;

            SceneManager.LoadScene(this.mainScene, LoadSceneMode.Single);
            for (int i = 0; i < this.additiveScenes.Count; ++i)
            {
                SceneManager.LoadScene(this.additiveScenes[i], LoadSceneMode.Additive);
            }
        }

        public void ResetData()
        {
            this.mainScene = SceneManager.GetActiveScene().name;
            this.additiveScenes = new List<string>();
        }
    }
}