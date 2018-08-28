using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeerViewer.Modules
{
	/// <summary>
	/// BeerViewer's component base interface
	/// </summary>
	public interface IBeerComponent
	{
		/// <summary>
		/// Component name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Initialize after component ready to use
		/// </summary>
		void Initialize();

		/// <summary>
		/// Failed to ready (Denied by user?)
		/// </summary>
		void ReadyFailed();
	}

	public interface IBeerComponentGuid
	{
		/// <summary>
		/// Component identifier
		/// </summary>
		string Guid { get; }
	}

	public interface IBeerComponentMetadata : IBeerComponentGuid
	{
		/// <summary>
		/// Component name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Component description
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Component version
		/// </summary>
		string Version { get; }

		/// <summary>
		/// Component author
		/// </summary>
		string Author { get; }
	}

	[Flags]
	public enum BeerComponentPermission : int
	{
		/// <summary>
		/// No permissions
		/// </summary>
		CP_NONE = 0x0000,

		/// <summary>
		/// Permission to use network handler
		/// </summary>
		CP_NETWORK = 0x0001,

		/// <summary>
		/// Permission to use modifiable network handler
		/// </summary>
		CP_NETWORK_MODIFIABLE = 0x0002,

		/// <summary>
		/// Permission to notify to user
		/// </summary>
		CP_NOTIFY = 0x0004,

		/// <summary>
		/// Permission to use main tab display area (right)
		/// </summary>
		CP_MAINTAB = 0x0008,

		/// <summary>
		/// Permission to use sub tab display area (bottom)
		/// </summary>
		CP_SUBTAB = 0x0010,
	}
}
