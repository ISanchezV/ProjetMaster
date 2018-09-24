namespace GameCreator.Core
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using GameCreator.Core.Hooks;
    using GameCreator.Variables;

	[System.Serializable]
    public class TargetRigidbody
	{
		public enum Target
		{
            Rigidbody,
			Invoker,
		}

		// PROPERTIES: ----------------------------------------------------------------------------

        public Target target = Target.Rigidbody;
        public RigidbodyProperty rigidbody;

        // INITIALIZERS: --------------------------------------------------------------------------

        public TargetRigidbody() { }

        public TargetRigidbody(TargetRigidbody.Target target)
        {
            this.target = target;
        }

		// PUBLIC METHODS: ------------------------------------------------------------------------

        public Rigidbody GetRigidbody(GameObject invoker)
		{
            Rigidbody result = null;

			switch (this.target)
			{
			case Target.Invoker:
                result = invoker.GetComponent<Rigidbody>();
				break;
                
            case Target.Rigidbody:
                result = this.rigidbody.GetValue();
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
            case Target.Rigidbody: 
                Rigidbody value = this.rigidbody.GetValue();
                result = (value == null ? "(none)" : value.gameObject.name);
                break;
			case Target.Invoker: 
                result = "Invoker"; 
                break;
			}

			return result;
		}
	}
}