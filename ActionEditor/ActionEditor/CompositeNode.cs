using System;

namespace ActionEditor
{
	public class CompositeNode : Node
	{
		public Node[] Nodes = new Node[0];

		public void Invalidate()
		{
			InputSignals = new InputSignal[0];
			OutputSignals = new OutputSignal[0];

			foreach (var node in Nodes)
			{
				var inputSignalNode = node as InputSignalNode;
				if (inputSignalNode != null)
				{
					var inputSignal = new InputSignal() { Name = inputSignalNode.Name, Owner = this, Action = () => { inputSignalNode.OutputSignal.InputSignals.Send(); } };
					ArrayUtility.AddItem(ref InputSignals, inputSignal);
					continue;
				}

				var outputSignalNode = node as OutputSignalNode;
				if (outputSignalNode != null)
				{
					var outputSignal = new OutputSignal() { Name = outputSignalNode.Name };
					ArrayUtility.AddItem(ref OutputSignals, outputSignal);
					outputSignalNode.InputSignal.Action = () => { outputSignal.InputSignals.Send(); };
					continue;
				}
			}
		}
	}
}