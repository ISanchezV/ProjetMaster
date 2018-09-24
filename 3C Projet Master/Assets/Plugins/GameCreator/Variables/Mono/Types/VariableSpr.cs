namespace GameCreator.Variables
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class VariableSpr : VariableGeneric<Sprite>
    {
        public new const string NAME = "Sprite";

        // INITIALIZERS: --------------------------------------------------------------------------

        public VariableSpr()
        { }

        public VariableSpr(Sprite value) : base(value)
        { }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public new static Variable.DataType GetDataType()
        {
            return Variable.DataType.Sprite;
        }
    }
}