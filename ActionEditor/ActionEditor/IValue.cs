using System;

namespace ActionEditor
{
	public interface IValue
	{
		Type Type { get; }
		object ObjectValue { get; set; }
	}

	public interface IValue<T> : IValue
	{
		T Value { get; set; }
	}
}
