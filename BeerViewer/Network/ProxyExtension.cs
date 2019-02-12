using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BeerViewer.Network
{
	internal static class ProxyExtension
	{
		private static Random rd = new Random();

		public static string DPIHeader(this string HeadersInOrder)
		{
			var lines = HeadersInOrder.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var list = new List<string>();
			foreach(var x in lines)
			{
				var offset = x.IndexOf(":");
				if (offset>=0)
				{
					var header = x.Substring(0, offset).Trim();
					var body = x.Substring(offset + 1).Trim();
					if (header.ToLower() == "host")
					{
						header = header.ShuffleCase();
						body = body.ShuffleCase();
					}
					list.Add(header + ": " + body);
				}
			}
			list.Add(""); // Empty line
			return string.Join("\r\n", list.ToArray());
		}

		public static string DPIRequestLine(this string RequestLine)
		{
			var offset = RequestLine.IndexOf(" ");
			if (offset < 0) return RequestLine;

			return RequestLine.Substring(0, offset) + new string(' ', rd.Next(1, 4)) + RequestLine.Substring(offset);
		}

		private static string ShuffleCase(this string Text)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < Text.Length; i++)
			{
				var c = Text[i];
				if (c >= 'a' && c <= 'z' && rd.Next() % 2 == 0)
					c = (char)(c - 97 + 65);
				else if (c >= 'A' && c <= 'Z' && rd.Next() % 2 == 0)
					c = (char)(c + 97 - 65);
				sb.Append(c);
			}
			return sb.ToString();
		}
	}

	internal class ProxyHandlerContainer<T> : IEnumerable<KeyValuePair<int, T>>, IEnumerable, IReadOnlyDictionary<int, T>
	{
		private const int INVALID_KEY = -1;

		private int keyCounter { get; set; }
		private ConcurrentDictionary<int, T> dict { get; }

		public IEnumerable<int> Keys => this.dict.Keys;
		public IEnumerable<T> Values => this.dict.Values;

		public int Count => this.dict.Count;
		public bool IsReadOnly => false;

		public T this[int key]
		{
			get => this.dict[key];
		}

		public ProxyHandlerContainer() : base()
		{
			this.keyCounter = 0;
			this.dict = new ConcurrentDictionary<int, T>();
		}

		public bool TryGetValue(int key, out T value) => this.dict.TryGetValue(key, out value);
		public int TryAdd(T Handler)
		{
			var key = this.getAvailableKey();
			var ret = this.dict.TryAdd(key, Handler);
			if (ret) return key;
			return ProxyHandlerContainer<T>.INVALID_KEY;
		}
		public bool TryRemove(int Key, out T Value) => this.dict.TryRemove(Key, out Value);

		public bool ContainsKey(int key) => this.dict.ContainsKey(key);

		IEnumerator<KeyValuePair<int, T>> IEnumerable<KeyValuePair<int, T>>.GetEnumerator() => this.dict.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => this.dict.GetEnumerator();

		private int getAvailableKey()
		{
			while (this.ContainsKey(this.keyCounter))
			{
				if (this.keyCounter >= int.MaxValue)
					this.keyCounter = 0; // Repeat from 0, find removed key
				else
					this.keyCounter++;
			}
			return this.keyCounter;
		}
	}
}
