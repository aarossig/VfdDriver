using System;
using System.IO.Ports;

namespace VfdDriver
{
	public class VdfDisplay : IDisposable
	{
		private SerialPort serialPort;

		public VdfDisplay (string serialPort, int baudRate = 38400)
		{
			this.serialPort = new SerialPort ();
			this.serialPort.PortName = serialPort;
			this.serialPort.BaudRate = baudRate;
			this.serialPort.StopBits = StopBits.One;
			this.serialPort.Parity = Parity.None;
			this.serialPort.Handshake = Handshake.None;
			this.serialPort.Open ();
		}

		public void Initialize()
		{
			var cmd = new byte[] { 0x1B, 0x40 };
			serialPort.Write (cmd, 0, cmd.Length);
		}

		public void Reset()
		{
			var cmd = new Byte[] { 0x1B, 0x58, 0xFF };
			serialPort.Write (cmd, 0, cmd.Length);
		}

		public void Dispose()
		{
			serialPort.Dispose ();
		}
	}
}

