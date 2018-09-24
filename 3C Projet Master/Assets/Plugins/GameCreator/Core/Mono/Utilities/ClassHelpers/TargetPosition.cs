namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;

	[System.Serializable]
	public class TargetPosition
	{
		public enum Target
		{
			Player,
			Invoker,
			Transform,
			Position
		}

		// PROPERTIES: ----------------------------------------------------------------------------

		public Target target = Target.Position;
		public Vector3 offset = Vector3.zero;

        public TransformProperty targetTransform = new TransformProperty();
        public Vector3Property targetPosition = new Vector3Property();

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetPosition() 
        { }

        public TargetPosition(TargetPosition.Target target)
        {
            this.target = target;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 GetPosition(GameObject invoker)
		{
			Vector3 position = Vector3.zero;
			Vector3 offset = this.offset;

			switch (this.target)
			{
			case Target.Player :
				if (HookPlayer.Instance != null)
				{
                    position = HookPlayer.Instance.transform.position;
                    offset = HookPlayer.Instance.transform.TransformDirection(this.offset);
				}
				break;

			case Target.Invoker:
                position = invoker.transform.position;
				break;

			case Target.Transform:
				if (this.targetTransform != null)
				{
                    Transform targetTrans = this.targetTransform.GetValue();
                    if (targetTrans != null)
                    {
                        position = targetTrans.position;
                        offset = this.targetTransform.GetValue().TransformDirection(this.offset);
                    }
				}
				break;

			case Target.Position:
                position = this.targetPosition.GetValue();
                offset = Vector3.zero;
				break;
			}

			return position + offset;
		}

        public Vector3 GetDirection(GameObject invoker)
        {
            Vector3 direction = Vector3.zero;
            switch (this.target)
            {
                case Target.Player:
                    if (HookPlayer.Instance != null)
                    {
                        Vector3 playerPosition = HookPlayer.Instance.transform.position;
                        playerPosition += HookPlayer.Instance.transform.TransformDirection(this.offset);
                        direction = playerPosition - invoker.transform.position;
                    }
                    break;

                case Target.Invoker:
                    direction = invoker.transform.forward;
                    break;

                case Target.Transform:
                    if (this.targetTransform != null)
                    {
                        Transform targetTrans = this.targetTransform.GetValue();
                        if (targetTrans != null)
                        {
                            Vector3 tarPosition = targetTrans.position;
                            tarPosition += this.targetTransform.GetValue().TransformDirection(this.offset);

                            direction = tarPosition - invoker.transform.position;
                        }
                    }
                    break;

                case Target.Position:
                    direction = this.targetPosition.GetValue() - invoker.transform.position;
                    break;
            }

            return direction.normalized;
        }

        public Quaternion GetRotation(GameObject invoker)
		{
			Quaternion rotation = invoker.transform.rotation;
			switch (this.target)
			{
			case Target.Player :
				if (HookPlayer.Instance != null) rotation = HookPlayer.Instance.transform.rotation;
				break;

			case Target.Transform:
                if (this.targetTransform != null) rotation = this.targetTransform.GetValue().rotation;
				break;
			}

			return rotation;
		}

		public override string ToString ()
		{
			string result = "(unknown)";
			switch (this.target)
			{
			case Target.Player : result = "Player"; break;
			case Target.Invoker: result = "Invoker"; break;
            case Target.Transform: 
                Transform valueTransform = this.targetTransform.GetValue();
                result = (valueTransform == null ? "(none)" : valueTransform.gameObject.name);
                break;
			case Target.Position: 
                Vector3 valuePosition = this.targetPosition.GetValue();
                result = string.Format(
                    "position({0:0.00},{1:0.00},{2:0.00}]",
                    valuePosition.x, valuePosition.y, valuePosition.z
                );
                break;
			}

			return result;
		}
	}
}