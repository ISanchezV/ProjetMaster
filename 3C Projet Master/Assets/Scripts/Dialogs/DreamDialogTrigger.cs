using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DreamDialogTrigger : MonoBehaviour {

	public string dialog;
	public Text textBox;

	void OnTriggerEnter(Collider c) {
		if (c.CompareTag ("Player")) {
			textBox.text = dialog;
		}
	}
}
