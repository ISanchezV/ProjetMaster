namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[AddComponentMenu("")]
    public class ActionCameraShakeBurst : IAction 
	{
        public float duration = 1.0f;
        [Range(0.0f, 30.0f)] public float roughness = 10.0f;
        [Range(0.0f, 10.0f)] public float magnitude = 0.5f;

        public bool shakePosition = true;
        public bool shakeRotation = true;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override bool InstantExecute(GameObject target, IAction[] actions, int index)
        {
            if (HookCamera.Instance != null)
            {
                CameraController cameraController = HookCamera.Instance.Get<CameraController>();
                if (cameraController != null)
                {
                    cameraController.AddShake(new CameraShake(
                        this.duration,
                        this.roughness,
                        this.magnitude,
                        this.shakePosition,
                        this.shakeRotation
                    ));
                }
            }

            return true;
        }

		// +--------------------------------------------------------------------------------------+
		// | EDITOR                                                                               |
		// +--------------------------------------------------------------------------------------+

		#if UNITY_EDITOR

		public static new string NAME = "Camera/Camera Shake Burst";
		private const string NODE_TITLE = "Burst shake Camera";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spDuration;
		private SerializedProperty spRoughness;
		private SerializedProperty spMagnitude;
        private SerializedProperty spShakePosition;
        private SerializedProperty spShakeRotation;

		// INSPECTOR METHODS: ---------------------------------------------------------------------

		public override string GetNodeTitle()
		{
            return NODE_TITLE;
		}

		protected override void OnEnableEditorChild ()
		{
            this.spDuration = this.serializedObject.FindProperty("duration");
            this.spRoughness = this.serializedObject.FindProperty("roughness");
            this.spMagnitude = this.serializedObject.FindProperty("magnitude");
            this.spShakePosition = this.serializedObject.FindProperty("shakePosition");
            this.spShakeRotation = this.serializedObject.FindProperty("shakeRotation");
		}

		protected override void OnDisableEditorChild ()
		{
            this.spDuration = null;
            this.spRoughness = null;
            this.spMagnitude = null;
            this.spShakePosition = null;
            this.spShakeRotation = null;
		}

		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spRoughness);
            EditorGUILayout.PropertyField(this.spMagnitude);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(this.spShakePosition);
            EditorGUILayout.PropertyField(this.spShakeRotation);

			this.serializedObject.ApplyModifiedProperties();
		}

		#endif
	}
}