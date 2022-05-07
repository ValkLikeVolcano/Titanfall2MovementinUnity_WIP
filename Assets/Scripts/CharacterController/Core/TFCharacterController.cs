using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	[RequireComponent(typeof(KinematicCharacterMotor))]
	public class TFCharacterController : MonoBehaviour, ICharacterController
	{
		void Awake()
		{
			_motor = GetComponent<KinematicCharacterMotor>();
			
			/*
			 * Problem:
			 * you are passing high level control down to a low level control
			 * Thus losing control of your character controller
			 * 
			 * Fix:
			 * there is a potential fix, you can either instead, put the interfaces
			 * into the motor, and allow the motor for us to modify the internals instead
			 * The good thing for this is we can modify the motor all we want
			 * but also won't affect any of it's movement code and updates, yey!
			 *
			 * but for now, let's just write state machine!
			 * -- yours truly, Environment.FailFast("Sofa");
			 */
			_motor.CharacterController = this;

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
