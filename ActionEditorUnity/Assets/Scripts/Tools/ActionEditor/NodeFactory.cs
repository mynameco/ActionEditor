using System.Collections.Generic;
using System;
using UnityEngine;

namespace ActionEditor
{
	public static class NodeFactory
	{
		static NodeFactory()
		{
			var types = typeof(NodeFactory).Assembly.GetTypes();
			foreach (var type in types)
			{
				if (!type.IsDefined(typeof(NodeAttribute), false))
					continue;

				if (!typeof(Node).IsAssignableFrom(type))
					continue;

				// Какие то проблемы с захватом
				var nodeType = type;

				var method = new Func<Node>(() =>
				{
					//Debug.Log("Create node : " + nodeType.Name);

					var instance = Activator.CreateInstance(nodeType);
					var item = (Node)instance;
					return item;
				});

				lock (factories)
				{
					factories.Add(type.Name, method);
				}
			}
		}

		public static Node Create(string name)
		{
			Func<Node> func;

			lock (factories)
			{
				if (!factories.TryGetValue(name, out func))
					return null;
			}

			return func();
		}

		public static IEnumerable<string> Types
		{
			get
			{
				lock (factories)
				{
					var list = new List<string>(factories.Keys);
					return list;
				}
			}
		}

		private static Dictionary<string, Func<Node>> factories = new Dictionary<string, Func<Node>>();
	}
}