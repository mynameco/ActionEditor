using System;

namespace ActionEditor
{
	public abstract class Node
	{
		public InputSignal[] InputSignals = new InputSignal[0];
		public OutputSignal[] OutputSignals = new OutputSignal[0];
		public InputValue[] InputValues = new InputValue[0];
		public OutputValue[] OutputValues = new OutputValue[0];
	}
}