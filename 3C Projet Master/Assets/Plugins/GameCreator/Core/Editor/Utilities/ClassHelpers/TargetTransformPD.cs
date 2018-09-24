namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(TargetTransform))]
    public class TargetTransformPD : TargetGenericPD
	{
        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            if (option == (int)TargetTransform.Target.Transform)
            {
                return property.FindPropertyRelative("transform");
            }

            return null;
        }
	}
}