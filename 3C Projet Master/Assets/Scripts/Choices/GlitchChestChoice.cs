using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchChestChoice : MonoBehaviour {

	public PQuestions pquestions;

	void OnMouseDown() {
		pquestions.dataArray [16].Answers = true;
	}
}
