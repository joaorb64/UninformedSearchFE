using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tuple<T1, T2>
{
	public T1 First { get; private set; }
	public T2 Second { get; private set; }
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

public static class Tuple
{
	public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
	{
		var tuple = new Tuple<T1, T2>(first, second);
		return tuple;
	}
}

public class Main : MonoBehaviour {

	enum tipoBusca {
		largura,
		profundidade,
		astar,
		guloso,
		custouniforme
	}

	public Level level;

	public GameObject red;
	public GameObject blue;

	public GameObject alm;
	public TilePiece startTile;

	public bool[] visitado;

	public int custo = 0;
	public bool encontrou = false;

	public int visitados = 0;

	public Text txCusto;
	public Dropdown dropdown;

	void Start () {
		foreach (TilePiece tile in level.nodes) {
			tile.red = GameObject.Instantiate (red, tile.transform.position, Quaternion.identity);
			tile.blue = GameObject.Instantiate (blue, tile.transform.position, Quaternion.identity);
		}

		startTile = level.nodes [0];
	}

	void Reset () {
		encontrou = false;
		custo = 0;
		visitados = 0;

		level.ClearVisited ();
	}

	void Update () {
		if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit[] rh = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition));

			foreach (RaycastHit hit in rh) {
				TilePiece tile = hit.collider.gameObject.GetComponent<TilePiece> ();
				if (tile != null) {
					Reset ();

					if (dropdown.value == 0) {
						StartCoroutine(BuscaLargura(startTile, tile));
					}
					else if (dropdown.value == 1) {
						StartCoroutine(BuscaProfundidade(startTile, tile));
					}
					else if (dropdown.value == 2) {
						StartCoroutine(AStar(startTile, tile));
					}
					else if (dropdown.value == 3) {
						StartCoroutine(Guloso(startTile, tile));
					}
					else if (dropdown.value == 4) {
						StartCoroutine(CustoUniforme(startTile, tile));
					}

					break;
				}
			}
		}

		if (Input.GetMouseButtonDown (1) && !EventSystem.current.IsPointerOverGameObject ()) {
			RaycastHit[] rh = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition));

			foreach (RaycastHit hit in rh) {
				TilePiece tile = hit.collider.gameObject.GetComponent<TilePiece> ();
				if (tile != null) {
					Reset ();
					startTile = tile;
					break;
				}
			}
		}

		alm.transform.position = startTile.transform.position + Vector3.up * 0.6f;

		txCusto.text = "Custo: " + custo + "\nVisitados: " + visitados;
	}

	#region BuscaProfundidade

	public IEnumerator BuscaProfundidade (TilePiece curr, TilePiece end) {
		
		curr.visitado = true;

		if (curr == end) {
			encontrou = true;
		}

		yield return new WaitForSeconds(0.01f);

		foreach (TilePiece v in curr.vizinhos) {
			if (v.visitado == false && !encontrou) {
				visitados += 1;
				yield return StartCoroutine (BuscaProfundidade (v, end));
			}
		}
	}

	#endregion

	#region BuscaLargura

	public IEnumerator BuscaLargura (TilePiece curr, TilePiece end) {
		curr.visitado = true;

		List<TilePiece> f = new List<TilePiece> ();
		f.Add (curr);

		while (f.Count > 0) {
			TilePiece v = f [0];

			foreach (TilePiece vizinho in v.vizinhos) {
				if (!encontrou) {
					if (!vizinho.visitado) {
						vizinho.visitado = true;
						visitados += 1;
						f.Add (vizinho);
						if (vizinho == end) {
							encontrou = true;
							yield break;
						}
					}
				}
			}

			f.Remove (v);

			yield return new WaitForSeconds(0.01f);
		}
	}

	#endregion

	#region Guloso

	public IEnumerator Guloso (TilePiece curr, TilePiece end) {

		curr.visitado = true;

		if (curr == end) {
			encontrou = true;
		}

		if (!encontrou) {
			List<TilePiece> vizinhos = new List<TilePiece> ();
			vizinhos.AddRange (curr.vizinhos);

			while(vizinhos.Count > 0 && !encontrou) {
				int i = 0;
				TilePiece vizinhoMenorHeuristica = null;

				for(i = 0; i < vizinhos.Count; i++) {
					if (!vizinhos [i].visitado) {
						if (vizinhoMenorHeuristica == null) {
							vizinhoMenorHeuristica = vizinhos [i];
						} else if (Vector3.Distance (vizinhos [i].transform.position, end.transform.position) / 2.0f <
						           Vector3.Distance (vizinhoMenorHeuristica.transform.position, end.transform.position) / 2.0f) {
							vizinhoMenorHeuristica = vizinhos [i];
						}
					}
				}

				if (vizinhoMenorHeuristica != null) {
					vizinhos.Remove (vizinhoMenorHeuristica);
					custo += vizinhoMenorHeuristica.cost;
					vizinhoMenorHeuristica.visitado = true;
					vizinhoMenorHeuristica.selecionado = true;
					visitados += 1;
					yield return new WaitForSeconds (0.02f);
					yield return StartCoroutine (Guloso (vizinhoMenorHeuristica, end));
				}
				else {
					break;
				}
			}
		}
	}

	#endregion

	#region AStar

	public IEnumerator AStar (TilePiece curr, TilePiece end) {

		List<TilePiece> openSet = new List<TilePiece> ();
		openSet.Add (curr);

		TilePiece[] cameFrom = new TilePiece[level.nodes.Count];

		for (int i = 0; i < cameFrom.Length; i++)
			cameFrom[i] = null;

		float[] gScore = new float[level.nodes.Count];

		for (int i = 0; i < gScore.Length; i++)
			gScore[i] = 99999;
		
		gScore[level.nodes.FindIndex (d => d == curr)] = 0;

		float[] fScore = new float[level.nodes.Count];

		for (int i = 0; i < fScore.Length; i++)
			fScore[i] = 99999;

		fScore[level.nodes.FindIndex (d=>d == curr)] = (Vector3.Distance(curr.transform.position, end.transform.position) / 2.0f);

		while (openSet.Count > 0) {
			float menor = 99999;
			int menori = -1;

			for (int i = 0; i < openSet.Count; i++) {
				int id = level.nodes.FindIndex (d => d == curr);

				if (fScore [id] < menor) {
					menor = fScore [id];
					menori = i;
				}
			}

			TilePiece current = openSet [menori];

			if (current == end) {
				encontrou = true;

				TilePiece t = end;

				while (t != curr) {
					t.selecionado = true;
					custo += t.cost;
					t = cameFrom[level.nodes.FindIndex (d=>d == t)];
				}

				yield break;
			}

			openSet.Remove (current);

			//Para cada vizinho
			foreach (TilePiece v in current.vizinhos) {

				// A distância do início até o vizinho
				float tentativeGScore = gScore[level.nodes.FindIndex(d=>d == current)] + v.cost;

				if (tentativeGScore >= gScore [level.nodes.FindIndex(d=>d == v)])
					continue; // Esse caminho não é melhor

				// Descobriu um nodo novo
				if (!openSet.Contains (v)) {
					visitados += 1;
					openSet.Add (v);
					v.visitado = true;
				}

				// Melhor caminho até agora. Salvar o caminho
				cameFrom[level.nodes.FindIndex (d=>d == v)] = current;

				gScore [level.nodes.FindIndex (d=>d == v)] = tentativeGScore;

				fScore [level.nodes.FindIndex (d=>d == v)] = gScore [level.nodes.FindIndex(d=>d == v)]
					+ (Vector3.Distance (v.transform.position, end.transform.position) / 2.0f);
			}

			yield return new WaitForSeconds (0.01f);
		}
	}

	#endregion

	#region CustoUniforme

	public IEnumerator CustoUniforme (TilePiece curr, TilePiece end) {
		curr.visitado = true;

		List<Tuple<int, List<TilePiece>>> rotas = new List<Tuple<int, List<TilePiece>>> ();

		rotas.Add (new Tuple<int, List<TilePiece>> (0, new List<TilePiece>()));
		rotas [0].Second.Add (curr);

		while (!encontrou) {
			// Seleciona a rota mais curta conhecida
			int menori = 0;
			int menor = rotas[0].First;

			for (int i = 1; i < rotas.Count; i++) {
				if (rotas [i].First < menor) {
					menori = i;
					menor = rotas [i].First;
				}
			}

			Tuple<int, List<TilePiece>> rota = rotas[menori];

			// Se a rota chega ao fim, finaliza
			if (rota.Second [rota.Second.Count - 1] == end) {
				encontrou = true;

				foreach (TilePiece t in rota.Second) {
					t.selecionado = true;
					custo += t.cost;
				}

				custo -= rota.Second [0].cost;

				yield break;
			}
			// Se a rota não chega ao fim
			else {
				rotas.Remove (rota);

				foreach (TilePiece vizinho in rota.Second [rota.Second.Count - 1].vizinhos) {
					if (!encontrou) {
						if (!vizinho.visitado) {
							vizinho.visitado = true;
							visitados += 1;

							List<TilePiece> novaRota = new List<TilePiece>();
							novaRota.AddRange (rota.Second);
							novaRota.Add (vizinho);

							rotas.Add (
								new Tuple<int, List<TilePiece>> (
									rota.First + vizinho.cost, novaRota
								)
							);
						}
					}
				}
			}

			yield return new WaitForSeconds(0.01f);
		}
	}

	#endregion
}
