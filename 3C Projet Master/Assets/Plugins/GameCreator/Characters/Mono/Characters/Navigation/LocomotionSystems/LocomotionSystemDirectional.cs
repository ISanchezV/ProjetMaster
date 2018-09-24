namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	public class LocomotionSystemDirectional : ILocomotionSystem 
	{
		// PROPERTIES: ----------------------------------------------------------------------------

		protected Vector3 desiredDirection = Vector3.zero;

		// OVERRIDE METHODS: ----------------------------------------------------------------------

		public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
		{
			if (this.characterLocomotion.navmeshAgent != null)
			{
				this.characterLocomotion.navmeshAgent.updatePosition = false;
				this.characterLocomotion.navmeshAgent.updateUpAxis = false;
			}
			
			Vector3 targetDirection = this.desiredDirection;

			float speed = this.CalculateSpeed(targetDirection, this.characterLocomotion.characterController.isGrounded);
			Quaternion targetRotation = this.UpdateRotation(targetDirection);

			this.UpdateAnimationConstraints(ref targetDirection, ref targetRotation);

            targetDirection = Vector3.ClampMagnitude(Vector3.Scale(targetDirection, HORIZONTAL_PLANE), 1.0f);
            targetDirection *= speed;
			targetDirection += Vector3.up * this.characterLocomotion.verticalSpeed;

			this.characterLocomotion.characterController.Move(targetDirection * Time.deltaTime);
			this.characterLocomotion.characterController.transform.rotation = targetRotation;

			if (this.characterLocomotion.navmeshAgent != null && this.characterLocomotion.navmeshAgent.isOnNavMesh)
			{
				Vector3 position = this.characterLocomotion.characterController.transform.position;
				this.characterLocomotion.navmeshAgent.Warp(position);
                return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
			}

			return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
		}

		public override void OnDestroy ()
		{
			return;
		}

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public void SetDirection(Vector3 direction, TargetRotation rotation = null)
		{
			this.desiredDirection = direction;
            if (rotation != null)
            {

            }
		}
	}
}