using System;

namespace scrollinverse
{
	public partial class Main : Gtk.Window
	{
		public Main () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

