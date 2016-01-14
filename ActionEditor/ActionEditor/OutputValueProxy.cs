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
			get { return value.InputValues.GetValue<object>(); }
			set { this.value.InputValues.SetValue<object>(value); }
		}

		private OutputValue value;
	}
}
