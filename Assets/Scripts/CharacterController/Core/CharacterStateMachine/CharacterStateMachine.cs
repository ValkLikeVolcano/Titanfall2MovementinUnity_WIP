using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterStateMachine : StateMachine<CharacterStateMachine>
	{
		public CharacterStateMachine(PilotData pilotData, KinematicCharacterMotor motor)
		{
			this.pilotData = pilotData;
			this.motor = motor;
		}

		public void Update()
		{
			State?.Update();
		}

		public void UpdateVelocity(ref Vector3 velocity, float deltaTime)
		{
			State?.UpdateVelocity(ref velocity, deltaTime);
		}

		public void BeforeCharacterUpdate(float deltaTime)
		{
			State?.BeforeCharacterUpdate(deltaTime);
		}
		
		public void Start()
		{
			SetState(new CharacterGroundedState(this));
		}

		public void SetInputs(PlayerCharacterInputs inputs) => Input = inputs;

		public PlayerCharacterInputs Input { get; private set; }

		public readonly PilotData pilotData;

		public readonly KinematicCharacterMotor motor;
	}
}
