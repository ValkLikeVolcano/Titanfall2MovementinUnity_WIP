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

			if (target == null) return;

			DoStateMachineStateDescription();
		}
		void DoStateMachineStateDescription()
		{
			var controller = (TFCharacterController)target;
			
			GUILayout.Label("== STATE MACHINE ==");

			GUILayout.Label(controller.GetEditorDescription());
		}
	}
}
