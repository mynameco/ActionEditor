using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ActionEditor
{
	[Serializable]
	public class ConnectionStore
	{
		public string OutputName;
		public int InputIndex;
		public string InputName;
	}
}