using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterAirMoveState : State<CharacterStateMachine>
	{
		public CharacterAirMoveState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		public override string GetEditorDescription() => "Air Move State";

		public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			KinematicCharacterMotor motor = stateMachine.motor;

			Vector3 moveVector = stateMachine.calculatedInputs.MoveVector;

			PilotData data = stateMachine.pilotData;

			if (moveVector.sqrMagnitude > 0f)
			{
				Vector3 addedVelocity = moveVector * data.airAccelerationSpeed * deltaTime;

				Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(velocity, motor.CharacterUp);

				// Limit air velocity from inputs
				if (currentVelocityOnInputsPlane.magnitude < data.maxAirMoveSpeed)
				{
					// clamp addedVel to make total vel not exceed max vel on inputs plane
					Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, data.maxAirMoveSpeed);
					addedVelocity = newTotal - currentVelocityOnInputsPlane;
				}
				else
				{
					// Make sure added vel doesn't go in the direction of the already-exceeding velocity
					if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
						addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
				}

				// Prevent air-climbing sloped walls
				if (motor.GroundingStatus.FoundAnyGround)
				{
					if (Vector3.Dot(velocity + addedVelocity, addedVelocity) > 0f)
					{
						Vector3 perpendicularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
						addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpendicularObstructionNormal);
					}
				}

				// Apply added velocity
				velocity += addedVelocity;
			}
		}
	}
}
