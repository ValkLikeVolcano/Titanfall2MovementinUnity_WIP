using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	/// <summary>
	///     A state that handles normal movement
	/// </summary>
	public class CharacterGroundMoveAndJumpState : State<CharacterStateMachine>
	{
		// Note: we could actually still go down the rabbit hole by adding another state machine that handles only jumping
		// but I'm too lazy so yes :D
		
		public CharacterGroundMoveAndJumpState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		public override void Update()
		{
			UpdateInputs();
		}

		public override string GetEditorDescription() => "Ground Move And Jump State";

		public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			DoMove(ref velocity, deltaTime, GetMaxStableSpeed());

			if (_wantToJump)
				DoJump(ref velocity);
		}

		const float DEFAULT_MAX_STABLE_DENIVELATION_ANGLE = 180f;

		bool _wantToJump;

		// Note that the weird jumping forward thing after previous movement is not from this
		void DoJump(ref Vector3 velocity)
		{
			KinematicCharacterMotor motor = stateMachine.motor;
			CharacterCalculatedInputs calculatedInputs = stateMachine.calculatedInputs;

			Vector3 jumpDirection = motor.CharacterUp;

			// force the player to unsnap from the ground
			motor.ForceUnground();

			velocity += jumpDirection * GetJumpSpeed() - Vector3.Project(velocity, motor.CharacterUp);
			velocity += calculatedInputs.MoveVector * GetJumpScalableForwardSpeed();
		}

		void UpdateInputs()
		{
			PlayerCharacterInputs inputs = stateMachine.Inputs;

			if (inputs.jumpDown)
				_wantToJump = true;
		}

		void DoMove(ref Vector3 velocity, float deltaTime, float maxStableMoveSpeed)
		{
			Vector3 moveInputVector = stateMachine.calculatedInputs.MoveVector;

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
			Vector3 targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

			// Smooth movement Velocity
			velocity = Vector3.Lerp(velocity, targetMovementVelocity, 1f - Mathf.Exp(-stateMachine.pilotData.stableMovementSharpness * deltaTime));
		}

		float GetMaxStableSpeed()
		{
			PilotData pilotData = stateMachine.pilotData;

			return GetIsSprinting() ? pilotData.maxStableSprintSpeed : pilotData.maxStableWalkSpeed;
		}

		float GetJumpSpeed()
		{
			PilotData pilotData = stateMachine.pilotData;

			return GetIsSprinting() ? pilotData.sprintJumpUpSpeed : pilotData.walkJumpUpSpeed;
		}

		float GetJumpScalableForwardSpeed()
		{
			PilotData pilotData = stateMachine.pilotData;
			
			return GetIsSprinting() ? pilotData.sprintForwardSpeed : pilotData.walkForwardSpeed;
		}

		bool GetIsSprinting() => stateMachine.calculatedInputs.SprintRequest;
	}
}
