namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class RigidbodyProperty : BaseProperty<Rigidbody>
    {
        public RigidbodyProperty() : base() { }
        public RigidbodyProperty(Rigidbody value) : base(value) { }
    }
}