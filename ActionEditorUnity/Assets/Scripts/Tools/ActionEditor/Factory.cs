using System.Collections.Generic;
using System;

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

				var method = new Func<Node>(() =>
				{
					var item = (Node)Activator.CreateInstance(type);
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

		private static Dictionary<string, Func<Node>> factories = new Dictionary<string, Func<Node>>();
	}
}