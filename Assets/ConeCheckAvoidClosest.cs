using UnityEngine;
using System.Collections;

public class ConeCheckAvoidClosest : ConeCheckAvoidAll {
	protected override IEnumerator AvoidOthers(){
		while (true) {
			yield return new WaitForEndOfFrame ();

			Vector3 avoidancePosition = Vector3.zero;
			int evading = 0;
			float closestDistance = Mathf.Infinity;

			foreach (GameObject go in m_tracking) {

				float thisDistance = Vector3.Distance (go.transform.position, this.transform.position);

				//if there are characters within the cone do the steering
				if (Vector3.Dot (transform.up, go.transform.position - transform.position) < m_coneThreshold && thisDistance < closestDistance) {
					closestDistance = thisDistance;
					avoidancePosition = go.transform.position;
					evading++;
				}
			}


			if (evading == 0) {
				m_avoidOthersTorque = new Vector3 (0, 0, 0);
				continue;
			}

			avoidancePosition = avoidancePosition * 1 / evading * Time.deltaTime * 100.0f;

			com = avoidancePosition;

			//else return with no steering


			//add rotation away from the center of mass

			Vector3 direction = (avoidancePosition  - this.transform.position).normalized * -1;

			Vector3 headingDistance = Vector3.Cross (transform.up, direction);

			Vector3 Torque = Vector3.Cross (transform.up, direction);

			Torque = Torque.normalized * m_maxAngularAcceleration;

			//Scale down the torque to prevent overshooting of the target

			Torque = Torque * Mathf.Lerp (0.0f, 1.0f, headingDistance.magnitude - m_rigidBody.angularVelocity.magnitude) * Time.deltaTime * 100.0f;
			Debug.DrawRay (transform.position, Torque, Color.red);
			m_avoidOthersTorque = Torque;
		}
	}
}
