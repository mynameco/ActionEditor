/*using System;
using System.Collections.Generic;
using UniLinq;
using UnityEditor;
using UnityEngine;

namespace GraphView
{
	public enum SlotLabel
	{
		None,
		Before,
		After
	}

	public enum CurveDirection
	{
		None,
		Left,
		Right
	}

	public enum DrawMode
	{
		None,
		WithoutInspectors,
		WithoutSeparators,
		SignalsOneLine,
		Minimal
	}

	public abstract class GraphWindow<NodeType> :
		EditorWindow
		where NodeType : GraphNode
	{
		private void OnEnable()
		{
			selection = null;
			nodes.Clear();
			ClearEditors();

			UpdateTypeNodeList();
		}

		private void OnDisable()
		{
			selection = null;
			nodes.Clear();
			ClearEditors();

			typeNodeList = new Type[0];
		}

		public class NodeSlotInfo
		{
			public GraphNode Node;
			public SlotInfo Slot;
			public int offset;
			public int index;
		}

		public class OffsetInfo
		{
			public Vector2 Offset;
			public GameObject Object;
		}

		private Vector2 ScrollOffset
		{
			get
			{
				if (scrollOffsetIndex < 0 || scrollOffsetIndex >= scrollOffsets.Count)
					return scrollOffsetCommon;
				return scrollOffsets[scrollOffsetIndex].Offset;
			}
			set
			{
				if (scrollOffsetIndex < 0 || scrollOffsetIndex >= scrollOffsets.Count)
					scrollOffsetCommon = value;
				else
					scrollOffsets[scrollOffsetIndex].Offset = value;
			}
		}

		private void ComputeOffsets(GameObject selection)
		{
			for (int index = 0; index < scrollOffsets.Count;)
			{
				if (scrollOffsets[index].Object == selection)
				{
					scrollOffsetIndex = index;
					return;
				}
				else if (scrollOffsets[index].Object == null)
				{
					scrollOffsets.RemoveAt(index);
				}
				else
				{
					index++;
				}
			}

			if (nodes.Count == 0)
			{
				scrollOffsetIndex = -1;
				return;
			}

			if (scrollOffsets.Count >= maxScrollOffsets)
				scrollOffsets.RemoveAt(0);

			scrollOffsets.Add(new OffsetInfo()
			{
				Object = selection,
				Offset = scrollOffsetCommon
			});
			scrollOffsetIndex = scrollOffsets.Count - 1;
		}

		private void Update()
		{
			var invalidate = false;

			if (EditorApplication.isPlaying != isPlaying)
			{
				isPlaying = EditorApplication.isPlaying;
				invalidate = true;
			}

			if (Selection.activeGameObject != selection)
			{
				invalidate = true;
			}
			else if (Selection.activeGameObject != null)
			{
				var selection2 = Selection.activeGameObject.GetComponents(typeof(NodeType));
				if (selection2.Length != nodes.Count)
				{
					invalidate = true;
				}
			}
			else if (selection == null && nodes.Count != 0)
			{
				invalidate = true;
			}

			if (invalidate)
			{
				nodes.Clear();
				ClearEditors();

				selection = Selection.activeGameObject;
				if (selection
					// Префабы не рисуем
					// && PrefabUtility.GetPrefabType(selection) != PrefabType.Prefab
					)
				{
					foreach (GraphNode node in selection.GetComponents(typeof(NodeType)))
					{
						node.InitialiseGraphNode();
						nodes.Add(node);
					}
					editors = new GraphNodeEditor[nodes.Count];
				}

				ComputeOffsets(selection);
			}

			Repaint();
		}

		private void UpdateAutoMove()
		{
			if (dragVisible)
			{
				if ((lastAutomoveTime + automoveTimeout) < EditorApplication.timeSinceStartup)
				{
					lastAutomoveTime = EditorApplication.timeSinceStartup;

					var mousePosition = Event.current.mousePosition;
					if (mousePosition.x < (automoveBorder + viewOffset.x))
						ScrollOffset = new Vector2(ScrollOffset.x - automoveDelta, ScrollOffset.y);
					if (mousePosition.x > (position.width - automoveBorder - 15))
						ScrollOffset = new Vector2(ScrollOffset.x + automoveDelta, ScrollOffset.y);
					if (mousePosition.y < (automoveBorder + viewOffset.y))
						ScrollOffset = new Vector2(ScrollOffset.x, ScrollOffset.y - automoveDelta);
					if (mousePosition.y > (position.height - automoveBorder - 15))
						ScrollOffset = new Vector2(ScrollOffset.x, ScrollOffset.y + automoveDelta);
				}
			}
		}

		private void OnGUI()
		{
			UpdateAutomoveState();
			UpdateAutoMove();

			UpdateDisconnect();
			UpdateDragAndDrop();
			UpdateZoomView();
			UpdateDragView();
			UpdateContext();
			UpdateRecursiveMove();

			UpdateZoomValues();

			PrepareStyles();
			DrawHeader();
			DrawControls();
		}

		private void UpdateAutomoveState()
		{
			if (Event.current.type == EventType.mouseDown)
			{
				lastAutomoveTime = EditorApplication.timeSinceStartup;
			}
		}

		private void UpdateZoomValues()
		{
			if (Event.current.type == EventType.layout)
			{
				var scaleValue2 = minScaleValue + (zoomValue * (1 - minScaleValue));

				if (scaleValue != scaleValue2)
				{
					// TODO Немогу найти активный контрол
					var pivot = new Vector2(0, 0);
					ScrollOffset = ComputeOffsetScaleByPivot(pivot, scaleValue2);
				}

				scaleValue = scaleValue2;

				if (zoomValue < 0.2)
				{
					drawMode = DrawMode.Minimal;
					currentWindowStyle = windowStyleMinimal;
					currentLabelStyle = labelStyleMinimal;
				}
				else if (zoomValue < 0.4)
				{
					drawMode = DrawMode.SignalsOneLine;
					currentWindowStyle = windowStyle2;
					currentLabelStyle = labelStyleMinimal;
				}
				else if (zoomValue < 0.6)
				{
					drawMode = DrawMode.WithoutSeparators;
					currentWindowStyle = windowStyle2;
					currentLabelStyle = labelStyleMinimal;
				}
				else if (zoomValue < 0.8)
				{
					drawMode = DrawMode.WithoutInspectors;
					currentWindowStyle = GUI.skin.window;
					currentLabelStyle = GUI.skin.label;
				}
				else
				{
					drawMode = DrawMode.None;
					currentWindowStyle = GUI.skin.window;
					currentLabelStyle = GUI.skin.label;
				}
			}
		}

		private void UpdateRecursiveMove()
		{
			if (Event.current.type == EventType.keyDown)
			{
				if (Event.current.keyCode == KeyCode.LeftControl)
					leftControl = true;
			}
			else if (Event.current.type == EventType.keyUp)
			{
				if (Event.current.keyCode == KeyCode.LeftControl)
					leftControl = false;
			}
		}

		private void UpdateContext()
		{
			if (selection == null)
				return;

			if (Event.current.type == EventType.ContextClick)
			{
				contextMousePosition = Event.current.mousePosition;

				var menu = new GenericMenu();

				foreach (var type in typeNodeList)
				{
					var info = type.GetCustomAttributes(typeof(NodePathAttribute), false).Cast<NodePathAttribute>().FirstOrDefault();
					if (info != null)
					{
						var path = info.Path + "/" + type.Name;
						if (!string.IsNullOrEmpty(info.Tooltip))
							path += "\t" + info.Tooltip;

						menu.AddItem(new GUIContent(path), false, CreateTypeNode, type);
					}
				}

				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Update Type Node Lits"), false, UpdateTypeNodeList);
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Show All Inspectors"), false, ShowAllInspectors, true);
				menu.AddItem(new GUIContent("Hide All Inspectors"), false, ShowAllInspectors, false);
				menu.AddSeparator("");
				//menu.AddItem(new GUIContent("Normal Draw Mode"), false, ChangeDrawMode, DrawMode.None);
				//menu.AddItem(new GUIContent("Without Inspectors Draw Mode"), false, ChangeDrawMode, DrawMode.WithoutInspectors);
				//menu.AddItem(new GUIContent("Without Separators Draw Mode"), false, ChangeDrawMode, DrawMode.WithoutSeparators);
				//menu.AddItem(new GUIContent("Signals One Line Draw Mode"), false, ChangeDrawMode, DrawMode.SignalsOneLine);
				//menu.AddItem(new GUIContent("Minimal Draw Mode"), false, ChangeDrawMode, DrawMode.Minimal);
				//menu.AddSeparator("");
				menu.AddItem(new GUIContent("Refresh Nodes"), false, RefreshNodes);
				menu.AddItem(new GUIContent("Reposition Nodes"), false, RepositionNodes);
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Remove All Nodes"), false, RemoveAllNodes);

				menu.ShowAsContext();

				Event.current.Use();
			}
		}

		private void UpdateTypeNodeList()
		{
			var types = new List<Type>();
			foreach (var type in typeof(NodeType).Assembly.GetTypes())
			{
				if (type != typeof(NodeType) && typeof(NodeType).IsAssignableFrom(type))
				{
					types.Add(type);
				}
			}

			typeNodeList = types.ToArray();
		}

		private void CreateTypeNode(object userData)
		{
			if (!selection)
				return;

			var type = (Type)userData;
			var node = (NodeType)selection.AddComponent(type);
			node.WindowPosition = (contextMousePosition + ScrollOffset) / scaleValue;
			node.MarkAsDirty();

			ComputeOffsets(node.gameObject);
		}

		private void ShowAllInspectors(object userData)
		{
			var show = (bool)userData;

			foreach (var node in nodes)
			{
				node.ShowInspector = show;
			}
		}

		//private void ChangeDrawMode(object userData)
		//{
		//drawMode = (DrawMode)userData;
		//}

		private void RemoveAllNodes()
		{
			foreach (var node in nodes)
			{
				var gameObject = node.gameObject;
				GameObject.DestroyImmediate(node);
				EditorUtility.SetDirty(gameObject);
			}
			nodes.Clear();
		}

		private void RefreshNodes()
		{
			if (!selection)
				return;

			foreach (var node in nodes)
			{
				node.CurveColor = ColorPreset.LightGray;

				if (node.WindowPosition.x < 0 || node.WindowPosition.y < 0 || node.WindowPosition.x > (mapWidth - 100) || node.WindowPosition.y > (mapHeight - 100))
				{
					node.WindowPosition.x = 0;
					node.WindowPosition.y = 0;
					node.MarkAsDirty();
				}

				foreach (var slot in node.OutSlots)
				{
					var list = slot.GetConnections().ToList();
					var count = list.Count;
					for (int index = 0; index < list.Count;)
					{
						var connection = list[index];
						if (!connection)
						{
							Debug.LogWarning("Empty connection");
							list.RemoveAt(index);
						}
						else if (connection.gameObject != selection.gameObject)
						{
							Debug.LogWarning("Connection is different objects");
							list.RemoveAt(index);
						}
						else
						{
							index++;
						}
					}

					if (count != list.Count)
					{
						slot.ClearConnections();
						foreach (var item in list)
							slot.AddConnection(item);
					}
				}
			}
		}

		private void UpdateZoomView()
		{
			//if (Event.current.type != EventType.repaint && Event.current.type != EventType.layout)
			//Debug.Log(Event.current.type + " : " + leftControl);

			if (leftControl &&
				Event.current.type == EventType.scrollWheel)
			{
				zoomValue += (Event.current.delta.y * 0.01f);
				zoomValue = Mathf.Clamp01(zoomValue);
				var scaleValue2 = minScaleValue + (zoomValue * (1 - minScaleValue));

				ScrollOffset = ComputeOffsetScaleByPivot(Event.current.mousePosition + ScrollOffset, scaleValue2);

				Event.current.Use();
			}
		}

		private Vector2 ComputeOffsetScaleByPivot(Vector2 absPosition, float newScale)
		{
			var prevPosition = new Vector2(absPosition.x / (mapWidth * scaleValue), absPosition.y / (mapHeight * scaleValue));
			var nextAbsolutePosition = new Vector2(mapWidth * newScale * prevPosition.x, mapHeight * newScale * prevPosition.y);
			var offset = nextAbsolutePosition - (absPosition - ScrollOffset);
			offset = new Vector2((int)offset.x, (int)offset.y);

			//Debug.Log("Abs  : " + absPosition + " , rel : " + prevPosition);
			//Debug.Log("Abs : " + nextAbsolutePosition + " , offset : " + offset);

			return offset;
		}

		private void UpdateDragView()
		{
			if (Event.current.type == EventType.mouseDrag &&
				Event.current.button == 2)
			{
				ScrollOffset -= Event.current.delta;
				Event.current.Use();
			}
		}

		private void DrawControls()
		{
			ScrollOffset = GUI.BeginScrollView(new Rect(0, viewOffset.y, position.width, position.height - viewOffset.y), ScrollOffset, new Rect(0, 0, (int)(mapWidth * scaleValue), (int)(mapHeight * scaleValue)));

			BeginWindows();

			Vector2 moveDiff2 = Vector2.zero;
			GraphNode moveNode = null;

			for (int index = 0; index < nodes.Count; index++)
			{
				var node = nodes[index];
				var diff = GraphNodeDrawUtility.DrawNodeWindow(index, node, drawMode, scaleValue, currentWindowStyle, WindowCallback);

				if (leftControl && diff != Vector2.zero)
				{
					moveNode = node;
					moveDiff2 = diff;
				}
			}

			EndWindows();

			if (moveNode != null)
			{
				var compute = new List<GraphNode>();
				compute.Add(moveNode);

				RecursiveMove(moveNode, moveDiff2, compute, moveNode.WindowPosition.x);
			}

			Handles.BeginGUI();

			foreach (var node in nodes)
				GraphNodeDrawUtility.DrawNodeConnections(node, position, inSlotDirection, outSlotDirection, outMultiSlots, scaleValue);

			DrawDragLine();

			Handles.EndGUI();

			foreach (var node in nodes)
				GraphNodeDrawUtility.DrawNodeButtons(node, inSlotDirection, outSlotDirection, horizontalSymbol, outMultiSlots, connectionStyle);

			GUI.EndScrollView();
		}

		private void PrepareStyles()
		{
			if (currentWindowStyle == null)
				currentWindowStyle = GUI.skin.window;

			if (windowStyleMinimal == null)
			{
				windowStyleMinimal = new GUIStyle(GUI.skin.box);
				windowStyleMinimal.font = EditorStyles.miniFont;
				windowStyleMinimal.wordWrap = false;
			}

			if (windowStyle2 == null)
			{
				windowStyle2 = new GUIStyle(GUI.skin.window);
				windowStyle2.font = EditorStyles.miniFont;
			}

			if (currentLabelStyle == null)
				currentLabelStyle = GUI.skin.label;

			if (labelStyleMinimal == null)
			{
				labelStyleMinimal = new GUIStyle(GUI.skin.label);
				labelStyleMinimal.font = EditorStyles.miniFont;
			}

			if (connectionStyle == null)
				connectionStyle = new GUIStyle(EditorStyles.miniLabel);
		}

		private void DrawHeader()
		{
			float offset = 6;
			float width = position.width;
			float scaleWidth = 100;
			float buttonWidth = 70;

			GUI.Box(new Rect(0, 0, width, viewOffset.y), "", EditorStyles.toolbar);

			if (GUI.Button(new Rect(offset, 0, buttonWidth, viewOffset.y), "Reposition", EditorStyles.toolbarButton))
			{
				RepositionNodes();
			}

			zoomValue = GUI.HorizontalSlider(new Rect(width - scaleWidth - offset * 2, 0, scaleWidth, viewOffset.y), zoomValue, 0, 1);
		}

		private void RecursiveMove(GraphNode node, Vector2 diff, List<GraphNode> compute, float startPosition)
		{
			foreach (var slot in node.OutSlots)
			{
				foreach (var connection in slot.GetConnections())
				{
					if (!connection)
						continue;

					if (compute.Contains(connection))
						continue;

					if (connection.WindowPosition.x < startPosition)
						continue;

					connection.WindowPosition -= diff;
					connection.MarkAsDirty();

					compute.Add(connection);
					RecursiveMove(connection, diff, compute, startPosition);
				}
			}
		}

		private void UpdateDisconnect()
		{
			if (Event.current.type == EventType.ContextClick)
			{
				var inSlot = GetInSlotByPosition(Event.current.mousePosition);
				if (inSlot != null)
				{
					foreach (var window in nodes)
					{
						foreach (var slot in window.OutSlots)
						{
							slot.RemoveConnection(inSlot);
						}
					}

					Event.current.Use();
				}
				else
				{
					var outSlot = GetOutSlotByPosition(Event.current.mousePosition);
					if (outSlot != null)
					{
						outSlot.Slot.ClearConnections();

						Event.current.Use();
					}
				}
			}
		}

		private void UpdateDragAndDrop()
		{
			if (Event.current.type == EventType.mouseDown)
			{
				firstDrag = true;
				storeMousePoint = Event.current.mousePosition;
			}
			else if (Event.current.type == EventType.mouseDrag)
			{
				if (firstDrag)
				{
					firstDrag = false;
					sourceDragSlot = GetOutSlotByPosition(storeMousePoint);

					if (sourceDragSlot != null)
					{
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.objectReferences = new UnityEngine.Object[0];
						DragAndDrop.paths = new string[0];
						DragAndDrop.StartDrag("Create connection");
						Event.current.Use();
						dragVisible = true;
					}
				}
			}
			else if (Event.current.type == EventType.DragExited)
			{
				dragVisible = false;
			}
			else if (Event.current.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();

				targetDragSlot = GetInSlotByPosition(Event.current.mousePosition);
				if (targetDragSlot != null)
					TryConnect(sourceDragSlot, targetDragSlot);

				sourceDragSlot = null;
				targetDragSlot = null;
				dragVisible = false;
			}
			else if (Event.current.type == EventType.dragUpdated)
			{
				if (sourceDragSlot != null)
				{
					var target = GetInSlotByPosition(Event.current.mousePosition);
					if (target != null)
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					else
						DragAndDrop.visualMode = DragAndDropVisualMode.Move;
					dragVisible = true;
				}
			}
		}

		private void DrawDragLine()
		{
			if (IsDragLine())
			{
				GraphNodeDrawUtility.CurveFromTo(
					GraphNodeDrawUtility.GetCurvePosition(sourceDragSlot.Slot.Point + new Vector2(sourceDragSlot.offset, 0), outSlotDirection),
					Event.current.mousePosition,
					outSlotDirection,
					CurveDirection.None,
					ColorPreset.LightGray,
					scaleValue);
			}
		}

		private bool IsDragLine()
		{
			return sourceDragSlot != null && dragVisible;
		}

		private void TryConnect(NodeSlotInfo sourceSlot, GraphNode targetSlot)
		{
			if (!IsCanConnect(sourceSlot, targetSlot))
				return;

			sourceSlot.Slot.AddConnection(targetSlot);
		}

		private bool IsCanConnect(NodeSlotInfo sourceSlot, GraphNode targetSlot)
		{
			foreach (var connection in sourceSlot.Slot.GetConnections())
			{
				if (connection == targetSlot)
				{
					return false;
				}
			}

			return true;
		}

		private GraphNode GetInSlotByPosition(Vector2 position)
		{
			foreach (var node in nodes)
			{
				if (GraphNodeDrawUtility.IsInSlot(position + ScrollOffset, node, slotSize, inSlotDirection, viewOffset))
					return node;
			}
			return null;
		}

		private NodeSlotInfo GetOutSlotByPosition(Vector2 position)
		{
			foreach (var node in nodes)
			{
				int offset = 0;
				int index = -1;
				var slot = GraphNodeDrawUtility.GetOutSlot(position + ScrollOffset, node, slotSize, outSlotDirection, outMultiSlots, ref offset, ref index, viewOffset);
				if (slot != null)
				{
					var result = new NodeSlotInfo();
					result.Slot = slot;
					result.Node = node;
					result.offset = offset;
					result.index = index;
					return result;
				}
			}
			return null;
		}

		private void WindowCallback(int id)
		{
			if (nodes[id].ShowInspector)
			{
				if (editors[id] == null)
				{
					editors[id] = (GraphNodeEditor)Editor.CreateEditor(nodes[id]);
					editors[id].windowContext = true;
				}

				if (editors[id].countField == 0)
				{
					nodes[id].ShowInspector = false;
				}
			}

			GraphNodeDrawUtility.WindowCallback(id, nodes[id], editors[id], inSlotLabelPlace, outSlotLabelPlace, inSlotDirection, outSlotDirection, drawMode, scaleValue, currentLabelStyle);
		}

		private void ClearEditors()
		{
			foreach (var editor in editors)
			{
				if (editor)
					DestroyImmediate(editor);
			}

			editors = new GraphNodeEditor[0];
		}

		private void RepositionNodes()
		{
			var result = GraphNodeUtility.RepositionNodes(nodes);

			var startNodes = result.Where(o => o.GraphId != -1).ToList();
			startNodes.Sort((a, b) => { return a.GraphId.CompareTo(b.GraphId); });

			//var compute = new List<GraphNode>();
			var heights = new float[nodes.Count];
			//var deep = 0;

			Debug.Log(startNodes.Count);

			foreach (var node in startNodes)
			{
				var maxHeight = heights.Max();
				for (int index = 0; index < heights.Length; index++)
					heights[index] = maxHeight;

				node.Node.WindowPosition = new Vector2(0, 0);// repositionOffset + new Vector2(deep * repositionDistance.x, heights[deep]);
				node.Node.MarkAsDirty();

				//RepositionNode(node.Node, compute, heights, deep + 1);
			}
		}

		private void RepositionNode(GraphNode parentNode, List<GraphNode> compute, float[] heights, int deep)
		{
			var nodes = new List<GraphNode>();
			foreach (var slot in parentNode.OutSlots)
			{
				foreach (var node in slot.GetConnections())
				{
					if (!node)
						continue;

					if (compute.Contains(node))
						continue;
					compute.Add(node);

					nodes.Add(node);
				}
			}

			//nodes.Sort(SortByOutput);
			foreach (var node in nodes)
			{
				node.WindowPosition = repositionOffset + new Vector2(deep * repositionDistance.x, heights[deep]);
				node.MarkAsDirty();
				heights[deep] += repositionDistance.y;

				RepositionNode(node, compute, heights, deep + 1);
			}
		}

		private const int mapWidth = 30000;
		private const int mapHeight = 5000;

		private bool firstDrag;
		private Vector2 storeMousePoint;

		private NodeSlotInfo sourceDragSlot;
		private GraphNode targetDragSlot;
		private bool dragVisible;
		private Vector2 contextMousePosition;

		private List<OffsetInfo> scrollOffsets = new List<OffsetInfo>();
		private int scrollOffsetIndex = -1;
		private int maxScrollOffsets = 10;
		private Vector2 scrollOffsetCommon;

		private GameObject selection;
		private List<GraphNode> nodes = new List<GraphNode>();
		private GraphNodeEditor[] editors = new GraphNodeEditor[0];
		private Type[] typeNodeList = new Type[0];

		private bool leftControl;
		private bool isPlaying;
		private float zoomValue = 1;
		private float scaleValue;

		private Vector2 repositionDistance = new Vector2(350, 200);
		private Vector2 repositionOffset = new Vector2(30, 30);
		private Vector2 slotSize = new Vector2(14, 14);
		protected SlotLabel inSlotLabelPlace = SlotLabel.Before;
		protected SlotLabel outSlotLabelPlace = SlotLabel.After;
		protected CurveDirection inSlotDirection = CurveDirection.Left;
		protected CurveDirection outSlotDirection = CurveDirection.Right;
		protected string horizontalSymbol = "▶";
		protected bool outMultiSlots;

		private DrawMode drawMode;

		private float minScaleValue = 0.2f;
		private GUIStyle windowStyleMinimal;
		private GUIStyle windowStyle2;
		private GUIStyle currentWindowStyle;
		private GUIStyle labelStyleMinimal;
		private GUIStyle currentLabelStyle;
		private GUIStyle connectionStyle;
		private Vector2 viewOffset = new Vector2(0, 18);

		private double lastAutomoveTime;
		private float automoveTimeout = 0.1f;
		private float automoveBorder = 30;
		private float automoveDelta = 40;
	}
}*/