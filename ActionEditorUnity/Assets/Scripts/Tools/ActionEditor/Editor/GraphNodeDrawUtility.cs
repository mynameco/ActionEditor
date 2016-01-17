/*using System;
using UnityEditor;
using UnityEngine;

namespace GraphView
{
	public static class GraphNodeDrawUtility
	{
		public static void DrawNodeConnections(GraphNode node, Rect position, CurveDirection inSlotDirection, CurveDirection outSlotDirection, bool outMultiSlots, float scaleValue)
		{
			foreach (var slot in node.OutSlots)
			{
				foreach (var connection in slot.GetConnections())
				{
					if (connection == null)
						continue;

					var from = GetCurvePosition(slot.Point, outSlotDirection);
					var to = GetCurvePosition(connection.InSlotPoint, inSlotDirection);

					if ((from.x - (curveTypeDistance * scaleValue)) <= to.x)
					{
						CurveFromTo(
							from,
							to,
							outSlotDirection,
							inSlotDirection,
							connection.CurveColor,
							scaleValue);
					}
					else
					{
						CurveFromTo2(
							from,
							to,
							outSlotDirection,
							inSlotDirection,
							connection.CurveColor, scaleValue);
					}
				}
			}
		}

		public static void DrawNodeButtons(GraphNode node, CurveDirection inSlotDirection, CurveDirection outSlotDirection, string horizontalSymbol, bool outMultiSlots, GUIStyle connectionStyle)
		{
			if (node.InSlotEnable)
			{
				var rect = GetConnectionRect(node.InSlotPoint, inSlotDirection);
				GUI.Label(rect, horizontalSymbol, connectionStyle);
			}

			foreach (var slot in node.OutSlots)
			{
				var rect = GetConnectionRect(slot.Point, outSlotDirection);
				GUI.Label(rect, horizontalSymbol, connectionStyle);
			}
		}

		public static Vector2 DrawNodeWindow(int index, GraphNode node, DrawMode drawMode, float scaleValue, GUIStyle style, GUI.WindowFunction callback)
		{
			var diff = Vector2.zero;

			var saveColor = GUI.backgroundColor;

			var oldPosition = new Vector2((int)(node.WindowPosition.x * scaleValue), (int)(node.WindowPosition.y * scaleValue));

			if (node.Running)
			{
				// TODO не работает со скейлом
				GUI.backgroundColor = node.BorderColor;
				GUI.Box(new Rect(oldPosition.x - borderSize, oldPosition.y - borderSize, node.WindowSize.x + borderSize * 2, node.WindowSize.y + borderSize * 2), "", GUI.skin.button);
			}

			GUI.backgroundColor = node.BackColor;

			var rect = GUILayout.Window(
				index,
				new Rect(oldPosition.x, oldPosition.y, 0, 0),
				callback,
				node.Name,
				style,
				new GUILayoutOption[] { GUILayout.MinHeight((int)(minWindowHeight * scaleValue)), GUILayout.MinWidth((int)(minWindowWidth * scaleValue)) });

			if (Event.current.type == EventType.repaint)
				node.WindowSize = new Vector2(rect.width, rect.height);

			var position = new Vector2(rect.x, rect.y);
			if (oldPosition != position)
			{
				var diff2 = oldPosition - position;
				diff = new Vector2((int)(diff2.x / scaleValue), (int)(diff2.y / scaleValue));

				node.WindowPosition -= diff;
				node.MarkAsDirty();
			}

			GUI.backgroundColor = saveColor;

			return diff;
		}

		public static void WindowCallback(int id, GraphNode node, GraphNodeEditor editor, SlotLabel inSlotLabelPlace, SlotLabel outSlotLabelPlace, CurveDirection inSlotDirection, CurveDirection outSlotDirection, DrawMode drawMode, float scaleValue, GUIStyle labelStyle)
		{
			if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Show Inspector"), node.ShowInspector, ShowHideInspectorCallback, node);
				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Select Node"), false, SelectNodeCallback, node);
				menu.AddItem(new GUIContent("Destroy Node"), false, DestroyNodeCallback, node);

				node.OnContextMenu(menu);

				menu.ShowAsContext();

				Event.current.Use();
			}

			GUILayout.BeginVertical();

			if (drawMode == DrawMode.Minimal)
			{
				DrawSlotsMinimal(node, inSlotDirection, outSlotDirection, scaleValue, labelStyle);
			}
			else if (drawMode == DrawMode.SignalsOneLine)
			{
				DrawSlotsSingleLine(node, inSlotDirection, outSlotDirection, scaleValue, labelStyle);
			}
			else if (drawMode == DrawMode.WithoutSeparators)
			{
				DrawSlots(node, inSlotDirection, outSlotDirection, scaleValue, labelStyle);
			}
			else
			{
				bool needSeparator = false;
				needSeparator = DrawHint(node, needSeparator, labelStyle);

				if (inSlotLabelPlace == SlotLabel.Before)
					needSeparator = DrawInSlots(node, needSeparator, inSlotDirection, scaleValue, labelStyle);
				if (outSlotLabelPlace == SlotLabel.Before)
					needSeparator = DrawOutSlots(node, needSeparator, outSlotDirection, scaleValue, labelStyle);

				if (drawMode == DrawMode.None)
					needSeparator = DrawInspector(node, editor, needSeparator);

				if (inSlotLabelPlace == SlotLabel.After)
					needSeparator = DrawInSlots(node, needSeparator, inSlotDirection, scaleValue, labelStyle);
				if (outSlotLabelPlace == SlotLabel.After)
					needSeparator = DrawOutSlots(node, needSeparator, outSlotDirection, scaleValue, labelStyle);
			}

			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		private static void DrawSlotsMinimal(GraphNode node, CurveDirection inSlotDirection, CurveDirection outSlotDirection, float scaleValue, GUIStyle labelStyle)
		{
			var outSlot = node.OutSlots.Length != 0 ? node.OutSlots[0].Point : Vector2.zero;
			DrawTwoLabels(node, "", ref node.InSlotPoint, "", ref outSlot, scaleValue, labelStyle);
			foreach (var slot in node.OutSlots)
				slot.Point = outSlot;
		}

		private static void DrawSlotsSingleLine(GraphNode node, CurveDirection inSlotDirection, CurveDirection outSlotDirection, float scaleValue, GUIStyle labelStyle)
		{
			var outSlot = node.OutSlots.Length != 0 ? node.OutSlots[0].Point : Vector2.zero;
			DrawTwoLabels(node, (node.InSlotEnable ? node.InSlotName : ""), ref node.InSlotPoint, (node.OutSlots.Length != 0 ? "Output" : ""), ref outSlot, scaleValue, labelStyle);
			foreach (var slot in node.OutSlots)
				slot.Point = outSlot;
		}

		private static void DrawSlots(GraphNode node, CurveDirection inSlotDirection, CurveDirection outSlotDirection, float scaleValue, GUIStyle labelStyle)
		{
			if (node.OutSlots.Length != 0)
			{
				for (int index = 0; index < node.OutSlots.Length; index++)
				{
					var slot = node.OutSlots[index];

					if (index == 0 && node.InSlotEnable)
					{
						DrawTwoLabels(node, node.InSlotName, ref node.InSlotPoint, slot.Name, ref slot.Point, scaleValue, labelStyle);
					}
					else
					{
						DrawLabel(node, outSlotDirection, slot.Name, ref slot.Point, scaleValue, labelStyle);
					}
				}
			}
			else if (node.InSlotEnable)
			{
				DrawLabel(node, inSlotDirection, node.InSlotName, ref node.InSlotPoint, scaleValue, labelStyle);
			}
		}

		private static bool DrawInSlots(GraphNode node, bool needSeparator, CurveDirection inSlotDirection, float scaleValue, GUIStyle labelStyle)
		{
			if (node.InSlotEnable)
			{
				if (needSeparator)
					GUILayout.Box(GUIContent.none, Separator, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

				DrawLabel(node, inSlotDirection, node.InSlotName, ref node.InSlotPoint, scaleValue, labelStyle);

				needSeparator = true;
			}
			return needSeparator;
		}

		private static bool DrawOutSlots(GraphNode node, bool needSeparator, CurveDirection outSlotDirection, float scaleValue, GUIStyle labelStyle)
		{
			if (node.OutSlots.Length != 0)
			{
				if (needSeparator)
					GUILayout.Box(GUIContent.none, Separator, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

				foreach (var slot in node.OutSlots)
				{
					DrawLabel(node, outSlotDirection, slot.Name, ref slot.Point, scaleValue, labelStyle);
				}

				needSeparator = true;
			}

			return needSeparator;
		}

		private static bool DrawHint(GraphNode node, bool needSeparator, GUIStyle labelStyle)
		{
			if (string.IsNullOrEmpty(node.Hint))
				return false;

			if (needSeparator)
				GUILayout.Box(GUIContent.none, Separator, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(node.Hint, labelStyle);
			EditorGUILayout.EndHorizontal();

			return true;
		}

		private static void DrawTwoLabels(GraphNode node, string inName, ref Vector2 inRect, string outName, ref Vector2 outRect, float scaleValue, GUIStyle labelStyle)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(inName, labelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label(outName, labelStyle);
			EditorGUILayout.EndHorizontal();

			if (Event.current.type == EventType.repaint)
			{
				var rect = GUILayoutUtility.GetLastRect();
                inRect = GetSlotPoint(rect, new Vector2((int)(node.WindowPosition.x * scaleValue), (int)(node.WindowPosition.y * scaleValue)), node.WindowSize, CurveDirection.Left);
				outRect = GetSlotPoint(rect, new Vector2((int)(node.WindowPosition.x * scaleValue), (int)(node.WindowPosition.y * scaleValue)), node.WindowSize, CurveDirection.Right);
			}
		}

		private static void DrawLabel(GraphNode node, CurveDirection slotDirection, string name, ref Vector2 rect, float scaleValue, GUIStyle labelStyle)
		{
			if (slotDirection == CurveDirection.Left)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(name, labelStyle);
				EditorGUILayout.EndHorizontal();

				if (Event.current.type == EventType.repaint)
					rect = GetSlotPoint(GUILayoutUtility.GetLastRect(), new Vector2((int)(node.WindowPosition.x * scaleValue), (int)(node.WindowPosition.y * scaleValue)), node.WindowSize, slotDirection);
			}
			else if (slotDirection == CurveDirection.Right)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label(name, labelStyle);
				EditorGUILayout.EndHorizontal();

				if (Event.current.type == EventType.repaint)
					rect = GetSlotPoint(GUILayoutUtility.GetLastRect(), new Vector2((int)(node.WindowPosition.x * scaleValue), (int)(node.WindowPosition.y * scaleValue)), node.WindowSize, slotDirection);
			}
		}

		private static bool DrawInspector(GraphNode node, GraphNodeEditor editor, bool needSeparator)
		{
			if (node.ShowInspector)
			{
				if (needSeparator)
					GUILayout.Box(GUIContent.none, Separator, GUILayout.ExpandWidth(true), GUILayout.Height(1f));

				editor.OnInspectorGUI();

				needSeparator = true;
			}
			return needSeparator;
		}

		private static void ShowHideInspectorCallback(object userData)
		{
			var node = (GraphNode)userData;
			node.ShowInspector = !node.ShowInspector;
		}

		private static void DestroyNodeCallback(object userData)
		{
			var node = (GraphNode)userData;
			var gameObject = node.gameObject;
			GameObject.DestroyImmediate(node);
			EditorUtility.SetDirty(gameObject);
		}

		private static void SelectNodeCallback(object userData)
		{
			var node = (GraphNode)userData;
			ExpandInspector(node);
		}

		public static void CurveFromTo(Vector2 start, Vector2 end, CurveDirection startDirection, CurveDirection endDirection, Color color, float scaleValue)
		{
			Handles.DrawBezier(
				start,
				end,
				GetTangent(start, startDirection, scaleValue),
				GetTangent(end, endDirection, scaleValue),
				color,
				null,
				2f);
		}

		public static void CurveFromTo2(Vector2 start, Vector2 end, CurveDirection startDirection, CurveDirection endDirection, Color color, float scaleValue)
		{
			var leftOffset = (start.y < end.y) ? (curveTypeOffset * scaleValue) : (-curveTypeOffset * scaleValue);
			var starty1 = start.y + leftOffset;
			var endy1 = end.y + -leftOffset;

			var start1 = new Vector2(end.x, endy1);
			var end1 = new Vector2(start.x, starty1);

			var tangentEnd = (end1 - start1).normalized * tangentOffset * scaleValue;
			var tangentStart = new Vector2(-tangentEnd.x, -tangentEnd.y);

			Handles.DrawBezier(
				start,
				end1,
				GetTangent(start, startDirection, scaleValue),
				end1 + tangentEnd,
				color,
				null,
				2f);

			Handles.DrawBezier(
				start1,
				end,
				start1 + tangentStart,
				GetTangent(end, endDirection, scaleValue),
				color,
				null,
				2f);

			Handles.DrawBezier(
				start1,
				end1,
				start1,
				end1,
				color,
				null,
				2f);
		}

		public static Vector2 GetTangent(Vector2 point, CurveDirection direction, float scaleValue)
		{
			if (direction == CurveDirection.Left)
				point.x -= tangentOffset * scaleValue;
			else if (direction == CurveDirection.Right)
				point.x += tangentOffset * scaleValue;
			return point;
		}

		public static GUIStyle Separator
		{
			get
			{
				if (separator == null)
				{
					separator = new GUIStyle("box");
					separator.border.top = separator.border.bottom = 1;
					separator.margin.top = separator.margin.bottom = 5;
					separator.padding.top = separator.padding.bottom = 1;
				}

				return separator;
			}
		}

		private static void ExpandInspector(GraphNode node)
		{
			var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");

			var GetAllInspectorWindows = inspectorType.GetMethod("GetAllInspectorWindows");
			var array = (Array)GetAllInspectorWindows.Invoke(null, null);
			foreach (var inspectorWindow in array)
			{
				var GetInspectedObject = inspectorType.GetMethod("GetInspectedObject");
				var inspectedObject = (UnityEngine.Object)GetInspectedObject.Invoke(inspectorWindow, null);
				if (inspectedObject != node.gameObject)
					continue;

				var GetTracker = inspectorType.GetMethod("GetTracker");
				var tracker = (UnityEditor.ActiveEditorTracker)GetTracker.Invoke(inspectorWindow, null);
				if (tracker != null)
				{
					for (int index = 0; index < tracker.activeEditors.Length; index++)
					{
						var editor = tracker.activeEditors[index];
						var show = editor.target == node ? 1 : 0;
						tracker.SetVisible(index, show);

						UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(editor, show == 1);
						editor.Repaint();
					}
				}
			}
		}

		public static Vector2 GetCurvePosition(Vector2 position, CurveDirection direction)
		{
			return position;
		}

		private static Rect GetConnectionRect(Vector2 point, CurveDirection direction)
		{
			int leftOffect = -2;
			int rightOffect = 1;
			var size = new Vector2(16, 16);

			if (direction == CurveDirection.Left)
				return new Rect(point.x - size.x / 2 - leftOffect, point.y - size.y / 2, size.x, size.y);
			else if (direction == CurveDirection.Right)
				return new Rect(point.x - size.x / 2 + rightOffect, point.y - size.y / 2, size.x, size.y);

			return new Rect();
		}

		private static Vector2 GetSlotPoint(Rect labelRect, Vector2 windowPosition, Vector2 windowSize, CurveDirection direction)
		{
			int distance = 7;

			if (direction == CurveDirection.Left)
				return new Vector2(windowPosition.x - distance, labelRect.y + labelRect.height / 2 + windowPosition.y);
			else if (direction == CurveDirection.Right)
				return new Vector2(windowPosition.x + windowSize.x + distance, labelRect.y + labelRect.height / 2 + windowPosition.y);

			return Vector2.zero;
		}

		public static SlotInfo GetOutSlot(Vector2 mousePosition, GraphNode node, Vector2 slotSize, CurveDirection direction, bool outMultiSlots, ref int offsetResult, ref int outIndex, Vector2 offset)
		{
			foreach (var slot in node.OutSlots)
			{
				var rect = new Rect(slot.Point.x - slotSize.x / 2 + offset.x, slot.Point.y - slotSize.y / 2 + offset.y, slotSize.x, slotSize.y);
				//Debug.Log("Output - MousePosition : " + mousePosition + " , Rect : " + rect);
				if (rect.Contains(mousePosition))
					return slot;
			}
			return null;
		}

		public static bool IsInSlot(Vector2 mousePosition, GraphNode node, Vector2 slotSize, CurveDirection direction, Vector2 offset)
		{
			var rect = new Rect(node.InSlotPoint.x - slotSize.x / 2 + offset.x, node.InSlotPoint.y - slotSize.y / 2 + offset.y, slotSize.x, slotSize.y);
			//Debug.Log("Input - MousePosition : " + mousePosition + " , Rect : " + rect);
			return rect.Contains(mousePosition);
		}

		private static GUIStyle separator;
		private static int minWindowWidth = 180;
		private static int minWindowHeight = 45;
		private const int multiDistance = 14;
		private const float tangentOffset = 50;
		private const float curveTypeDistance = 100;
		private const float curveTypeOffset = 100;
		private const int borderSize = 4;
	}
}
*/