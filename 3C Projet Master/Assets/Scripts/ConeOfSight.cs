using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;

public class ConeOfSight : MonoBehaviour {

	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle = 110f;
	//public bool playerInsight;
	//public Vector3 personalLastSighting;

	public LayerMask playerLayerMask;
	public PersonalityAnswers pAnswers;

	void Start() {
		pAnswers = GameObject.FindGameObjectWithTag ("GameController").GetComponent<PersonalityAnswers> ();
	}

	void Update() {
		if (pAnswers.playerIsJumpingAround)
			FindVisibleTargets ();
	}

	public void FindVisibleTargets() {
		Collider[] targetsInViewRadius = Physics.OverlapSphere (transform.position, viewRadius, playerLayerMask);

		//We only check if we can see the player if he's within our sight sphere
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius [i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;

			//If we are looking in the direction of the player
			if (Vector3.Angle (transform.forward, dirToTarget) < viewAngle / 2) {
				float dstToTarget = Vector3.Distance (transform.position, target.position);

				//If there are no obstacles between us and the player, then we can see him
				if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, 0)) {
					WhoSeesPlayer ();
				}
			}
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
	}

	void WhoSeesPlayer() {
		switch (this.tag) {
		case "Priest":
		case "Guard":
			if (!(bool)VariablesManager.GetGlobal ("E3")) {
				VariablesManager.SetGlobal ("E3", true);
				this.GetComponent<GameCreator.Core.Actions> ().Execute ();
			}
			break;

		}
	}


}
