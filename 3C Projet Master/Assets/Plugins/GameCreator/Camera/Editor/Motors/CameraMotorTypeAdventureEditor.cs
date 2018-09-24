namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

    [CustomEditor(typeof(CameraMotorTypeAdventure))]
	public class CameraMotorTypeAdventureEditor : ICameraMotorTypeEditor 
	{
        private static readonly Color HANDLE_COLOR_BACKG = new Color(256f, 256f, 256f, 0.2f);
        private static readonly Color HANDLE_COLOR_PITCH_P = new Color(256f, 0f, 0f, 0.2f);
        private static readonly Color HANDLE_COLOR_PITCH_C = new Color(256f, 0f, 0f, 0.8f);

        private const string PROP_CAN_ZOOM = "allowZoom";
        private const string PROP_INITIAL_ZOOM = "initialZoom";
        private const string PROP_ZOOM_SENSITIVITY = "zoomSensitivity";
        private const string PROP_ZOOM_LIMITS = "zoomLimits";

        private const string PROP_TARGET = "target";
        private const string PROP_TARGET_OFFSET = "targetOffset";
        private const string PROP_ALLOW_ORBIT = "allowOrbitInput";
        private const string PROP_ORBIT_INPUT = "orbitInput";
        private const string PROP_SENSITIVITY = "sensitivity";
        private const string PROP_MAX_PITCH = "maxPitch";
        private const string PROP_AVOIDWALLCLIP = "avoidWallClip";
        private const string PROP_WALLCLIPLAYER = "wallClipLayerMask";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

        private SerializedProperty spAllowZoom;
        private SerializedProperty spInitialZoom;
        private SerializedProperty spZoomSensitivity;
        private SerializedProperty spZoomLimits;

        private SerializedProperty spTarget;
        private SerializedProperty spTargetOffset;

        private SerializedProperty spAllowOrbit;
        private SerializedProperty spOrbitInput;
        private SerializedProperty spSensitivity;
        private SerializedProperty spMaxPitch;
        private SerializedProperty spAvoidWallClip;
        private SerializedProperty spWallClipLayerMask;

		// INITIALIZE: -------------------------------------------------------------------------------------------------

		protected override void OnSubEnable()
		{
            this.spAllowZoom = serializedObject.FindProperty(PROP_CAN_ZOOM);
            this.spInitialZoom = serializedObject.FindProperty(PROP_INITIAL_ZOOM);
            this.spZoomSensitivity = serializedObject.FindProperty(PROP_ZOOM_SENSITIVITY);
            this.spZoomLimits = serializedObject.FindProperty(PROP_ZOOM_LIMITS);

            this.spTarget = serializedObject.FindProperty(PROP_TARGET);
            this.spTargetOffset = serializedObject.FindProperty(PROP_TARGET_OFFSET);
            this.spAllowOrbit = serializedObject.FindProperty(PROP_ALLOW_ORBIT);
            this.spOrbitInput = serializedObject.FindProperty(PROP_ORBIT_INPUT);
            this.spSensitivity = serializedObject.FindProperty(PROP_SENSITIVITY);
            this.spMaxPitch = serializedObject.FindProperty(PROP_MAX_PITCH);
            this.spAvoidWallClip = serializedObject.FindProperty(PROP_AVOIDWALLCLIP);
            this.spWallClipLayerMask = serializedObject.FindProperty(PROP_WALLCLIPLAYER);
		}

		// INSPECTOR GUI: ----------------------------------------------------------------------------------------------

		protected override bool OnSubInspectorGUI (Transform cameraMotorTransform)
		{
			serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);
            EditorGUILayout.PropertyField(this.spTargetOffset);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spAllowOrbit);

            EditorGUI.BeginDisabledGroup(!this.spAllowOrbit.boolValue);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(this.spOrbitInput);
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spSensitivity);
            EditorGUILayout.PropertyField(this.spMaxPitch);

			EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spAllowZoom);
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(!this.spAllowZoom.boolValue);
            EditorGUILayout.PropertyField(this.spInitialZoom);
            EditorGUILayout.PropertyField(this.spZoomSensitivity);
            EditorGUILayout.PropertyField(this.spZoomLimits);

            float limitX = this.spZoomLimits.vector2Value.x;
            float limitY = this.spZoomLimits.vector2Value.y;
            EditorGUILayout.MinMaxSlider("Zoom Limits", ref limitX, ref limitY, 0.0f, 20f);
            this.spZoomLimits.vector2Value = new Vector2(limitX, limitY);

			EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spAvoidWallClip);
            EditorGUILayout.PropertyField(this.spWallClipLayerMask);

			serializedObject.ApplyModifiedProperties();
			return false;
		}

        // SCENE GUI: --------------------------------------------------------------------------------------------------

        public override bool OnSubSceneGUI(Transform cameraMotorTransform)
        {
            if (Application.isPlaying) return false;
            serializedObject.Update();

            Handles.color = HANDLE_COLOR_BACKG;
            Handles.DrawSolidArc(
                cameraMotorTransform.position,
                cameraMotorTransform.TransformDirection(Vector3.right),
                cameraMotorTransform.TransformDirection(Vector3.up),
                180f,
                HandleUtility.GetHandleSize(cameraMotorTransform.position)
            );

            float angle = this.spMaxPitch.floatValue;
            Vector3 direction = Quaternion.AngleAxis(-angle / 2.0f, Vector3.right) * Vector3.forward;

            Handles.color = HANDLE_COLOR_PITCH_P;
            Handles.DrawSolidArc(
                cameraMotorTransform.position,
                cameraMotorTransform.TransformDirection(Vector3.right),
                cameraMotorTransform.TransformDirection(direction),
                angle,
                HandleUtility.GetHandleSize(cameraMotorTransform.position)
            );

            Handles.color = HANDLE_COLOR_PITCH_C;
            Handles.DrawWireArc(
                cameraMotorTransform.position,
                cameraMotorTransform.TransformDirection(Vector3.right),
                cameraMotorTransform.TransformDirection(direction),
                angle,
                HandleUtility.GetHandleSize(cameraMotorTransform.position)
            );

            serializedObject.ApplyModifiedProperties();
            return false;
        }

        public override bool ShowPreviewCamera()
        {
            return false;
        }
	}
}