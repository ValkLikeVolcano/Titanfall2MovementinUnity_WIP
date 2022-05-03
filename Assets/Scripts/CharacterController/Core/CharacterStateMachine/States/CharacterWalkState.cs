using UnityEngine;

namespace N1C_Movement
{
	public class CharacterWalkState : State<CharacterStateMachine>
	{
		public CharacterWalkState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}

		public override void EnteredHandler()
		{
			Debug.Log("[state enter] walk");
		}

		public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			// that was really cool
			PlayerCharacterInputs input = stateMachine.Input;
			velocity = new Vector3(input.moveAxisRight, 0f, input.moveAxisForward).normalized; // world sided (not camera rotated)
		}

		public override void ExitHandler()
		{
			Debug.Log("[state exit] walk");
		}
	}
}
