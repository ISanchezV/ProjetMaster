using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OpenDialogue : MonoBehaviour {

	public TextBoxManager mng;

	//El objeto texto en el canvas que vamos a usar
	public Text text;

	//El archivo de texto
	public TextAsset textFile;

	public bool startText;

	void OnTriggerEnter2D (Collider2D c) {
		if (c.tag == "Player") {
			mng = c.GetComponent<TextBoxManager> ();

			//Ahora le regalamos al cuadro de diálogo el texto que queremos que use esta vez
			mng.textFile = textFile;
			mng.text = text;

			mng.enabled = true;
		}
	}

	void OnTriggerExit2D (Collider2D c) {
		if (c.tag == "Player") {
			//Reseteamos todos los valores en el script del jugador para los diálogos y lo apagamos
			mng.text.text = "";
			mng.currentLine = 0;
			mng.counter = 0;
			mng.c = 0;
			mng.cut = false;
			mng.show = false;
			mng.dialogo.SetActive (false);
			mng.enabled = false;
		
		}
	}
}
