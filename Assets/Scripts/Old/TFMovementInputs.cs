using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N1C_Movement
{
    public class TFMovementInputs : MonoBehaviour
    {

        [SerializeField] private TFMovementController Character;
        [SerializeField] private TFMovementCamera PlayerCamera;

        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            PlayerCamera.SetFollowTransform(Character.CameraFollowPoint);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw("Mouse Y");
            float mouseLookAxisRight = Input.GetAxisRaw("Mouse X");
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            //Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Apply inputs to the camera
            PlayerCamera.UpdateWithInput(Time.deltaTime, lookInputVector);
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = PlayerCamera.Transform.rotation;
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.SprintDown = Input.GetKeyDown(KeyCode.LeftShift);
            characterInputs.SprintUp = Input.GetKeyUp(KeyCode.LeftShift);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            // Apply inputs to character
            Character.SetInputs(in characterInputs);
        }
    }
}
