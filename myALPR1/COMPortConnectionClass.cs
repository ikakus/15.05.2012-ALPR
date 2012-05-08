using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using myALPR1.Properties;
using System.IO.Ports;
using System.IO;


namespace myALPR1
{

    public class COMPortConnectionClass
    {
        private SerialPort comport;

        Settings settings = Settings.Default;


        public COMPortConnectionClass()
        {
            comport = new SerialPort();

        }

        public bool IsOpen()
        {
            return comport.IsOpen;
        }

        public void Open()
        {
            bool error = false;
            if (comport.IsOpen == true)
            {
                comport.Close();
            }
            else
            {
                //comport.PortName = ConfigForm.cmbPortName.Text;
                //comport.BaudRate = int.Parse(ConfigForm.cmbBaudRate.Text);
                //comport.Parity = (Parity)Enum.Parse(typeof(Parity), ConfigForm.cmbParity.Text);
                //comport.DataBits = int.Parse(ConfigForm.cmbDataBits.Text);
                //comport.StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigForm.cmbStopBits.Text);



                comport.PortName = settings.COMPortName;
                comport.BaudRate = settings.COMPortBaudRate;
                comport.Parity = settings.COMPortParity;
                comport.DataBits = settings.COMPortDataBits;
                comport.StopBits = settings.COMPortStopBits;

                // // comport.Handshake = SetPortHandshake(_serialPort.Handshake);
                try
                {
                    // Open the port
                    comport.Open();
                }
                catch (UnauthorizedAccessException) { error = true; }
                catch (IOException) { error = true; }
                catch (ArgumentException) { error = true; }

                if (error) MessageBox.Show("Could not open the COM port.  Most likely it is already in use, has been removed, or is unavailable.", "COM Port Unavalible", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                else
                {
                    //// Show the initial pin states
                    //UpdatePinState();
                    //chkDTR.Checked = comport.DtrEnable;
                    //chkRTS.Checked = comport.RtsEnable;
                }
            }
        }

        private string[] OrderedPortNames()
        {
            // Just a placeholder for a successful parsing of a string to an integer
            int num;

            // Order the serial port names in numberic order (if possible)
            return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
        }

        private string RefreshComPortList(IEnumerable<string> PreviousPortNames, string CurrentSelection, bool PortOpen)
        {
            // Create a new return report to populate
            string selected = null;

            // Retrieve the list of ports currently mounted by the operating system (sorted by name)
            string[] ports = SerialPort.GetPortNames();

            // First determain if there was a change (any additions or removals)
            bool updated = PreviousPortNames.Except(ports).Count() > 0 || ports.Except(PreviousPortNames).Count() > 0;

            // If there was a change, then select an appropriate default port
            if (updated)
            {
                // Use the correctly ordered set of port names
                ports = OrderedPortNames();

                // Find newest port if one or more were added
                string newest = SerialPort.GetPortNames().Except(PreviousPortNames).OrderBy(a => a).LastOrDefault();

                // If the port was already open... (see logic notes and reasoning in Notes.txt)
                if (PortOpen)
                {
                    if (ports.Contains(CurrentSelection)) selected = CurrentSelection;
                    else if (!String.IsNullOrEmpty(newest)) selected = newest;
                    else selected = ports.LastOrDefault();
                }
                else
                {
                    if (!String.IsNullOrEmpty(newest)) selected = newest;
                    else if (ports.Contains(CurrentSelection)) selected = CurrentSelection;
                    else selected = ports.LastOrDefault();
                }
            }

            // If there was a change to the port list, return the recommended default selection
            return selected;
        }

        private void tmrCheckComPorts_Tick(object sender, EventArgs e)
        {
            // checks to see if COM ports have been added or removed
            // since it is quite common now with USB-to-Serial adapters
            // RefreshComPortList();
        }

        public void enableDTR()
        {
            comport.DtrEnable = true;
        }

        public void disableDTR()
        {
            comport.DtrEnable = false;
        }

        public void enableRTS()
        {
            comport.RtsEnable = true;
        }

        public void disableRTS()
        {
            comport.RtsEnable = false;
        }

        
    }
}
