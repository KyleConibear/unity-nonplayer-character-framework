namespace Conibear {
	using UnityEngine.AI;
	using UnityEngine;

	public enum AIStates {
		Wander
	}

	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(NavMeshAgent))]
	public class NonPlayerCharacter : MonoBehaviour {
		#region Internal Fields

		private const bool Animator_ApplyRootMotion = false;

		private const bool NavMeshAgent_AutoTraverseOffMeshLink = false;

		private const string AnimatorParameter_xVelocity = "xVelocity";

		private const string AnimatorParameter_zVelocity = "zVelocity";

		private Animator m_Animator = null;

		private NavMeshAgent m_NavMeshAgent = null;

		private Vector3 m_Destination = Vector3.zero;

		#endregion


		#region Internal ShowOnly SerializeFields

		[Header("Debug Stats")]
		[ShowOnly] [SerializeField]
		private bool m_HasPath = false;

		[ShowOnly] [SerializeField]
		private bool m_IsTraversingOffMeshLink = false;

		#endregion


		#region SerializeFields

		[Header("AI")]
		[SerializeField]
		private AIStates aiState = AIStates.Wander;

		[Header("Wander")]
		[SerializeField]
		[Tooltip("The max wander range from current position")]
		private float m_Range = 50f;

		#endregion


		#region Internal Properties

		protected virtual Animator Animator {
			get {
				if (m_Animator == null) {
					m_Animator = GetComponent<Animator>();
					Print.NotInitializedWarning(this);
				}

				return m_Animator;
			}
		}

		protected virtual NavMeshAgent NavMeshAgent {
			get {
				if (m_NavMeshAgent == null) {
					m_NavMeshAgent = GetComponent<NavMeshAgent>();
					Print.NotInitializedWarning(this);
				}

				return m_NavMeshAgent;
			}
		}

		public bool HasPath => m_HasPath = this.NavMeshAgent.hasPath;

		public Vector3 Destination {
			get => m_Destination;
			set {
				m_Destination = value;
				this.NavMeshAgent.destination = m_Destination;
			}
		}

		protected bool HasArrivedAtDestination => this.NavMeshAgent.remainingDistance <= this.NavMeshAgent.stoppingDistance && !this.NavMeshAgent.pathPending || this.NavMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid;
		protected bool IsPathStaled => this.NavMeshAgent.isPathStale;
		protected Vector3 NormalizedMovement => this.NavMeshAgent.desiredVelocity.normalized;

		protected Vector3 ZAxisVector => Vector3.Project(this.NormalizedMovement, transform.forward);

		protected Vector3 XAxisVector => Vector3.Project(this.NormalizedMovement, transform.right);

		protected float ZAxisVelocity => this.ZAxisVector.magnitude * Vector3.Dot(this.ZAxisVector, transform.forward);

		protected float XAxisVelocity => this.XAxisVector.magnitude * Vector3.Dot(this.XAxisVector, transform.right);

		protected bool IsTraversingOffMeshLink => m_IsTraversingOffMeshLink = m_NavMeshAgent.isOnOffMeshLink;

		protected AIStates AIStates {
			get => aiState;
			set => aiState = value;
		}

		#endregion


		#region MonoBehaviour Methods

		/// <summary>
		/// First call in Execution Order used for initialization
		/// </summary>
		protected void Awake() {
			this.InitializeAnimator();
			this.InitializeNavMeshAgent();
		}

		/// <summary>
		/// Start is called before the first frame update
		/// </summary>
		protected virtual void Start() {
		}


		/// <summary>
		/// Update is called once per frame
		/// </summary>
		protected virtual void Update() {
			this.AnimateMovement();

			if (this.HasArrivedAtDestination) {
				this.DecideNextPosition();
			} else if (this.HasPath && this.IsPathStaled) {
				Print.Warning($"IsPathStaled: <{this.IsPathStaled}>");
			}
		}

		#endregion


		#region Internal Methods

		protected virtual void InitializeAnimator() {
			m_Animator = GetComponent<Animator>();
			this.Animator.applyRootMotion = Animator_ApplyRootMotion;
		}


		protected virtual void InitializeNavMeshAgent() {
			m_NavMeshAgent = GetComponent<NavMeshAgent>();
			this.NavMeshAgent.autoTraverseOffMeshLink = NavMeshAgent_AutoTraverseOffMeshLink;
		}

		protected virtual void AnimateMovement() {
			this.Animator.SetFloat(AnimatorParameter_zVelocity, this.ZAxisVelocity);
			this.Animator.SetFloat(AnimatorParameter_xVelocity, this.XAxisVelocity);
		}

		private void DecideNextPosition() {
			var nextPosition = transform.position;
			switch (this.AIStates) {
				case AIStates.Wander:
					nextPosition = this.NextWanderPosition();
					break;
			}

			this.Destination = nextPosition;
		}


		#region Wander

		private Vector3 NextWanderPosition() {
			Vector3 randomDirection = Random.insideUnitSphere * m_Range;
			randomDirection += transform.position;
			NavMeshHit hit;
			NavMesh.SamplePosition(randomDirection, out hit, m_Range, 1);
			return hit.position;
		}

		#endregion

		#endregion
	}
}