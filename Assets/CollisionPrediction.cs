using UnityEngine;
using System.Collections;

public class CollisionPrediction : ConeCheckAvoidAll {
	[SerializeField]protected float m_collisionTolerance = 2.0f;

	private Vector3 WhereTheseHit(GameObject a,GameObject b){



		Rigidbody rbA = a.GetComponent<Rigidbody> ();
		Rigidbody rbB = b.GetComponentInChildren<Rigidbody> ();

		Vector3 closestPoint = Vector3.zero;

		float closestDistance = m_collisionTolerance;

		for (float futureTime = 0.0f; futureTime < 4.0f; futureTime += 0.1f) {
			Vector3 posA = a.transform.position + rbA.velocity * futureTime;
			Vector3 posB = b.transform.position + rbB.velocity * futureTime;

			float distance = Vector3.Distance (posA, posB);

			if (distance < closestDistance) {
				closestDistance = distance;

				closestPoint = posB;
			}

		}


		return closestPoint;
	}

	protected override IEnumerator AvoidOthers(){
		while (true) {
			yield return new WaitForEndOfFrame ();

			Vector3 avoidancePosition = Vector3.zero;
			int evading = 0;
			float closestDistance = m_collisionTolerance;

			foreach (GameObject go in m_tracking) {

				float thisDistance = Vector3.Distance (go.transform.position, this.transform.position);

				Vector3 collisionPoint = WhereTheseHit (this.gameObject, go);

				if (collisionPoint == null) {
					continue;
				}

				//if there are characters within the cone do the steering
				if (Vector3.Distance(collisionPoint,this.transform.position) < closestDistance) {
					closestDistance = thisDistance;
					avoidancePosition = collisionPoint;
					evading++;
				}
			}


			if (evading == 0) {
				m_avoidOthersTorque = new Vector3 (0, 0, 0);
				continue;
			}


			//add rotation away from the center of mass

			Vector3 direction = (avoidancePosition  - this.transform.position).normalized * -1;

			Vector3 Torque = Vector3.Cross (transform.up, direction);

			Torque = Torque.normalized * m_maxAngularAcceleration * Time.deltaTime * 100.0f;

			//Scale down the torque to prevent overshooting of the target

			Debug.DrawRay (transform.position, Torque, Color.red);
			m_avoidOthersTorque = Torque;
		}
	}

}
