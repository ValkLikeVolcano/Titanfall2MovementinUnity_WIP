using UnityEngine;

namespace N1C_Movement
{
	public class TFMovementInputs : MonoBehaviour
	{
		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;

			_playerCamera.SetFollowTransform(_character.CameraFollowPoint);
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
				Cursor.lockState = CursorLockMode.Locked;

			HandleCharacterInput();
		}

		void LateUpdate()
		{
			HandleCameraInput();
		}

		const string HORIZONTAL_INPUT = "Horizontal";
		const string VERTICAL_INPUT   = "Vertical";

		[SerializeField] TFMovementController _character;

		[SerializeField] TFMovementCamera _playerCamera;

		void HandleCameraInput()
		{
			// Create the look input vector for the camera
			float mouseLookAxisUp = Input.GetAxisRaw("Mouse Y");
			float mouseLookAxisRight = Input.GetAxisRaw("Mouse X");
			
			var lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

			//Prevent moving the camera while the cursor isn't locked
			if (Cursor.lockState != CursorLockMode.Locked)
				lookInputVector = Vector3.zero;

			// Apply inputs to the camera
			_playerCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
		}

		void HandleCharacterInput()
		{
			var characterInputs = new PlayerCharacterInputs(
				Input.GetAxisRaw(VERTICAL_INPUT),
				Input.GetAxisRaw(HORIZONTAL_INPUT),
				_playerCamera.Transform.rotation,
				Input.GetKeyDown(KeyCode.Space),
				Input.GetKeyDown(KeyCode.LeftShift),
				Input.GetKeyUp(KeyCode.LeftShift),
				Input.GetKeyDown(KeyCode.C),
				Input.GetKeyUp(KeyCode.C)
			);

			// Apply inputs to character
			_character.SetInputs(in characterInputs);
		}
	}
}
