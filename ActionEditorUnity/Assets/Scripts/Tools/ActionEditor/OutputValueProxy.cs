using System;

namespace ActionEditor
{
	public class OutputValueProxy : IValue
	{
		public OutputValueProxy(OutputValue value)
		{
			this.value = value;
		}

		public Type Type
		{
			get { return value.Type; }
		}

		public object Value
		{
			get { return value.InputValues.GetValue(value.Type); }
			set { this.value.InputValues.SetValue(value, this.value.Type); }
		}

		private OutputValue value;
	}
}
