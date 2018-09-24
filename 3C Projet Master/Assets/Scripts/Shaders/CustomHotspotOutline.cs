using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHotspotOutline : MonoBehaviour {

	public Shader outlineShader;
	public GameObject objectToOutline;
	Shader[] initialShaders;
	float lineWidth = 1f;

	/*void Start() {
		outlineShader = (Shader)Resources.Load ("Materials/CustomOutline.shader");
	}*/

	void OnTriggerEnter(Collider c) {
		if (c.CompareTag ("Player")) {
			Material[] objectMaterials = objectToOutline.GetComponent<MeshRenderer> ().materials;
			initialShaders = new Shader[objectMaterials.Length];
			for (int i=0; i < objectMaterials.Length; i++) {
				initialShaders [i] = objectMaterials [i].shader;
			}

			switch (objectToOutline.tag) {
			case "Chest":
				lineWidth = 0.005f;
				break;
			default:
				lineWidth = 1f;
				break;
			}

			for (int i=0; i < objectMaterials.Length; i++) {
				objectMaterials [i].shader = outlineShader;
				objectMaterials [i].SetFloat ("_OutlineWidth", lineWidth);
				objectMaterials [i].SetColor ("_OutlineColor", Color.red);
			}
		}
	}

	void OnTriggerExit(Collider c) {
		if (c.CompareTag ("Player")) {
			Material[] objectMaterials = objectToOutline.GetComponent<MeshRenderer> ().materials;
			for (int i=0; i < objectMaterials.Length; i++) {
				objectMaterials [0].shader = initialShaders[i];
			}
		}
	}
}
