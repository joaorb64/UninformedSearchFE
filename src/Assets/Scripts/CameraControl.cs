using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Translate (-transform.right * 0.2f, Space.World);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Translate (transform.right * 0.2f, Space.World);
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.Translate (Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized  * 0.2f, Space.World);
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			transform.Translate (Vector3.ProjectOnPlane(-transform.forward, Vector3.up).normalized  * 0.2f, Space.World);
		}

		transform.Translate (Vector3.forward * Input.mouseScrollDelta [1] * 4 * 0.4f);
	}
}
