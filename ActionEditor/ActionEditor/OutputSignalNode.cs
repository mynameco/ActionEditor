using System;

namespace ActionEditor
{
	public class OutputSignalNode : Node
	{
		public OutputSignalNode()
		{
			InputSignal = new InputSignal() { Name = "Input", Owner = this, Action = () => { } };
			InputSignals = new InputSignal[] { InputSignal };
		}

		public string Name;
		public InputSignal InputSignal;
	}
}
