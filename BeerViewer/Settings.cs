using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BeerViewer
{
	public class Settings
	{
		#region Base
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		public class SettingValue<T>
		{
			#region Value property
			private T _Value;
			public T Value
			{
				get { return this._Value; }
				set
				{
					if (!object.Equals(this._Value, value))
					{
						this._Value = value;
						this.ValueChanged?.Invoke(this, EventArgs.Empty);
						this.Save();
					}
				}
			}
			#endregion

			public string Name { get; }
			public string Provider { get; }
			public event EventHandler ValueChanged;

			private string FilePath { get; }
				= Path.Combine(
					Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
					"Setting.ini"
				);

			public SettingValue(string Name, string Provider, T DefaultValue)
			{
				this.Name = Name;
				this.Provider = Provider;

				if (File.Exists(FilePath))
				{
					StringBuilder sb = new StringBuilder(255);
					GetPrivateProfileString(Provider, Name, DefaultValue.ToString(), sb, 255, FilePath);

					T v;
					if (this.Parse(sb.ToString(), out v))
					{
						this.Value = v;
						return;
					}
				}
				this.Value = DefaultValue;
			}
			public void Save()
			{
				WritePrivateProfileString(
					this.Provider,
					this.Name,
					this.Value.ToString(),
					FilePath
				);
			}

			private bool Parse(string sValue, out T rValue)
			{
				bool ret = false;
				try
				{
					switch (Type.GetTypeCode(typeof(T)))
					{
						case TypeCode.Char:
							{
								char v;
								ret = char.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.Int16:
							{
								short v;
								ret = short.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.Int32:
							{
								int v;
								ret = int.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.Int64:
							{
								long v;
								ret = long.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}

						case TypeCode.Byte:
							{
								byte v;
								ret = byte.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.UInt16:
							{
								ushort v;
								ret = ushort.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.UInt32:
							{
								uint v;
								ret = uint.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.UInt64:
							{
								ulong v;
								ret = ulong.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}

						case TypeCode.Single:
							{
								float v;
								ret = float.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.Double:
							{
								double v;
								ret = double.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.Boolean:
							{
								bool v;
								ret = bool.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.Decimal:
							{
								decimal v;
								ret = decimal.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}
						case TypeCode.DateTime:
							{
								DateTime v;
								ret = DateTime.TryParse(sValue, out v);
								rValue = (T)Convert.ChangeType(v, typeof(T));
								break;
							}

						default:
							rValue = (T)Convert.ChangeType(sValue, typeof(T));
							break;
					}
				}
				catch
				{
					rValue = default(T);
				}
				return ret;
			}
		}

		private static string getKey([CallerMemberName] string propertyName = "")
			=> nameof(Settings) + "." + propertyName;
		#endregion


		/// <summary>
		/// Current Language Code
		/// </summary>
		public static SettingValue<string> LanguageCode { get; } = new SettingValue<string>(getKey(), "Setting", "en");

		/// <summary>
		/// Includes LOS calculator first fleet's los?
		/// </summary>
		public static SettingValue<bool> IsLOSIncludeFirstFleet { get; } = new SettingValue<bool>(getKey(), "Setting", true);

		/// <summary>
		/// Includes LOS calculator seconds fleet's los?
		/// </summary>
		public static SettingValue<bool> IsLOSIncludeSecondFleet { get; } = new SettingValue<bool>(getKey(), "Setting", false);

		/// <summary>
		/// LOS calculator's formula
		/// </summary>
		public static SettingValue<string> LOSCalcType { get; } = new SettingValue<string>(getKey(), "Setting", "LOSCalc.Type4");
	}
}
