namespace GameCreator.Characters
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

    [AddComponentMenu("")]
	public class CharacterAnimatorEvents : MonoBehaviour 
	{
		// PROPERTIES: -------------------------------------------------------------------------------------------------

		private Character character;

		// INITIALIZER: ------------------------------------------------------------------------------------------------

		public void Setup(Character character)
		{
			this.character = character;
		}

		// EVENTS: -----------------------------------------------------------------------------------------------------

		public void Event_KeepPosition(float timeout)
		{
			StartCoroutine(this.CoroutineTrigger(CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION, timeout));
		}

		public void Event_KeepMovement(float timeout)
		{
			StartCoroutine(this.CoroutineTrigger(CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT, timeout));
		}

		// COROUTINES: -------------------------------------------------------------------------------------------------

		private IEnumerator CoroutineTrigger(CharacterLocomotion.ANIM_CONSTRAINT constraint, float timeout)
		{
			this.character.characterLocomotion.SetAnimatorConstraint(constraint);

			WaitForSeconds coroutine = new WaitForSeconds(timeout);
			yield return coroutine;

			this.character.characterLocomotion.SetAnimatorConstraint(CharacterLocomotion.ANIM_CONSTRAINT.NONE);
		}
	}
}