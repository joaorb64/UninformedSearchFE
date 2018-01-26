using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTextureScroll : MonoBehaviour {

	public Material mat;
	Vector2 offset;

	// Use this for initialization
	void Start () {
		mat = GetComponent<MeshRenderer> ().material;
		offset = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		offset += new Vector2 (-0.001f, 0f);
		mat.mainTextureOffset += new Vector2 (0.001f, 0.0005f);
		mat.SetTextureOffset("_DetailAlbedoMap", offset);
	}
}
