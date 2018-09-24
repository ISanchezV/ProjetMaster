using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class CharaController: MonoBehaviour {

	public float walkSpeed = 4.0f;
	public float runSpeed = 8.0f;
	public float crouchSpeed = 2.0f;
	public float speedRotX = 7.0f;
	public float speedRotY = 5.0f;
	public float maxLookUp = 30.0f;
	public float maxLookDown = -50.0f;

	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;

	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;

	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;

	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 1;

	private Vector3 moveDirection = Vector3.zero;
	private Vector3 contactPoint;

	private GameObject camera;
	private CharacterController controller;
	private Transform myTransform;
	private RaycastHit hit;

	private bool falling;
	private bool grounded = false;
	private bool playerControl = false;

	private float speed, fallStartLevel, rayDistance, yaw, pitch;
	private int jumpTimer;

	void Start() {
		controller = GetComponent<CharacterController>();
		myTransform = transform;
		speed = walkSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		jumpTimer = antiBunnyHopFactor;
		camera = GameObject.FindWithTag ("MainCamera");
	}

	void FixedUpdate() {
		float inputX = Input.GetAxis("Horizontal");
		float inputY = Input.GetAxis("Vertical");
		float xAxis = Input.GetAxis ("Mouse X");
		float yAxis = Input.GetAxis ("Mouse Y");
		// If diagonal movement, limit speed so the total doesn't exceed normal move speed
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f)? .7071f : 1.0f;

		if (grounded) {
			// If we were falling, and we fell a vertical distance greater than the threshold, we get a movement penalty
			if (falling) {
				falling = false;
				if (myTransform.position.y < fallStartLevel - fallingDamageThreshold)
					FallingAlert (fallStartLevel - myTransform.position.y);
			}

			//Reset of the speed value after the last frame
			speed = walkSpeed;

			//If the Run button is being pressed, set speed to running speed
			if (Input.GetButton("Run"))
				speed = runSpeed;
			//If the Crouch button is being pressed, set speed to crouching speed and isCrouching to true
			if (Input.GetButton ("Crouch")) {
				speed = crouchSpeed;
				GameControl.control.isCrouching = true;
			} else {
				if (GameControl.control.isCrouching)
					GameControl.control.isCrouching = false;
			}

			//Moving the player
			moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
			moveDirection = myTransform.TransformDirection(moveDirection) * speed;
			playerControl = true;

			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (!Input.GetButton("Jump"))
				jumpTimer++;
			else if (jumpTimer >= antiBunnyHopFactor) {
				moveDirection.y = jumpSpeed;
				jumpTimer = 0;
			}
		}
		else {
			// Set the height at which we started falling
			if (!falling) {
				falling = true;
				fallStartLevel = myTransform.position.y;
			}

		}

		// Apply gravity
		moveDirection.y -= gravity * Time.deltaTime;

		// Move the controller, and set grounded 
		grounded = (controller.Move(moveDirection * Time.deltaTime) & CollisionFlags.Below) != 0;

		//Rotation
		yaw += speedRotX * xAxis;
		pitch -= speedRotY * yAxis;
		//Locking camera values
		if (pitch < maxLookDown)
			pitch = maxLookDown;
		if (pitch > maxLookUp)
			pitch = maxLookUp;
		transform.eulerAngles = new Vector3 (0, yaw, 0);
		camera.transform.eulerAngles = new Vector3 (pitch, yaw, 0);
	}

	// Store point that we're in contact with for use in FixedUpdate if needed
	void OnControllerColliderHit (ControllerColliderHit hit) {
		contactPoint = hit.point;
	}

	//If the player falls from too high, he gets a movement penalty: no moving for a few seconds
	void FallingAlert (float fallDistance) {
		print ("Ouch! Fell " + fallDistance + " units!");
	}
}