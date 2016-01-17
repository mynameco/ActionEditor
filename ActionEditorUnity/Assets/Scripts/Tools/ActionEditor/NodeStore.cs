using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionEditor
{
	[Serializable]
	public class NodeStore
	{
		public string Type;
		public Vector2 Position;
		public List<ConnectionStore> Signals = new List<ConnectionStore>();
		public List<ConnectionStore> Values = new List<ConnectionStore>();
	}
}