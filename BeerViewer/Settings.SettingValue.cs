using BeerViewer.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer
{
	public partial class Settings
	{
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
							if (typeof(T) == typeof(WindowInfo))
							{
								rValue = (T)Convert.ChangeType(WindowInfo.Parse(sValue), typeof(T));
								ret = true;
								break;
							}
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

			#region Operators
			public static explicit operator T(SettingValue<T> v) => v.Value;

			public static bool operator true(SettingValue<T> v) => (bool)v;
			public static bool operator false(SettingValue<T> v) => !(bool)v;
			public static bool operator !(SettingValue<T> v) => !(bool)v;

			public static implicit operator string(SettingValue<T> v) => v.Value.ToString();

			public static implicit operator char(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (char)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (char)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (char)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (char)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (char)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (char)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (char)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (char)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (char)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (char)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (char)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (char)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (char)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					char n;
					if (char.TryParse(s, out n)) return n;
					return (char)0;
				}
				return (char)0; // unknown type
			}
			public static implicit operator sbyte(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (sbyte)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (sbyte)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (sbyte)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (sbyte)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (sbyte)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (sbyte)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (sbyte)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (sbyte)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (sbyte)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (sbyte)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (sbyte)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (sbyte)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (sbyte)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					sbyte n;
					if (sbyte.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator byte(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (byte)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (byte)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (byte)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (byte)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (byte)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (byte)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (byte)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (byte)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (byte)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (byte)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (byte)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (byte)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (byte)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					byte n;
					if (byte.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator short(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (short)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (short)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (short)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (short)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (short)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (short)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (short)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (short)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (short)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (short)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (short)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (short)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (short)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					short n;
					if (short.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator ushort(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (ushort)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (ushort)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (ushort)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (ushort)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (ushort)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (ushort)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (ushort)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (ushort)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (ushort)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (ushort)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (ushort)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (ushort)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (ushort)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					ushort n;
					if (ushort.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator int(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (int)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (int)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (int)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (int)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (int)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (int)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (int)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (int)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (int)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (int)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (int)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (int)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (int)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					int n;
					if (int.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator uint(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (uint)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (uint)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (uint)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (uint)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (uint)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (uint)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (uint)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (uint)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (uint)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (uint)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (uint)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (uint)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (uint)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					uint n;
					if (uint.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator long(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (long)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (long)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (long)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (long)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (long)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (long)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (long)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (long)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (long)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (long)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (long)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (long)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (long)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					long n;
					if (long.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator ulong(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (ulong)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (ulong)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (ulong)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (ulong)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (ulong)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (ulong)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (ulong)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (ulong)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (ulong)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (ulong)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (ulong)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (ulong)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (ulong)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					ulong n;
					if (ulong.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator float(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (float)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (float)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (float)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (float)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (float)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (float)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (float)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (float)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (float)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (float)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (float)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (float)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (float)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					float n;
					if (float.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator double(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (double)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (double)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (double)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (double)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (double)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (double)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (double)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (double)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (double)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (double)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (double)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (double)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (double)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					double n;
					if (double.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}
			public static implicit operator decimal(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (decimal)((v as SettingValue<bool>).Value ? 1 : 0);

				if (t == typeof(char)) return (decimal)(v as SettingValue<char>).Value;
				if (t == typeof(sbyte)) return (decimal)(v as SettingValue<sbyte>).Value;
				if (t == typeof(byte)) return (decimal)(v as SettingValue<byte>).Value;
				if (t == typeof(short)) return (decimal)(v as SettingValue<short>).Value;
				if (t == typeof(ushort)) return (decimal)(v as SettingValue<ushort>).Value;
				if (t == typeof(int)) return (decimal)(v as SettingValue<int>).Value;
				if (t == typeof(uint)) return (decimal)(v as SettingValue<uint>).Value;
				if (t == typeof(long)) return (decimal)(v as SettingValue<long>).Value;
				if (t == typeof(ulong)) return (decimal)(v as SettingValue<ulong>).Value;
				if (t == typeof(float)) return (decimal)(v as SettingValue<float>).Value;
				if (t == typeof(double)) return (decimal)(v as SettingValue<double>).Value;
				if (t == typeof(decimal)) return (decimal)(v as SettingValue<decimal>).Value;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					decimal n;
					if (decimal.TryParse(s, out n)) return n;
					return 0;
				}
				return 0; // unknown type
			}

			public static implicit operator bool(SettingValue<T> v)
			{
				var t = typeof(T);
				if (t == typeof(bool)) return (v as SettingValue<bool>).Value;

				if (t == typeof(char)) return (v as SettingValue<char>).Value != 0;
				if (t == typeof(sbyte)) return (v as SettingValue<sbyte>).Value != 0;
				if (t == typeof(byte)) return (v as SettingValue<byte>).Value != 0;
				if (t == typeof(short)) return (v as SettingValue<short>).Value != 0;
				if (t == typeof(ushort)) return (v as SettingValue<ushort>).Value != 0;
				if (t == typeof(int)) return (v as SettingValue<int>).Value != 0;
				if (t == typeof(uint)) return (v as SettingValue<uint>).Value != 0;
				if (t == typeof(long)) return (v as SettingValue<long>).Value != 0;
				if (t == typeof(ulong)) return (v as SettingValue<ulong>).Value != 0;
				if (t == typeof(float)) return (v as SettingValue<float>).Value != 0;
				if (t == typeof(double)) return (v as SettingValue<double>).Value != 0;
				if (t == typeof(decimal)) return (v as SettingValue<decimal>).Value != 0;

				if (t == typeof(string))
				{
					var s = (v as SettingValue<string>).Value;
					return !(s == null || s.Length == 0 || string.Compare(s, "false", true) == 0);
				}

				if (t.IsAssignableFrom(typeof(Nullable)))
					return v.Value != null;

				return false; // unknown type
			}
		}

		public sealed class SettableSettingValue<T> : SettingValue<T>
		{
			public string DisplayName { get; }
			public string Description { get; }
			public string Caution { get; }
			public object Enums { get; }

			public SettableSettingValue(string Name, string Provider, T DefaultValue, string DisplayName, string Description = "", string Caution = "", object Enums = null)
				: base(Name, Provider, DefaultValue) { }
		}
		#endregion
	}
}
