namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CharacterState : ScriptableObject
    {
        public enum StateType
        {
            Simple,
            Locomotion,
            Other
        }

        public StateType type = StateType.Simple;

        public AnimatorOverrideController rtcSimple;
        public AnimatorOverrideController rtcLocomotion;
        public RuntimeAnimatorController rtcOther;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public RuntimeAnimatorController GetRuntimeAnimatorController()
        {
            switch (this.type)
            {
                case StateType.Simple : return this.rtcSimple;
                case StateType.Locomotion : return this.rtcLocomotion;
                case StateType.Other : return this.rtcOther;
            }

            return null;
        }
    }
}