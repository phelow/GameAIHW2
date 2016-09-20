using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPathAndAvoidObstacles : PathFollow {
	[SerializeField]private float m_raycastDistance = 15.0f;
	[SerializeField]private float m_raycastRadius = 1.0f;

	[SerializeField]private LineRenderer m_lineRenderer;
	[SerializeField]private float m_raycastStep = .1f;

	[SerializeField]private float m_pathFollowTorqueModifier = .4f;
	[SerializeField]private float m_avoidanceTorqueModifier = .6f;
	[SerializeField]private float m_maxAngularVelocity = 1.0f;
	[SerializeField]private float m_sampleTime = .1f;

	[SerializeField]private GameObject m_left;
	[SerializeField]private GameObject m_right;
	private Vector3 m_avoidanceTorque;
	private List<Vector3> m_drawList;

	// Use this for initialization
	void Start () {
		this.StartCoroutine (this.FollowPath (m_firstNode));
		this.StartCoroutine (this.MasterMove ());	
	}

	protected IEnumerator FollowPath(Node nextNode){
		while (nextNode != null) {

			m_avoidanceTorque = new Vector3 (0,0,0);

			this.Avoid (RayCast(m_left.transform),RayCast(this.transform),RayCast(m_right.transform));
			float t = 0;
			while (t < m_sampleTime) {
				t += Time.deltaTime;
				this.Pursue (nextNode.gameObject);
				if (nextNode == null || Vector3.Distance (this.transform.position, nextNode.transform.position) <= m_minDistance) {
					nextNode = nextNode.GetNextNode ();
				}
				yield return new WaitForEndOfFrame ();
			}
			yield return new WaitForEndOfFrame ();
		}
	}
	Vector3 Torque;
	private bool Avoid(Vector3? left,Vector3? hit, Vector3? right){


		//if left is longer than right, turn left
		if ((left.HasValue == false && right.HasValue == true) || (left.HasValue == true && right.HasValue == true && Vector3.Distance (transform.position, (Vector3)left) > Vector3.Distance (transform.position, (Vector3)right))) {
			m_avoidanceTorque = (Vector3.Cross(m_left.transform.position ,transform.up)).normalized * m_maxAngularAcceleration;
			return false;
		}

		//if right is longer than left, turn right
		if ((right.HasValue == false && left.HasValue == true) || (left.HasValue == true && right.HasValue == true && Vector3.Distance (transform.position, (Vector3)left) < Vector3.Distance (transform.position, (Vector3)right))) {
			m_avoidanceTorque = (Vector3.Cross(m_right.transform.position ,transform.up)).normalized * m_maxAngularAcceleration;
			return false;
		}

		//		Torque = Vector3.Cross(transform.up,Vector3.left) * m_maxAngularAcceleration;
		//		Torque = Vector3.Cross(transform.up,Vector3.right) * m_maxAngularAcceleration;

		//		m_avoidanceTorque = Torque;
		return true;
	}

	public void Update(){
		
		Debug.DrawRay (transform.position, m_pathFollowTorque, Color.green);
		Debug.DrawRay (transform.position, m_avoidanceTorque, Color.blue);
	}

	private Vector3? RayCast(Transform startingpoint){
		Vector3 position = startingpoint.position;
		position += transform.up *1.0f;
		Debug.Log ("RAycasting");
		m_drawList = new List<Vector3> ();
		m_lineRenderer = startingpoint.GetComponent<LineRenderer> ();

		m_lineRenderer.SetPosition (0, position);

		for (float i = 0.0f; i < m_raycastDistance; i += m_raycastStep) {
			position += startingpoint.up * m_raycastStep;
			m_drawList.Add (position);
			m_lineRenderer.SetPosition (1, position);

			if (Physics.CheckSphere (position, m_raycastRadius)) {
				return position;
			}

		}

		Debug.Log ("Nothing found");

		return null;

	}

	protected virtual IEnumerator MasterMove(){
		while (true) {
			m_rigidBody.AddForce (m_pathFollowForce);

			if (m_avoidanceTorque.magnitude > .1f) {
				m_rigidBody.AddTorque (m_pathFollowTorque * m_pathFollowTorqueModifier * Time.deltaTime);
				m_rigidBody.AddTorque (m_avoidanceTorque * m_avoidanceTorqueModifier * Time.deltaTime);
			} else {
				m_rigidBody.AddTorque (m_pathFollowTorque * Time.deltaTime );

			}

			CheckSpeed ();

			yield return new WaitForEndOfFrame ();
		}
	}

	public override void Pursue(GameObject target){
		/*	Dynamic Seek:
			1. Linear Acceleration = target.position - character.position
			2. Clip to max acceleration
			3. Clip to max speed
			4. Add angular velocity*/

		//2. Clip to max acceleration

		//calcualte rotation
		Vector3 direction = (target.transform.position - this.transform.position).normalized;

		Vector3 headingDistance = Vector3.Cross (transform.up, direction);
		Vector3 Torque = Vector3.Cross (transform.up, direction);

		Torque = Torque.normalized * m_maxAngularAcceleration;


		//Scale down the torque to prevent overshooting of the target

		Torque = Torque * .1f;





		m_pathFollowTorque = Torque;
		m_pathFollowForce = Vector3.ClampMagnitude (transform.up * m_maxAccelerationMagnitude, Mathf.Abs (m_maxAccelerationMagnitude));

	}
}
