using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror_Choice : MonoBehaviour {

	public GameObject pedestals;

	void OnTriggerEnter(Collider c) {
		if (c.CompareTag ("Player")) {
			switch (this.tag) {
			case "M":
				GameControl.control.gender = false;
				break;
			case "F":
				GameControl.control.gender = true;
				break;
			}

			pedestals.SetActive (true);
			this.transform.parent.transform.parent.gameObject.SetActive (false);
		}
	}
}
