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

			inputSignalNode.Connect(custom1Node, inputSignalNode.OutputSignal.Name, "Input");
			custom1Node.Connect(outputSignalNode, "Output", outputSignalNode.InputSignal.Name);

			var rootNode = new CompositeNode();
			rootNode.Nodes = new Node[] { outputSignalNode, inputSignalNode };
			rootNode.Invalidate();

			var rootOutputSignal = rootNode.OutputSignals.FirstOrDefault();
			if (rootOutputSignal != null)
				ArrayUtility.AddItem(ref rootOutputSignal.InputSignals, new InputSignal() { Action = () => { Console.WriteLine("Signal"); } });

			var rootInputSignal = rootNode.InputSignals.FirstOrDefault();
			rootInputSignal.Send();
		}
	}
}
