using System;
using System.Linq;

namespace ActionEditor
{
	public static class ValueUtility
	{
		public static object GetValue(this InputValue[] inputValues, Type type)
		{
			foreach (var inputValue in inputValues)
				return inputValue.GetValue(type);
			return null;
		}

		public static object GetValue(this InputValue inputValue, Type type)
		{
			if (inputValue == null || inputValue.Value == null || inputValue.Value.Type != type)
				return null;

			return inputValue.Value.Value;
		}

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

			try
			{
				return (T)inputValue.Value.Value;
			}
			catch (Exception)
			{
			}
			return default(T);
		}

		public static void SetValue(this InputValue[] inputValues, object value, Type type)
		{
			foreach (var inputValue in inputValues)
				inputValue.SetValue(value, type);
		}

		public static void SetValue(this InputValue inputValue, object value, Type type)
		{
			if (inputValue == null || inputValue.Value == null || inputValue.Value.Type != type)
				return;

			inputValue.Value.Value = value;
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

			inputValue.Value.Value = value;
		}

		public static void AddInputValue(this OutputValue outputValue, InputValue inputValue)
		{
			ArrayUtility.Add(ref outputValue.InputValues, inputValue);
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
