using UnityEngine;

namespace N1C_Movement
{
	public class CharacterGroundedState : RootState<CharacterStateMachine>
	{
		public CharacterGroundedState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		public override void EnteredHandler()
		{
			Debug.Log("[state enter] grounded");

			// walk
			SetState(new CharacterMoveState(stateMachine.pilotData.maxStableWalkSpeed, stateMachine));
			// SetState(new CharacterSlideState(stateMachine));
			base.EnteredHandler();
		}

		public override void BeforeCharacterUpdate(float deltaTime)
		{
			// if stable on a ground (the standing not angled to be recognized as a wall) 
			if (stateMachine.motor.GroundingStatus.IsStableOnGround) return;
			// else

			SetState(new CharacterMidAirState(stateMachine));
		}

		public override void ExitHandler()
		{
			Debug.Log("[state exit] grounded");
			base.ExitHandler();
		}
	}
}
