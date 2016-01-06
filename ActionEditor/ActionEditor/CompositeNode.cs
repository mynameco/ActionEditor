using System;

namespace ActionEditor
{
	public class CompositeNode : Node
	{
		public Node[] Nodes = new Node[0];

		public void Invalidate()
		{
			foreach (var node in Nodes)
			{
				var inputSignalNode = node as InputSignalNode;
				if (inputSignalNode != null)
				{
					var inputSignal = new InputSignal() { Name = inputSignalNode.Name, Owner = this, Action = () => { inputSignalNode.OutputSignal.InputSignals.Send(); } };
					ArrayUtility.AddItem(ref InputSignals, inputSignal);
					continue;
				}
			}
		}
	}
}