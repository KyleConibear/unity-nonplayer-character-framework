namespace Conibear {
	using UnityEngine;


	public enum PathDisplayMode {
		None,
		Connections,
		Paths
	}

	public class AIWaypointNetwork : MonoBehaviour {
		#region SerializeFields

		[SerializeField]
		private Transform[] m_WaypointsTransform = new Transform[0];

		#endregion


		#region Internal Fields

		private PathDisplayMode m_DisplayMode = PathDisplayMode.None;

		private int m_PathStartIndex = 0;

		private int m_PathEndIndex = 0;

		#endregion


		#region Public Properties

		public PathDisplayMode DisplayMode {
			get => m_DisplayMode;
			set => m_DisplayMode = value;
		}

		public Transform[] WaypointsTransform => m_WaypointsTransform;

		public int PathStartIndex {
			get => m_PathStartIndex;
			set => m_PathStartIndex = value;
		}

		public int PathEndIndex {
			get => m_PathEndIndex;
			set => m_PathEndIndex = value;
		}

		public bool HasWaypoints => this.WaypointsTransform.Length > 0;

		public int WaypointLastIndex => this.WaypointsTransform.Length - 1;

		#endregion
	}
}