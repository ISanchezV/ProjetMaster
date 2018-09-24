namespace GameCreator.Core
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[System.Serializable]
	public class ScriptableObjectsContainer : ScriptableObject
	{
		// +-----------------------------------------------------------------------------------------------------------+
		// | EDITOR METHODS:                                                                                           |
		// +-----------------------------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public static bool ProjectContains(ScriptableObject instance)
		{
			return !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(instance.GetInstanceID()));
		}

		public static ScriptableObject AddToProject(ScriptableObject instance, UnityEngine.Object referencer)
		{
			string[] typeNamespaces = instance.GetType().ToString().Split(
				new char[]{'.'}, 
				StringSplitOptions.RemoveEmptyEntries
			);
				
			instance.name = string.Format(
				"{0}-{1}", 
				typeNamespaces[typeNamespaces.Length - 1],
				Guid.NewGuid().ToString("N")
			);

			string referencerPath = AssetDatabase.GetAssetPath(referencer);
			AssetDatabase.AddObjectToAsset(instance, AssetDatabase.GetAssetPath(referencer));
			AssetDatabase.ImportAsset(referencerPath);

			return instance;
		}

		public static void RemoveFromProject(ScriptableObject instance)
		{
			UnityEngine.Object root = null;
			if (PrefabUtility.GetPrefabType(instance) != PrefabType.None) root = PrefabUtility.GetCorrespondingObjectFromSource(instance);
			else root = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GetAssetPath(instance));

			ScriptableObject.DestroyImmediate(instance, true);
			if (root != null) AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(root));
		}

		#endif
	}
}