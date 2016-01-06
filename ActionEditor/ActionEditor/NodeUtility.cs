using System;

namespace ActionEditor2
{
	public static class NodeUtility
	{
		public static void Send(this InputSignal[] inputSignals)
		{
			foreach (var inputSignal in inputSignals)
				inputSignal.Send();
		}

		public static void Send(this InputSignal inputSignal)
		{
			if (inputSignal != null && inputSignal.Action != null)
				inputSignal.Action();
		}

		public static void AddInputSignal(this OutputSignal outputSignal, InputSignal inputSignal)
		{
			ArrayUtility.AddItem(ref outputSignal.InputSignals, inputSignal);
		}
	}
}