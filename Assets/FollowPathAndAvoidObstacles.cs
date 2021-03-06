﻿using UnityEngine;
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
	[SerializeField]private float m_startingDistance;
	[SerializeField]private BoxCollider m_boxCollider;
	private Vector3 m_avoidanceTorque;
	private List<Vector3> m_drawList;
	Vector3 Torque;

	// Use this for initialization
	void Start () {
		this.StartCoroutine (this.FollowPath (m_firstNode));
		this.StartCoroutine (this.MasterMove ());	
	}

	protected IEnumerator FollowPath(Node nextNode){
		while (nextNode != null) {

			m_avoidanceTorque = new Vector3 (0,0,0);
			if (TargetObscured (nextNode.transform.position)) {
				this.Avoid (RayCast (m_left.transform), RayCast (this.transform), RayCast (m_right.transform));
			}

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

	private bool Avoid(Vector3? left,Vector3? hit, Vector3? right){
		if (right.HasValue == false && left.HasValue == false) {
			return false;
		}

		//if right is longer than left, turn right
		if ((right.HasValue == false) || (left.HasValue == true && right.HasValue == true && Vector3.Distance (transform.position, (Vector3)left) < Vector3.Distance (transform.position, (Vector3)right))) {
			m_avoidanceTorque = (Vector3.Cross(transform.right ,transform.up)).normalized * -1 * m_maxAngularAcceleration;
			return false;
		}

		//if left is longer than right, turn left
		if ((left.HasValue == false) || (left.HasValue == true && right.HasValue == true && Vector3.Distance (transform.position, (Vector3)left) > Vector3.Distance (transform.position, (Vector3)right))) {
			m_avoidanceTorque = (Vector3.Cross(transform.right ,transform.up)).normalized * m_maxAngularAcceleration;
			return false;
		}


		if (hit.HasValue) {
			m_avoidanceTorque = Vector3.Cross((((Vector3) hit) - transform.position) * -1.0f, transform.up).normalized * m_maxAngularVelocity;
			return false;
		}
		return true;
	}

	public void Update(){
		Debug.DrawRay (transform.position, m_pathFollowTorque, Color.green);
		Debug.DrawRay (transform.position, m_avoidanceTorque, Color.blue);
	}

	private bool TargetObscured(Vector3 endingPoint){
		if(RayCast (m_left.transform,true).HasValue || RayCast (this.transform,true).HasValue || RayCast (m_right.transform,true).HasValue){
			return true;
		}


		Vector3 position = transform.position;
		Vector3 direction = (endingPoint - position).normalized;
		position += direction* m_startingDistance;

		m_lineRenderer.SetPosition (0, position);
		while (Vector3.Distance (position, endingPoint) > .3f) {
			m_lineRenderer.SetPosition (1, position);
			position += direction * m_raycastStep;
			if (Physics.CheckSphere (position, m_raycastRadius * m_boxCollider.bounds.size.x * 1.8f)) {
				return true;
			}
		}

		return false;
	}

	private Vector3? RayCast(Transform startingpoint, bool shortCheck = false){
		Vector3 position = startingpoint.position;
		position += transform.up * m_startingDistance;
		m_drawList = new List<Vector3> ();
		m_lineRenderer = startingpoint.GetComponent<LineRenderer> ();

		m_lineRenderer.SetPosition (0, position);


		for (float i = 0.0f; i < (shortCheck == false ?  m_raycastDistance : m_raycastDistance/10.0f); i += m_raycastStep) {
			position += startingpoint.up * m_raycastStep;
			m_drawList.Add (position);
			m_lineRenderer.SetPosition (1, position);

			if (Physics.CheckSphere (position, m_raycastRadius)) {
				return position;
			}
		}
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
