namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public abstract class CreatePrefabObject
	{
		public static T AddGameObjectToPrefab<T>(GameObject prefabRoot, string name) where T : MonoBehaviour
		{
			GameObject instancePrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
			string instanceChildName = string.Format("{0}-{1}", name, GameCreatorUtilities.RandomHash(8));
			GameObject instanceChild = new GameObject(instanceChildName);
			instanceChild.AddComponent<T>();

			instanceChild.transform.SetParent(instancePrefab.transform);
			PrefabUtility.ReplacePrefab(instancePrefab, prefabRoot, ReplacePrefabOptions.Default);
			UnityEngine.Object.DestroyImmediate(instancePrefab);

			GameObject prefabChild = prefabRoot.transform.Find(instanceChildName).gameObject;
			prefabChild.name = name;
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(prefabRoot));

			return prefabChild.GetComponent<T>();
		}

		public static void RemoveGameObjectFromPrefab(GameObject prefabRoot, GameObject prefabChild)
		{
			string prefabChildName = string.Format("{0}-{1}", prefabChild.name, GameCreatorUtilities.RandomHash(8));
			prefabChild.name = prefabChildName;

			GameObject instancePrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot);
			foreach(Transform instanceChildEntry in instancePrefab.transform)
			{
				if (instanceChildEntry.gameObject.name == prefabChildName)
				{
					UnityEngine.Object.DestroyImmediate(instanceChildEntry.gameObject);
					break;
				}
			}

			PrefabUtility.ReplacePrefab(instancePrefab, prefabRoot, ReplacePrefabOptions.ConnectToPrefab);
			UnityEngine.Object.DestroyImmediate(instancePrefab);
		}
	}
}