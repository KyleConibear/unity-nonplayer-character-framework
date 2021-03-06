namespace Conibear {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public enum DoorState {
		Open,
		Animating,
		Closed
	}

	public class SlidingDoorExample : MonoBehaviour {
		#region SerializeFields

		[SerializeField]
		private float m_SlidingDistance = 4;


		[SerializeField]
		[Range(0.5f, 3)]
		private float m_SlidngDuration = 1.5f;

		[SerializeField]
		private AnimationCurve m_SlideCurve = new AnimationCurve();

		#endregion


		#region Internal Fields

		private Vector3 m_OpenPosition = Vector3.zero;

		private Vector3 m_ClosedPosition = Vector3.zero;

		private DoorState m_DoorState = DoorState.Closed;

		private IEnumerator m_AnimateDoorCoroutine = null;

		#endregion


		#region MonoBehaviour Methods

		#endregion


		// Start is called before the first frame update
		private void Awake() {
			m_ClosedPosition = transform.position;
			m_OpenPosition = m_ClosedPosition + (Vector3.right * m_SlidingDistance);
		}


		// Update is called once per frame
		private void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				this.DoorInteraction();
			}
		}

		private void DoorInteraction() {
			switch (m_DoorState) {
				case DoorState.Closed:
					this.OpenDoor();
					break;
				case DoorState.Open:
					this.CloseDoor();
					break;
			}
		}

		private void OpenDoor() {
			if (m_AnimateDoorCoroutine == null) {
				m_AnimateDoorCoroutine = this.AnimateDoorCoroutine(DoorState.Open);
				StartCoroutine(m_AnimateDoorCoroutine);
			}
		}

		private void CloseDoor() {
			if (m_AnimateDoorCoroutine == null) {
				m_AnimateDoorCoroutine = this.AnimateDoorCoroutine(DoorState.Closed);
				StartCoroutine(m_AnimateDoorCoroutine);
			}
		}

		private IEnumerator AnimateDoorCoroutine(DoorState newState) {
			m_DoorState = DoorState.Animating;
			var startPosition = newState == DoorState.Open ? m_ClosedPosition : m_OpenPosition;
			var endPosition = newState == DoorState.Open ? m_OpenPosition : m_ClosedPosition;
			var time = 0f;

			while (time <= m_SlidngDuration) {
				var normalizedTime = time / m_SlidngDuration;
				transform.position = Math.PositionLerp(startPosition, endPosition, m_SlideCurve, normalizedTime);
				time += Time.deltaTime;
				yield return 0;
			}

			m_DoorState = newState;
			m_AnimateDoorCoroutine = null;
		}
	}
}