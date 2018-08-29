using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules.Communication
{
	public struct ObjectPathLevel
	{
		/// <summary>
		/// Property Name
		/// </summary>
		public string Name;

		/// <summary>
		/// Is this property array?
		/// </summary>
		public bool IsArray;

		/// <summary>
		/// Index of this property, when <see cref="IsArray"/> is true
		/// </summary>
		public int Index;

		public override string ToString()
			=> this.IsArray ? $"{Name}[{Index}]" : Name;
		// => $"{{Name: {Name}, IsArray: {IsArray}, Index: {Index}}}";

		public static ObjectPathLevel[] ParsePathLevels(string path)
		{
			var list = new List<ObjectPathLevel>();
			var nameBuffer = new StringBuilder();
			var indexBuffer = new StringBuilder();

			var arraying = false;
			var current = new ObjectPathLevel();
			for (var i = 0; i < path.Length; i++)
			{
				var c = path[i];

				if (arraying)
				{
					if (c == ']')
					{
						if (!int.TryParse(indexBuffer.ToString(), out current.Index))
							throw new Exception("Path parse error: Cannot parse index");

						indexBuffer.Clear();
						arraying = false;
						continue;
					}
					else if (c < '0' || c > '9')
						throw new ArgumentException("Path parse error: Invalid index value");
					indexBuffer.Append(c);
				}
				else if (c == '.')
				{
					current.Name = nameBuffer.ToString();
					nameBuffer.Clear();

					list.Add(current);
					current = new ObjectPathLevel();
				}
				else if (c == '[')
				{
					current.IsArray = true;
					arraying = true;
				}
				else
					nameBuffer.Append(c);
			}

			if (nameBuffer.Length > 0)
			{
				current.Name = nameBuffer.ToString();
				list.Add(current);
			}

			return list.ToArray();
		}
	}
}
