namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableStr : VariableGeneric<string>
    {
        public new const string NAME = "String";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableStr()
        { }

        public VariableStr(string value) : base(value)
        { }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.String;
        }
    }
}