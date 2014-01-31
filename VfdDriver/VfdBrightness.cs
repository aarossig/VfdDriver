using System;

namespace TheResistorNetwork.Drivers.VfdDriver
{
	public enum VfdBrightness : byte
	{
		Percent25 = 0x01,
		Percent50,
		Percent75,
		Percent100,
		Percent125,
		Percent150,
		Percent175,
		Percent200
	}
}

