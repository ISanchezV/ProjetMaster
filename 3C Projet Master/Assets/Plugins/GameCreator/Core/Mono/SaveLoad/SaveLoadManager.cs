namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

	[AddComponentMenu("Game Creator/Managers/SaveLoadManager", 100)]
	public class SaveLoadManager : Singleton<SaveLoadManager> 
	{
		private const string STORE_KEYFMT = "gamedata:{0:D2}:{1}";

		private static bool PROFILE_LOADED = false;
		private static int CURRENT_PROFILE = 0;

        // PROPERTIES: ----------------------------------------------------------------------------

        private ScenesData scenesData;
        private List<IGameSave> storage;
        private Dictionary<string, object> data;

        public UnityAction<int> onSave;
        public UnityAction<int> onLoad;

        // INITIALIZE: ----------------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            SaveLoadManager.Instance.WakeUp();
        }

		protected override void OnCreate() 
        {
            this.scenesData = new ScenesData(SceneManager.GetActiveScene().name);
            SaveLoadManager.Instance.Initialize(this.scenesData);

            SceneManager.sceneLoaded += this.OnLoadScene;
            SceneManager.sceneUnloaded += this.OnUnloadScene;
        }

        private void OnLoadScene(Scene scene, LoadSceneMode mode)
        {
            this.scenesData.Add(scene.name, mode);
        }

        private void OnUnloadScene(Scene scene)
        {
            this.scenesData.Remove(scene.name);
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void SetCurrentProfile(int profile)
		{
			SaveLoadManager.CURRENT_PROFILE = profile;
		}

		public int GetCurrentProfile()
		{
			return SaveLoadManager.CURRENT_PROFILE;
		}

		public void Initialize(IGameSave gameSave)
		{
            if (this.storage == null) this.storage = new List<IGameSave>();
            if (this.data == null) this.data = new Dictionary<string, object>();

			string key = gameSave.GetUniqueName();
            int index = -1;

            for (int i = this.storage.Count - 1; i >= 0; --i)
            {
                if (this.storage[i] == null)
                {
                    this.storage.RemoveAt(i);
                    continue;
                }

                if (this.storage[i].GetUniqueName() == key)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0) this.storage[index] = gameSave;
            else this.storage.Add(gameSave);

            if (this.data.ContainsKey(key))
            {
                gameSave.OnLoad(this.data[key]);
            }
            else if (SaveLoadManager.PROFILE_LOADED)
            {
                this.LoadItem(gameSave, SaveLoadManager.CURRENT_PROFILE);
            }
		}

		public void Save(int profile)
		{
            if (this.onSave != null) this.onSave.Invoke(profile);
            if (this.storage == null) this.storage = new List<IGameSave>();
            if (this.data == null) this.data = new Dictionary<string, object>();

            for (int i = this.storage.Count - 1; i >= 0; --i)
            {
                IGameSave item = this.storage[i];
                if (item == null)
                {
                    this.storage.RemoveAt(i);
                    continue;
                }

                object saveData = item.GetSaveData();
                if (saveData == null) continue;

                if (!saveData.GetType().IsSerializable)
                {
                    throw new NonSerializableException(saveData.GetType().ToString());
                }

                this.data[item.GetUniqueName()] = saveData;
            }

            foreach (KeyValuePair<string, object> item in this.data)
            {
                string serializedSaveData = JsonUtility.ToJson(item.Value, false);
                string key = this.GetKeyName(profile, item.Key);
                DataManager.SetString(key, serializedSaveData);
            }
		}

		public void Load(int profile)
		{
            if (this.onLoad != null) this.onLoad.Invoke(profile);
            if (this.storage == null) this.storage = new List<IGameSave>();
            this.data = new Dictionary<string, object>();

            for (int i = this.storage.Count - 1; i >= 0; --i)
            {
                IGameSave item = this.storage[i];
                if (item == null)
                {
                    this.storage.RemoveAt(i);
                    continue;
                }

                item.ResetData();
                this.LoadItem(item, profile);
            }

            SaveLoadManager.PROFILE_LOADED = true;
		}

        public void OnDestroyIGameSave(IGameSave gameSave)
        {
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && 
                 UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
            #endif

            this.data[gameSave.GetUniqueName()] = gameSave.GetSaveData();
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void LoadItem(IGameSave gameSave, int profile)
		{
			if (gameSave == null) return;
			
			string key = this.GetKeyName(profile, gameSave.GetUniqueName());
			string serializedData = DataManager.GetString(key);

			if (!string.IsNullOrEmpty(serializedData)) 
			{
				System.Type type = gameSave.GetSaveDataType();
                object genericData = JsonUtility.FromJson(serializedData, type);

				gameSave.OnLoad(genericData);
			}
		}

		private string GetKeyName(int profile, string key)
		{
			return string.Format(STORE_KEYFMT, profile, key);
		}

		// EXCEPTIONS: ----------------------------------------------------------------------------

		[System.Serializable]
		private class NonSerializableException : System.Exception
		{
            private const string MESSAGE = "Unable to serialize: {0}. Add [System.Serializable]";
			public NonSerializableException(string key) : base(string.Format(MESSAGE, key)) {}
		}
	}

	///////////////////////////////////////////////////////////////////////////////////////////////
	// INTERFACE ISAVEGAME: -----------------------------------------------------------------------
	///////////////////////////////////////////////////////////////////////////////////////////////

	public interface IGameSave
	{
		string GetUniqueName();

		System.Type GetSaveDataType();
		object GetSaveData();

		void ResetData();
        void OnLoad(object generic);
	}
}