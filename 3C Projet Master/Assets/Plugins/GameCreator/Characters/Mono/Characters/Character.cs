namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.AI;
	using GameCreator.Core;

	[RequireComponent(typeof(CharacterController))]
	[AddComponentMenu("Game Creator/Characters/Character", 100)]
	public class Character : MonoBehaviour
	{
		[System.Serializable]
		public class State
		{
			public Vector3 forwardSpeed;
			public float turnSpeed;
            public bool targetLock;
			public bool isGrounded;
			public float verticalSpeed;
			public Vector3 normal;

			public State()
			{
				this.forwardSpeed = Vector3.zero;
				this.turnSpeed = 0f;
                this.targetLock = false;
				this.isGrounded = false;
				this.verticalSpeed = 0f;
				this.normal = Vector3.zero;
			}
		}

		protected const string ERR_NOCAM = "No Camera found. Tag the camera as 'MainCamera' or add the CameraController component";

		private const float JUMP_ONAIR_TIME_OFFSET = 0.2f;

		// PROPERTIES: ----------------------------------------------------------------------------

		public CharacterLocomotion characterLocomotion;
		public Character.State characterState = new State();
		private CharacterAnimator characterAnimator;

        public UnityEvent onJump = new UnityEvent();

		// INITIALIZERS: --------------------------------------------------------------------------

		private void Awake()
		{
			this.CharacterAwake();
		}

		protected void CharacterAwake()
		{
			this.characterAnimator = GetComponent<CharacterAnimator>();
			this.characterLocomotion.Setup(this);
		}

		// UPDATE: --------------------------------------------------------------------------------

		private void Update()
		{
			this.CharacterUpdate();
		}

		protected void CharacterUpdate()
		{
			this.characterLocomotion.Update();
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public State GetCharacterState()
		{
			return this.characterState;
		}

		// GETTERS: -------------------------------------------------------------------------------

		public bool IsControllable()
		{
			if (this.characterLocomotion == null) return false;
			return this.characterLocomotion.isControllable;
		}

        public int GetCharacterMotion()
        {
            if (this.characterState == null) return 0;
            if (this.characterLocomotion == null) return 0;

            float speed = Mathf.Abs(this.characterState.forwardSpeed.magnitude);
            if (Mathf.Approximately(speed, 0.0f)) return 0;
            else if (this.characterLocomotion.canRun && speed > this.characterLocomotion.walkSpeed)
            {
                return 2;
            }

            return 1;
        }

        public bool IsGrounded()
        {
            if (this.characterState == null) return true;
            return this.characterState.isGrounded;
        }

        public CharacterAnimator GetCharacterAnimator()
        {
            return this.characterAnimator;
        }

		// JUMP: ----------------------------------------------------------------------------------

		public void Jump(float force)
		{
			if (this.characterLocomotion.Jump(force) && this.characterAnimator != null) 
			{
				this.characterAnimator.Jump();
			}
		}

		public void Jump()
		{
			if (this.characterLocomotion.Jump() && this.characterAnimator != null) 
			{
				this.characterAnimator.Jump();
			}
		}

		// HEAD TRACKER: --------------------------------------------------------------------------

		public CharacterHeadTrack GetHeadTracker()
		{
			if (this.characterAnimator == null) return null;
			return this.characterAnimator.GetHeadTracker();
		}

		// FLOOR COLLISION: -----------------------------------------------------------------------

		private void OnControllerColliderHit(ControllerColliderHit hit) 
		{
			Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
			if (hitRigidbody != null && !hitRigidbody.isKinematic)
			{
				Vector3 force = hit.controller.velocity * hitRigidbody.mass / Time.fixedDeltaTime;
				hitRigidbody.AddForceAtPosition(force, hit.point, ForceMode.Force);
			}

			float characterRadius = this.characterLocomotion.characterController.radius;
			if (Vector3.Distance(transform.position, hit.point) > characterRadius) return;
			if (hit.normal.y < 0.5f) return;
			
			this.characterLocomotion.terrainNormal = hit.normal;
		}      
	}
}