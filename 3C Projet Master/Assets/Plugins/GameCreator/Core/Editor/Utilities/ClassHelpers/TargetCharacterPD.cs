namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomPropertyDrawer(typeof(TargetCharacter))]
    public class TargetCharacterPD : TargetGenericPD
	{
        protected override SerializedProperty GetProperty(int option, SerializedProperty property)
        {
            if (option == (int)TargetCharacter.Target.Character)
            {
                return property.FindPropertyRelative("character");
            }

            return null;
        }
	}
}