using System;

namespace ActionEditor
{
	public class InputValueNode : Node
	{
		public InputValueNode(Type type)
		{
			OutputValue = new OutputValue() { Name = "Output", Type = type };
			OutputValues = new OutputValue[] { OutputValue };
		}

		public string Name;
		public OutputValue OutputValue;
	}
}
