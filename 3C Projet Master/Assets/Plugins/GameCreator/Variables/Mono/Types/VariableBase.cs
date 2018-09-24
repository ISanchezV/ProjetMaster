namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public abstract class VariableBase
    {
        public const string NAME = "Null";

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual bool CanSave()
        {
            return true;
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static Variable.DataType GetDataType()
        {
            return Variable.DataType.Null;
        }
    }
}