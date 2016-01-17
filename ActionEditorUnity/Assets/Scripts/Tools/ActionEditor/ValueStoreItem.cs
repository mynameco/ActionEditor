using System.Collections.Generic;

namespace ActionEditor
{
	public class ValueStoreItem<T>
	{
		public ValueStoreItem(List<T> values)
		{
			this.values = values;
		}

		public T Get(int index)
		{
			if (index < 0 || index >= values.Count)
				return default(T);
			return values[index];
		}

		public void Set(int index, T value)
		{
			if (index < 0 || index >= values.Count)
				return;
			values[index] = value;
		}

		public int Add(T value)
		{
			values.Add(value);
			return values.Count - 1;
		}

		public void Clear()
		{
			values.Clear();
		}

		private List<T> values;
	}
}