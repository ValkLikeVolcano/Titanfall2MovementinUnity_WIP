using UnityEngine;

namespace N1C_Movement
{
	public readonly struct PlayerCharacterInputs
	{
		public PlayerCharacterInputs(float moveAxisForward, float moveAxisRight, Quaternion cameraRotation,
			bool jumpDown, bool sprintDown, bool sprintUp, bool crouchDown, bool crouchUp)
		{
			this.moveAxisForward = moveAxisForward;
			this.moveAxisRight = moveAxisRight;
			this.cameraRotation = cameraRotation;
			this.jumpDown = jumpDown;
			this.sprintDown = sprintDown;
			this.sprintUp = sprintUp;
			this.crouchDown = crouchDown;
			this.crouchUp = crouchUp;
		}

		public readonly float moveAxisForward;
		public readonly float moveAxisRight;

		public readonly Quaternion cameraRotation;

		public readonly bool jumpDown;
		public readonly bool sprintDown;
		public readonly bool sprintUp;
		public readonly bool crouchDown;
		public readonly bool crouchUp;
	}
}
