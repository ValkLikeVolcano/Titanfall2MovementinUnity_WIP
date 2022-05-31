using System.Globalization;
using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterSlideState : State<CharacterStateMachine>
	{
		public CharacterSlideState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		protected override void ActivateHandler()
		{
			_deltaHeight = stateMachine.motor.transform.position.y;
		}

		public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			DoSlide(ref velocity, deltaTime);
		}

		public override string GetEditorDescription() => "Slide State";

		void DoSlide(ref Vector3 velocity, float deltaTime)
		{
			KinematicCharacterMotor motor = stateMachine.motor;
			PilotData pilotData = stateMachine.pilotData;

			_deltaHeight -= stateMachine.motor.transform.position.y;

			//Increase Ground Stickiness
			// note: cannot comprehend the pressure that the understanding of a holy ground Stickiness gives :)
			// motor.MaxStableDenivelationAngle = p.slideGroundStickAngle;

			Vector3 effectiveGroundNormal = motor.GroundingStatus.GroundNormal;

			velocity = motor.GetDirectionTangentToSurface(velocity, effectiveGroundNormal) * velocity.magnitude;


			//Check Height Difference
			if (motor.LastGroundingStatus.IsStableOnGround && _deltaHeight - motor.transform.position.y >= pilotData.minSlideHeightDiff)
			{
				//Add Velocity on Slope
				if (velocity.magnitude <= pilotData.maxStableSlideSpeed)
					velocity += motor.CharacterForward * pilotData.slopeAccelerationSpeed * deltaTime;
			}
			else if (motor.LastGroundingStatus.IsStableOnGround && _deltaHeight - motor.transform.position.y < -pilotData.minSlideHeightDiff)
				velocity *= 1f / (1f + pilotData.slopeUpDrag * deltaTime);

			//Add Drag with Grace Time for B-hopping
			else if (motor.LastGroundingStatus.IsStableOnGround && (_deltaHeight - motor.transform.position.y < pilotData.minSlideHeightDiff ||
																	_deltaHeight - motor.transform.position.y > -pilotData.minSlideHeightDiff))
				velocity *= 1f / (1f + pilotData.slideDrag * deltaTime);

			Debug.Log($"[delta height] {_deltaHeight.ToString(CultureInfo.InvariantCulture)}");

			_deltaHeight = motor.transform.position.y;
		}

		float _deltaHeight;
	}
}
