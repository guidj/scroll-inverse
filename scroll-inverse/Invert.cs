using System;
using System.Management;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace ScrollInvert
{
	public class Invert
	{
		private const String VENDOR_LABEL = "VID";
		private const Int32 INVERSION_OFF = 0;
		private const Int32 INVERSION_ON = 1;

		public Invert ()
		{
		}

		public static List<Device> LoadDevices()
		{
			SelectQuery Sq = new SelectQuery("Win32_PointingDevice");
			ManagementObjectSearcher objOSDetails = new ManagementObjectSearcher(Sq);
			ManagementObjectCollection osDetailsCollection = objOSDetails.Get();

			List<Device> devices = new List<Device>();

			foreach (ManagementObject mo in osDetailsCollection)
			{
				string deviceId = (string)mo["DeviceID"];

				string vendorId = null;

				foreach(string token in deviceId.Split('\\'))
				{
					if (token.Contains(VENDOR_LABEL))
					{
						vendorId = token;
						break;
					}
				}

				if (vendorId != null)
				{
					var name = (string)mo["Caption"];

					devices.Add(new Device(vendorId, name));
				}

			}

			return devices;
		}

		public static void InvertMouseScrolling()
		{
			List<Device> devices = LoadDevices();

			if (devices == null) {
				throw new DeviceNotFoundException ("Device not found");
			} else if (devices.Count == 0) {
				throw new DeviceNotFoundException ("Device not found");
			}

			var baseKey = Registry.LocalMachine;

			var hidKey = baseKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\HID");

			//StreamWriter writer = new StreamWriter("keys.txt");

			foreach (Device device in devices)
			{
				var deviceKeys = hidKey.OpenSubKey(device.ID);

				//writer.WriteLine("Vendor ID: " + device.ID);

				foreach (var subKey in deviceKeys.GetSubKeyNames())
				{
					var innerKey = deviceKeys.OpenSubKey(subKey);
					//writer.WriteLine(subKey);

					var deviceParamKey = innerKey.OpenSubKey("Device Parameters", true);

					if (deviceParamKey != null)
					{
						var value = (Int32)deviceParamKey.GetValue("FlipFlopWheel");

						if (value == INVERSION_OFF)
						{
							value = INVERSION_ON;
						}
						else
						{
							value = INVERSION_OFF;
						}

						try
						{
							deviceParamKey.SetValue("FlipFlopWheel", value);
						}
						catch (UnauthorizedAccessException exc)
						{
							throw exc;
						}

						//writer.WriteLine(value);
					}
				}
			}

			//writer.Close();
		}

		public static bool IsInverted()
		{
			List<Device> devices = LoadDevices();

			if (devices == null)
			{
				throw new DeviceNotFoundException("Failed to load mouse");
			}
			else if (devices.Count == 0)
			{
				//MessageBox.Show("Failed to load device mouse.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				throw new DeviceNotFoundException("Failed to load mouse");
			}

			var baseKey = Registry.LocalMachine;

			var hidKey = baseKey.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\HID");

			foreach (Device device in devices)
			{
				var deviceKeys = hidKey.OpenSubKey(device.ID);

				foreach (var subKey in deviceKeys.GetSubKeyNames())
				{
					var innerKey = deviceKeys.OpenSubKey(subKey);

					var deviceParamKey = innerKey.OpenSubKey("Device Parameters");

					if (deviceParamKey != null)
					{
						var value = (Int32)deviceParamKey.GetValue("FlipFlopWheel");

						if (value == INVERSION_OFF)
						{
							return false;
						}
						else
						{
							return true;
						}
					}
				}
			}

			return false;
		}

	}
}

