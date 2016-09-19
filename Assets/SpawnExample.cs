using UnityEngine;
using System.Collections;

public class SpawnExample : MonoBehaviour {
	[SerializeField]private GameObject m_example;
	[SerializeField]private float m_runTime = 120.0f;


	// Use this for initialization
	public void OnMouseDown () {
		StartCoroutine (ManageExample ());	
	}

	private IEnumerator ManageExample(){
		GameObject example = GameObject.Instantiate (m_example, transform) as GameObject;
		example.transform.localPosition = Vector3.zero;
		yield return new WaitForSeconds (m_runTime);
		Destroy (example);
	}

}
