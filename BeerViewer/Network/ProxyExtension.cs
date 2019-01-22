using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BeerViewer.Network
{
	public static class ProxyExtension
	{
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
