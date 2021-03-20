using System;

namespace Conibear {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;

	[RequireComponent(typeof(NavMeshAgent))]
	public class NavMeshAgentBehaviour : MonoBehaviour {
		
		
		#region Internal Fields

		private NavMeshAgent m_NavMeshAgent = null;

		#endregion

		private void Awake() {
			m_NavMeshAgent = GetComponent<NavMeshAgent>();
		}

		// Start is called before the first frame update
		void Start() {
		}

		// Update is called once per frame
		void Update() {
		}
	}
}