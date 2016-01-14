using System;

namespace ActionEditor
{
	public class OutputValueNode : Node
	{
		public OutputValueNode(Type type)
		{
			Type = type;
			InputValue = new InputValue() { Name = "Input", Owner = this, Value = null };
			InputValues = new InputValue[] { InputValue };
		}

		public string Name;
		public Type Type;
		public InputValue InputValue;
	}
}
