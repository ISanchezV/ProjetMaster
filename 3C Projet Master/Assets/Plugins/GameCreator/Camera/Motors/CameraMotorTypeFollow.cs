namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
    using GameCreator.Core;
	using GameCreator.Core.Hooks;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeFollow : ICameraMotorType 
	{
		public static new string NAME = "Follow Camera";

        // PROPERTIES: ----------------------------------------------------------------------------

        public TargetTransform anchor = new TargetTransform();
        public Vector3 anchorOffset = new Vector3(0, 2, -3);
        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void UpdateMotor()
        {
            Transform target = this.anchor.GetTransform(gameObject);
            if (target == null) return;

            transform.position = target.position + this.anchorOffset;
            transform.LookAt(this.lookAt.GetPosition(gameObject));
        }

		public override Vector3 GetPosition (CameraController camera)
		{
            return transform.position;
		}

        public override Vector3 GetDirection(CameraController camera)
        {
            return this.lookAt.GetDirection(gameObject);
        }
	}
}