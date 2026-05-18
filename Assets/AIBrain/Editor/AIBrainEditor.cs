using Brain.Graph;
using Tools.Editor;
using UnityEditor;
using UnityEngine;

namespace Brain
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(AIBrain))]
	public class AIBrainEditor : Editor
	{
		protected MMReorderableList _list;
		protected SerializedProperty _brainActive;
		protected SerializedProperty _timeInThisState;
		protected SerializedProperty _owner;
		protected SerializedProperty _actionsFrequency;
		protected SerializedProperty _decisionFrequency;
		protected SerializedProperty _randomizeFrequencies;
		protected SerializedProperty _randomActionFrequency;
		protected SerializedProperty _randomDecisionFrequency;

		protected virtual void OnEnable()
		{
			_list = new MMReorderableList(serializedObject.FindProperty("States"));
			_list.elementNameProperty = "States";
			_list.elementDisplayType = MMReorderableList.ElementDisplayType.Expandable;

			_brainActive = serializedObject.FindProperty("BrainActive");
			_timeInThisState = serializedObject.FindProperty("TimeInThisState");
			_owner = serializedObject.FindProperty("Owner");
			_actionsFrequency = serializedObject.FindProperty("ActionsFrequency");
			_decisionFrequency = serializedObject.FindProperty("DecisionFrequency");
            
			_randomizeFrequencies = serializedObject.FindProperty("RandomizeFrequencies");
			_randomActionFrequency = serializedObject.FindProperty("RandomActionFrequency");
			_randomDecisionFrequency = serializedObject.FindProperty("RandomDecisionFrequency");
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open AI graph"))
			{
				AIBrainNodeEditorWindow.ShowWindow();
			}
			
			serializedObject.Update();

			AIBrain brain = (AIBrain)target;
			EditorGUILayout.LabelField("First State", brain.FirstState != null ? brain.FirstState.StateName : "null");
			
			_list.DoLayoutList();
			EditorGUILayout.PropertyField(_timeInThisState);
			EditorGUILayout.PropertyField(_brainActive);
			
			serializedObject.ApplyModifiedProperties();


			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Current State: ", brain.CurrentState != null ? brain.CurrentState.StateName : "None");

			EditorGUILayout.LabelField("Target", brain.Target != null ? brain.Target.ToString() : "null");
		}
	}
}