using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace BeerViewer.Models.Raw
{
	public class MasterTable<T> : IReadOnlyDictionary<int, T> where T : class, IIdentifiable
	{
		protected readonly IDictionary<int, T> dictionary;
		public T this[int key] => this.dictionary.ContainsKey(key) ? this.dictionary[key] : null;

		public MasterTable() : this(new List<T>()) { }
		public MasterTable(IEnumerable<T> source)
		{
			this.dictionary = source.ToDictionary(x => x.Id);
		}

		public IEnumerable<int> Keys => this.dictionary.Keys;
		public IEnumerable<T> Values => this.dictionary.Values;
		public int Count => this.dictionary.Count;

		public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
			=> this.dictionary.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> this.GetEnumerator();

		public bool ContainsKey(int key)
			=> this.dictionary.ContainsKey(key);

		public bool TryGetValue(int key, out T value)
			=> this.dictionary.TryGetValue(key, out value);
	}
}
