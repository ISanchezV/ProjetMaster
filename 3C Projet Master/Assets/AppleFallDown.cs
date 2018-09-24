using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;

public class AppleFallDown : MonoBehaviour {

	public float appleCount = 0;
	Rigidbody rb;

	void OnTriggerEnter(Collider c) {
		if (c.CompareTag ("Rock")) {
			appleCount++;
			VariablesManager.SetGlobal ("AppleCount", appleCount);
		}
	}

	void OnCollisionEnter(Collision col) {
		if (col.collider.CompareTag ("Rock")) {
			transform.SetParent (null);
			rb.useGravity = true;

		}
	}

	void Start() {
		rb = GetComponent<Rigidbody> ();
	}


	void Update() {
		if (appleCount == 4) {
			GetComponent<Collider> ().isTrigger = false;
		}
	}


}
