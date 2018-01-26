using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePiece : MonoBehaviour {

	public bool canGoUp = true;
	public bool canGoDown = true;
	public bool canGoLeft = true;
	public bool canGoRight = true;
	public int cost = 1;

	public List<TilePiece> vizinhos;

	public bool visitado = false;
	public bool selecionado = false;

	public GameObject red, blue;

	public void Update () {
		if (visitado && !selecionado) {
			red.SetActive (true);
			blue.SetActive (false);
		} else if (selecionado) {
			red.SetActive (false);
			blue.SetActive (true);
		} else {
			red.SetActive (false);
			blue.SetActive (false);
		}
	}
}
