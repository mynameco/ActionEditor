using System;

namespace ActionEditor
{
	public class SimpleValue<T> : IValue
	{
		public SimpleValue(T value)
		{
			this.value = value;
		}

		public Type Type
		{
			get { return typeof(T); }
		}

		public object Value
		{
			get { return value; }
			set
			{
				try
				{
					this.value = (T)value;
				}
				catch (Exception)
				{
					this.value = default(T);
				}
			}
		}

		private T value;
	}
}
