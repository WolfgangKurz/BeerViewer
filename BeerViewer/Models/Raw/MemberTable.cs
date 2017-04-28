using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace BeerViewer.Models.Raw
{
	public class MemberTable<T> : MasterTable<T> where T : class, IIdentifiable
	{
		public MemberTable() : this(new List<T>()) { }
		public MemberTable(IEnumerable<T> source) : base(source) { }

		internal void Add(T value)
		{
			this.dictionary.Add(value.Id, value);
		}
		internal void Remove(T value)
		{
			this.dictionary.Remove(value.Id);
		}
		internal void Remove(int id)
		{
			this.dictionary.Remove(id);
		}
	}
}
