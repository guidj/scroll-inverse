using System;
using Gtk;
using ScrollInvert;

public partial class MainWindow: Gtk.Window
{
	const String aboutMessage = "\nThis tool allows you invert the scroll direction " +
								"\nof your mouse in Windows 8 PCs." +
								"\n" +
	                            "\nYou must run it with Admin privileges, and unplug your mouse for 5 seconds after" +
	                            "\neach change.\n" +
	                            "\n" +
	                            "\nIt's based on a guide by Kevin Becker.";

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{

		Build ();

		try{

			SetStateLabel ();

		}catch (DeviceNotFoundException){
			MessageDialog msg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "No HID mouse device was found.");
			if ((ResponseType) msg.Run() == ResponseType.Close) {
				msg.Destroy();
			}
		}catch (UnauthorizedAccessException){
			MessageDialog msg = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, "You need Administrative rights to run this tool.");
			if ((ResponseType) msg.Run() == ResponseType.Close) {
				msg.Destroy();
			}
		}

	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	private void SetStateLabel()
	{
		bool inverted = Invert.IsInverted ();

		if (inverted)
		{
			this.inversionStatusLabel.Markup = "<b>ON</b>";

			Gdk.Color col = new Gdk.Color();
			Gdk.Color.Parse("green", ref col);

			this.inversionStatusLabel.ModifyFg (StateType.Normal, col);
		}
		else 
		{

			this.inversionStatusLabel.LabelProp = "<b>OFF</b>";

			Gdk.Color col = new Gdk.Color();
			Gdk.Color.Parse("red", ref col);

			this.inversionStatusLabel.ModifyFg (StateType.Normal, col);
		}
	}


	protected void OnInvertButtonClicked (object sender, EventArgs e)
	{
		try{
			Invert.InvertMouseScrolling ();
			this.SetStateLabel ();

			MessageDialog msg = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Unplug your mouse for 5 seconds.");
			if ((ResponseType) msg.Run() == ResponseType.Ok) {
				msg.Destroy();
			}

		}catch (DeviceNotFoundException){
			MessageDialog msg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, "No HID mouse device was found.");

			if ((ResponseType) msg.Run() == ResponseType.Close) {
				msg.Destroy();
			}
		}catch (UnauthorizedAccessException){
			MessageDialog msg = new MessageDialog(this, DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, "You need Administrative rights to run this tool.");

			if ((ResponseType) msg.Run() == ResponseType.Close) {
				msg.Destroy();
			}
		}
	}

	protected void OnAboutButtonClicked (object sender, EventArgs e)
	{
		/*var aboutDialog = new AppAbout();
		aboutDialog.Show ();

		if ((ResponseType) aboutDialog.Run() == ResponseType.Close) {
			aboutDialog.Destroy();
		}*/
		this.ShowMessage (this, "About", aboutMessage);
	}

	protected void ShowMessage (Window parent, string title, string message)
	{
		Dialog dialog = null;
		try {
			dialog = new Dialog (title, parent,
				DialogFlags.DestroyWithParent | DialogFlags.Modal,
				ResponseType.Close);
			dialog.VBox.Add (new Label (message));
			dialog.ShowAll ();

			dialog.Run ();
		} finally {
			if (dialog != null)
				dialog.Destroy ();
		}
	}
}
