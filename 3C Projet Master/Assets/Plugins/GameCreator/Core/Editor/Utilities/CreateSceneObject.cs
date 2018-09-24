namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	public abstract class CreateSceneObject
	{
        private const string NAME_FMT = "{0} ({1})";

        // PUBLIC METHODS: ------------------------------------------------------------------------

		public static GameObject Create(string name, bool autoSelect = true)
		{
            if (GameObject.Find(name) != null)
            {
                int index = 1;
                string uniqueName = "";

                do 
                {
                    uniqueName = string.Format(NAME_FMT, name, index);
                    ++index;
                } while (GameObject.Find(uniqueName) != null);
                name = uniqueName;
            }

			GameObject newGameObject = new GameObject(name);
            return CreateSceneObject.Create(newGameObject, autoSelect);
		}

		public static GameObject Create(GameObject newGameObject, bool autoSelect = true)
		{
            if (Selection.activeTransform != null && Selection.activeGameObject != newGameObject)
            {
                newGameObject.transform.SetParent(Selection.activeTransform);
                newGameObject.transform.localPosition = Vector3.zero;
            }
            else if (SceneView.currentDrawingSceneView != null)
            {
                Transform camTrans = SceneView.currentDrawingSceneView.camera.transform;
                Ray ray = new Ray(camTrans.position, camTrans.TransformDirection(Vector3.forward));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    newGameObject.transform.position = hit.point;
                }
            }

			if (autoSelect) Selection.activeGameObject = newGameObject;
			return newGameObject;
		}
	}
}