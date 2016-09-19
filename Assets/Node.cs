using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
	[SerializeField]private Node m_nextNode;

	public Node GetNextNode(){
		return m_nextNode;
	}
}
