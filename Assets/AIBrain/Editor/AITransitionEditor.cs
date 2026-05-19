using Brain.Graph;
using Tools.Editor;
using UnityEditor;
using UnityEngine;

namespace Brain
{
	[CustomPropertyDrawer(typeof(AITransition))]
	public class AITransitionEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			
			// Сохраняем текущий уровень отступа
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			// Рисуем поля
			position.height = EditorGUIUtility.singleLineHeight;
                
			EditorGUI.PropertyField(position, property.FindPropertyRelative("Label"), new GUIContent("Label"));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.PropertyField(position, property.FindPropertyRelative("Decision"), new GUIContent("Decision"));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			SerializedProperty trueNameProp = property.FindPropertyRelative("StateTrueIndex");
			string trueName = trueNameProp.intValue == -1 ? "null" : 
				$"Index state [{trueNameProp.intValue.ToString()}]";
			
			SerializedProperty falseNameProp = property.FindPropertyRelative("StateFalseIndex");
			string falseName = falseNameProp.intValue == -1 ? "null" : 
				$"Index state [{falseNameProp.intValue.ToString()}]";
			
			EditorGUI.LabelField(position, new GUIContent("AIStateTrue"), new GUIContent(trueName));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.LabelField(position, new GUIContent("AIStateFalse"), new GUIContent(falseName));
			
			// Восстанавливаем отступ
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			// Заголовок + 4 поля + отступы
			return EditorGUIUtility.singleLineHeight * 5 + EditorGUIUtility.standardVerticalSpacing * 4;
		}
	}
}