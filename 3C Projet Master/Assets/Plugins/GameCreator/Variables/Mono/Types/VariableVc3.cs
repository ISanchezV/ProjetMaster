namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableVc3 : VariableGeneric<Vector3>
    {
        public new const string NAME = "Vector3";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableVc3()
        { }

        public VariableVc3(Vector3 value) : base(value)
        { }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Vector3;
        }
    }
}