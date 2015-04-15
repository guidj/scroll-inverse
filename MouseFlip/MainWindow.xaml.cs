using ScrollInverse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace ScrollInverse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String VENDOR_LABEL = "VID";
        private static Int32 INVERSION_OFF = 0;
        private static Int32 INVERSION_ON = 1;

        private List<Device> loadDevices()
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

        private void InvertMouseScrolling()
        {
            List<Device> devices = loadDevices();

            if (devices == null)
            {
                throw new DeviceNotFoundException();
            }
            else if (devices.Count == 0)
            {
                throw new DeviceNotFoundException();
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

        private bool IsInverted()
        {
            List<Device> devices = loadDevices();

            if (devices == null)
            {
                throw new DeviceNotFoundException();
            }
            else if (devices.Count == 0)
            {
                throw new DeviceNotFoundException();
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

        private void setStateLabel()
        {
            bool inverted = this.IsInverted();

            if (inverted)
            {
                this.StateLabel.Content = "ON";
                this.StateLabel.Foreground = new SolidColorBrush(Colors.Green);
            }
            else 
            {
                this.StateLabel.Content = "OFF";
                this.StateLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        public void Initialize()
        {
            try
            {
                setStateLabel();
            }
            catch (DeviceNotFoundException)
            {
                MessageBox.Show("HID mouse device not found.", "No Device", MessageBoxButton.OK, MessageBoxImage.Error);
                //Application.Current.Shutdown();
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            Initialize();            
        }

        private void buttonToggleInversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InvertMouseScrolling();
                setStateLabel();
            }
            catch (DeviceNotFoundException)
            {
                MessageBox.Show("HID mouse device not found.", "No Device", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("No sufficient rights. You need to run the application as Administrator.", "Permissions", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
    }
}
