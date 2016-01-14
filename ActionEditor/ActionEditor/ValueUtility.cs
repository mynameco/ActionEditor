using System.Linq;

namespace ActionEditor
{
	public static class ValueUtility
	{
		public static T GetValue<T>(this InputValue[] inputValues)
		{
			foreach (var inputValue in inputValues)
				return inputValue.GetValue<T>();
			return default(T);
		}

		public static T GetValue<T>(this InputValue inputValue)
		{
			if (inputValue == null || inputValue.Value == null || inputValue.Value.Type != typeof(T))
				return default(T);

			var valueT = inputValue.Value as IValue<T>;
			if (valueT == null)
				return default(T);

			return valueT.Value;
		}

		public static void SetValue<T>(this InputValue[] inputValues, T value)
		{
			foreach (var inputValue in inputValues)
				inputValue.SetValue<T>(value);
		}

		public static void SetValue<T>(this InputValue inputValue, T value)
		{
			if (inputValue == null || inputValue.Value == null || inputValue.Value.Type != typeof(T))
				return;

			var valueT = inputValue.Value as IValue<T>;
			if (valueT == null)
				return;

			valueT.Value = value;
		}

		public static void AddInputValue(this OutputValue outputValue, InputValue inputValue)
		{
			ArrayUtility.AddItem(ref outputValue.InputValues, inputValue);
		}

		public static void ConnectValue(this Node outputNode, Node inputNode, string outputName, string inputName)
		{
			var output = outputNode.OutputValues.FirstOrDefault(o => o.Name == outputName);
			if (output != null)
			{
				var input = inputNode.InputValues.FirstOrDefault(o => o.Name == inputName);
				if (input != null)
					output.AddInputValue(input);
			}
		}
	}
}
