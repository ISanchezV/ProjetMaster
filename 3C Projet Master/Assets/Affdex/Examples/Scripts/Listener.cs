using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Affdex;
using GameCreator.Variables;

public class Listener : ImageResultsListener
{
	public float browFurrow;
	public float valence;
	public float lastValence;
	public float anger;

	public int moodSwing;

    //public Text textArea;
    public override void onFaceFound(float timestamp, int faceId)
    {
        Debug.Log("Found the face");
    }

    public override void onFaceLost(float timestamp, int faceId)
    {
        Debug.Log("Lost the face");
    }
    
    public override void onImageResults(Dictionary<int, Face> faces)
    {
		//Debug.Log ("Got face results");

        if (faces.Count > 0)
        {
			
			//Check for N4
			if (moodSwing < 4) {
				faces [0].Emotions.TryGetValue (Emotions.Valence, out valence);
				if (lastValence - valence > 20) {
					moodSwing++;
					if (!(bool)VariablesManager.GetGlobal ("N4") && moodSwing == 3) {
						VariablesManager.SetGlobal ("N4", true);
					}
				}
			}

			//Check for N3
			if ((bool)VariablesManager.GetGlobal ("CheckN3")) {
				//Debug.Log ("Starting now");
				faces [0].Emotions.TryGetValue (Emotions.Anger, out anger);
				faces [0].Expressions.TryGetValue (Expressions.BrowFurrow, out browFurrow);
				if (anger > 15 || browFurrow > 25) {
					if (!(bool)VariablesManager.GetGlobal ("N3")) {
						VariablesManager.SetGlobal ("N3", true);
					}

				}
			} 

			faces [0].Emotions.TryGetValue (Emotions.Valence, out lastValence);
        }
       
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}