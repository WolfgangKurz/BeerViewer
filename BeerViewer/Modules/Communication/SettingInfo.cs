using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BeerViewer.Settings;

namespace BeerViewer.Modules.Communication
{
	public class SettingInfo
	{
		public string Type { get; private set; }

		public string Name { get; private set; }
		public string Provider { get; private set; }
		public object Value { get; private set; }

		public string DisplayName { get; private set; }
		public string Description { get; private set; }
		public string Caution { get; private set; }

		public IEnumerable<object> Enums { get; private set; }
		public string TypeName { get; private set; }

		private SettingInfo() { }
		public static SettingInfo Create(object settingValue)
		{
			var m = typeof(SettableSettingValue<>).MakeGenericType(((dynamic)settingValue).Type);
			var input = (dynamic)Convert.ChangeType(settingValue, m);
			if (input == null) return null;

			var output = new SettingInfo();
			output.Type = ((Type)((dynamic)settingValue).Type).Name;

			{
				var _type = (Type)((dynamic)settingValue).Type;
				if (
					_type == typeof(char) || _type == typeof(Char)
					|| _type == typeof(short) || _type == typeof(Int16)
					|| _type == typeof(int) || _type == typeof(Int32)
					|| _type == typeof(long) || _type == typeof(Int64)
					|| _type == typeof(byte) || _type == typeof(Byte)
					|| _type == typeof(ushort) || _type == typeof(UInt16)
					|| _type == typeof(uint) || _type == typeof(UInt32)
					|| _type == typeof(ulong) || _type == typeof(UInt64)
					|| _type == typeof(float) || _type == typeof(Single)
					|| _type == typeof(double) || _type == typeof(Double)
					|| _type == typeof(decimal) || _type == typeof(Decimal)
				) output.Type = "Number";
			}

			output.Name = input.Name;
			output.Provider = input.Provider;
			output.Value = input.Value;

			output.DisplayName = input.DisplayName;
			output.Description = input.Description;
			output.Caution = input.Caution;

			output.Enums = input.Enums;
			output.TypeName = input.Type.Name;
			return output;
		}
	}
}
