namespace GameCreator.Variables
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;

    [Serializable]
    public abstract class BaseProperty<T>
    {
        public enum OPTION
        {
            Value,
            UseGlobalVariable,
            UseLocalVariable
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public OPTION optionIndex = OPTION.Value;
        public T value;

        public HelperGlobalVariable global = new HelperGlobalVariable();
        public HelperLocalVariable local = new HelperLocalVariable();

        // INITIALIZERS: --------------------------------------------------------------------------

        protected BaseProperty()
        {
            this.value = default(T);
            this.SetupVariables();
        }

        protected BaseProperty(T value)
        {
            this.value = value;
            this.SetupVariables();
        }

        private void SetupVariables()
        {
            this.global = this.global ?? new HelperGlobalVariable();
            this.local = this.local ?? new HelperLocalVariable();
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T GetValue()
        {
            switch (this.optionIndex)
            {
                case OPTION.Value: return this.value;
                case OPTION.UseGlobalVariable : return (T)this.global.Get();
                case OPTION.UseLocalVariable: return (T)this.local.Get();
            }

            return default(T);
        }

		// OVERRIDERS: ----------------------------------------------------------------------------

		public override string ToString()
		{
            #pragma warning disable RECS0017
            switch (this.optionIndex)
            {
                case OPTION.Value : return (this.value == null ? "(none)" : this.value.ToString());
                case OPTION.UseGlobalVariable: return this.global.ToString();
                case OPTION.UseLocalVariable: return this.local.ToString();
            }
            #pragma warning restore RECS0017

            return "unknown";
		}
	}
}