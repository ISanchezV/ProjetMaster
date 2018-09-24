using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator;
using GameCreator.Variables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TrajectoryPredictor))]

public class InstantiateThrowableObject : MonoBehaviour {

	public Transform newParent;
	TrajectoryPredictor tp;
	Rigidbody rb;

	public float velocity = 10f;

	public GameObject outline;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		outline = gameObject.transform.GetChild (0).transform.GetChild (0).gameObject;

		VariablesManager.SetLocal(this.gameObject, "Throwing", VariablesManager.GetGlobal("ThrowingSomething"), true);

		if (!outline.activeInHierarchy) {
			//outline.enabled = false;
			tp = GetComponent<TrajectoryPredictor> ();
			tp.drawDebugOnPrediction = true;

			newParent = GameObject.FindGameObjectWithTag ("ThrowPoint").transform;
			rb.isKinematic = true;
			transform.SetParent (newParent);
			transform.position = newParent.transform.position;
			transform.rotation = newParent.transform.rotation;
		}
	}

	void Update() {
		if (!outline.activeInHierarchy) {
			if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
				velocity += Input.GetAxis ("Mouse ScrollWheel") * 10;
			}

			if (velocity > 50) {
				velocity = 50;
			} else if (velocity < 0) {
				velocity = 0;
			}

			if (Input.GetKeyDown (KeyCode.I) || Input.GetKeyDown(KeyCode.Escape)) {
				switch(this.tag) {
				case "Rock":
					GameCreator.Inventory.InventoryManager.Instance.AddItemToInventory (217416404, 1);
					Destroy (this.gameObject);
					break;
				}
			}

			if (Input.GetKeyDown (KeyCode.E)) {
				Throw();
			}
		}

	}

	void LateUpdate() {
		if (!outline.activeInHierarchy) {
			tp.debugLineDuration = Time.unscaledDeltaTime;
			tp.Predict3D (transform.position, transform.forward * velocity, Physics.gravity, 0);
		}

	}

	void Throw() {
		rb.isKinematic = false;
		rb.AddForce(transform.forward * velocity, ForceMode.Impulse);
		transform.parent = null;
		tp.drawDebugOnPrediction = false;
		VariablesManager.SetLocal(this.gameObject, "Throwing", false, true);
		VariablesManager.SetGlobal("ThrowingSomething", false);
		outline.SetActive(true);
	}

}
