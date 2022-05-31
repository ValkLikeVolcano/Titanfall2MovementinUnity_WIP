using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterMidAirState : RootState<CharacterStateMachine>
	{
		public CharacterMidAirState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		protected override void ActivateHandler()
		{
			SetChildState(new CharacterAirMoveState(stateMachine));
		}
		
		protected override string GetRootDescription() => "Mid Air State";

		protected override void UpdateVelocityRoot(ref Vector3 velocity, float deltaTime)
		{
			PilotData data = stateMachine.pilotData;

			// Gravity
			velocity += data.gravity * deltaTime;

			// Drag
			velocity *= 1f / (1f + data.drag * deltaTime);
		}

		protected override void BeforeCharacterUpdateRoot(float deltaTime)
		{
			if (CheckWalls(out _))
			{
				stateMachine.SetState(new CharacterWallState(stateMachine));
			}
		}

		protected override void PostGroundingUpdateRoot(float deltaTime)
		{
			// if standing on stable wall
			if (stateMachine.motor.GroundingStatus.IsStableOnGround)
			{
				stateMachine.SetState(new CharacterGroundedState(stateMachine));
			}
		}

		bool CheckWalls(out RaycastHit resultHit)
		{
			KinematicCharacterMotor motor = stateMachine.motor;
			PilotData data = stateMachine.pilotData;

			const int RAY_COUNT = 8;
			bool hitAnything = false;

			resultHit = default;

			/*
			 * shoot rays with rayCount amount, in a circle below the player
			 * singleAngle = 360 / rayCount
			*/
			const float SINGLE_ANGLE = 360f / RAY_COUNT;

			float minDistance = float.MaxValue;

			for (int i = 0; i < RAY_COUNT; ++i)
			{
				Vector3 rayDirection = GetDirectionByIndex(SINGLE_ANGLE, i);

				if (!Physics.Raycast(motor.Transform.position, rayDirection, out RaycastHit hit, data.maxWallDistance, data.wallLayer)) continue;
				// else

				float distance = Vector3.Distance(hit.point, motor.Transform.position);

				if (distance > minDistance) continue;

				hitAnything = true;

				minDistance = distance;
				resultHit = hit;
			}

			return hitAnything;

			Vector3 GetDirectionByIndex(float singleAngleInDegrees, float index)
			{
				float angleInDegrees = singleAngleInDegrees * index;

				float angleInRadians = Mathf.Deg2Rad * angleInDegrees;

				var direction = new Vector3(Mathf.Cos(angleInRadians), 0f, Mathf.Sin(angleInRadians));

				return direction;
			}
		}
	}
}
