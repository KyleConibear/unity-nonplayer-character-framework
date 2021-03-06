namespace Conibear {
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.AI;

	[CustomEditor(typeof(AIWaypointNetwork))]
	public class AIWaypointNetworkEditor : Editor {
		/// <summary>
		/// Called by Unity Editor when the Inspector needs repainting for an AIWaypointNetwork Component
		/// </summary>
		public override void OnInspectorGUI() {
			// Get reference to selected component
			AIWaypointNetwork network = (AIWaypointNetwork) target;

			if (!network.HasWaypoints) {
				DrawDefaultInspector();
				return;
			}

			// Show the Display Mode Enumeration Selector
			network.DisplayMode = (PathDisplayMode) EditorGUILayout.EnumPopup("Display Mode", network.DisplayMode);

			// If we are in Paths display mode then display the integer sliders for the Start and End waypoint indices
			if (network.DisplayMode == PathDisplayMode.Paths) {
				network.PathStartIndex =
					EditorGUILayout.IntSlider("Waypoint Start", network.PathStartIndex, 0, network.WaypointLastIndex);
				network.PathEndIndex = EditorGUILayout.IntSlider("Waypoint End", network.PathEndIndex, 0, network.WaypointLastIndex);
			}

			// Tell Unity to do its default drawing of all serialized members that are NOT hidden in the inspector
			DrawDefaultInspector();
		}


		/// <summary>
		/// Implementing this functions means the Unity Editor will call it when
		/// the Scene View is being repainted. This gives us a hook to do our
		/// own rendering to the scene view.
		/// </summary>
		void OnSceneGUI() {
			// Get a reference to the component being rendered
			AIWaypointNetwork network = (AIWaypointNetwork) target;

			if (!network.HasWaypoints) {
				return;
			}

			// Fetch all waypoints from the network and render a label for each one
			for (int i = 0; i < network.WaypointsTransform.Length; i++) {
				if (network.WaypointsTransform[i] != null)
					Handles.Label(network.WaypointsTransform[i].position, $"Waypoint {i}");
			}

			// If we are in connections mode then we will to draw lines
			// connecting all waypoints
			if (network.DisplayMode == PathDisplayMode.Connections) {
				// Allocate array of vector to store the polyline positions
				Vector3[] linePoints = new Vector3[network.WaypointsTransform.Length + 1];

				// Loop through each waypoint + one additional interation
				for (int i = 0; i <= network.WaypointsTransform.Length; i++) {
					// Calculate the waypoint index with wrap-around in the
					// last loop iteration
					int index = i != network.WaypointsTransform.Length ? i : 0;

					// Fetch the position of the waypoint for this iteration and
					// copy into our vector array.
					if (network.WaypointsTransform[index] != null)
						linePoints[i] = network.WaypointsTransform[index].position;
					else
						linePoints[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
				}

				// Set the Handle color to Cyan
				Handles.color = Color.cyan;

				// Render the polyline in the scene view by passing in our list of waypoint positions
				Handles.DrawPolyLine(linePoints);
			} else
				// We are in paths mode so to proper navmesh path search and render result
			if (network.DisplayMode == PathDisplayMode.Paths) {
				// Allocate a new NavMeshPath
				NavMeshPath path = new NavMeshPath();

				// Assuming both the start and end waypoint indices selected are ligit
				if (network.WaypointsTransform[network.PathStartIndex] != null && network.WaypointsTransform[network.PathEndIndex] != null) {
					// Fetch their positions from the waypoint network
					Vector3 from = network.WaypointsTransform[network.PathStartIndex].position;
					Vector3 to = network.WaypointsTransform[network.PathEndIndex].position;

					// Request a path search on the nav mesh. This will return the path between
					// from and to vectors
					NavMesh.CalculatePath(from, to, NavMesh.AllAreas, path);

					// Set Handles color to Yellow
					Handles.color = Color.yellow;

					// Draw a polyline passing int he path's corner points
					Handles.DrawPolyLine(path.corners);
				}
			}
		}
	}
}