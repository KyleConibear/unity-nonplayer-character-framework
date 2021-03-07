namespace Conibear {
	using UnityEngine;
	using UnityEditor;
	using UnityEngine.AI;

	[CustomEditor(typeof(NonPlayerCharacter), true)]
	public class NonPlayerCharacterEditor : Editor {
		#region Internal Fields

		private GUIStyle m_Style = new GUIStyle();

		#endregion


		#region Editor Methods

		/// <summary>
		/// Implementing this functions means the Unity Editor will call it when
		/// the Scene View is being repainted. This gives us a hook to do our
		/// own rendering to the scene view.
		/// </summary>
		private void OnSceneGUI() {
			NonPlayerCharacter character = (NonPlayerCharacter) target;

			if (character == null || !character.HasPath)
				return;


			m_Style.alignment = TextAnchor.UpperCenter;
			m_Style.normal.textColor = Color.magenta;

			Handles.Label(character.Destination, $"{character.gameObject.name}\nDestination", m_Style);

			NavMeshPath path = new NavMeshPath();

			Vector3 agentPosition = character.transform.position;

			NavMesh.CalculatePath(agentPosition, character.Destination, NavMesh.AllAreas, path);

			Handles.color = Color.magenta;

			Handles.DrawPolyLine(path.corners);
		}

		#endregion
	}
}