namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using GameCreator.Core;

    [CustomEditor(typeof(LocalVariables))]
    public class LocalVariablesEditor : GenericVariablesEditor<MBVariableEditor, MBVariable>
    {
        private const string PROP_UNIQUEID = "uniqueID";

        // PROPERTIES: ----------------------------------------------------------------------------

        private LocalVariables instance;
        private SerializedProperty spUniqueID;

		// INITIALIZERS: --------------------------------------------------------------------------

        protected override void OnEnable()
        {
            this.instance = (LocalVariables)this.target;
            base.OnEnable();

            this.spUniqueID = serializedObject.FindProperty(PROP_UNIQUEID);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override void OnInspectorGUI()
		{
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spUniqueID);

            serializedObject.ApplyModifiedProperties();
		}

		protected override MBVariable[] GetReferences()
        {
            return this.instance.references;
        }
   
        protected override string GetReferenceName(int index)
        {
            if (index < 0 || index >= this.instance.references.Length)
            {
                return "<i>Unbound Variable</i>";
            }

            if (this.instance.references[index] == null)
            {
                return "<i>Undefined Variable</i>";
            }

            return this.subEditors[index].GetName();
        }

        protected override Variable.DataType GetReferenceType(int index)
        {
            Variable variable = this.instance.references[index].variable;
            return (Variable.DataType)variable.type;
        }

        protected override bool MatchSearch(int index, string search, int tagsMask)
        {
            if (index >= this.subEditors.Length) return false;
            if (this.subEditors[index] == null) return false;

            return this.subEditors[index].MatchSearch(search, tagsMask);
        }

        protected override MBVariable CreateReferenceInstance(string name)
        {
            MBVariable variable = this.instance.gameObject.AddComponent<MBVariable>();
            variable.variable.name = name;
            return variable;
        }

		protected override void DeleteReferenceInstance(int index)
		{
            MBVariable source = (MBVariable)this.spReferences
                .GetArrayElementAtIndex(index)
                .objectReferenceValue;

            this.spReferences.RemoveFromObjectArrayAt(index);
            this.RemoveSubEditorsElement(index);
            DestroyImmediate(source, true);
		}

		protected override Tag[] GetReferenceTags(int index)
		{
            return new Tag[0];
		}

        // HIERARCHY CONTEXT MENU: ----------------------------------------------------------------

        [MenuItem("GameObject/Game Creator/Other/Local Variables", false, 0)]
        public static void CreateLocalVariables()
        {
            GameObject instance = CreateSceneObject.Create("Local Variables");
            instance.AddComponent<LocalVariables>();
        }
	}
}