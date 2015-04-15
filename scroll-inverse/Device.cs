using System;

namespace ScrollInvert
{
	public class Device
	{
		public Device() { }

		public Device(string ID, string Name)
		{
			this.ID = ID;
			this.Name = Name;
		}

		public string ID { get; set; }

		public string Name { get; set; }
	}
}

