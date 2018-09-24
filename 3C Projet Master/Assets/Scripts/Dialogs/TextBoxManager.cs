using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextBoxManager : MonoBehaviour {

	public GameObject dialogo;
	//public GameObject sigue;

	//El objeto texto en el canvas
	public Text text;

	//El archivo de texto
	public TextAsset textFile;

	//public List<string> lines;
	public string[] lines;
	public string temp;
	public int currentLine;
	public int endLine;
	public int counter, c;
	public bool show, cut, startNewLine;
	public float lineTime;

	// Use this for initialization
	void Start () {
		
		//Si el archivo existe, lo cargamos y se divide en líneas que se guardan en una lista
		if (textFile != null) {

			lines = textFile.text.Split ('\n');
		}

		//endLine es la última línea que se carga
		if (endLine == 0) {
			endLine = lines.Length - 1;
		}



		//dialogo.SetActive (true);
	}

	void Update () {
		//El texto en la pantalla es el de la línea actual que se saca de la lista de líneas
		text.text = lines [currentLine];

		/*if (!cut) {
			temp = lines [currentLine];
			lineTime = Time.time;
			show = false;
			cut = true;
		}

		if (cut && counter < temp.Length) {
			if ((Time.time - lineTime) >= 0.1f) {
				text.text = text.text + temp [counter];
				lineTime = Time.time;
				counter++;
			}
		} else {
			if (!show)
				show = true;
			//if (!sigue.activeInHierarchy)
				//sigue.SetActive (true);
			if (startNewLine) {
				//sigue.SetActive (false);
				counter = 0;
				cut = false;
				startNewLine = false;
			}
		}*/

		/* Apretando E el texto avanza hasta enseñar toda la línea.
		 * La siguiente vez, enseña la línea de después*/
		if (Input.GetKeyDown (KeyCode.E)) {
			if (!show) {
				text.text = text.text + temp.Remove(0, counter);
				counter = temp.Length;
				//sigue.SetActive (true);
				show = true;
			} else {
				//Si esta línea no es la última del archivo
				if (currentLine < endLine) {
					//sigue.SetActive (false);
					currentLine++;
					//c++;
					startNewLine = true;
					//Si es la tercera línea que sale en la pantalla, salta
					/*if (c == 3) {
						text.text = "";
						c = 0;
					} else {*/
						text.text = "";
					//

					show = false;
				} else if (currentLine == endLine) {
					text.text = "";
					dialogo.SetActive (false);
				}
			}
		}
	}
		

}
