namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class TransformProperty : BaseProperty<Transform>
    {
        public TransformProperty() : base() { }
        public TransformProperty(Transform value) : base(value) { }
    }
}