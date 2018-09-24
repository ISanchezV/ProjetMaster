namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	public abstract class ILocomotionSystem
	{
		public class TargetRotation
		{
			public bool hasRotation;
			public Quaternion rotation;

			public TargetRotation(bool hasRotation = false, Vector3 direction = default(Vector3))
			{
				this.hasRotation = hasRotation;
				this.rotation = (hasRotation ? Quaternion.LookRotation(direction) : Quaternion.identity);
			}
		}

		// CONSTANT PROPERTIES: -------------------------------------------------------------------

		protected static readonly Vector3 HORIZONTAL_PLANE = new Vector3(1,0,1);
		protected const float SLOW_THRESHOLD = 1.0f;
		protected const float STOP_THRESHOLD = 0.05f;

		// PROPERTIES: ----------------------------------------------------------------------------

		protected CharacterLocomotion characterLocomotion;
        public Vector3 aimDirection { get; private set; }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Setup(CharacterLocomotion characterLocomotion)
        {
            this.characterLocomotion = characterLocomotion;
        }

		// ABSTRACT METHODS: ----------------------------------------------------------------------

		public abstract CharacterLocomotion.LOCOMOTION_SYSTEM Update();
		public abstract void OnDestroy();

		// CHARACTER CONTROLLER METHODS: ----------------------------------------------------------

		protected Quaternion UpdateRotation(Vector3 targetDirection)
		{
			Quaternion targetRotation = this.characterLocomotion.character.transform.rotation;
            this.aimDirection = this.characterLocomotion.character.transform.forward;

            if (this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection &&
                targetDirection != Vector3.zero)
            {
                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(targetDirection);
                this.aimDirection = dstRotation * Vector3.forward;

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * this.characterLocomotion.angularSpeed
                );
            }
            else if (this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.CameraDirection &&
                HookCamera.Instance != null)
            {
                Vector3 camDirection = HookCamera.Instance.transform.TransformDirection(Vector3.forward);
                this.aimDirection = camDirection;

                camDirection.Scale(new Vector3(1, 0, 1));

                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(camDirection);

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * this.characterLocomotion.angularSpeed
                );
            }
            else if (this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.Target)
            {
                Vector3 target = this.characterLocomotion.faceDirectionTarget.GetPosition(null);
                Vector3 direction = target - this.characterLocomotion.character.transform.position;
                this.aimDirection = direction;

                direction.Scale(new Vector3(1, 0, 1));

                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(direction);

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * this.characterLocomotion.angularSpeed
                );
            }
            else if (this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.GroundPlaneCursor)
            {
                Camera camera = null;
                if (camera == null)
                {
                    if (HookCamera.Instance != null) camera = HookCamera.Instance.Get<Camera>();
                    if (camera == null && Camera.main != null) camera = Camera.main;
                }

                Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
                Transform character = this.characterLocomotion.character.transform;

                Plane plane = new Plane(Vector3.up, character.position);
                float rayDistance = 0.0f;

                if (plane.Raycast(cameraRay, out rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    Vector3 target = Vector3.MoveTowards(character.position, cursor, 1f);
                    Vector3 direction = target - this.characterLocomotion.character.transform.position;
                    direction.Scale(new Vector3(1, 0, 1));

                    Quaternion srcRotation = character.rotation;
                    Quaternion dstRotation = Quaternion.LookRotation(direction);
                    this.aimDirection = dstRotation * Vector3.forward;

                    targetRotation = Quaternion.RotateTowards(
                        srcRotation,
                        dstRotation,
                        Time.deltaTime * this.characterLocomotion.angularSpeed
                    );
                }
            }

			return targetRotation;
		}

		protected float CalculateSpeed(Vector3 targetDirection, bool isGrounded)
		{
			float speed = (this.characterLocomotion.canRun 
				? this.characterLocomotion.runSpeed 
				: this.characterLocomotion.walkSpeed
			);

            if (this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection &&
                targetDirection != Vector3.zero)
			{
				Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
				Quaternion dstRotation = Quaternion.LookRotation(targetDirection);
                float angle = Quaternion.Angle(srcRotation, dstRotation) / 180.0f;
                float speedDampening = Mathf.Clamp(1.0f - angle, 0.5f, 1.0f);
				speed *= speedDampening;
			}

			return speed;
		}

		protected virtual void UpdateAnimationConstraints(ref Vector3 targetDirection, ref Quaternion targetRotation)
		{
			if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT) 
			{
				if (targetDirection == Vector3.zero) 
				{
					targetDirection = this.characterLocomotion.characterController.transform.forward;
				}
			}

			if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION) 
			{
				targetDirection = Vector3.zero;
				targetRotation = this.characterLocomotion.characterController.transform.rotation;
			}
		}
	}
}