namespace GameCreator.Camera
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
	using GameCreator.Characters;

    [AddComponentMenu("")]
	[System.Serializable]
	public class CameraMotorTypeFirstPerson : ICameraMotorType 
	{
        public enum RotateInput
        {
            MouseMove,
            HoldLeftMouse,
            HoldRightMouse,
            HoldMiddleMouse
        }

        private const string INPUT_MOUSE_X = "Mouse X";
        private const string INPUT_MOUSE_Y = "Mouse Y";
        private const string INPUT_MOUSE_W = "Mouse ScrollWheel";

		public static new string NAME = "First Person Camera";

		// PROPERTIES: -------------------------------------------------------------------------------------------------

		public Vector3 positionOffset = new Vector3(0f, 2f, 0f);

		private float rotationX = 0.0f;
		private float rotationY = 0.0f;
		private float targetRotationX = 0.0f;
		private float targetRotationY = 0.0f;

		private float rotationXVelocity = 0.0f;
		private float rotationYVelocity = 0.0f;
		private PlayerCharacter player;

        public RotateInput rotateInput = RotateInput.MouseMove;
		public Vector2 sensitivity = new Vector2(2f, 2f);

		[Range(0.0f, 180f)] 
		public float maxPitch = 120f;

		[Range(0f, 1f)]
		public float smoothRotation = 0.1f;

        public float headbobPeriod = 0.5f;
        public Vector3 headbobAmount = new Vector3(0.05f, 0.05f, 0.01f);

		// OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void EnableMotor()
        {
            Transform target = this.GetTarget();
            this.rotationX = 0.0f;
            this.rotationY = 0.0f;

            if (target != null)
            {
                this.rotationX = target.transform.rotation.eulerAngles.y;
                this.rotationY = target.transform.rotation.eulerAngles.x;
            }

            this.targetRotationX = this.rotationX;
            this.targetRotationY = this.rotationY;
        }

		public override Vector3 GetPosition (CameraController camera)
		{
			Transform target = this.GetTarget();
			if (target == null) return base.GetPosition(camera);

            Vector3 position = target.position + target.TransformDirection(this.positionOffset);
            return position + target.TransformDirection(this.Headbob());
		}

		public override Vector3 GetDirection (CameraController camera)
		{
			if (HookPlayer.Instance != null)
			{
				if (this.player == null) this.player = HookPlayer.Instance.Get<PlayerCharacter>();
				if (!this.player.IsControllable()) return camera.transform.TransformDirection(Vector3.forward);
			}

            bool inputConditions = false;
            if (this.rotateInput == RotateInput.MouseMove) inputConditions = true;
            else if (this.rotateInput == RotateInput.HoldLeftMouse && Input.GetMouseButton(0)) inputConditions = true;
            else if (this.rotateInput == RotateInput.HoldRightMouse && Input.GetMouseButton(1)) inputConditions = true;
            else if (this.rotateInput == RotateInput.HoldMiddleMouse && Input.GetMouseButton(2)) inputConditions = true;
            else inputConditions = false;

            if (inputConditions)
            {
                this.targetRotationX += Input.GetAxis(INPUT_MOUSE_X) * this.sensitivity.x;
                this.targetRotationY += Input.GetAxis(INPUT_MOUSE_Y) * this.sensitivity.y;
            }

			this.targetRotationX %= 360f;
			this.targetRotationY %= 360f;

			this.targetRotationY = Mathf.Clamp(
				this.targetRotationY, 
				-this.maxPitch/2.0f,
				this.maxPitch/2.0f
			);

			this.rotationX = Mathf.SmoothDampAngle(
				this.rotationX, this.targetRotationX, 
				ref this.rotationXVelocity, this.smoothRotation
			);

			this.rotationY = Mathf.SmoothDampAngle(
				this.rotationY, this.targetRotationY, 
				ref this.rotationYVelocity, this.smoothRotation
			);

            if (float.IsNaN(this.rotationX)) this.rotationX = this.targetRotationX;
            if (float.IsNaN(this.rotationY)) this.rotationY = this.targetRotationY;

			Quaternion quaternionX = Quaternion.AngleAxis(this.rotationX, Vector3.up);
			Quaternion quaternionY = Quaternion.AngleAxis(this.rotationY, Vector3.left);

			return (quaternionX * quaternionY) * Vector3.forward;
		}

        public override bool UseSmoothRotation()
        {
            return false;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Transform GetTarget()
		{
			if (HookPlayer.Instance != null) return HookPlayer.Instance.transform;
			return null;
		}

		private static float ClampAngle (float angle, float min, float max)
		{
            return Mathf.Clamp(angle % 360f, min, max);
		}

        private Vector3 Headbob()
        {
            if (HookPlayer.Instance == null) return Vector3.zero;
            PlayerCharacter playerCharacter = HookPlayer.Instance.Get<PlayerCharacter>();
            if (playerCharacter == null) return Vector3.zero;

            Character.State state = playerCharacter.GetCharacterState();
            if (!state.isGrounded) return Vector3.zero;

            float speed = Mathf.Abs(state.forwardSpeed.magnitude);

            return new Vector3(
                Mathf.Sin((speed / this.headbobPeriod * 2.0f) * Time.time) * this.headbobAmount.x,
                Mathf.Sin((speed / this.headbobPeriod * 1.0f) * Time.time) * this.headbobAmount.y,
                Mathf.Sin((speed / this.headbobPeriod * 1.0f) * Time.time) * this.headbobAmount.z
            );
        }
	}
}