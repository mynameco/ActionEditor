/*using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GraphView
{
	public static class GraphNodeUtility
	{
		public class NodeInfo
		{
			public GraphNode Node;
			public int Deep;
			public int GraphId;
		}

		public static NodeInfo[] RepositionNodes(IEnumerable<GraphNode> nodes)
		{
			//Debug.Log("RepositionNodes");

			var resultNodes = new Dictionary<GraphNode, NodeInfo>();
			foreach (var node in nodes)
				resultNodes.Add(node, new NodeInfo() { Node = node, Deep = -1, GraphId = -1 });

			var startNodes = nodes.Where(o => !HasInputs(o, nodes)).ToArray();

			//Debug.Log("Start nodes : " + startNodes.Length);

			var computeNodes = new HashSet<GraphNode>();

			int lastIndex = 0;
			foreach (var node in startNodes)
			{
				computeNodes.Clear();
				var index = GetGraphIndex(node, resultNodes, computeNodes);
				if (index == -1)
				{
					index = lastIndex;
					lastIndex++;
				}

				resultNodes[node].GraphId = index;

				//Debug.Log("Index : " + index);
			}

			foreach (var node in startNodes)
			{
				var deep = 0;
				computeNodes.Clear();
				ComputeDeep(node, deep, resultNodes, computeNodes);
			}

			return resultNodes.Values.ToArray();
		}

		private static bool HasInputs(GraphNode node, IEnumerable<GraphNode> nodes)
		{
			if (!node.InSlotEnable)
				return false;

			foreach (var node2 in nodes)
			{
				foreach (var slot in node2.OutSlots)
				{
					foreach (var connection in slot.GetConnections())
					{
						if (connection == node)
							return true;
					}
				}
			}

			return false;
		}

		private static int GetGraphIndex(GraphNode node, Dictionary<GraphNode, NodeInfo> resultNodes, HashSet<GraphNode> computeNodes)
		{
			foreach (var slot in node.OutSlots)
			{
				foreach (var connection in slot.GetConnections())
				{
					if (!connection)
						continue;

					if (computeNodes.Contains(connection))
						continue;

					computeNodes.Add(connection);

					var info = resultNodes[connection];
					if (info.GraphId != -1)
						return info.GraphId;

					var id = GetGraphIndex(connection, resultNodes, computeNodes);
					if (id != -1)
						return id;
				}
			}
			return -1;
		}

		private static void ComputeDeep(GraphNode node, int deep, Dictionary<GraphNode, NodeInfo> resultNodes, HashSet<GraphNode> computeNodes)
		{
			foreach (var slot in node.OutSlots)
			{
				foreach (var connection in slot.GetConnections())
				{
					if (!connection)
						continue;

					if (computeNodes.Contains(connection))
						continue;

					computeNodes.Add(connection);

					var info = resultNodes[connection];

					if (info.Deep >= (deep + 1))
						continue;

					info.Deep = deep + 1;

					ComputeDeep(connection, deep + 1, resultNodes, computeNodes);
                }
			}
		}
	}
}*/