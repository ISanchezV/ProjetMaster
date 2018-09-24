using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;

public class PersonalityAnswers : MonoBehaviour {

	public PQuestions pquestions;

	public int count = 0;
	public float startTime;
	bool E2 = false;

	public bool playerIsJumpingAround;

	// Use this for initialization
	void Start () {
		Debug.Log (pquestions.dataArray [0].Personalitytype);
		
	}
	
	// Update is called once per frame
	void Update () {
		if (playerIsJumpingAround)
			playerIsJumpingAround = false;
		
		if (!(bool)VariablesManager.GetGlobal("E2") || !(bool)VariablesManager.GetGlobal("E3")) {
			if (Input.GetKeyDown (KeyCode.Space) && count == 0) {
				startTime = Time.time;
				count++;

			} else if (Input.GetKeyDown (KeyCode.Space)) {
				count++;
				if (count > 2 && (Time.time - startTime) <= 1) {
					if (!(bool)VariablesManager.GetGlobal ("E2")) {
						VariablesManager.SetGlobal ("E2", true);
					}
					if (!(bool)VariablesManager.GetGlobal ("E3")) {
						playerIsJumpingAround = true;
					}
				} else {
					if (count > 2)
						count = 0;
				}
			}
		}
	}
}
