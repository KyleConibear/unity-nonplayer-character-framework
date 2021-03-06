using System.Collections;
using Conibear;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentExample : MonoBehaviour {
	#region Internal ShowOnly SerializeFields

	[Header("Debug Stats")]
	[ShowOnly] [SerializeField]
	private bool m_IsTraversingOffMeshLink = false;

	#endregion


	#region SerializeFields

	[SerializeField]
	private AIWaypointNetwork m_WaypointNetwork = null;

	[Header("Jump")]
	[SerializeField]
	[Range(1, 3)]
	private float m_JumpHeight = 2f;

	[SerializeField]
	[Range(0.5f, 1.5f)]
	private float m_JumpDuration = 1f;

	[SerializeField]
	private AnimationCurve m_JumpCurve = new AnimationCurve();

	#endregion


	#region Internal Fields

	private NavMeshAgent m_NavMeshAgent = null;

	private int m_CurrentIndex = 0;

	private NavMeshPathStatus m_PathStatus = NavMeshPathStatus.PathInvalid;

	private IEnumerator m_JumpIEnumerable = null;

	#endregion


	#region Internal Properties

	protected NavMeshAgent NavMeshAgent {
		get {
			if (m_NavMeshAgent == null) {
				m_NavMeshAgent = GetComponent<NavMeshAgent>();
			}

			return m_NavMeshAgent;
		}
	} 

	#endregion


	#region Public Properties

	public bool IsTraversingOffMeshLink => m_IsTraversingOffMeshLink = m_NavMeshAgent.isOnOffMeshLink;

	public bool IsJumping => m_JumpIEnumerable != null;

	#endregion


	#region MonoBehaviour

	private void Start() {
		m_NavMeshAgent = GetComponent<NavMeshAgent>();

		if (m_WaypointNetwork == null) return;

		SetNextDestination(false);
	}

	private void Update() {
		var hasPath = this.NavMeshAgent.hasPath;
		var pathPending = this.NavMeshAgent.pathPending;
		var pathStale = this.NavMeshAgent.isPathStale;
		var pathStatus = this.NavMeshAgent.pathStatus;

		if (this.IsTraversingOffMeshLink) {
			this.Jump();
		}

		// If we don't have a path and one isn't pending then set the next
		// waypoint as the target, otherwise if path is stale regenerate path
		if ((this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance && !pathPending) || pathStatus == NavMeshPathStatus.PathInvalid)
			this.SetNextDestination(true);
		else if (this.NavMeshAgent.isPathStale)
			this.SetNextDestination(false);
	}

	#endregion


	#region Internal Methods

	private void SetNextDestination(bool increment) {
		// If no network return
		if (!m_WaypointNetwork) return;

		// Calculatehow much the current waypoint index needs to be incremented
		int incStep = increment ? 1 : 0;
		Transform nextWaypointTransform = null;

		// Calculate index of next waypoint factoring in the increment with wrap-around and fetch waypoint 
		int nextWaypoint = (m_CurrentIndex + incStep >= m_WaypointNetwork.WaypointsTransform.Length) ? 0 : m_CurrentIndex + incStep;
		nextWaypointTransform = m_WaypointNetwork.WaypointsTransform[nextWaypoint];

		// Assuming we have a valid waypoint transform
		if (nextWaypointTransform != null) {
			// Update the current waypoint index, assign its position as the NavMeshAgents
			// Destination and then return
			m_CurrentIndex = nextWaypoint;
			m_NavMeshAgent.destination = nextWaypointTransform.position;
			return;
		}

		// We did not find a valid waypoint in the list for this iteration
		m_CurrentIndex = nextWaypoint;
	}

	private void Jump() {
		if (IsJumping)
			return;

		m_JumpIEnumerable = this.JumpCoroutine(m_JumpHeight, m_JumpDuration);
		StartCoroutine(m_JumpIEnumerable);
	}

	private IEnumerator JumpCoroutine(float height, float duration) {
		var data = this.NavMeshAgent.currentOffMeshLinkData;
		var startPosition = this.NavMeshAgent.transform.position;
		var endposition = data.endPos + (this.NavMeshAgent.baseOffset * Vector3.up);
		var time = 0f;

		while (time <= duration) {
			var normalizedTime = time / duration;
			this.NavMeshAgent.transform.position = Vector3.Lerp(startPosition, endposition, normalizedTime) + (m_JumpCurve.Evaluate(time) * (Vector3.up * height));
			time += Time.deltaTime;
			yield return 0;
		}

		m_NavMeshAgent.CompleteOffMeshLink();

		m_JumpIEnumerable = null;
	}

	#endregion
}