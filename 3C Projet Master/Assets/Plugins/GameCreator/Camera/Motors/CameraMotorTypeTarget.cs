namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;
	using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeTarget : ICameraMotorType 
	{
		public static new string NAME = "Target Camera";

        // PROPERTIES: ----------------------------------------------------------------------------

        public float anchorDistance = 3.0f;
        public float horizontalOffset = 0.5f;

        public TargetTransform anchor = new TargetTransform();
        public Vector3 anchorOffset;

        public TargetPosition target = new TargetPosition(TargetPosition.Target.Invoker);

		// OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void UpdateMotor()
        {
            Transform anchorTransform = this.anchor.GetTransform(gameObject);
            if (anchorTransform == null) return;

            Vector3 anchorPosition = anchorTransform.position + this.anchorOffset;
            Vector3 targetPosition = this.target.GetPosition(gameObject);

            transform.position = anchorPosition;
            transform.LookAt(targetPosition);

            Vector3 forwardDir = (anchorPosition - targetPosition).normalized;
            Vector3 lateralDir = transform.right;

            transform.position += forwardDir * this.anchorDistance;
            transform.position += lateralDir * this.horizontalOffset;
        }

        public override Vector3 GetPosition(CameraController camera)
        {
            return transform.position;
        }

        public override Vector3 GetDirection(CameraController camera)
        {
            return this.target.GetDirection(gameObject);
        }
	}
}