namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomPropertyDrawer(typeof(TargetGameObject))]
    public class TargetGameObjectPD : TargetGenericPD
	{
        public const string PROP_GAMEOBJECT = "gameObject";

        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            if (option == (int)TargetGameObject.Target.GameObject)
            {
                return property.FindPropertyRelative(PROP_GAMEOBJECT);
            }

            return null;
        }
	}
}