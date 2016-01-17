using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionEditor
{
	[Serializable]
	public class ValueStore
	{
		public ValueStore()
		{
			Objects = new ValueStoreItem<Object>(objects);
			Ints = new ValueStoreItem<int>(ints);
			Floats = new ValueStoreItem<float>(floats);
			Strings = new ValueStoreItem<string>(strings);
			Vectors3 = new ValueStoreItem<Vector3>(vectors3);
		}

		public void Clear()
		{
			Objects.Clear();
			Ints.Clear();
			Floats.Clear();
			Strings.Clear();
			Vectors3.Clear();
		}

		[SerializeField]
		private List<Object> objects = new List<Object>();
		[SerializeField]
		private List<int> ints = new List<int>();
		[SerializeField]
		private List<float> floats = new List<float>();
		[SerializeField]
		private List<string> strings = new List<string>();
		[SerializeField]
		private List<Vector3> vectors3 = new List<Vector3>();

		public ValueStoreItem<Object> Objects;
		public ValueStoreItem<int> Ints;
		public ValueStoreItem<float> Floats;
		public ValueStoreItem<string> Strings;
		public ValueStoreItem<Vector3> Vectors3;
	}
}