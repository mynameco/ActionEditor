using System;

namespace ActionEditor
{
	public static class ArrayUtility
	{
		public static void AddItem<T>(ref T[] array, T item)
		{
			var array2 = new T[array.Length + 1];
			Array.Copy(array, array2, array.Length);
			array2[array.Length] = item;
			array = array2;
        }
	}
}