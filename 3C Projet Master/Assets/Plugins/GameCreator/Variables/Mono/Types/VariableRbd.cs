namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableRbd : VariableGeneric<Rigidbody>
    {
        public new const string NAME = "Rigidbody";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableRbd()
        { }

        public VariableRbd(Rigidbody value) : base(value)
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override bool CanSave()
        {
            return false;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Rigidbody;
        }
    }
}