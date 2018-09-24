namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Core;

    [AddComponentMenu("")]
	[System.Serializable]
    public class ICameraMotorType : MonoBehaviour
	{
		public static string NAME = "ICameraMotor";

		// PROPERTIES: ----------------------------------------------------------------------------

		protected CameraMotor cameraMotor;

        // INITIALIZE: ----------------------------------------------------------------------------

        public void Initialize(CameraMotor cameraMotor)
		{
			this.cameraMotor = cameraMotor;
		}

        private void Update()
        {
            this.UpdateMotor();
        }

        // ABSTRACT AND VIRTUAL METHODS: ----------------------------------------------------------

        public virtual void EnableMotor()  { return; }
        public virtual void DisableMotor() { return; }
		public virtual void UpdateMotor()  { return; }

		public virtual Vector3 GetPosition(CameraController camera) 
		{ 
			return this.cameraMotor.transform.position; 
		}

		public virtual Vector3 GetDirection(CameraController camera) 
		{
            return this.transform.TransformDirection(Vector3.forward);
		}

        public virtual bool UseSmoothPosition()
        {
            return true;
        }

        public virtual bool UseSmoothRotation()
        {
            return true;
        }
	}
}