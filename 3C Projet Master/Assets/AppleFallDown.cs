using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;

public class AppleFallDown : MonoBehaviour {

	public float appleCount = 0;
	Rigidbody rb;

	void OnCollisionEnter(Collision col) {
		if (col.collider.CompareTag ("Rock")) {
			if (appleCount == 4) {
				transform.SetParent (null);
				rb.useGravity = true;
				rb.isKinematic = false;
			} else {
				appleCount++;
				VariablesManager.SetGlobal ("AppleCount", appleCount);
			}

		}
	}

	void Start() {
		rb = GetComponent<Rigidbody> ();

	}




}
