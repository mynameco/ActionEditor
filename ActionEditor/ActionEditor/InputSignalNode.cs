using System;

namespace ActionEditor2
{
	public class InputSignalNode : Node
	{
		public InputSignalNode()
		{
			OutputSignal = new OutputSignal() { Name = "Output" };
			OutputSignals = new OutputSignal[] { OutputSignal };
		}

		public string Name;
		public OutputSignal OutputSignal;
	}
}