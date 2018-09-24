namespace GameCreator.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core.Hooks;
    using GameCreator.Variables;

    #if UNITY_EDITOR
    using UnityEditor;
    #endif

    [AddComponentMenu("")]
    public class ActionTransformMove : IAction
    {
        public TargetTransform target = new TargetTransform(TargetTransform.Target.Player);

        public TargetPosition moveTo = new TargetPosition();
        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);

        public NumberProperty duration = new NumberProperty(1.0f);
        public Easing.EaseType easing = Easing.EaseType.QuadInOut;

        // EXECUTABLE: ----------------------------------------------------------------------------

        public override IEnumerator Execute(GameObject target, IAction[] actions, int index)
        {
            Transform targetTrans = this.target.GetTransform(target);
            if (targetTrans != null)
            {
                Vector3 position1 = targetTrans.position;
                Vector3 position2 = this.moveTo.GetPosition(target);

                Quaternion rotation1 = targetTrans.rotation;
                Quaternion rotation2 = Quaternion.LookRotation(this.lookAt.GetDirection(target));

                float vDuration = this.duration.GetValue();
                float initTime = Time.time;

                while (Time.time - initTime < vDuration)
                {
                    if (targetTrans == null) break;
                    float t = (Time.time - initTime)/vDuration;
                    float easeValue = Easing.GetEase(this.easing, 0.0f, 1.0f, t);

                    targetTrans.position = Vector3.Lerp(
                        position1,
                        position2,
                        easeValue
                    );

                    targetTrans.rotation = Quaternion.Lerp(
                        rotation1,
                        rotation2,
                        easeValue
                    );

                    yield return null;
                }
            }

            yield return 0;
        }

        // +--------------------------------------------------------------------------------------+
        // | EDITOR                                                                               |
        // +--------------------------------------------------------------------------------------+

        #if UNITY_EDITOR

        public static new string NAME = "Object/Transform Move";
        private const string NODE_TITLE = "Move {0}";

        // PROPERTIES: ----------------------------------------------------------------------------

        private SerializedProperty spTarget;
        private SerializedProperty spMoveTo;
        private SerializedProperty spLookAt;
        private SerializedProperty spDuration;
        private SerializedProperty spEasing;

        // INSPECTOR METHODS: ---------------------------------------------------------------------

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, this.target.ToString());
        }

        protected override void OnEnableEditorChild()
        {
            this.spTarget = serializedObject.FindProperty("target");
            this.spMoveTo = serializedObject.FindProperty("moveTo");
            this.spLookAt = serializedObject.FindProperty("lookAt");
            this.spDuration = serializedObject.FindProperty("duration");
            this.spEasing = serializedObject.FindProperty("easing");
        }

        protected override void OnDisableEditorChild()
        {
            this.spTarget = null;
            this.spMoveTo = null;
            this.spLookAt = null;
            this.spDuration = null;
            this.spEasing = null;
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.PropertyField(this.spTarget);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spMoveTo);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spLookAt);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.spDuration);
            EditorGUILayout.PropertyField(this.spEasing);

            this.serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}