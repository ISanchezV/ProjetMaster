namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Game Creator/Characters/Character Animator", 100)]
	public class CharacterAnimator : MonoBehaviour 
	{
        private const float NORMAL_SMOOTH = 0.1f;
        private const float MAX_FALL_FORCE_SPEED = -5.0f;

        public const string LAYER_BASE = "Base";
		public const string LAYER_IK = "IK";

		private const string EXC_NO_CHARACTER = "No CharacterNavigatorController found on gameObject";
		private const string EXC_NO_ANIMATOR  = "No Animator attached to CharacterNavigationAnimator";

        private class AnimFloat
        {
            private bool setup = false;
            private float value = 0.0f;
            private float velocity = 0.0f;

            public float Get(float target, float smooth)
            {
                if (!this.setup)
                {
                    this.value = target;
                    this.velocity = 0.0f;
                    this.setup = true;
                }

                this.value = Mathf.SmoothDamp(
                    this.value,
                    target,
                    ref velocity,
                    smooth
                );

                return this.value;
            }

            public void Set(float value)
            {
                this.value = value;
                this.velocity = 0.0f;
            }
        }

		// PROPERTIES: ----------------------------------------------------------------------------

		public Animator animator;
		private Character character;
		
        private CharacterAnimatorEvents animEvents;
        private CharacterAnimation characterAnimation;
        private CharacterAttachments characterAttachments;

		private CharacterHeadTrack headTrack;
        private CharacterFootIK footIK;
        private CharacterHandIK handIK;

        public string moveForwardSpeed = "MoveForward";
        public string moveSidesSpeed = "MoveSides";
        public string movementSpeed = "Movement";
        public string targetLock = "TargetLock";
		public string isGrounded = "IsGrounded";
		public string verticalSpeed = "VerticalSpeed";
		public string normalX = "NormalX";
        public string normalY = "NormalY";
        public string normalZ = "NormalZ";
		public string jump = "Jump";
        public string fallForce = "FallForce";
        public string timeScale = "TimeScale";

        private int paramHashMoveForwardSpeed;
        private int paramHashMoveSidesSpeed;
        private int paramHashMovementSpeed;
        private int paramHashTargetLock;
		private int paramHashIsGrounded;
		private int paramHashVerticalSpeed;
		private int paramHashNormalX;
        private int paramHashNormalY;
        private int paramHashNormalZ;
		private int paramHashJump;
        private int paramHashFallForce;
        private int paramHashTimeScale;

        private Dictionary<int, AnimFloat> paramValues = new Dictionary<int, AnimFloat>();

        public bool useFootIK = true;
        public bool useHandIK = true;
        public bool useSmartHeadIK = true;

        private bool wasGrounded = false;

		// INITIALIZERS: --------------------------------------------------------------------------

		private void Awake()
		{
			this.character = gameObject.GetComponent<Character>();
            this.characterAnimation = new CharacterAnimation(this);

			this.paramHashMoveForwardSpeed = Animator.StringToHash(this.moveForwardSpeed);
			this.paramHashMoveSidesSpeed = Animator.StringToHash(this.moveSidesSpeed);
            this.paramHashMovementSpeed = Animator.StringToHash(this.movementSpeed);
            this.paramHashTargetLock = Animator.StringToHash(this.targetLock);
			this.paramHashIsGrounded = Animator.StringToHash(this.isGrounded);
			this.paramHashVerticalSpeed = Animator.StringToHash(this.verticalSpeed);
            this.paramHashNormalX = Animator.StringToHash(this.normalX);
            this.paramHashNormalY = Animator.StringToHash(this.normalY);
            this.paramHashNormalZ = Animator.StringToHash(this.normalZ);
			this.paramHashJump = Animator.StringToHash(this.jump);
            this.paramHashFallForce = Animator.StringToHash(this.fallForce);
            this.paramHashTimeScale = Animator.StringToHash(this.timeScale);

            this.paramValues.Add(this.paramHashMoveForwardSpeed, new AnimFloat());
            this.paramValues.Add(this.paramHashMoveSidesSpeed, new AnimFloat());
            this.paramValues.Add(this.paramHashMovementSpeed, new AnimFloat());
            this.paramValues.Add(this.paramHashTargetLock, new AnimFloat());
            this.paramValues.Add(this.paramHashVerticalSpeed, new AnimFloat());
            this.paramValues.Add(this.paramHashNormalX, new AnimFloat());
            this.paramValues.Add(this.paramHashNormalY, new AnimFloat());
            this.paramValues.Add(this.paramHashNormalZ, new AnimFloat());
            this.paramValues.Add(this.paramHashFallForce, new AnimFloat());
		}

        private void OnDestroy()
        {
            this.characterAnimation.OnDestroy();
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
		{
			if (this.character  == null) throw new UnityException(EXC_NO_CHARACTER);
			if (this.animator   == null) throw new UnityException(EXC_NO_ANIMATOR);
			if (this.animEvents == null) this.GenerateAnimatorEvents();
            if (this.characterAttachments == null) this.GenerateCharacterAttachments();
            if (this.characterAnimation != null) this.characterAnimation.Update();

            if (this.useFootIK && this.footIK == null) this.GenerateFootIK();
            if (this.useHandIK && this.handIK == null) this.GenerateHandIK();
            if (this.useSmartHeadIK && this.headTrack == null)
            {
                if (this.GetHeadTracker() != null) this.headTrack.Untrack();
            }

			Character.State state = this.character.GetCharacterState();
            Vector3 direction = (state.forwardSpeed.magnitude < 0.01f 
                ? Vector3.zero
                : state.forwardSpeed
            );

            direction = Vector3.Scale(direction, Vector3.one * (1.0f / this.character.characterLocomotion.runSpeed));

            if (state.isGrounded && !this.wasGrounded)
            {
                float fallForceAmount = this.animator.GetFloat(this.paramHashVerticalSpeed);
                fallForceAmount = Mathf.Clamp01(fallForceAmount / MAX_FALL_FORCE_SPEED);
                this.paramValues[this.paramHashFallForce].Set(fallForceAmount);
            }

            float paramMoveForwardSpeed = this.paramValues[this.paramHashMoveForwardSpeed].Get(direction.z, 0.1f);
            float paramMoveSidesSpeed = this.paramValues[this.paramHashMoveSidesSpeed].Get(direction.x, 0.2f);
            float paramMovementSpeed = this.paramValues[this.paramHashMovementSpeed].Get(
                Vector3.Scale(direction, new Vector3(1,0,1)).magnitude,
                0.1f
            );
            float paramVerticalSpeed = this.paramValues[this.paramHashVerticalSpeed].Get(state.verticalSpeed, 0.1f);

            this.animator.SetFloat(this.paramHashMoveForwardSpeed, paramMoveForwardSpeed);
            this.animator.SetFloat(this.paramHashMoveSidesSpeed, paramMoveSidesSpeed);
            this.animator.SetFloat(this.paramHashMovementSpeed, paramMovementSpeed);
            this.animator.SetBool(this.paramHashIsGrounded, state.isGrounded);
            this.animator.SetFloat(this.paramHashVerticalSpeed, paramVerticalSpeed);
            this.animator.SetFloat(this.paramHashTimeScale, Time.timeScale);

            float paramFallForce = this.paramValues[this.paramHashFallForce].Get(0.0f, 0.25f);
            this.animator.SetFloat(this.paramHashFallForce, paramFallForce);

            if (this.character.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                float paramTargetLock = this.paramValues[this.paramHashTargetLock].Get(0.0f, 0.1f);
                this.animator.SetFloat(this.paramHashTargetLock, paramTargetLock);
            }
            else
            {
                float paramTargetLock = this.paramValues[this.paramHashTargetLock].Get(1.0f, 0.1f);
                this.animator.SetFloat(this.paramHashTargetLock, paramTargetLock);
            }

            this.Normals(state);
            this.wasGrounded = state.isGrounded;
		}

        private void Normals(Character.State state)
        {
            Vector3 normal = Vector3.up;
            if (state.isGrounded)
            {
                normal = this.character.transform.InverseTransformDirection(state.normal);
            }

            float paramNormalX = this.paramValues[this.paramHashNormalX].Get(normal.x, NORMAL_SMOOTH);
            float paramNormalY = this.paramValues[this.paramHashNormalY].Get(normal.y, NORMAL_SMOOTH);
            float paramNormalZ = this.paramValues[this.paramHashNormalZ].Get(normal.z, NORMAL_SMOOTH);

            this.animator.SetFloat(this.paramHashNormalX, paramNormalX);
            this.animator.SetFloat(this.paramHashNormalY, paramNormalY);
            this.animator.SetFloat(this.paramHashNormalZ, paramNormalZ);
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

		public CharacterHeadTrack GetHeadTracker()
		{
			if (this.headTrack == null)
			{
				this.headTrack = gameObject.GetComponentInChildren<CharacterHeadTrack>();
				if (this.headTrack == null && this.animator != null && this.animator.isHuman)
				{
					this.headTrack = this.animator.gameObject.AddComponent<CharacterHeadTrack>();
				}
			}

			return this.headTrack;
		}

		public void Jump()
		{
			this.animator.SetTrigger(this.paramHashJump);
		}

        public Transform GetHeadTransform()
        {
            if (!this.animator.isHuman) return transform;
            Transform head = this.animator.GetBoneTransform(HumanBodyBones.Head);
            return head ?? transform;
        }

        public void PlayGesture(AnimationClip clip, AvatarMask avatarMask = null)
        {
            this.characterAnimation.PlayGesture(clip, avatarMask);
        }

        public void SetState(RuntimeAnimatorController rtc, AvatarMask avatarMask,
                             float weight, float time, CharacterAnimation.Layer layer)
        {
            this.characterAnimation.SetState(rtc, avatarMask, weight, time, layer);
        }

        public void SetState(AnimationClip clip, AvatarMask avatarMask,
                             float weight, float time, CharacterAnimation.Layer layer)
        {
            this.characterAnimation.SetState(clip, avatarMask, weight, time, layer);
        }

        public void ResetState(float time, CharacterAnimation.Layer layer)
        {
            this.characterAnimation.ResetState(time, layer);
        }

        public void ChangeStateWeight(CharacterAnimation.Layer layer, float weight)
        {
            this.characterAnimation.ChangeStateWeight(layer, weight);
        }

        public void ResetControllerTopology(RuntimeAnimatorController runtimeController)
        {
            this.characterAnimation.SetupTopology(runtimeController);
        }

        public CharacterAttachments GetCharacterAttachments()
        {
            return this.characterAttachments;
        }

        public CharacterHandIK GetCharacterHandIK()
        {
            return this.handIK;
        }

		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void GenerateAnimatorEvents()
		{
			this.animEvents = this.animator.gameObject.AddComponent<CharacterAnimatorEvents>();
			this.animEvents.Setup(this.character);
		}

        private void GenerateCharacterAttachments()
        {
            this.characterAttachments = this.animator.gameObject.AddComponent<CharacterAttachments>();
            this.characterAttachments.Setup(this.animator);
        }

        private void GenerateFootIK()
        {
            if (this.animator != null && this.animator.isHuman)
            {
                this.footIK = this.animator.gameObject.AddComponent<CharacterFootIK>();
            }
        }

        private void GenerateHandIK()
        {
            if (this.animator != null && this.animator.isHuman)
            {
                this.handIK = this.animator.gameObject.AddComponent<CharacterHandIK>();
            }
        }
	}
}