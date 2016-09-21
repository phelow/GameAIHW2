using UnityEngine;
using System.Collections;

public class PathFollow : MonoBehaviour {
	[SerializeField]protected Node m_firstNode;
	[SerializeField]private float m_maxAcceleration;
	[SerializeField]private float m_maxSpeed = 3.0f;
	[SerializeField]protected Rigidbody m_rigidBody;
	[SerializeField]protected float m_maxAccelerationMagnitude;
	[SerializeField]protected float m_minDistance;
	[SerializeField]protected float m_maxAngularAcceleration;

	protected Vector3 m_pathFollowTorque;
	protected Vector3 m_pathFollowForce;


	// Use this for initialization
	void Start () {
		this.StartCoroutine (this.FollowPath (m_firstNode));
		this.StartCoroutine (this.MasterMove ());
	}

	protected IEnumerator FollowPath(Node nextNode){
		while (nextNode != null) {
			
			this.Pursue (nextNode.gameObject);

			if (nextNode == null || Vector3.Distance (this.transform.position, nextNode.transform.position) <= m_minDistance) {
				nextNode = nextNode.GetNextNode();
			}

			yield return new WaitForEndOfFrame ();
		}
	}

	protected virtual GameObject FindNearestTarget(string targetLabel, Vector3 position){
		GameObject closestTarget = null;
		float minDist = Mathf.Infinity;
		foreach(GameObject go in GameObject.FindGameObjectsWithTag (targetLabel)){
			float distance = Vector3.Distance (position, go.transform.position);
			if (distance < minDist) {
				minDist = distance;
				closestTarget = go;
			}
		}

		return closestTarget;
	}
	
	protected virtual IEnumerator MasterMove(){
		while (true) {
			m_rigidBody.AddForce (m_pathFollowForce* Time.deltaTime * 100.0f);
			m_rigidBody.AddTorque (m_pathFollowTorque* Time.deltaTime * 100.0f);

			CheckSpeed ();

			yield return new WaitForEndOfFrame ();
		}
	}

	protected void CheckSpeed(){
		if (m_rigidBody.velocity.magnitude > m_maxSpeed) {
			m_rigidBody.velocity = m_rigidBody.velocity.normalized * m_maxSpeed;
		}
	}

	public virtual void Pursue(GameObject target){
		//calcualte rotation
		Vector3 direction = (target.transform.position - this.transform.position).normalized;

		Vector3 headingDistance = Vector3.Cross (transform.up, direction);
		Vector3 Torque = Vector3.Cross (transform.up, direction);

		Torque = Torque.normalized * m_maxAngularAcceleration;


		//Scale down the torque to prevent overshooting of the target

		Torque = Torque * Mathf.Lerp (0.0f, 1.0f, headingDistance.magnitude - m_rigidBody.angularVelocity.magnitude);

		Debug.DrawRay (transform.position, direction);

		m_pathFollowTorque = Torque;
		m_pathFollowForce = Vector3.ClampMagnitude (transform.up * m_maxAccelerationMagnitude, Mathf.Abs (m_maxAccelerationMagnitude));

	}
}
