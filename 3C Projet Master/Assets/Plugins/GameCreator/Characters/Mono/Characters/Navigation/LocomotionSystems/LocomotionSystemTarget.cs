namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.AI;
	using GameCreator.Core;
	using GameCreator.Core.Hooks;

	public class LocomotionSystemTarget : ILocomotionSystem
	{
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private bool move = false;
		private bool usingNavmesh;
		private NavMeshPath path;

		private Vector3 targetPosition;
		private TargetRotation targetRotation;

		private UnityAction onFinishCallback;

		// OVERRIDE METHODS: -------------------------------------------------------------------------------------------

		public override CharacterLocomotion.LOCOMOTION_SYSTEM Update()
		{
            if (!this.move)
			{
				if (!this.usingNavmesh)
				{
					Vector3 defaultDirection = Vector3.up * this.characterLocomotion.verticalSpeed * Time.deltaTime;
					this.characterLocomotion.characterController.Move(defaultDirection);
				}

				return (this.usingNavmesh
					? CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent
					: CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController
				);
			}

			if (this.usingNavmesh)
			{
				NavMeshAgent agent = this.characterLocomotion.navmeshAgent;
				CharacterController controller = this.characterLocomotion.characterController;

				if (agent.pathPending || !agent.hasPath || agent.pathStatus != NavMeshPathStatus.PathComplete)
				{
                    if (agent.pathEndPosition == agent.transform.position && !agent.hasPath)
                    {
                        this.Stopping();
                    }

					return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
				}

                float remainingDistance = agent.remainingDistance;
				bool isGrounded = agent.isOnOffMeshLink;
				agent.speed = this.CalculateSpeed(controller.transform.forward, isGrounded);
				agent.angularSpeed = this.characterLocomotion.angularSpeed;

				agent.isStopped = false;
				agent.updateRotation = true;

                if (remainingDistance <= STOP_THRESHOLD)
				{
                    agent.updateRotation = true;
					this.Stopping();
				}
                else if (remainingDistance <= SLOW_THRESHOLD)
				{
                    this.Slowing(remainingDistance);
				}
				else
				{
					this.Moving();
				}

				this.UpdateNavmeshAnimationConstraints();
			}
			else
			{
				if (this.characterLocomotion.navmeshAgent != null)
				{
					this.characterLocomotion.navmeshAgent.updatePosition = false;
					this.characterLocomotion.navmeshAgent.updateUpAxis = false;
				}

				CharacterController controller = this.characterLocomotion.characterController;
				Vector3 targetPosition = Vector3.Scale(this.targetPosition, HORIZONTAL_PLANE);
				targetPosition += Vector3.up * controller.transform.position.y;
				Vector3 targetDirection = (targetPosition - controller.transform.position).normalized;

				float speed = this.CalculateSpeed(targetDirection, controller.isGrounded);
				Quaternion targetRotation = this.UpdateRotation(targetDirection);

				this.UpdateAnimationConstraints(ref targetDirection, ref targetRotation);

				targetDirection = Vector3.Scale(targetDirection, HORIZONTAL_PLANE) * speed;
				targetDirection += Vector3.up * this.characterLocomotion.verticalSpeed;

				controller.Move(targetDirection * Time.deltaTime);
				controller.transform.rotation = targetRotation;

				if (this.characterLocomotion.navmeshAgent != null && this.characterLocomotion.navmeshAgent.isOnNavMesh)
				{
					Vector3 position = this.characterLocomotion.characterController.transform.position;
					this.characterLocomotion.navmeshAgent.Warp(position);
				}

				float remainingDistance = (Vector3.Distance(
                    Vector3.Scale(controller.transform.position, HORIZONTAL_PLANE),
                    Vector3.Scale(this.targetPosition, HORIZONTAL_PLANE)
                ));

				if (remainingDistance <= STOP_THRESHOLD)
				{
					this.Stopping();
				}
				else if (remainingDistance <= SLOW_THRESHOLD)
				{
					this.Slowing(remainingDistance);
				}
			}

			if (this.usingNavmesh) return CharacterLocomotion.LOCOMOTION_SYSTEM.NavigationMeshAgent;
			else return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
		}

		public override void OnDestroy()
		{

		}

		// PRIVATE METHODS: --------------------------------------------------------------------------------------------

		private void Stopping ()
		{
			if (this.characterLocomotion.navmeshAgent != null)
			{
				this.characterLocomotion.navmeshAgent.isStopped = true;
			}

			this.FinishMovement();
			this.move = false;

            if (this.targetRotation.hasRotation &&
                this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                this.characterLocomotion.character.transform.rotation = this.targetRotation.rotation;
            }
		}

		private void Slowing (float distanceToDestination)
		{
			float tDistance = 1f - (distanceToDestination / SLOW_THRESHOLD);
			Quaternion targetRotation = this.characterLocomotion.character.transform.rotation;

            if (this.targetRotation.hasRotation &&
                this.characterLocomotion.faceDirection == CharacterLocomotion.FACE_DIRECTION.MovementDirection)
            {
                targetRotation = this.targetRotation.rotation;
            }

            this.characterLocomotion.character.transform.rotation = Quaternion.Lerp(
                this.characterLocomotion.character.transform.rotation,
                targetRotation,
                tDistance
            );
		}

		private void Moving ()
		{
			Quaternion targetRotation = this.UpdateRotation(this.characterLocomotion.navmeshAgent.desiredVelocity);
			this.characterLocomotion.character.transform.rotation = targetRotation;
		}

		private void UpdateNavmeshAnimationConstraints()
		{
			NavMeshAgent agent = this.characterLocomotion.navmeshAgent;
			if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT)
			{
				if (agent.velocity == Vector3.zero)
				{
					agent.Move(agent.transform.forward * agent.speed * Time.deltaTime);
				}
			}

			if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION)
			{
				agent.isStopped = true;
			}
		}

		private void FinishMovement()
		{
			if (this.onFinishCallback != null)
			{
				this.onFinishCallback.Invoke();
				this.onFinishCallback = null;
			}
		}

		// PUBLIC METHODS: ---------------------------------------------------------------------------------------------

		public void SetTarget(Ray ray, TargetRotation rotation = null, UnityAction callback = null)
		{
            RaycastHit hit;

            int layerMask = Physics.DefaultRaycastLayers;
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, queryTrigger))
			{
				this.SetTarget(hit.point, rotation, callback);
			}
		}

		public void SetTarget(Vector3 position, TargetRotation rotation = null, UnityAction callback = null)
		{
			this.move = true;
			this.usingNavmesh = false;
			this.onFinishCallback = callback;

			if (this.characterLocomotion.canUseNavigationMesh)
			{
				NavMeshHit hit;
				if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas)) position = hit.position;

				this.path = new NavMeshPath();
				bool pathFound = NavMesh.CalculatePath(
					this.characterLocomotion.characterController.transform.position,
					position,
					NavMesh.AllAreas,
					this.path
				);

                if (pathFound)
                {
                    Debug.DrawLine(position, position + Vector3.up, Color.green, 0.5f);

                    this.usingNavmesh = true;
                    this.characterLocomotion.navmeshAgent.updatePosition = true;
                    this.characterLocomotion.navmeshAgent.updateUpAxis = true;

                    this.characterLocomotion.navmeshAgent.isStopped = false;
                    this.characterLocomotion.navmeshAgent.SetPath(this.path);
                }
			}

			this.targetPosition = position;
			this.targetRotation = (rotation == null ? new TargetRotation() : rotation);
		}
	}
}
