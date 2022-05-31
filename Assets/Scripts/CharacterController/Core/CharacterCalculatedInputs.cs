using KinematicCharacterController;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterCalculatedInputs
	{
		public void Update(in PlayerCharacterInputs inputs, KinematicCharacterMotor motor)
		{
			Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward), 1f);

			Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, motor.CharacterUp).normalized;

			if (cameraPlanarDirection.sqrMagnitude == 0f)
				cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, motor.CharacterUp).normalized;

			Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

			MoveVector = cameraPlanarRotation * moveInputVector;
			LookVector = cameraPlanarDirection;

			if (inputs.sprintDown)
				SprintRequest = true;
			
			else if (inputs.sprintUp)
				SprintRequest = false;
		}

		public Vector3 MoveVector { get; private set; }
		public Vector3 LookVector { get; private set; }

		public bool SprintRequest { get; private set; }
		public bool CanSprint { get; private set; }
	}
}
