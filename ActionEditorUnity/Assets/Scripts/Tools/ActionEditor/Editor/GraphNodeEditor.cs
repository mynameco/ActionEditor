/*using GraphView;
using UniLinq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphNode), true)]
public class GraphNodeEditor :
	Editor
{
	public bool windowContext;
	public int countField;
	private GraphNode targetNode;

	private void OnEnable()
	{
		targetNode = target as GraphNode;

		if (targetNode != null)
		{
			countField = GetCountField();
		}
	}

	private int GetCountField()
	{
		var result = 0;

		serializedObject.Update();
		SerializedProperty iterator = serializedObject.GetIterator();
		bool enterChildren = true;
		while (iterator.NextVisible(enterChildren))
		{
			if (iterator.name == "m_Script")
			{
			}
			else if (targetNode.OutSlots.FirstOrDefault(o => o.FieldInfo.Name == iterator.name) != null)
			{
			}
			else
			{
				result++;
			}

			enterChildren = false;
		}

		return result;
	}

	public override void OnInspectorGUI()
	{
		if (serializedObject.targetObject == null)
			return;

		if (windowContext)
		{
			EditorGUIUtility.labelWidth = 120;
			EditorGUIUtility.fieldWidth = 120;
		}

		EditorGUI.BeginChangeCheck();
		serializedObject.Update();
		SerializedProperty iterator = serializedObject.GetIterator();
		bool enterChildren = true;
		while (iterator.NextVisible(enterChildren))
		{
			if (windowContext)
			{
				//Debug.Log(iterator.name);
				if (iterator.name == "m_Script")
				{
					enterChildren = false;
					continue;
				}
				if (targetNode.OutSlots.FirstOrDefault(o=>o.FieldInfo.Name == iterator.name) != null)
				{
					enterChildren = false;
					continue;
				}
			}

			EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
			enterChildren = false;
		}
		serializedObject.ApplyModifiedProperties();
		EditorGUI.EndChangeCheck();
	}
}
*/