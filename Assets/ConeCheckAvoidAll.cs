using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConeCheckAvoidAll : PathFollow {

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


			if (evading == 0) {
				m_avoidOthersTorque = new Vector3 (0, 0, 0);
				continue;
			}

			centerOfMass = centerOfMass * 1 / evading;

			com = centerOfMass;

			//else return with no steering


			//add rotation away from the center of mass

			Vector3 direction = (centerOfMass  - this.transform.position).normalized * -1;

			Vector3 headingDistance = Vector3.Cross (transform.up, direction);

			Vector3 Torque = Vector3.Cross (transform.up, direction);
			Torque = Torque.normalized * m_maxAngularAcceleration;

			//Scale down the torque to prevent overshooting of the target

			Torque = Torque * Mathf.Lerp (0.0f, 1.0f, headingDistance.magnitude - m_rigidBody.angularVelocity.magnitude);

			Debug.DrawRay (transform.position,Torque,Color.red);

			m_avoidOthersTorque = Torque;
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
