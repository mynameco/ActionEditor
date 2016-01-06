using System;

namespace ActionEditor2
{
	public abstract class Node
	{
		public InputSignal[] InputSignals = new InputSignal[0];
		public OutputSignal[] OutputSignals = new OutputSignal[0];
	}
}