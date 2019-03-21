using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BeerViewer.Modules
{
	internal class ComponentService : IDisposable
	{
		public static ComponentService Instance { get; } = new ComponentService();

		private CompositionContainer container;

		private Dictionary<Guid, Component> loadedComponents;

#pragma warning disable 649
		[ImportMany(RequiredCreationPolicy = CreationPolicy.Shared)]
		private IEnumerable<Lazy<IBeerComponent, IBeerComponentMetadata>> importedAll;
#pragma warning restore 649

		private static class Cache<TContract>
		{
			private static List<TContract> components;
			public static List<TContract> Components => components ?? (components = new List<TContract>());
		}

		public Component[] Components => this.loadedComponents.Values.ToArray();

		private ComponentService() { }

		public void Initialize()
		{
			var currentDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
			if (currentDir == null)
			{
				this.loadedComponents = new Dictionary<Guid, Component>();
				return;
			}

			var componentsDir = Path.Combine(currentDir, "Components");
			if (!Directory.Exists(componentsDir))
			{
				this.loadedComponents = new Dictionary<Guid, Component>();
				return;
			}

			var catalog = new AggregateCatalog(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			var components = Directory.EnumerateFiles(componentsDir, "*.dll", SearchOption.AllDirectories);

			foreach (var component in components)
			{
				var filepath = component;
				var filename = Path.GetFileName(filepath);

				try
				{
					var asmCatalog = new AssemblyCatalog(filepath);

					if (asmCatalog.Parts.ToList().Count > 0)
						catalog.Catalogs.Add(asmCatalog);
				}
				catch (ReflectionTypeLoadException ex)
				{
					Logger.Log(
						"Failed to load component '{1}' with ReflectionTypeLoadException.{0}{2}",
						Environment.NewLine,
						filename,
						string.Join(Environment.NewLine, ex.LoaderExceptions.Select(x => x.Message))
					);
				}
				catch (BadImageFormatException ex)
				{
					Logger.Log(
						"Failed to load component '{1}' with BadImageFormatException.{0}{2}",
						Environment.NewLine,
						filename,
						ex.Message
					);
				}
				catch (FileLoadException ex)
				{
					Logger.Log(
						"Failed to load component '{1}' with FileLoadException.{0}{2}",
						Environment.NewLine,
						filename,
						ex.Message
					);
				}
			}

			this.container = new CompositionContainer(catalog);
			this.container.ComposeParts(this);

			this.loadedComponents = this.Load(this.importedAll)
				.ToDictionary(x => x.Id);
		}

		/*
		public IComponentNotifier GetNotifier()
			=> new AggregateNotifier(Cache<IComponentNotifier>.Components);
		*/

		public TContract[] Get<TContract>()
			=> Cache<TContract>.Components.ToArray();


		private IEnumerable<Component> Load(IEnumerable<Lazy<IBeerComponent, IBeerComponentMetadata>> imported)
		{
			var ids = new HashSet<Guid>();

			foreach (var lazy in imported)
			{
				Guid guid;
				if (!Guid.TryParse(lazy.Metadata.Guid, out guid)) continue;

				var component = new Component(lazy.Metadata);
				var success = false;

				try
				{
					lazy.Value.Initialize();
					success = true;
				}
				catch (CompositionException ex)
				{
					var infos = ex.RootCauses
						.Select(x => x as ComposablePartException)
						.Select(x => x?.Element.Origin as AssemblyCatalog);

					foreach (var info in infos)
					{
						var filepath = info?.Assembly?.Location;
						var filename = Path.GetFileName(filepath);

						Logger.Log(
							$"Failed to load component '{1}' with CompositionException.{0}{2}",
							Environment.NewLine,
							filename,
							component.Metadata.ToString(),
							ex.Message
						);
					}
				}
				catch (Exception ex)
				{
					Logger.Log(
						$"Failed to load component '{1}' with {2}.{0}{3}{0}{4}",
						Environment.NewLine,
						component.Metadata.Name,
						ex.GetType().Name,
						component.Metadata.ToString(),
						ex.Message
					);
				}

				if (!ids.Add(component.Id))
				{
					Logger.Log("Failed to load component '{0}'. Guid has duplicated.", component.Metadata.Name);
					success = false;
				}

				if (success)
					yield return component;
			}
		}

		private void Load<TContract>(IEnumerable<Lazy<TContract, IBeerComponentGuid>> imported) where TContract : class
		{
			foreach (var lazy in imported)
			{
				Guid guid;
				if (!Guid.TryParse(lazy.Metadata.Guid, out guid)) continue;

				Component component;
				if (!this.loadedComponents.TryGetValue(guid, out component)) continue;

				try
				{
					var function = lazy.Value;

					component.Add(function);
					Cache<TContract>.Components.Add(function);
				}
				catch (Exception ex)
				{
					Logger.Log(
						"Failed to load component '{1}' with FileLoadException.{0}{2}",
						Environment.NewLine,
						component?.Metadata?.Name ?? "*Unknown",
						ex.Message
					);
				}
			}
		}

		public void Dispose()
		{
			this.container?.Dispose();
		}
	}
}
