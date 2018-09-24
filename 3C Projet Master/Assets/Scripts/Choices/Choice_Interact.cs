using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice_Interact : MonoBehaviour {

	public GameObject pedestals;
	public GameObject cages;
	public GameObject terrain;

	void Start() {
		terrain = GameObject.Find ("Terrain");
	}

	public void MakeChoice() {
		switch (this.tag) {
		case "red":
			GameControl.control.FaveColor = (int)GameControl.FaveColorChoices.Red;
			pedestals.SetActive (false);
			cages.SetActive (true);
			break;
		case "yellow":
			GameControl.control.FaveColor = (int)GameControl.FaveColorChoices.Yellow;
			pedestals.SetActive (false);
			cages.SetActive (true);
			break;
		case "blue":
			GameControl.control.FaveColor = (int)GameControl.FaveColorChoices.Blue;
			pedestals.SetActive (false);
			cages.SetActive (true);
			break;
		case "green":
			GameControl.control.FaveColor = (int)GameControl.FaveColorChoices.Green;
			pedestals.SetActive (false);
			cages.SetActive (true);
			break;
		case "purple":
			GameControl.control.FaveColor = (int)GameControl.FaveColorChoices.Purple;
			pedestals.SetActive (false);
			cages.SetActive (true);
			break;
		case "rodent":
			GameControl.control.FaveAnimal = (int)GameControl.FaveAnimalChoices.Rodent;
			FallEndDream ();
			break;
		case "bird":
			GameControl.control.FaveAnimal = (int)GameControl.FaveAnimalChoices.Bird;
			FallEndDream ();
			break;
		case "reptile":
			GameControl.control.FaveAnimal = (int)GameControl.FaveAnimalChoices.Reptile;
			FallEndDream ();
			break;
		}


	}

	public void FallEndDream() {
		
		//Change scene after falling for 10 seconds
		GameControl.control.StartACoroutineWhenDestroyed();
		terrain.SetActive (false);

	}



}
