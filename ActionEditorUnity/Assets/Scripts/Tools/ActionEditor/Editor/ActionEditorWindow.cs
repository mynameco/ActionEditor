using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace ActionEditor
{
	public class ActionEditorWindow : EditorWindow
	{
		[MenuItem("Window/Action Editor Window")]
		public static void ShowWindow()
		{
			GetWindow(typeof(ActionEditorWindow), false, "Action Editor");
		}

		private void OnEnable()
		{
			selection = null;
			ClearAll();
		}

		private void OnDisable()
		{
			selection = null;
			ClearAll();
		}

		private void Update()
		{
			var needUpdate = false;

			if (EditorApplication.isPlaying != isPlaying)
			{
				isPlaying = EditorApplication.isPlaying;
				needUpdate = true;
			}

			if (Selection.activeGameObject != selection)
			{
				needUpdate = true;
			}

			if (needUpdate)
			{
				ClearAll();

				selection = Selection.activeGameObject;
				if (selection != null)
				{
					var actions = selection.GetComponent<ActionBehaviour>();
					if (actions != null)
					{
						actions.CreateIfNeed();
						InvalidateAll(actions.ActionGraph);
					}
				}
			}

			Repaint();
		}

		private void OnGUI()
		{
			if (graph == null)
				return;

			UpdateMoveInvalidate();
			UpdateContextMenu();

			DrawNodes();
		}

		private void ClearAll()
		{
			graph = null;
			moveInvalidate = false;
		}

		private void InvalidateAll(ActionGraph graph)
		{
			this.graph = graph;
		}

		private void DrawNodes()
		{
			BeginWindows();

			for (int index = 0; index < graph.Nodes.Length; index++)
			{
				var node = graph.Nodes[index];
				DrawNode(node, index);
			}

			EndWindows();
		}

		private void DrawNode(Node node, int index)
		{
			var rect = GUILayout.Window(
				index,
				new Rect(node.Position.x, node.Position.y, 0, 0),
				WindowCallback,
				node.GetType().Name,
				GUI.skin.window,
				new GUILayoutOption[] { GUILayout.MinHeight(minWindowHeight), GUILayout.MinWidth(minWindowWidth) });

			if ((int)rect.x != (int)node.Position.x || (int)rect.y != (int)node.Position.y)
			{
				node.Position = new Vector2((int)rect.x, (int)rect.y);
				moveInvalidate = true;
			}
		}

		private void WindowCallback(int id)
		{
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				var node = graph.Nodes[id];
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Destroy Node"), false, DestroyNodeCallback, node);

				menu.ShowAsContext();

				Event.current.Use();
			}

			GUILayout.BeginVertical();
			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		private void UpdateMoveInvalidate()
		{
			if (Event.current.type == EventType.mouseUp)
			{
				if (moveInvalidate)
				{
					moveInvalidate = false;
					SaveAll();
				}
			}
		}

		private void SaveAll()
		{
			if (selection == null)
				return;

			var actions = selection.GetComponent<ActionBehaviour>();
			if (actions == null)
				return;

			actions.Save();
		}

		private void UpdateContextMenu()
		{
			if (selection == null)
				return;

			if (Event.current.type == EventType.ContextClick)
			{
				contextMousePosition = Event.current.mousePosition;

				var menu = new GenericMenu();

				foreach (var type in NodeFactory.Types)
				{
					menu.AddItem(new GUIContent(type), false, CreateNodeCallback, type);
				}

				menu.ShowAsContext();

				Event.current.Use();
			}
		}

		private void CreateNodeCallback(object userData)
		{
			if (selection == null)
				return;

			var type = (string)userData;
			var node = NodeFactory.Create(type);
			if (node == null)
				return;

			node.Position = contextMousePosition;
			ArrayUtility.Add(ref graph.Nodes, node);

			SaveAll();
		}

		private void DestroyNodeCallback(object userData)
		{
			if (selection == null)
				return;

			var node = (Node)userData;
			if (!graph.Nodes.Contains(node))
				return;

			ArrayUtility.Remove(ref graph.Nodes, node);

			SaveAll();
		}

		private bool isPlaying;
		private GameObject selection;
		private ActionGraph graph;

		private static int minWindowWidth = 180;
		private static int minWindowHeight = 45;

		private bool moveInvalidate;
		private Vector2 contextMousePosition;
	}
}