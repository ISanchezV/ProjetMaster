namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableTrn : VariableGeneric<Transform>
    {
        public new const string NAME = "Transform";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableTrn()
        { }

        public VariableTrn(Transform value) : base(value)
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override bool CanSave()
        {
            return false;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Transform;
        }
    }
}