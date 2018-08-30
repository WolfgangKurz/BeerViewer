using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules.Communication
{
	internal static class Serialization
	{
		public static bool Serializable(object obj)
		{
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

			// Dictionary
			else if (typeof(IDictionary).IsAssignableFrom(type))
			{
				var dict = (IDictionary)obj;
				foreach (DictionaryEntry kvp in dict)
				{
					var fieldName = Convert.ToString(kvp.Key);
					if (!Serializable(kvp.Value)) return false;
				}
				return true;
			}

			// Array
			else if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				var enumerable = (IEnumerable)obj;

				foreach (object arrObj in enumerable)
				{
					if (!Serializable(arrObj)) return false;
				}
				return true;
			}

			// Class/structs to Dictionary (key,value pairs)
			else if (!type.IsPrimitive && !type.IsEnum)
			{
				var fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					// var fieldName = fields[i].Name;
					var fieldValue = fields[i].GetValue(obj);
					if(!Serializable(fieldValue)) return false;
				}

				var properties = type.GetProperties();
				for (int i = 0; i < properties.Length; i++)
				{
					// var propertyName = properties[i].Name;
					var propertyValue = properties[i].GetValue(obj);
					if (!Serializable(propertyValue)) return false;
				}
				return true;
			}
			else
				return false;
		}
	}
}
