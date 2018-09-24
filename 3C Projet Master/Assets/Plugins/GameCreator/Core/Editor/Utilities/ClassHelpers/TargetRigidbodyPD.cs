namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomPropertyDrawer(typeof(TargetRigidbody))]
    public class TargetRigidbodyPD : TargetGenericPD
	{
        public const string PROP_RIGIDBODY = "rigidbody";

        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            if (option == (int)TargetRigidbody.Target.Rigidbody)
            {
                return property.FindPropertyRelative(PROP_RIGIDBODY);
            }

            return null;
        }
	}
}