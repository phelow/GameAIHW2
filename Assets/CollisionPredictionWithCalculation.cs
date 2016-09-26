using UnityEngine;
using System.Collections;

public class CollisionPredictionWithCalculation : CollisionPrediction {
	private Vector3? WhereTheseHit(GameObject a,GameObject b){
		Rigidbody rbA = a.GetComponent<Rigidbody> ();
		Rigidbody rbB = b.GetComponentInChildren<Rigidbody> ();

		Vector3 closestPoint = Vector3.zero;

		float closestDistance = m_collisionTolerance;

		Vector3 vecA = a.transform.position + rbA.velocity;
		Vector3 vecB = b.transform.position + rbB.velocity;

		float timeOfClosest = Vector3.Dot (vecA, vecB)/vecB.magnitude;

		Vector3 aPositionClosest = a.transform.position + rbA.velocity * timeOfClosest;
		Vector3 bPositionClosest = b.transform.position + rbB.velocity * timeOfClosest;

		if (Vector3.Distance (aPositionClosest, bPositionClosest) > this.m_collisionTolerance) {
			return null;
		}



		return aPositionClosest;
	}
}
