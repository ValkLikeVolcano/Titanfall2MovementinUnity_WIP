using UnityEngine;

namespace N1C_Movement
{
	public class CharacterGroundedState : RootState<CharacterStateMachine>
	{
		public CharacterGroundedState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		protected override void ActivateHandler()
		{
			// walk
			SetChildState(new CharacterGroundMoveAndJumpState(stateMachine));
			
			// SetState(new CharacterSlideState(stateMachine));
		}

		protected override string GetRootDescription() => "Grounded State";

		protected override void BeforeCharacterUpdateRoot(float deltaTime)
		{
			// if stable on a ground (the standing not angled to be recognized as a wall) 
			if (stateMachine.motor.GroundingStatus.IsStableOnGround) return;
			// else
			
			stateMachine.SetState(new CharacterMidAirState(stateMachine));
		}
	}
}
