using System;
using System.Linq;
using ActionEditor2;

namespace ActionEditor
{
	class Program
	{
		static void Main(string[] args)
		{
			var node1 = new CustomNode();
			var inputSignal1 = new InputSignal() { Name = "Input1", Owner = node1, Action = () => { Console.WriteLine("Input1"); } };
			node1.InputSignals = new InputSignal[] { inputSignal1 };

			var node2 = new InputSignalNode();
			node2.Name = "Input1";
			node2.OutputSignals.First().AddInputSignal(inputSignal1);

			//node2.Signals.First().Slots.Send();

			var rootNode = new CompositeNode();
			rootNode.Nodes = new Node[] { node1, node2 };
			rootNode.Invalidate();

			var rootInputSignal = rootNode.InputSignals.FirstOrDefault();
			rootInputSignal.Send();
		}
	}
}
