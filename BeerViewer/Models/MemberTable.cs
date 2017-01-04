using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Models
{
	/// <summary>
	/// 정수 키를 Id로 가지는 칸코레 유저 데이터 테이블을 정의
	/// </summary>
	public class MemberTable<TValue> : IReadOnlyDictionary<int, TValue> where TValue : class, IIdentifiable
	{
		private readonly IDictionary<int, TValue> dictionary;

		/// <summary>
		/// Id를 통한 요소 접근, 없으면 null
		/// </summary>
		public TValue this[int key] => this.dictionary.ContainsKey(key) ? this.dictionary[key] : null;

		public MemberTable() : this(new List<TValue>()) { }
		public MemberTable(IEnumerable<TValue> source)
		{
			this.dictionary = source.ToDictionary(x => x.Id);
		}

		internal void Add(TValue value)
		{
			this.dictionary.Add(value.Id, value);
		}
		internal void Remove(TValue value)
		{
			this.dictionary.Remove(value.Id);
		}
		internal void Remove(int id)
		{
			this.dictionary.Remove(id);
		}

		#region IReadOnlyDictionary<TK, TV> members
		public IEnumerable<int> Keys => this.dictionary.Keys;
		public IEnumerable<TValue> Values => this.dictionary.Values;
		public int Count => this.dictionary.Count;

		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public bool ContainsKey(int key)
		{
			return this.dictionary.ContainsKey(key);
		}
		public bool TryGetValue(int key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}
		#endregion
	}
}
