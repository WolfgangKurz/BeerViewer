using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace BeerViewer.Win32
{
	[StructLayout(LayoutKind.Sequential)]
	internal class DVTARGETDEVICE
	{
		public ushort tdSize;
		public uint tdDeviceNameOffset;
		public ushort tdDriverNameOffset;
		public ushort tdExtDevmodeOffset;
		public ushort tdPortNameOffset;
		public byte tdData;
	}
}
