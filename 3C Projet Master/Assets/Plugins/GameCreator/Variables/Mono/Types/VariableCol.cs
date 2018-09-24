namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableCol : VariableGeneric<Color>
    {
        public new const string NAME = "Color";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableCol() : base(Color.white)
        { }

        public VariableCol(Color value) : base(value)
        { }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Color;
        }
    }
}