namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
	public class ActionCharacterMoveTo : IAction 
	{
		public enum MOVE_TO
		{
			Position,
			Transform,
			Marker
		}

        public TargetCharacter target = new TargetCharacter();

		public MOVE_TO moveTo = MOVE_TO.Position;
		public bool waitUntilArrives = true;

		public Vector3 position;
		public new Transform transform;
		public NavigationMarker marker;

        public bool cancelable = false;
        public float cancelDelay = 1.0f;

		private bool isCharacterMoving = false;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (this.waitUntilArrives) return false;
            Character charTarget = this.target.GetCharacter(target);

            Vector3 cPosition = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            this.GetTarget(ref cPosition, ref cRotation);

            charTarget.characterLocomotion.SetTarget(cPosition, cRotation);
            return true;
        }

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
		{
            Character charTarget = this.target.GetCharacter(target);

            Vector3 cPosition  = Vector3.zero;
            ILocomotionSystem.TargetRotation cRotation = null;
            this.GetTarget(ref cPosition, ref cRotation);

            this.isCharacterMoving = true;
            bool previousIsControllable = charTarget.characterLocomotion.isControllable;
            charTarget.characterLocomotion.SetIsControllable(false);

            charTarget.characterLocomotion.SetTarget(cPosition, cRotation, this.CharacterArrivedCallback);

            bool canceled = false;
            float initTime = Time.time;

            while (this.isCharacterMoving && !canceled)
            {
                if (this.cancelable && (Time.time - initTime) >= this.cancelDelay)
                {
                    canceled = Input.anyKey;
                }

                yield return null;
            }

            charTarget.characterLocomotion.SetIsControllable(previousIsControllable);

            if (canceled) yield return int.MaxValue;
			else yield return 0;
		}

		public void CharacterArrivedCallback()
		{
			this.isCharacterMoving = false;
		}

        private void GetTarget(ref Vector3 cPosition, ref ILocomotionSystem.TargetRotation cRotation)
        {
            switch (this.moveTo)
            {
                case MOVE_TO.Position: cPosition = this.position; break;
                case MOVE_TO.Transform: cPosition = this.transform.position; break;
                case MOVE_TO.Marker:
                    cPosition = this.marker.transform.position;
                    cRotation = new ILocomotionSystem.TargetRotation(true, this.marker.transform.forward);
                    break;
            }
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Character/Move Character";
		private const string NODE_TITLE = "Move {0} to {1} {2}";

        private static readonly GUIContent GC_CANCEL = new GUIContent("Cancelable");

		// PROPERTIES: ----------------------------------------------------------------------------

		private SerializedProperty spTarget;
		private SerializedProperty spMoveTo;
		private SerializedProperty spWaitUntilArrives;
		private SerializedProperty spPosition;
		private SerializedProperty spTransform;
		private SerializedProperty spMarker;

        private SerializedProperty spCancelable;
        private SerializedProperty spCancelDelay;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
			string value = "none";
			switch (this.moveTo)
			{
			case MOVE_TO.Position : 
				value = string.Format("({0},{1},{2})", this.position.x, this.position.y, this.position.z); 
				break;

			case MOVE_TO.Transform :
				value = (this.transform == null ? "nothing" : this.transform.gameObject.name);
				break;
			case MOVE_TO.Marker :
				value = (this.marker == null ? "nothing" : this.marker.gameObject.name);
				break;
			}

			return string.Format(
				NODE_TITLE, 
                this.target.ToString(),
				this.moveTo, 
				value
			);
		}

		protected override void OnEnableEditorChild ()
		{
			this.spTarget = this.serializedObject.FindProperty("target");
			this.spMoveTo = this.serializedObject.FindProperty("moveTo");
			this.spWaitUntilArrives = this.serializedObject.FindProperty("waitUntilArrives");
			this.spPosition = this.serializedObject.FindProperty("position");
			this.spTransform = this.serializedObject.FindProperty("transform");
			this.spMarker = this.serializedObject.FindProperty("marker");
            this.spCancelable = this.serializedObject.FindProperty("cancelable");
            this.spCancelDelay = this.serializedObject.FindProperty("cancelDelay");
		}

		protected override void OnDisableEditorChild ()
		{
			return;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spMoveTo);

			switch ((MOVE_TO)this.spMoveTo.intValue)
			{
			case MOVE_TO.Position  : EditorGUILayout.PropertyField(this.spPosition);  break;
			case MOVE_TO.Transform : EditorGUILayout.PropertyField(this.spTransform); break;
			case MOVE_TO.Marker    : EditorGUILayout.PropertyField(this.spMarker);    break;
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.spWaitUntilArrives);
            if (this.spWaitUntilArrives.boolValue)
            {
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.textField);
                Rect rectLabel = new Rect(
                    rect.x,
                    rect.y,
                    EditorGUIUtility.labelWidth,
                    rect.height
                );
                Rect rectCancenable = new Rect(
                    rectLabel.x + rectLabel.width,
                    rectLabel.y,
                    20f,
                    rectLabel.height
                );
                Rect rectDelay = new Rect(
                    rectCancenable.x + rectCancenable.width,
                    rectCancenable.y,
                    rect.width - (rectLabel.width + rectCancenable.width),
                    rectCancenable.height
                );

                EditorGUI.LabelField(rectLabel, GC_CANCEL);
                EditorGUI.PropertyField(rectCancenable, this.spCancelable, GUIContent.none);

                EditorGUI.BeginDisabledGroup(!this.spCancelable.boolValue);
                EditorGUI.PropertyField(rectDelay, this.spCancelDelay, GUIContent.none);
                EditorGUI.EndDisabledGroup();
            }

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}