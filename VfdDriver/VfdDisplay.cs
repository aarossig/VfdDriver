using System;
using System.IO.Ports;
using System.Threading;

namespace TheResistorNetwork.Drivers.VfdDriver
{
	public class VdfDisplay : IDisposable
	{
		private SerialPort serialPort;

		private VfdDisplayMode displayMode;
		private VfdBrightness brightness;
		private VfdBlinkMode blinkMode;
		private VfdCursorMode cursorMode;
		private VfdCustomCharacterMode customCharacterMode;

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="TheResistorNetwork.VfdDriver.VdfDisplay"/> class.
		/// </summary>
		/// <param name="serialPort">Serial port.</param>
		/// <param name="baudRate">Baud rate.</param>
		public VdfDisplay (string serialPort, int baudRate = 38400)
		{
			this.serialPort = new SerialPort ();
			this.serialPort.PortName = serialPort;
			this.serialPort.BaudRate = baudRate;
			this.serialPort.StopBits = StopBits.One;
			this.serialPort.Parity = Parity.None;
			this.serialPort.Handshake = Handshake.None;
			this.serialPort.Open ();

			displayMode = VfdDisplayMode.Flickerless;
			brightness = VfdBrightness.Percent200;
			blinkMode = VfdBlinkMode.Off;
			cursorMode = VfdCursorMode.Underline;
			customCharacterMode = VfdCustomCharacterMode.Disabled;
		}

		/// <summary>
		/// Gets or sets the mode of the display.
		/// </summary>
		/// <value>The mode.</value>
		public VfdDisplayMode Mode
		{
			get {
				return displayMode;
			}

			set {
				switch (displayMode) {
					case VfdDisplayMode.Flickerless:
						{
							var cmd = new byte[] { 0x1B, 0x45 };
							serialPort.Write (cmd, 0, cmd.Length);
						}
						break;
					case VfdDisplayMode.Quick:
						{
							var cmd = new byte[] { 0x1B, 0x53 };
							serialPort.Write (cmd, 0, cmd.Length);
						}
						break;
					default:
						break;
				}

				displayMode = value;

				Thread.Sleep (1);
			}
		}

		/// <summary>
		/// Gets or sets the custom character mode.
		/// </summary>
		/// <value>The custom character mode.</value>
		public VfdCustomCharacterMode CustomCharacterMode
		{
			get {
				return customCharacterMode;
			}

			set {
				customCharacterMode = value;

				var cmd = new byte[] { 0x1B, 0x25, (byte)customCharacterMode };
				serialPort.Write (cmd, 0, cmd.Length);

				Thread.Sleep (1);
			}
		}

		/// <summary>
		/// Gets or sets the cursor mode.
		/// </summary>
		/// <value>The cursor mode.</value>
		public VfdCursorMode CursorMode
		{
			get {
				return cursorMode;
			}

			set {
				cursorMode = value;

				var cmd = new byte[] { (byte)cursorMode };
				serialPort.Write(cmd, 0, cmd.Length);

				Thread.Sleep (1);
			}
		}

		/// <summary>
		/// Gets or sets the display brightness.
		/// </summary>
		/// <value>The brightness.</value>
		public VfdBrightness Brightness
		{
			get {
				return brightness;
			}

			set {
				brightness = value;

				var cmd = new byte[] { 0x1F, 0x58, (byte)brightness };
				serialPort.Write (cmd, 0, cmd.Length);

				Thread.Sleep (1);
			}
		}

		/// <summary>
		/// Gets or sets the blink mode.
		/// </summary>
		/// <value>The blink mode.</value>
		public VfdBlinkMode BlinkMode 
		{
			get {
				return blinkMode;
			}

			set {
				blinkMode = value;

				var cmd = new byte [] { 0x1B, (byte)blinkMode };
				serialPort.Write (cmd, 0, cmd.Length);

				Thread.Sleep (1);
			}
		}

		/// <summary>
		/// Clear display and return settings to initial state. Software
		/// settings return to power-on state. Jumper settings are not
		/// re-loaded.
		/// </summary>
		public void Initialize()
		{
			var cmd = new byte[] { 0x1B, 0x40 };
			serialPort.Write (cmd, 0, cmd.Length);

			Thread.Sleep (1);
		}

		/// <summary>
		/// Transition to state immediately after power-on. Jumper settings are
		/// re-loaded – baud rate (for serial interface), and test mode
		/// settings. Receive buffer is also cleared.
		/// </summary>
		public void Reset()
		{
			var cmd = new byte[] { 0x1B, 0x58, 0xFF };
			serialPort.Write (cmd, 0, cmd.Length);

			Thread.Sleep (1);
		}

		/// <summary>
		/// Clear the screen.
		/// </summary>
		public void Clear()
		{
			var cmd = new byte[] { 0x0C };
			serialPort.Write (cmd, 0, cmd.Length);

			Thread.Sleep (1);
		}

		/// <summary>
		/// Write the specified data to the display.
		/// </summary>
		/// <param name="data">Data.</param>
		public void Write(byte[] data)
		{
			for (int i = 0; i < data.Length; i++) {
				serialPort.Write (data, i, 1);

				if (i % 24 == 0) {
					Thread.Sleep (1);
				}
			}
		}

		/// <summary>
		/// Sets the position of the cursor.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void SetCursorPosition(byte x, byte y)
		{
			if (x < 0 || x > 23) {
				throw new ArgumentException (
					"X must be greater than 0 and less than 24.");
			}

			if(y < 0 || y > 5)
			{
				throw new ArgumentException (
					"Y must be greater than 0 and less than 6.");
			}

			var cmd = new byte[] { 0x1F, 0x24, x, 0x00, y, 0x00 };
			serialPort.Write (cmd, 0, cmd.Length);

			Thread.Sleep (1);
		}

		/// <summary>
		/// Defines the custom characters.
		/// </summary>
		/// <param name="startCode">Start code.</param>
		/// <param name="endCode">End code.</param>
		/// <param name="data">Data.</param>
		public void DefineCustomCharacters(byte startCode, byte endCode,
			byte[] data)
		{
			if (data.Length != 5) {
				throw new ArgumentException (
					"Defined data must be 5 bytes.");
			}

			var cmd = new byte[] { 0x1B, 0x26, 0x01, startCode, endCode, 0x05 };
			serialPort.Write (cmd, 0, cmd.Length);
			serialPort.Write (data, 0, data.Length);
		}

		/// <summary>
		/// Releases all resource used by the
		/// <see cref="TheResistorNetwork.VfdDriver.VdfDisplay"/> object.
		/// </summary>
		public void Dispose()
		{
			serialPort.Dispose ();
		}
	}
}

