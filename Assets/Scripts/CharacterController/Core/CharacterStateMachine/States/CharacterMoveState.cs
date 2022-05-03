using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	/// <summary>
	///     A state that handles normal movement
	/// </summary>
	public class CharacterMoveState : State<CharacterStateMachine>
	{
		public CharacterMoveState(float maxStableSpeed, CharacterStateMachine stateMachine) : base(stateMachine) => _maxStableSpeed = maxStableSpeed;

		public override void EnteredHandler()
		{
			Debug.Log("[state enter] move {}");
		}

		public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			// that was really cool
			Vector3 moveInputVector = CalculateMoveVector();

			KinematicCharacterMotor motor = stateMachine.motor;
			motor.MaxStableDenivelationAngle = DEFAULT_MAX_STABLE_DENIVELATION_ANGLE;

			float currentVelocityMagnitude = velocity.magnitude;

			Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

			if (currentVelocityMagnitude > 0f && motor.GroundingStatus.SnappingPrevented)
			{
				// Take the normal from where we're coming from
				Vector3 groundPointToCharacter = motor.TransientPosition - motor.GroundingStatus.GroundPoint;
				
				if (Vector3.Dot(velocity, groundPointToCharacter) >= 0f)
					effectiveGroundNormal = motor.GroundingStatus.OuterGroundNormal;
				else
					effectiveGroundNormal = motor.GroundingStatus.InnerGroundNormal;
			}

			// Reorient velocity on slope
			velocity = motor.GetDirectionTangentToSurface(velocity, effectiveGroundNormal) * currentVelocityMagnitude;

			// Calculate target velocity
			Vector3 inputRight = Vector3.Cross(moveInputVector, motor.CharacterUp);
			Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * moveInputVector.magnitude;
			Vector3 targetMovementVelocity = reorientedInput * _maxStableSpeed;

			// Smooth movement Velocity
			velocity = Vector3.Lerp(velocity, targetMovementVelocity, 1f - Mathf.Exp(-stateMachine.pilotData.stableMovementSharpness * deltaTime));
		}

		public override void ExitHandler()
		{
			Debug.Log("[state exit] move");
		}

		const float DEFAULT_MAX_STABLE_DENIVELATION_ANGLE = 180f;

		readonly float _maxStableSpeed;

		Vector3 CalculateMoveVector()
		{
			PlayerCharacterInputs input = stateMachine.Input;
			KinematicCharacterMotor motor = stateMachine.motor;

			Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(input.moveAxisRight, 0f, input.moveAxisForward), 1f);

			Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(input.cameraRotation * Vector3.forward, motor.CharacterUp).normalized;
			Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

			Vector3 resultMoveVector = cameraPlanarRotation * moveInputVector;
			return resultMoveVector;
		}
	}
}
