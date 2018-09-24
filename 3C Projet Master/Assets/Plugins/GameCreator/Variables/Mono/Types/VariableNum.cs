namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableNum : VariableGeneric<float>
    {
        public new const string NAME = "Number";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableNum()
        { }

        public VariableNum(float value) : base(value)
        { }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Number;
        }
    }
}