using System;

namespace ActionEditor
{
	public interface IValue
	{
		Type Type { get; }
		object Value { get; set; }
	}
}
