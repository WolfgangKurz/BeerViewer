using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules.Communication
{
	internal static class Serialization
	{
#if LOG_SERIALIZATION
		private static StreamWriter writer;
		static Serialization()
		{
			writer = new StreamWriter(new FileStream("serializable.txt", FileMode.Create));
		}
#endif

		public static bool Serializable(object obj, string name = "...", int depth = 0)
		{
#if LOG_SERIALIZATION
			{
				var x = obj?.ToString() ?? "";
				x = x.Replace(Environment.NewLine, "\\n");
				writer.WriteLine($"{new string(' ', depth)}{name}: {x}");
			}
#endif
			if (obj == null) return true;

			var type = obj.GetType();
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null) type = underlyingType;

			if (type == typeof(bool))
				return true;
			else if (type == typeof(int)) // Int32
				return true;
			else if (type == typeof(string))
				return true;
			else if (type == typeof(double))
				return true;
			else if (type == typeof(decimal))
				return true;
			else if (type == typeof(sbyte))
				return true;
			else if (type == typeof(short)) // Int16
				return true;
			else if (type == typeof(long)) // Int64
				return true;
			else if (type == typeof(byte))
				return true;
			else if (type == typeof(ushort)) // UInt16
				return true;
			else if (type == typeof(uint)) // UInt32
				return true;
			else if (type == typeof(ulong)) // UInt64
				return true;
			else if (type == typeof(float)) // Single
				return true;
			else if (type == typeof(char))
				return true;
			else if (type == typeof(DateTime))
				return true;

			// Serialize enum to sbyte, short, int, long, byte, ushort, uint, ulong (check type of enum)
			else if (type.IsEnum)
			{
				var subType = Enum.GetUnderlyingType(type);
				return (subType == typeof(SByte) ||
					subType == typeof(Int16) ||
					subType == typeof(Int32) ||
					subType == typeof(Byte) ||
					subType == typeof(UInt16) ||
					subType == typeof(Int64) ||
					subType == typeof(UInt32) ||
					subType == typeof(UInt64));
			}

			// Dictionary
			if (typeof(IDictionary).IsAssignableFrom(type))
			{
				var dict = (IDictionary)obj;
				foreach (DictionaryEntry kvp in dict)
				{
					var fieldName = Convert.ToString(kvp.Key);
					if (!Serializable(kvp.Value, fieldName, depth + 1)) return false;
				}
				return true;
			}

			// Array
			else if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				var enumerable = (IEnumerable)obj;

				foreach (object arrObj in enumerable)
				{
					if (!Serializable(arrObj, "___", depth + 1)) return false;
				}
				return true;
			}

			// Class/structs to Dictionary (key,value pairs)
			else if (!type.IsPrimitive && !type.IsEnum)
			{
				var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
				for (int i = 0; i < fields.Length; i++)
				{
					var fieldName = fields[i].Name;
					var fieldValue = fields[i].GetValue(obj);
					if (!Serializable(fieldValue, fieldName, depth + 1)) return false;
				}

				var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				for (int i = 0; i < properties.Length; i++)
				{
					var propertyName = properties[i].Name;
					var propertyValue = properties[i].GetValue(obj);
					if (!Serializable(propertyValue, propertyName, depth + 1)) return false;
				}
				return true;
			}
			else
				return false;
		}
	}
}
