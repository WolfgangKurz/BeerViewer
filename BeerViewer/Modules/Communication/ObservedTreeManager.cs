using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace BeerViewer.Modules.Communication
{
	internal class ObservedTreeManager
	{
		private Dictionary<string, object> NamespaceTable { get; }
		private List<KeyValuePair<string, ObservedTreeData>> NamespaceTree { get; }

		private object lockObject = new object();

		public ObservedTreeManager()
		{
			this.NamespaceTable = new Dictionary<string, object>();
			this.NamespaceTree = new List<KeyValuePair<string, ObservedTreeData>>();
		}

		/// <summary>
		/// Register object to namespace table
		/// </summary>
		/// <param name="Namespace"></param>
		/// <param name="Object"></param>
		public void AddNamespaceObject(string Namespace, object Object)
		{
			if (string.IsNullOrWhiteSpace(Namespace)) throw new ArgumentException(nameof(Namespace));
			if (Object == null) throw new ArgumentNullException(nameof(Object));

			this.NamespaceTable.Add(Namespace, Object);
		}
		public void RemoveNamespace(string Namespace)
		{
			// Drop all from tree

			this.NamespaceTable.Remove(Namespace);
		}

		protected ObservedTreeData RegisterPartialTree(ObservedTreeData root, string Path, int Index = 0, Action<object> ResultCallback = null)
		{
			var levels = ObjectPathLevel.ParsePathLevels(Path ?? ""); // A.B[2].C -> [A:literal, B:array[2], C:literal]

			var cursor = root;
			var result = cursor.Object;
			ObservedTreeData output = null;

			for (var i = 0; i < levels.Length; i++)
			{
				if (i < Index) continue;

				var level = levels[i];

				if (result != null)
				{
					var type = result.GetType();
					var member = type.GetMember(level.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty)
						.FirstOrDefault();
					if (member == null)
						throw new Exception($"Path not exists");

					if (member.MemberType == MemberTypes.Field)
						result = ((FieldInfo)member).GetValue(result);
					else if (member.MemberType == MemberTypes.Property)
						result = ((PropertyInfo)member).GetValue(result);
					else
						throw new NotImplementedException("Only Field or Property can be measured");

					if (level.IsArray)
					{
						type = result.GetType();
						var indexer = type.GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance);
						if (indexer == null)
							throw new Exception("Tried to access as Array to not Array object");

						result = indexer.Invoke(result, new object[] { level.Index });
					}
				}

				var _new = new Func<ObservedTreeData, int, ObservedTreeData>((_cursor, _Index) =>
				{
					ObservedTreeData item = null;

					item = new ObservedTreeData(
						level,
						result,
						new PropertyChangedEventHandler((s, e) =>
						{
							if (e.PropertyName == item.Child?.Name)
							{
								item.Child.Dispose();
								RegisterPartialTree(item, Path, _Index + 1, ResultCallback);
							}
						})
					);
					return item;
				})(cursor, i);

				if (cursor.IsRoot)
				{
					output = _new;
					cursor = _new;
				}
				else
				{
					cursor.Child = _new;
					cursor = _new;
				}
			}

			ResultCallback?.Invoke(result);
			return output;
		}
		public void RegisterTree(string Namespace, string Path, Action<object> ResultCallback)
		{
			if (!this.NamespaceTable.ContainsKey(Namespace))
				throw new Exception("Namespace \"" + Namespace + "\" not registered");

			ObservedTreeData root = null;
			root = new ObservedTreeData(
					new ObjectPathLevel
					{
						Name = Namespace,
						IsArray = false,
						Index = 0
					},
					this.NamespaceTable[Namespace],
					(s, e) =>
					{
						root.Child.Dispose();
						RegisterPartialTree(root, Path, 0, ResultCallback); // Index is 0, root is namespace
					},
					true
				);

			var item = RegisterPartialTree(root, Path, 0, ResultCallback);
			this.NamespaceTree.Add(
				new KeyValuePair<string, ObservedTreeData>(Path, item)
			);
		}
		public object GetData(string Namespace, string Path)
		{
			if (!this.NamespaceTable.ContainsKey(Namespace))
				throw new Exception("Namespace \"" + Namespace + "\" not registered");

			var levels = ObjectPathLevel.ParsePathLevels(Path ?? ""); // A.B[2].C -> [A:literal, B:array[2], C:literal]
			object result = this.NamespaceTable[Namespace];

			for (var levelCursor = 0; levelCursor < levels.Length; levelCursor++)
			{
				if (result == null) break;

				// Current object information
				var pathCursor = levels[levelCursor];

				var type = result.GetType();
				var member = type.GetMember(pathCursor.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty)
					.FirstOrDefault();
				if (member == null)
					throw new Exception($"Path \"{Namespace}.{Path}\" not exists");

				if (member.MemberType == MemberTypes.Field)
					result = ((FieldInfo)member).GetValue(result);
				else if (member.MemberType == MemberTypes.Property)
					result = ((PropertyInfo)member).GetValue(result);
				else
					throw new NotImplementedException("Only Field or Property can be measured");

				if (pathCursor.IsArray)
				{
					type = result.GetType();
					var indexer = type.GetMethod("get_Item", BindingFlags.Public | BindingFlags.Instance);
					if (indexer == null)
						throw new Exception("Tried to access as Array to not Array object");

					result = indexer.Invoke(result, new object[] { pathCursor.Index });
				}
			}
			return result;
		}

		public void Test(ObservedTreeData root = null, int level = 0)
		{
			if (root == null)
			{
				if (level > 0) return;

				System.Diagnostics.Debug.WriteLine("Homeport");
				foreach (var item in this.NamespaceTree)
					Test(item.Value, 1);
				return;
			}

			System.Diagnostics.Debug.WriteLine(
				"{0}{1}{2}",
				new string(' ', level * 2),
				level > 0 ? "- " : "",
				root.Path
			);
			Test(root.Child, level + 1);
		}
	}
}
