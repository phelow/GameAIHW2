using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
	[SerializeField]private Rigidbody2D m_rigidbody;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			m_rigidbody.AddForce (Vector2.left * Time.deltaTime * 10.0f);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			m_rigidbody.AddForce (Vector2.right * Time.deltaTime * 10.0f);
		}
	}
}
