namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;

	[System.Serializable]
	public class TargetTransform
	{
		public enum Target
		{
			Player,
			Invoker,
			Transform
		}

		// PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.Transform;
        public TransformProperty transform;

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetTransform()
        { }

        public TargetTransform(TargetTransform.Target target)
        {
            this.target = target;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public Transform GetTransform(GameObject invoker)
		{
            Transform result = null;

			switch (this.target)
			{
			case Target.Player :
				if (HookPlayer.Instance != null) result = HookPlayer.Instance.transform;
				break;

			case Target.Invoker:
				result = invoker.transform;
				break;

			case Target.Transform:
                result = this.transform.GetValue();
				break;
			}

			return result;
		}

        public Vector3 GetDirection(GameObject invoker)
		{
            Transform directionTarget = this.GetTransform(invoker);
			Vector3 direction = Vector3.zero;

            if (directionTarget != null) direction = directionTarget.position - invoker.transform.position;
			return direction.normalized;
		}

		// UTILITIES: -----------------------------------------------------------------------------

		public override string ToString ()
		{
			string result = "(unknown)";
			switch (this.target)
			{
			case Target.Player : result = "Player"; break;
			case Target.Invoker: result = "Invoker"; break;
			case Target.Transform: 
                Transform value = this.transform.GetValue();
                result = (value == null ? "(none)" : value.gameObject.name);
                break;
			}

			return result;
		}
	}
}