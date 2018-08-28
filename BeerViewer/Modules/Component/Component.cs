using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules
{
	public class Component : IEnumerable
	{
		private readonly List<object> functions = new List<object>();

		public Guid Id { get; }
		public ComponentMetadata Metadata { get; }

		internal Component(IBeerComponentMetadata metadata)
		{
			this.Id = new Guid(metadata.Guid);
			this.Metadata = new ComponentMetadata
			{
				Name = metadata.Name,
				Description = metadata.Description,
				Version = metadata.Version,
				Author = metadata.Author,
			};
		}

		internal void Add<TContract>(TContract function) where TContract : class
		{
			this.functions.Add(function);
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> this.functions.GetEnumerator();
	}

	[Serializable]
	public class ComponentMetadata
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Version { get; set; }
		public string Author { get; set; }
	}
}
