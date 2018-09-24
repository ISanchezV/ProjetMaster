namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("")]
    public class CharacterFootIK : MonoBehaviour
    {
        private const float FOOT_OFFSET_Y = 0.1f;
        private const float SMOOTH_POSITION = 0.1f;
        private const float SMOOTH_ROTATION = 0.1f;
        private const float SMOOTH_WEIGHT   = 0.2f;
        private const float BODY_MAX_INCLINE = 10f;

        private static readonly int IK_L_FOOT = Animator.StringToHash("IK/leftFoot");
        private static readonly int IK_R_FOOT = Animator.StringToHash("IK/rightFoot");

        private class Foot
        {
            public bool hit;
            public int weightID;
            public AvatarIKGoal footIK;
            public Transform foot;

            public float height = 0.0f;
            public Vector3 normal = Vector3.up;

            public Foot(Transform foot, AvatarIKGoal footIK, int weightID)
            {
                this.hit = false;
                this.weightID = weightID;
                this.footIK = footIK;
                this.foot = foot;
            }

            public float GetWeight(Animator animator)
            {
                return animator.GetFloat(this.weightID);
            }
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private int ikLayerIndex = 0;
        private Animator animator;
        private CharacterController controller;

        private Foot leftFoot;
        private Foot rightFoot;

        private float defaultOffset = 0.0f;
        private float speedPosition = 0.0f;
        private float speedRotation = 0.0f;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.Setup();
        }

        public void Setup()
        {
            this.animator = gameObject.GetComponentInChildren<Animator>();
            this.controller = gameObject.GetComponentInParent<CharacterController>();
            if (this.animator == null || !this.animator.isHuman || this.controller == null) return;
            this.ikLayerIndex = this.animator.GetLayerIndex(CharacterAnimator.LAYER_IK);

            Transform lFoot = this.animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            Transform rFoot = this.animator.GetBoneTransform(HumanBodyBones.RightFoot);

            this.leftFoot = new Foot(lFoot, AvatarIKGoal.LeftFoot, IK_L_FOOT);
            this.rightFoot = new Foot(rFoot, AvatarIKGoal.RightFoot, IK_R_FOOT);

            this.defaultOffset = transform.localPosition.y;
        }

        private void LateUpdate()
        {
            this.WeightCompensationPosition();
            this.WeightCompensationRotation();
        }

        // IK METHODS: ----------------------------------------------------------------------------

        private void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != this.ikLayerIndex) return;
            if (this.animator == null || !this.animator.isHuman || this.controller == null) return;

            if (this.controller.isGrounded)
            {
                UpdateFoot(this.leftFoot);
                UpdateFoot(this.rightFoot);

                SetFoot(this.leftFoot);
                SetFoot(this.rightFoot);
            }
        }

        private void UpdateFoot(Foot foot)
        {
            RaycastHit hit;

            float rayMagnitude = this.controller.height/2.0f;
            Vector3 rayPosition = foot.foot.position;
            rayPosition.y += rayMagnitude/2.0f;

            int layerMask = Physics.DefaultRaycastLayers;
            QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

            if (Physics.Raycast(rayPosition, -Vector3.up, out hit, rayMagnitude, layerMask, queryTrigger))
            {
                foot.hit = true;
                foot.height = hit.point.y;
                foot.normal = hit.normal;
            }
            else
            {
                foot.hit = false;
                foot.height = foot.foot.position.y;
            }
        }

        private void SetFoot(Foot foot)
        {
            float weight = foot.GetWeight(this.animator);

            if (foot.hit)
            {
                Vector3 rotationAxis = Vector3.Cross(Vector3.up, foot.normal);
                float angle = Vector3.Angle(transform.up, foot.normal);
                Quaternion rotation = Quaternion.AngleAxis(angle * weight, rotationAxis);

                this.animator.SetIKRotationWeight(foot.footIK, weight);
                this.animator.SetIKRotation(foot.footIK, rotation * this.animator.GetIKRotation(foot.footIK));

                float baseHeight = this.transform.position.y - FOOT_OFFSET_Y;
                float animHeight = (foot.foot.position.y - baseHeight) / (rotation * Vector3.up).y;
                Vector3 position = new Vector3(
                    foot.foot.position.x, 
                    Mathf.Max(foot.height, baseHeight) + animHeight, 
                    foot.foot.position.z
                );

                this.animator.SetIKPositionWeight(foot.footIK, weight);
                this.animator.SetIKPosition(foot.footIK, position);
            }
            else
            {
                this.animator.SetIKPositionWeight(foot.footIK, weight);
                this.animator.SetIKRotationWeight(foot.footIK, weight);
            }
        }

        // WEIGHT COMPENSATION: -------------------------------------------------------------------

        private void WeightCompensationPosition()
        {
            float position = this.controller.transform.position.y + this.defaultOffset;

            if (this.controller.isGrounded)
            {
                float targetHeight = transform.position.y;

                if (this.leftFoot.hit && this.leftFoot.height < targetHeight) targetHeight = this.leftFoot.height;
                if (this.rightFoot.hit && this.rightFoot.height < targetHeight) targetHeight = this.rightFoot.height;

                targetHeight += FOOT_OFFSET_Y;
                if (position > targetHeight)
                {
                    float maxDistance = this.controller.transform.position.y + this.defaultOffset;
                    maxDistance -= this.controller.height * 0.075f;
                    position = Mathf.Max(targetHeight, maxDistance);
                }
            }

            float yAxis = Mathf.SmoothDamp(
                transform.position.y,
                position,
                ref this.speedPosition,
                SMOOTH_POSITION
            );

            transform.position = new Vector3(transform.position.x, yAxis, transform.position.z);
        }

        private void WeightCompensationRotation()
        {
            float pitch = transform.localRotation.eulerAngles.x;

            if (this.controller.isGrounded)
            {
                Vector3 avgNormal = (this.leftFoot.normal + this.rightFoot.normal) * 0.5f;
                avgNormal = transform.InverseTransformDirection(avgNormal).normalized;
                pitch = Mathf.LerpUnclamped(0f, BODY_MAX_INCLINE, -avgNormal.z);
            }

            pitch = Mathf.SmoothDampAngle(
                transform.localRotation.eulerAngles.x,
                pitch,
                ref this.speedRotation,
                SMOOTH_ROTATION
            );

            transform.localRotation = Quaternion.Euler(
                pitch,
                transform.localRotation.eulerAngles.y,
                transform.localRotation.eulerAngles.z
            );
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private Vector3 GetControllerBase()
        {
            Vector3 position = this.controller.transform.TransformPoint(this.controller.center);
            position.y -= (this.controller.height * 0.5f - this.controller.radius);

            return position;
        }
    }
}