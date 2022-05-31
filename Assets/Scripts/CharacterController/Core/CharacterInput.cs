using UnityEngine;

namespace N1C_Movement
{
	/// <summary>
	///     Calculates all input for character to use
	/// </summary>
	public class CharacterInput : MonoBehaviour
	{
		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		void Update()
		{
			if (Input.GetMouseButtonDown(0))
				Cursor.lockState = CursorLockMode.Locked;

			UpdateCharacterInput();
		}

		void LateUpdate()
		{
			UpdateCameraInput();
		}

		const string HORIZONTAL_INPUT = "Horizontal";
		const string VERTICAL_INPUT   = "Vertical";

		[SerializeField] TFCharacterController _tfCharacterController;
		[SerializeField] TFMovementCamera      _movementCamera;

		void UpdateCameraInput()
		{
			// Create the look input vector for the camera
			float mouseLookAxisUp = Input.GetAxisRaw("Mouse Y");
			float mouseLookAxisRight = Input.GetAxisRaw("Mouse X");

			var lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

			//Prevent moving the camera while the cursor isn't locked
			if (Cursor.lockState != CursorLockMode.Locked)
				lookInputVector = Vector3.zero;

			_movementCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
		}

		void UpdateCharacterInput()
		{
			var characterInputs = new PlayerCharacterInputs(
				Input.GetAxisRaw(VERTICAL_INPUT),
				Input.GetAxisRaw(HORIZONTAL_INPUT),
				_movementCamera.Transform.rotation,
				Input.GetKeyDown(KeyCode.Space),
				Input.GetKeyDown(KeyCode.LeftShift),
				Input.GetKeyUp(KeyCode.LeftShift),
				Input.GetKeyDown(KeyCode.C),
				Input.GetKeyUp(KeyCode.C)
			);

			_tfCharacterController.SetInputs(characterInputs);
		}
	}
}
