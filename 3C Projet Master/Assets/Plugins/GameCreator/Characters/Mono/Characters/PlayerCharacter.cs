namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.AI;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("Game Creator/Characters/Player Character", 100)]
    public class PlayerCharacter : Character
    {
        public enum INPUT_TYPE
        {
            PointAndClick,
            Directional,
            FollowPointer
        }

        public enum MOUSE_BUTTON
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }

        private const string AXIS_H = "Horizontal";
        private const string AXIS_V = "Vertical";

        // PROPERTIES: ----------------------------------------------------------------------------

        public INPUT_TYPE inputType = INPUT_TYPE.Directional;
        public MOUSE_BUTTON mouseButtonMove = MOUSE_BUTTON.LeftClick;
        public KeyCode jumpKey = KeyCode.Space;

        private bool uiConstrained = false;

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            HookPlayer hookPlayer = gameObject.GetComponent<HookPlayer>();
            if (hookPlayer == null) gameObject.AddComponent<HookPlayer>();

            this.CharacterAwake();
        }

        // UPDATE: --------------------------------------------------------------------------------

        private void Update()
        {
            switch (this.inputType)
            {
                case INPUT_TYPE.Directional: this.UpdateInputDirectional(); break;
                case INPUT_TYPE.PointAndClick: this.UpdateInputPointClick(); break;
                case INPUT_TYPE.FollowPointer: this.UpdateInputFollowPointer(); break;
            }

            if (this.IsControllable())
            {
                if (Input.GetKeyDown(this.jumpKey)) this.Jump();
            }

            this.CharacterUpdate();
        }

        private void UpdateInputDirectional()
        {
            Vector3 direction = Vector3.zero;
            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                direction = new Vector3(touchDirection.x, 0.0f, touchDirection.y);
            }
            else
            {
                direction = new Vector3(
                    Input.GetAxis(AXIS_H),
                    0.0f,
                    Input.GetAxis(AXIS_V)
                );
            }

            Camera maincam = this.GetMainCamera();
            if (maincam == null) return;

            Vector3 moveDirection = maincam.transform.TransformDirection(direction);
            moveDirection.Scale(new Vector3(1, 0, 1));
            moveDirection.Normalize();
            this.characterLocomotion.SetDirectionalDirection(moveDirection);
        }

        private void UpdateInputPointClick()
        {
            if (!this.IsControllable()) return;
            this.UpdateUIConstraints();

            if (Input.GetMouseButtonDown((int)this.mouseButtonMove) && !this.uiConstrained)
            {
                Camera maincam = this.GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);
                this.characterLocomotion.SetTarget(cameraRay, null, null);
            }
        }

        private void UpdateInputFollowPointer()
        {
            if (!this.IsControllable()) return;
            this.UpdateUIConstraints();

            if (Input.GetMouseButton((int)this.mouseButtonMove) && !this.uiConstrained)
            {
                if (HookPlayer.Instance == null) return;

                Camera maincam = this.GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);

                Transform player = HookPlayer.Instance.transform;
                Plane groundPlane = new Plane(Vector3.up, player.position);
                float rayDistance = 0.0f;

                if (groundPlane.Raycast(cameraRay, out rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    if (Vector3.Distance(player.position, cursor) >= 0.05f)
                    {
                        Vector3 target = Vector3.MoveTowards(player.position, cursor, 1f);
                        this.characterLocomotion.SetTarget(target, null, null);
                    }
                }
            }
        }

		private Camera GetMainCamera()
		{
			if (HookCamera.Instance != null) return HookCamera.Instance.Get<Camera>();
			if (Camera.main != null) return Camera.main;

			Debug.LogError(ERR_NOCAM);
			return null;
		}

        private void UpdateUIConstraints()
        {
            int buttonID = (int)this.mouseButtonMove;
            if (Input.GetMouseButtonDown(buttonID))
            {
                bool pointerOverUI = EventSystem.current.IsPointerOverGameObject(buttonID);
                this.uiConstrained = pointerOverUI;
            }

            #if UNITY_IOS || UNITY_ANDROID
            for (int i = 0; i < Input.touches.Length; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                int fingerID = Input.GetTouch(i).fingerId;
                bool pointerOverUI = EventSystem.current.IsPointerOverGameObject(fingerID);
                if (pointerOverUI) this.uiConstrained = true;
            }
            #endif
        }
	}
}