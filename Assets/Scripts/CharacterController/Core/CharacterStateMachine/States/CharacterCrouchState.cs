using System.Collections;
using UnityEngine;

namespace N1C_Movement
{
	public class CharacterCrouchState : RootState<CharacterStateMachine>
	{
		public CharacterCrouchState(CharacterStateMachine stateMachine) : base(stateMachine)
		{
		}
		
		bool _isVisuallyCrouching;
		bool _isChangingCrouchPosition;
		
		IEnumerator CrouchLerp(float endHeight, Vector3 resultLocalScale, bool isCrouchingDown)
		{
			if (_isChangingCrouchPosition)
				yield break;

			_isChangingCrouchPosition = true;

			PilotData data = stateMachine.pilotData;

			Transform meshRoot = stateMachine.meshRoot;

			float offsetDirection = isCrouchingDown ? -1f: 1f;
			
			while (meshRoot.localScale.y >= endHeight)
			{
				meshRoot.localScale = Vector3.Slerp(
					meshRoot.localScale,
					new Vector3(1f, endHeight + offsetDirection, 1f),
					data.crouchDownSpeed * Time.deltaTime
				);

				yield return null;
			}

			meshRoot.localScale = resultLocalScale;

			_isVisuallyCrouching = isCrouchingDown;

			_isChangingCrouchPosition = false;

			yield return null;
		}

		/*IEnumerator CrouchLerp()
		{
			if (_isChangingCrouchPosition)
				yield break;


			_isChangingCrouchPosition = true;

			const float END_HEIGHT = 0.5f;

			while (MeshRoot.localScale.y >= END_HEIGHT)
			{
				MeshRoot.localScale = Vector3.Slerp(MeshRoot.localScale, new Vector3(1f, END_HEIGHT - 0.1f, 1f), p.crouchDownSpeed * Time.deltaTime);

				yield return null;
			}

			MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);

			_isVisuallyCrouching = true;

			_isChangingCrouchPosition = false;

			yield return null;
		}

		IEnumerator StandUpLerp()
		{
			if (_isChangingCrouchPosition)
				yield break;

			_isChangingCrouchPosition = true;

			const float END_HEIGHT = 1f;

			while (MeshRoot.localScale.y <= END_HEIGHT)
			{
				MeshRoot.localScale = Vector3.Slerp(MeshRoot.localScale, new Vector3(1f, END_HEIGHT + 0.1f, 1f), p.crouchDownSpeed * Time.deltaTime);

				yield return null;
			}

			MeshRoot.localScale = new Vector3(1f, 1f, 1f);

			_isVisuallyCrouching = false;

			_isChangingCrouchPosition = false;

			yield return null;
		}*/
	}
}
