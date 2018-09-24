namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AddComponentMenu("Game Creator/Camera/Camera Motor", 100)]
	public class CameraMotor : MonoBehaviour 
	{
        public static CameraMotor MAIN_MOTOR;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool isMainCameraMotor = false;
		public ICameraMotorType cameraMotorType;

		// INITIALIZE: ----------------------------------------------------------------------------

		private void Awake()
		{
            if (this.isMainCameraMotor) MAIN_MOTOR = this;

            this.cameraMotorType.Initialize(this);
            Camera attachedCamera = GetComponent<Camera>();
            if (attachedCamera != null) attachedCamera.enabled = false;
		}

		// GIZMOS: --------------------------------------------------------------------------------

		private void OnDrawGizmos()
		{
            Gizmos.DrawIcon(transform.position, "GameCreator/Camera/motor", true);
		}
	}
}