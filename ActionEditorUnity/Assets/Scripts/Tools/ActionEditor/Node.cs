using UnityEngine;

namespace ActionEditor
{
	public abstract class Node
	{
		public Vector2 Position;

		public InputSignal[] InputSignals = new InputSignal[0];
		public OutputSignal[] OutputSignals = new OutputSignal[0];
		public InputValue[] InputValues = new InputValue[0];
		public OutputValue[] OutputValues = new OutputValue[0];

		public virtual void Load(NodeStore nodeStore)
		{
			Position = nodeStore.Position;
        }

		public virtual void Save(NodeStore nodeStore)
		{
			nodeStore.Position = Position;
        }
	}
}