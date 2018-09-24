namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class DataManager
	{
		// GETTER METHODS: ---------------------------------------------------------------------------------------------

		public static float GetFloat(string key, float defaultValue = 0.0f)
		{
			return PlayerPrefs.GetFloat(key, defaultValue);
		}

		public static int GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(key, defaultValue);
		}

		public static string GetString(string key, string defaultValue = "")
		{
            return PlayerPrefs.GetString(key, defaultValue);
		}

		// SETTER METHODS: ---------------------------------------------------------------------------------------------

		public static void SetFloat(string key, float value)
		{
			PlayerPrefs.SetFloat(key, value);
		}

		public static void SetInt(string key, int value)
		{
			PlayerPrefs.SetInt(key, value);
		}

		public static void SetString(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
		}

		// OTHER METHODS: ----------------------------------------------------------------------------------------------

		public static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}

		public static void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(key);
		}

		public static bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(key);
		}

		public static void Save()
		{
			PlayerPrefs.Save();
		}
	}
}