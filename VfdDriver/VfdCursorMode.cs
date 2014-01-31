using System;

namespace TheResistorNetwork.Drivers.VfdDriver
{
	public enum VfdCursorMode : byte
	{
		Underline = 0x13,
		Off = 0x14,
		Block = 0x15,
		UnderlineBlink = 0x16
	}
}

