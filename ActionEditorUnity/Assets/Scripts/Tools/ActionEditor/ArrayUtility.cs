using System;
using System.Collections.Generic;

namespace ActionEditor
{
	public static class ArrayUtility
	{
		public static void Add<T>(ref T[] array, T item)
		{
			var array2 = new T[array.Length + 1];
			Array.Copy(array, array2, array.Length);
			array2[array.Length] = item;
			array = array2;
		}

		public static void Remove<T>(ref T[] array, T item)
		{
			var list = new List<T>(array);
			list.Remove(item);
			array = list.ToArray();
		}
	}
}