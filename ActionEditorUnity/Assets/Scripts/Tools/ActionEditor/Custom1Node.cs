using System;

namespace ActionEditor
{
	[Node]
	public class Custom1Node : Node
	{
		public Custom1Node()
		{
			var outputSignal = new OutputSignal() { Name = "Output" };
			ArrayUtility.Add(ref OutputSignals, outputSignal);

			var inputSignal = new InputSignal() { Name = "Input", Owner = this, Action = () => { outputSignal.InputSignals.Send(); } };
			ArrayUtility.Add(ref InputSignals, inputSignal);
		}
	}
}
