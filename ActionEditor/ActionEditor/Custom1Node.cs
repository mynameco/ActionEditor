using System;

namespace ActionEditor
{
	public class Custom1Node : Node
	{
		public Custom1Node()
		{
			var outputSignal = new OutputSignal() { Name = "Output" };
			ArrayUtility.AddItem(ref OutputSignals, outputSignal);

			var inputSignal = new InputSignal() { Name = "Input", Owner = this, Action = () => { outputSignal.InputSignals.Send(); } };
			ArrayUtility.AddItem(ref InputSignals, inputSignal);
		}
	}
}
