using System;

namespace ActionEditor
{
	[Node]
	public class CompositeNode : Node
	{
		public Node[] Nodes = new Node[0];

		public void Invalidate()
		{
			InputSignals = new InputSignal[0];
			OutputSignals = new OutputSignal[0];
			InputValues = new InputValue[0];
			OutputValues = new OutputValue[0];

			foreach (var node in Nodes)
			{
				var inputSignalNode = node as InputSignalNode;
				if (inputSignalNode != null)
				{
					var inputSignal = new InputSignal() { Name = inputSignalNode.Name, Owner = this, Action = () => { inputSignalNode.OutputSignal.InputSignals.Send(); } };
					ArrayUtility.Add(ref InputSignals, inputSignal);
					continue;
				}

				var outputSignalNode = node as OutputSignalNode;
				if (outputSignalNode != null)
				{
					var outputSignal = new OutputSignal() { Name = outputSignalNode.Name };
					ArrayUtility.Add(ref OutputSignals, outputSignal);
					outputSignalNode.InputSignal.Action = () => { outputSignal.InputSignals.Send(); };
					continue;
				}

				var inputValueNode = node as InputValueNode;
				if (inputValueNode != null)
				{
					var inputValue = new InputValue() { Name = inputValueNode.Name, Owner = this, Value = new OutputValueProxy(inputValueNode.OutputValue) };
					ArrayUtility.Add(ref InputValues, inputValue);
					continue;
				}

				var outputValueNode = node as OutputValueNode;
				if (outputValueNode != null)
				{
					var outputValue = new OutputValue() { Name = outputValueNode.Name, Type = outputValueNode.Type };
					ArrayUtility.Add(ref OutputValues, outputValue);
					outputValueNode.InputValue.Value = new OutputValueProxy(outputValue);
					continue;
				}
			}
		}
	}
}