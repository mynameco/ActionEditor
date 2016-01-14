using System.Linq;

namespace ActionEditor
{
	public static class SignalUtility
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

		public static void ConnectSignal(this Node outputNode, Node inputNode, string outputName, string inputName)
		{
			var output = outputNode.OutputSignals.FirstOrDefault(o => o.Name == outputName);
			if (output != null)
			{
				var input = inputNode.InputSignals.FirstOrDefault(o => o.Name == inputName);
				if (input != null)
					output.AddInputSignal(input);
			}
		}
	}
}