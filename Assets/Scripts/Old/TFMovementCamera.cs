using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace N1C_Movement
{
    public class TFMovementCamera : MonoBehaviour
    {
		public Camera Camera;

		[HideInInspector]
		public Vector3 Angles;

		public float sensitivity = 1.2f;
		public float wallRunTilt = 15f;
		public float baseFov = 90f;
    	public float maxFov = 140f;
		public float RotationSharpness = 10000f;

		[HideInInspector]
		public float Distance = 0f;

		[Range(-90f, 90f)]
		public float DefaultVerticalAngle = 20f;
		[Range(-90f, 90f)]
		public float MinVerticalAngle = -90f;
		[Range(-90f, 90f)]
		public float MaxVerticalAngle = 90f;

		public Transform Transform { get; private set; }
		public Transform FollowTransform { get; private set; }
		public Vector3 PlanarDirection { get; set; }
		private Vector3 _currentFollowPosition;

		public float TargetDistance { get; set; }
		private float _targetVerticalAngle;
		private float _currentDistance;
		float fov;
    	float curTilt = 0;

		private void OnValidate()
		{
			DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
			Distance = 0f;
		}

		private void Awake()
		{
			Transform = this.transform;

			_currentDistance = Distance;
			TargetDistance = _currentDistance;

			_targetVerticalAngle = 0f;

			PlanarDirection = Vector3.forward;

			curTilt = transform.localEulerAngles.z;
		}

		public void SetFollowTransform(Transform t)
		{
			FollowTransform = t;
			PlanarDirection = FollowTransform.forward;
			_currentFollowPosition = FollowTransform.position;
		}

		public void UpdateWithInput(float deltaTime, Vector3 rotationInput)
		{
			Quaternion rotationFromInput = Quaternion.Euler(FollowTransform.up * (rotationInput.x * sensitivity));
			PlanarDirection = rotationFromInput * PlanarDirection;
			PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up));
			Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up);

			_targetVerticalAngle -= (rotationInput.y * sensitivity);
			_targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
			Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
			Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

			Transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, transform.eulerAngles.z);

			_currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-10000 * deltaTime));

			Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

			Transform.position = targetPosition;
		}

		public IEnumerator TiltCam(int side = 0)
		{
			float targetRot = side * wallRunTilt;
			// float currentRot = Transform.rotation.z;

			while(Mathf.Abs(Transform.eulerAngles.z - targetRot) > 0.1f)
			{
				var newRot = Mathf.LerpAngle(Transform.eulerAngles.z, targetRot, 8f * Time.deltaTime);
				
				// sus, you didn't add spaces before your comments... >:(

				//ඞඞඞඞඞඞඞඞඞඞSUSඞඞඞඞඞඞඞඞඞඞඞ
				Transform.rotation = Quaternion.Euler(Transform.eulerAngles.x, Transform.eulerAngles.y, newRot);
				//print(Mathf.Abs(Transform.eulerAngles.z - targetRot) > 0.1f);
				yield return null;
			}
		}
	}
}