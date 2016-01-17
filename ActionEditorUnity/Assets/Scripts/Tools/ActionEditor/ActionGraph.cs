using System;
using System.Collections.Generic;

namespace ActionEditor
{
	public class ActionGraph : CompositeNode
	{
		public void Load(ActionGraphStore store)
		{
			var nodes = new List<Node>();
			foreach (var item in store.Nodes)
			{
				var node = NodeFactory.Create(item.Type);
				if (node == null)
					continue;

				node.Load(item);
				nodes.Add(node);
			}

			Nodes = nodes.ToArray();
		}

		public void Save(ActionGraphStore store)
		{
            foreach (var node in Nodes)
			{
				if (node == null)
					continue;

				var nodeStore = new NodeStore();
				nodeStore.Type = node.GetType().Name;
				node.Save(nodeStore);
				store.Nodes.Add(nodeStore);
            }
		}
	}
}