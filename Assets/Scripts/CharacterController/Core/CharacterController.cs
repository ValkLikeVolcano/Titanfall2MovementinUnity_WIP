using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	[RequireComponent(typeof(KinematicCharacterMotor))]
	public class CharacterController : MonoBehaviour, ICharacterController
	{
		void Awake()
		{
			_motor = GetComponent<KinematicCharacterMotor>();
			_motor.CharacterController = this; // sussy baka

			_stateMachine = new CharacterStateMachine(_pilotData, _motor);
		}

		void Start()
		{
			_stateMachine.Start();
		}

		void Update()
		{
			_stateMachine.Update();
		}

		public void SetInputs(PlayerCharacterInputs inputs)
		{
			_stateMachine.SetInputs(inputs);
		}

		[SerializeField] PilotData _pilotData;

		[SerializeField] Transform _cameraFollowPoint;

		KinematicCharacterMotor _motor;

		CharacterStateMachine _stateMachine;

		#region Interface methods

		public void AfterCharacterUpdate(float deltaTime)
		{

		}

		public void BeforeCharacterUpdate(float deltaTime)
		{
			if (_motor.GroundingStatus.IsStableOnGround)
			{
				
			}
		}

		public bool IsColliderValidForCollisions(Collider collider) => true;

		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{

		}

		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{

		}

		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{

		}

		public void PostGroundingUpdate(float deltaTime)
		{

		}

		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
			Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{

		}

		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{

		}

		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			_stateMachine.UpdateVelocity(ref currentVelocity, deltaTime);
		}

		#endregion
	}
}
