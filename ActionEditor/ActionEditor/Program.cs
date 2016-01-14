using System;
using System.Linq;

namespace ActionEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			var inputSignalNode = new InputSignalNode();
			inputSignalNode.Name = "Input";

			var outputSignalNode = new OutputSignalNode();
			outputSignalNode.Name = "Output";

			var custom1Node = new Custom1Node();

			inputSignalNode.ConnectSignal(custom1Node, "Output", "Input");
			custom1Node.ConnectSignal(outputSignalNode, "Output", "Input");

			var inputValueNode = new InputValueNode(typeof(string));
			inputValueNode.Name = "Input";

			var outputValueNode = new OutputValueNode(typeof(string));
			outputValueNode.Name = "Output";

			inputValueNode.ConnectValue(outputValueNode, "Output", "Input");

			var rootNode = new CompositeNode();
			rootNode.Nodes = new Node[] { outputSignalNode, inputSignalNode, inputValueNode, outputValueNode };
			rootNode.Invalidate();

			var rootOutputSignal = rootNode.OutputSignals.FirstOrDefault();
			if (rootOutputSignal != null)
				ArrayUtility.AddItem(ref rootOutputSignal.InputSignals, new InputSignal() { Action = () => { Console.WriteLine("TestSignal"); } });

			var rootInputSignal = rootNode.InputSignals.FirstOrDefault();
			rootInputSignal.Send();

			var rootOutputValue = rootNode.OutputValues.FirstOrDefault();
			if (rootOutputValue != null)
				ArrayUtility.AddItem(ref rootOutputValue.InputValues, new InputValue() { Value = new SimpleValue<string>("TestValue") });

			var rootInputValue = rootNode.InputValues.FirstOrDefault();
			if (rootInputValue != null)
			{
				var resultValue = rootInputValue.GetValue<string>();
				Console.WriteLine(resultValue);
			}
		}
	}
}
