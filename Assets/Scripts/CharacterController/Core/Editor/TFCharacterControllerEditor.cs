using N1C_Movement;
using UnityEditor;
using UnityEngine;

namespace KinematicCharacterController
{
	[CustomEditor(typeof(TFCharacterController))]
	public class TFCharacterControllerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			DoStateMachineStateDescription();
		}
		void DoStateMachineStateDescription()
		{
			var controller = (TFCharacterController)target;

			if (controller.TryGetEditorDescription(out string description))
			{
				GUILayout.Label(description, EditorStyles.helpBox);
			}
		}
	}
}
