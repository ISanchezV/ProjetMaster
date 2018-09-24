﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (ConeOfSight))]
public class FOVEditor : Editor {

	void OnSceneGUI() {
		ConeOfSight fov = (ConeOfSight)target;
		Handles.color = Color.white;
		Handles.DrawWireArc (fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
		Vector3 viewAngleA = fov.DirFromAngle (-fov.viewAngle / 2, false);
		Vector3 viewAngleB = fov.DirFromAngle (fov.viewAngle / 2, false);

		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
		Handles.DrawLine (fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);
	}
}