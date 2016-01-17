using System;
using System.Collections.Generic;

namespace ActionEditor
{
	[Serializable]
	public class ActionGraphStore
	{
		public void Clear()
		{
			Nodes.Clear();
        }

		public List<NodeStore> Nodes = new List<NodeStore>();
	}
}