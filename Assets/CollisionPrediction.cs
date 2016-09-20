using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CollisionPrediction : PathFollow {

	protected List<GameObject> m_tracking; 
	[SerializeField]protected float m_coneThreshold;

	[SerializeField]private float m_avoidOthersRatio = .7f;
	[SerializeField]private float m_followPathRatio = .3f;
	protected Vector3 com;

	protected Vector3 m_avoidOthersTorque;

	// Use this for initialization
	void Awake () {
		m_tracking = new List<GameObject>();

		StartCoroutine (MasterMove ());
		StartCoroutine (AvoidOthers());
		StartCoroutine (FollowPath (m_firstNode));
	}

	protected override IEnumerator MasterMove(){
		while (true) {
			m_rigidBody.AddForce (m_pathFollowForce);

			Vector3 Torque = new Vector3(0,0,0);

			if (m_avoidOthersTorque.magnitude > .1f) {
				Torque = m_pathFollowTorque * m_followPathRatio + m_avoidOthersTorque * m_avoidOthersRatio;
			} else {
				Torque = m_pathFollowTorque;
			}

			if (Torque.magnitude > m_maxAngularAcceleration) {
				Torque = Torque.normalized * m_maxAngularAcceleration;
			}
			m_rigidBody.AddTorque (Torque);
			CheckSpeed ();
			yield return new WaitForEndOfFrame ();
		}
	}

	void OnDrawGizmos(){
		Gizmos.DrawSphere (com, .1f);
	}

	protected virtual IEnumerator AvoidOthers(){
		while (true) {
			yield return new WaitForEndOfFrame ();

			Vector3 centerOfMass = Vector3.zero;
			int evading = 0;
			foreach (GameObject go in m_tracking) {
				//if there are characters within the cone do the steering
				if (Vector3.Dot (transform.up, go.transform.position - transform.position) < m_coneThreshold) {
					centerOfMass = centerOfMass + go.transform.position;
					evading++;
				}
			}
		}
	}


	void OnTriggerEnter(Collider coll)
	{
		m_tracking.Add (coll.gameObject);
	}

	void OnTriggerExit(Collider coll){
		m_tracking.Remove (coll.gameObject);
	}
}
