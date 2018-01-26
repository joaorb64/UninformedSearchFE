using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	public List<TilePiece> nodes;

	void Awake () {
		nodes = new List<TilePiece> ();

		foreach(TilePiece t in transform.GetComponentsInChildren<TilePiece>()){

			t.vizinhos = new List<TilePiece> ();

			if (t.canGoUp) {
				RaycastHit[] rh = Physics.RaycastAll (t.transform.position + Vector3.forward * 2 + Vector3.up * 10, Vector3.down);

				foreach (RaycastHit hit in rh) {
					TilePiece other = hit.collider.gameObject.GetComponent<TilePiece> ();
					if (other != null) {
						if (other.canGoDown) {
							t.vizinhos.Add(other);
						}
						break;
					}
				}
			}

			if (t.canGoDown) {
				RaycastHit[] rh = Physics.RaycastAll (t.transform.position + Vector3.back * 2 + Vector3.up * 10, Vector3.down);

				foreach (RaycastHit hit in rh) {
					TilePiece other = hit.collider.gameObject.GetComponent<TilePiece> ();
					if (other != null) {
						if (other.canGoUp) {
							t.vizinhos.Add(other);
						}
						break;
					}
				}
			}

			if (t.canGoLeft) {
				RaycastHit[] rh = Physics.RaycastAll (t.transform.position + Vector3.left * 2 + Vector3.up * 10, Vector3.down);

				foreach (RaycastHit hit in rh) {
					TilePiece other = hit.collider.gameObject.GetComponent<TilePiece> ();
					if (other != null) {
						if (other.canGoRight) {
							t.vizinhos.Add(other);
						}
						break;
					}
				}
			}

			if (t.canGoRight) {
				RaycastHit[] rh = Physics.RaycastAll (t.transform.position + Vector3.right * 2 + Vector3.up * 10, Vector3.down);

				foreach (RaycastHit hit in rh) {
					TilePiece other = hit.collider.gameObject.GetComponent<TilePiece> ();
					if (other != null) {
						if (other.canGoLeft) {
							t.vizinhos.Add(other);
						}
						break;
					}
				}
			}

			nodes.Add (t);
		}
	}

	public void ClearVisited () {
		foreach(TilePiece t in nodes){
			t.visitado = false;
			t.selecionado = false;
		}
	}
}
