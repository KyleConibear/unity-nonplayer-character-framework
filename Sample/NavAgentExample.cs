using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentExample : MonoBehaviour {
	[SerializeField]
	private AIWaypointNetwork WaypointNetwork = null;

	private NavMeshAgent _navAgent = null;

	private int CurrentIndex = 0;
	private NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;

	void Start() {
		_navAgent = GetComponent<NavMeshAgent>();

		if (WaypointNetwork == null) return;

		SetNextDestination(false);
	}


	void Update() {
		var hasPath = _navAgent.hasPath;
		var pathPending = _navAgent.pathPending;
		var pathStale = _navAgent.isPathStale;
		var pathStatus = _navAgent.pathStatus;

		// If we don't have a path and one isn't pending then set the next
		// waypoint as the target, otherwise if path is stale regenerate path
		if ((_navAgent.remainingDistance <=_navAgent.stoppingDistance && !pathPending) || pathStatus == NavMeshPathStatus.PathInvalid)
			SetNextDestination(true);
		else if (_navAgent.isPathStale)
			SetNextDestination(false);
	}

	void SetNextDestination(bool increment) {
		// If no network return
		if (!WaypointNetwork) return;

		// Calculatehow much the current waypoint index needs to be incremented
		int incStep = increment ? 1 : 0;
		Transform nextWaypointTransform = null;

		// Calculate index of next waypoint factoring in the increment with wrap-around and fetch waypoint 
		int nextWaypoint = (CurrentIndex + incStep >= WaypointNetwork.Waypoints.Count) ? 0 : CurrentIndex + incStep;
		nextWaypointTransform = WaypointNetwork.Waypoints[nextWaypoint];

		// Assuming we have a valid waypoint transform
		if (nextWaypointTransform != null) {
			// Update the current waypoint index, assign its position as the NavMeshAgents
			// Destination and then return
			CurrentIndex = nextWaypoint;
			_navAgent.destination = nextWaypointTransform.position;
			return;
		}

		// We did not find a valid waypoint in the list for this iteration
		CurrentIndex = nextWaypoint;
	}
}