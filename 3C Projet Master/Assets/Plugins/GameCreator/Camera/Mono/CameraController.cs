﻿namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	[RequireComponent(typeof(HookCamera))]
	[AddComponentMenu("Game Creator/Camera/Camera Controller", 100)]
	public class CameraController : MonoBehaviour 
	{
		[System.Serializable]
		public class CameraSmoothTime
		{
			[Range(0.0f, 1.0f)] public float positionDuration = 0.1f;
			[Range(0.0f, 1.0f)] public float rotationDuration = 0.0f;

			private Vector3 positionVelocity = Vector3.zero;
			private Vector3 rotationVelocity = Vector3.zero;

			public CameraSmoothTime()
			{
				this.positionVelocity = Vector3.zero;
				this.rotationVelocity = Vector3.zero;
			}

			public Vector3 GetNextPosition(Vector3 currentPosition, Vector3 targetPosition)
			{
				return Vector3.SmoothDamp(
					currentPosition,
					targetPosition,
					ref this.positionVelocity,
					this.positionDuration
				);
			}

			public Vector3 GetNextDirection(Vector3 currentDirection, Vector3 targetDirection)
			{
                return Vector3.SmoothDamp(
					currentDirection, 
					targetDirection, 
					ref this.rotationVelocity, 
					this.rotationDuration
				);
			}
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		public CameraMotor currentCameraMotor;

		private float transitionTime = -1000.0f;
		private float transitionDuration = 0.0f;

        private Vector3 transitionPrevPosition = Vector3.zero;
        private Vector3 transitionPrevDirection = Vector3.forward;
		
        [HideInInspector] public CameraSmoothTime cameraSmoothTime = new CameraSmoothTime();

        private CameraShake sustainShake = null;
        private List<CameraShake> shakes = new List<CameraShake>();

		// INITIALIZE: ----------------------------------------------------------------------------

		private void Start()
		{
			if (this.currentCameraMotor != null) 
			{
                this.currentCameraMotor.cameraMotorType.EnableMotor();
			}

			this.transitionTime = Time.time;
			this.transitionDuration = 0.0f;
		}

		// UPDATE: --------------------------------------------------------------------------------

		private void LateUpdate()
		{
			if (this.currentCameraMotor != null)
			{
                this.UpdatePosition();
				this.UpdateRotation();
                this.UpdateShake();
			}
		}

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void UpdatePosition()
		{
            float elapsedTime = Mathf.Max(Time.time - this.transitionTime, 0.0f);
            float t = (this.transitionDuration <= 0.0f ? 1.0f : elapsedTime / this.transitionDuration);

            if (t >= 1.0f)
            {
                Vector3 targetPosition = this.currentCameraMotor.cameraMotorType.GetPosition(this);
                if (this.currentCameraMotor.cameraMotorType.UseSmoothPosition())
                {
                    targetPosition = this.cameraSmoothTime.GetNextPosition(
                        transform.position,
                        targetPosition
                    );
                }

                transform.position = targetPosition;
                this.transitionPrevPosition = targetPosition;
            }
            else
            {
                transform.position = Vector3.Slerp(
                    transitionPrevPosition, 
                    this.currentCameraMotor.cameraMotorType.GetPosition(this), 
                    t
                );
            }
		}

		private void UpdateRotation()
		{
            float elapsedTime = Mathf.Max(Time.time - this.transitionTime, 0.0f);
            if (Mathf.Approximately(elapsedTime, 0.0f)) return;

			float t = (this.transitionDuration <= 0.0f ? 1.0f : elapsedTime / this.transitionDuration);

            if (t >= 1.0f)
            {
                Vector3 targetDirection = this.currentCameraMotor.cameraMotorType.GetDirection(this);
                if (this.currentCameraMotor.cameraMotorType.UseSmoothRotation())
                {
                    targetDirection = this.cameraSmoothTime.GetNextDirection(
                        transform.TransformDirection(Vector3.forward),
                        targetDirection
                    );
                }

                transform.rotation = Quaternion.LookRotation(
                    targetDirection,
                    Vector3.up
                );

                this.transitionPrevDirection = transform.TransformDirection(Vector3.forward);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(Vector3.Slerp(
                    this.transitionPrevDirection, 
                    this.currentCameraMotor.cameraMotorType.GetDirection(this), 
                    t
                ));
            }
		}

        private void UpdateShake()
        {
            Vector3 additivePosition = Vector3.zero;
            Vector3 additiveRotation = Vector3.zero;

            if (this.sustainShake != null)
            {
                Vector3 amount = this.sustainShake.Update();
                additivePosition += Vector3.Scale(amount, this.sustainShake.GetInfluencePosition());
                additiveRotation += Vector3.Scale(amount, this.sustainShake.GetInfluenceRotation());
            }

            for (int i = this.shakes.Count - 1; i >= 0; i--)
            {
                Vector3 amount = this.shakes[i].Update() * this.shakes[i].GetEasing();
                additivePosition += Vector3.Scale(amount, this.shakes[i].GetInfluencePosition());
                additiveRotation += Vector3.Scale(amount, this.shakes[i].GetInfluenceRotation());

                if (this.shakes[i].IsComplete()) this.shakes.RemoveAt(i);
            }

            transform.localPosition += additivePosition * CameraShake.COEF_SHAKE_POSITION;
            transform.localEulerAngles += additiveRotation * CameraShake.COEF_SHAKE_ROTATION;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public void ChangeCameraMotor(CameraMotor cameraMotor, float transitionDuration = 0.0f)
		{
			if (this.currentCameraMotor != null && cameraMotor != null && 
				this.currentCameraMotor.GetInstanceID() == cameraMotor.GetInstanceID())
			{
				return;
			}

            if (this.currentCameraMotor != null) this.currentCameraMotor.cameraMotorType.DisableMotor();
			this.currentCameraMotor = cameraMotor;
            if (this.currentCameraMotor != null) this.currentCameraMotor.cameraMotorType.EnableMotor();

			if (Mathf.Approximately(transitionDuration, 0.0f))
			{
				transform.position = this.currentCameraMotor.cameraMotorType.GetPosition(this);
				Vector3 deltaDirection = this.currentCameraMotor.cameraMotorType.GetDirection(this);
				transform.rotation = Quaternion.LookRotation(deltaDirection, Vector3.up);
			}

			this.transitionTime = Time.time;
			this.transitionDuration = transitionDuration;
		}

        public void AddShake(CameraShake cameraShake)
        {
            this.shakes.Add(cameraShake);
        }

        public void SetSustainShake(CameraShake cameraShake)
        {
            this.sustainShake = cameraShake;
        }

        public void StopSustainShake(float fadeTime = 1.0f)
        {
            this.shakes.Add(new CameraShake(fadeTime, this.sustainShake));
            this.sustainShake = null;
        }
	}
}