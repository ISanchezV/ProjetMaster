namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Core;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeFixed : ICameraMotorType 
	{
		public static new string NAME = "Fixed Camera";

        public TargetPosition lookAt = new TargetPosition(TargetPosition.Target.Invoker);

		public override Vector3 GetDirection(CameraController camera)
		{
            return this.lookAt.GetDirection(gameObject);
		}
	}
}