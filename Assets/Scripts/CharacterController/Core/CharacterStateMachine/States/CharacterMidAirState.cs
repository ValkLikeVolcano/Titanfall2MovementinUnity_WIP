using UnityEngine;

namespace N1C_Movement
{
	public class CharacterMidAirState : RootState<CharacterStateMachine>
	{
		public CharacterMidAirState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		public override void EnteredHandler()
		{
			Debug.Log("[state enter] mid air~");
			
			base.EnteredHandler();
		}

		public override void BeforeCharacterUpdate(float deltaTime)
		{
			// if not in a stable ground (the standing normal is angled to be recognized as a wall)
			if (!stateMachine.motor.GroundingStatus.IsStableOnGround) return;
			// else
			
			SetState(new CharacterGroundedState(stateMachine));
		}
	}
}
