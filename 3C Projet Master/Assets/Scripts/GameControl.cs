using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameCreator.Variables;

public class GameControl : MonoBehaviour {

	//There is only one object GameControl, and it's always the same between the scenes
	public static GameControl control;

	//Global variables accessible from all the objects in the game
	public float playerHeight = 2.0f;
	public float crouchHeight = 1.0f;
	public bool isCrouching;
	public bool holding = false;
	public GameObject objHeld;
	public GameObject itemPrefab;
	//All the objects the player owns, the ID helps identifying which object we use each time
	public List<GameObject> inventory = new List<GameObject> ();

	private GameObject player;

	//List of tags for each category
	public List<String> interactable = new List<String>();

	//The different choices the player makes are stored here as variables accessible by all scripts
	public bool gender; //0 = Male, 1 = Female
	public enum FaveColorChoices {Red, Yellow, Blue, Green, Purple};
	public enum FaveAnimalChoices {Bird, Rodent, Reptile};
	public int FaveColor = -1;
	public int FaveAnimal = -1;


	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
		VariablesManager.SetGlobal ("ThrowingSomething", false);
	}

	void Awake () {

		//If the permanent object doesn't exist
		if (control == null) {

			//Don't destroy this object when changing scenes
			DontDestroyOnLoad (gameObject);

			//The permanent object is the one with this script
			control = this;

			//If the permanent object isn't the one with this script
		} else if (control != this) {

			//Destroy this object
			Destroy (gameObject);
		}
	}
		

	void Update() {
		if (player == null)
			player = GameObject.FindGameObjectWithTag ("Player");
		
		/*if (isCrouching)
			Crouch ();
		else
			GetUp ();*/
	}

	/*void Crouch() {
		if (player.GetComponent<CapsuleCollider> ().height != crouchHeight)
			player.GetComponent<CapsuleCollider> ().height = crouchHeight;
		if (player.GetComponent<CharacterController> ().height != crouchHeight)
			player.GetComponent<CharacterController> ().height = crouchHeight;
	}
		
	void GetUp() {
		if (player.GetComponent<CapsuleCollider> ().height != playerHeight)
			player.GetComponent<CapsuleCollider> ().height = playerHeight;
		if (player.GetComponent<CharacterController> ().height != playerHeight)
			player.GetComponent<CharacterController> ().height = playerHeight;
	}
*/
	public void StartACoroutineWhenDestroyed() {
		StartCoroutine(ChangeSceneAfter10("Camp"));
	}


	public IEnumerator ChangeSceneAfter10(string sceneName) {
		//TODO the text
		yield return new WaitForSeconds(3);
		SceneManager.LoadScene ("Camp");
	}
}