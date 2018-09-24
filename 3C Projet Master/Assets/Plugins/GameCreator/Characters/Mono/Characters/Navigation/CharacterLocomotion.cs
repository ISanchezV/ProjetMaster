namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;
	using UnityEngine.Events;
    using GameCreator.Core;

	[System.Serializable]
	public class CharacterLocomotion
	{
		public enum ANIM_CONSTRAINT
		{
			NONE,
			KEEP_MOVEMENT,
			KEEP_POSITION
		}

		public enum LOCOMOTION_SYSTEM
		{
			CharacterController,
			NavigationMeshAgent
		}

        public enum FACE_DIRECTION
        {
            MovementDirection,
            CameraDirection,
            Target,
            GroundPlaneCursor
        }

		// CONSTANTS: -----------------------------------------------------------------------------

		private const float GROUND_CHECK_DISTANCE = 0.1f;
		private const float JUMP_ONAIR_TIME_OFFSET = 0.2f;
		private const float MIN_TIME_BETWEEN_JUMPS = 0.2f;

		private const float ACCELERATION = 25f;

		// PROPERTIES: ----------------------------------------------------------------------------

		public bool isControllable = true;
		public float walkSpeed = 2.0f;
		public float runSpeed = 4.0f;
		[Range(0, 720f)]
		public float angularSpeed = 120f;

		public bool canRun = true;
		public bool canJump = true;
		public float jumpForce = 4.0f;

		[HideInInspector] public Vector3 terrainNormal = Vector3.up;
		[HideInInspector] public float verticalSpeed = 0.0f;

		// ADVANCED PROPERTIES: -------------------------------------------------------------------

        public FACE_DIRECTION faceDirection = FACE_DIRECTION.MovementDirection;
        public TargetPosition faceDirectionTarget = new TargetPosition();

		[Tooltip("Check this if you want to use Unity's NavMesh and have a map baked")]
		public bool canUseNavigationMesh = false;

		// INNER PROPERTIES: ----------------------------------------------------------------------

		private float lastGroundTime = -100f;
		private float lastJumpTime = -100f;

		[HideInInspector] public Character character;

		[HideInInspector] public ANIM_CONSTRAINT animatorConstraint;
		[HideInInspector] public CharacterController characterController;
		[HideInInspector] public NavMeshAgent navmeshAgent;
	
		private ILocomotionSystem currentLocomotionSystem;

		// INITIALIZERS: --------------------------------------------------------------------------

		public void Setup(Character character)
		{
			this.character = character;
			this.characterController = this.character.GetComponent<CharacterController>();

			this.GenerateNavmeshAgent();
			this.SetDirectionalDirection(Vector3.zero);
		}

		// UPDATE: --------------------------------------------------------------------------------

		public void Update()
		{
			LOCOMOTION_SYSTEM locomotionSystem = LOCOMOTION_SYSTEM.CharacterController;
            if (this.currentLocomotionSystem != null)
            {
                locomotionSystem = this.currentLocomotionSystem.Update();
            }

			switch (locomotionSystem)
			{
			case LOCOMOTION_SYSTEM.CharacterController:
				this.UpdateVerticalSpeed(this.characterController.isGrounded);
				break;
            
			case LOCOMOTION_SYSTEM.NavigationMeshAgent:
				this.UpdateVerticalSpeed(!this.navmeshAgent.isOnOffMeshLink);
				break;
			}
			
			this.UpdateCharacterState(locomotionSystem);
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public bool Jump()
		{
			return this.Jump(this.jumpForce);
		}

		public bool Jump(float jumpForce)
		{
			bool isGrounded = (
				this.characterController.isGrounded || 
				Time.time < this.lastGroundTime + JUMP_ONAIR_TIME_OFFSET
			);

			if (this.canJump && isGrounded && this.lastJumpTime + MIN_TIME_BETWEEN_JUMPS < Time.time)
			{
                if (this.character.onJump != null) this.character.onJump.Invoke();
				this.verticalSpeed = jumpForce;
				this.lastJumpTime = Time.time;
				return true;
			}

			return false;
		}

		public void SetAnimatorConstraint(ANIM_CONSTRAINT constraint)
		{
			this.animatorConstraint = constraint;
		}

        public void ChangeHeight(float height)
        {
            if (this.characterController != null)
            {
                this.characterController.height = height;
                this.characterController.center = Vector3.up * (height / 2.0f);
            }

            if (this.navmeshAgent != null)
            {
                this.navmeshAgent.height = height;
            }
        }

		public void SetIsControllable(bool isControllable)
		{
			this.isControllable = isControllable;
			if (!isControllable)
			{
				this.SetDirectionalDirection(Vector3.zero);
			}
		}

        public Vector3 GetAimDirection()
        {
            return this.currentLocomotionSystem.aimDirection;
        }

		// PUBLIC LOCOMOTION METHODS: -------------------------------------------------------------

        public void SetDirectionalDirection(Vector3 direction, ILocomotionSystem.TargetRotation rotation = null)
		{
			this.ChangeLocomotionSystem<LocomotionSystemDirectional>();
            ((LocomotionSystemDirectional)this.currentLocomotionSystem).SetDirection(direction, rotation);
		}

		public void SetTarget(Ray ray, ILocomotionSystem.TargetRotation rotation = null, UnityAction callback = null)
		{
			this.ChangeLocomotionSystem<LocomotionSystemTarget>();
			((LocomotionSystemTarget)this.currentLocomotionSystem).SetTarget(ray, rotation, callback);
		}

		public void SetTarget (Vector3 position, ILocomotionSystem.TargetRotation rotation = null, UnityAction callback = null)
		{
			this.ChangeLocomotionSystem<LocomotionSystemTarget>();
			((LocomotionSystemTarget)this.currentLocomotionSystem).SetTarget(position, rotation, callback);
		}

        public void FollowTarget(Transform transform, float minRadius, float maxRadius)
        {
            this.ChangeLocomotionSystem<LocomotionSystemFollow>();
            ((LocomotionSystemFollow)this.currentLocomotionSystem).SetFollow(transform, minRadius, maxRadius);
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void GenerateNavmeshAgent()
		{
			if (!this.canUseNavigationMesh) return;

            if (this.navmeshAgent == null) this.navmeshAgent = this.character.gameObject.GetComponent<NavMeshAgent>();
            if (this.navmeshAgent == null) this.navmeshAgent = this.character.gameObject.AddComponent<NavMeshAgent>();

			this.navmeshAgent.updatePosition = false;
			this.navmeshAgent.updateRotation = false;
			this.navmeshAgent.updateUpAxis = false;
			this.navmeshAgent.radius = this.characterController.radius;
			this.navmeshAgent.height = this.characterController.height;
			this.navmeshAgent.acceleration = ACCELERATION;
		}

		private void ChangeLocomotionSystem<TLS>() where TLS : ILocomotionSystem, new()
		{
			if (this.currentLocomotionSystem != null && typeof(TLS) == this.currentLocomotionSystem.GetType()) return;
			if (this.currentLocomotionSystem != null) this.currentLocomotionSystem.OnDestroy();

			this.currentLocomotionSystem = new TLS();
			this.currentLocomotionSystem.Setup(this);
		}

		private void UpdateVerticalSpeed(bool isGrounded)
		{
            this.verticalSpeed += Physics.gravity.y * Time.deltaTime;
			if (isGrounded)
			{
				this.lastGroundTime = Time.time;
                if (this.verticalSpeed < Physics.gravity.y)
				{
                    this.verticalSpeed = Physics.gravity.y;
				}
			}
		}

		private void UpdateCharacterState(LOCOMOTION_SYSTEM locomotionSystem)
		{
			Vector3 worldVelocity = Vector3.zero;
			bool isGrounded = true;
			switch (locomotionSystem)
			{
			case LOCOMOTION_SYSTEM.CharacterController:
				worldVelocity = this.characterController.velocity;
				isGrounded = this.characterController.isGrounded;
				break;

			case LOCOMOTION_SYSTEM.NavigationMeshAgent:
				worldVelocity = this.navmeshAgent.velocity;
				isGrounded = !this.navmeshAgent.isOnOffMeshLink;
				break;
			}

            Vector3 localVelocity = this.character.transform.InverseTransformDirection(worldVelocity);
			this.character.characterState.forwardSpeed = localVelocity;
			this.character.characterState.turnSpeed = Mathf.Atan2(localVelocity.x, localVelocity.z);
			this.character.characterState.verticalSpeed = worldVelocity.y;

			this.character.characterState.isGrounded = isGrounded;
			this.character.characterState.normal = this.terrainNormal;
		}

		private bool IsGrounded()
		{
			RaycastHit hitInfo;
			#if UNITY_EDITOR
			Debug.DrawLine(
				this.character.transform.position + (Vector3.up * 0.01f), 
				this.character.transform.position + (Vector3.up * 0.01f) + (Vector3.down * GROUND_CHECK_DISTANCE)
			);
			#endif

			Vector3 position = this.character.transform.position + (Vector3.up * 0.01f);
            int layerMask = Physics.DefaultRaycastLayers;
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;
            if (Physics.Raycast(position, Vector3.down, out hitInfo, GROUND_CHECK_DISTANCE, layerMask, queryTrigger))
			{
				return true;
			}

			return false;
		}
	}
}