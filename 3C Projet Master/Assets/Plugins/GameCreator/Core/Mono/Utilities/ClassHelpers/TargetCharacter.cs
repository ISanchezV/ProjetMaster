namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Characters;
    using GameCreator.Variables;

	[System.Serializable]
	public class TargetCharacter
	{
		public enum Target
		{
			Player,
			Invoker,
			Character
		}

		// PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.Character;
        public Character character;

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetCharacter() { }

        public TargetCharacter(TargetCharacter.Target target)
        {
            this.target = target;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public Character GetCharacter(GameObject invoker)
		{
            Character result = null;

			switch (this.target)
			{
			case Target.Player :
                if (HookPlayer.Instance != null) result = HookPlayer.Instance.Get<Character>();
				break;

			case Target.Invoker:
                result = invoker.GetComponentInChildren<Character>();
				break;

                case Target.Character:
				if (this.character != null) result = this.character;
				break;
			}

			return result;
		}

		// UTILITIES: -----------------------------------------------------------------------------

		public override string ToString ()
		{
			string result = "(unknown)";
			switch (this.target)
			{
			case Target.Player : result = "Player"; break;
			case Target.Invoker: result = "Invoker"; break;
            case Target.Character:
                result = (this.character == null ? "(none)" : this.character.gameObject.name);
				break;
			}

			return result;
		}
	}
}