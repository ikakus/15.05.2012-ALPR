using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using myALPR1.Properties;
using System.IO.Ports;

namespace myALPR1
{
    public partial class ConfigurationsForm : Form
    {

        //string fileLoc = @"config.ini";
        public ConfigurationsForm( COMPortConnectionClass com )
        {
            InitializeComponent();
            //loadSettings();
            LoadSettingsGlobal();

            comport = com;
        }

         COMPortConnectionClass comport;
        private Settings settings = Settings.Default;
        private bool conStringChanged = false;
        private string IP;
        private string User;
        private string Password;
        private string ConString;




        public string getIP()
        {
            return IP;
        }

        public string getUser()
        {
            return User;
        }
        public string getPass()
        {
            return Password;
        }
        public string getConString()
        {
            return ConString;
        }

        private int GetNthIndex(string s, char t, int n)//for Convert() Function
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == t)
                {
                    count++;
                    if (count == n)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        //public void createDefaultConfFile()
        //{

        //    FileStream fs = null;
        //    fs = File.Create(fileLoc);
        //    fs.Close();
        //    using (StreamWriter sw = new StreamWriter(fileLoc))
        //    {
        //        sw.WriteLine("ServerIP=192.168.1.162");
        //        sw.WriteLine("User=Admin");
        //        sw.WriteLine("Password=123456");
        //        sw.WriteLine("conString=Data Source=" + System.Environment.MachineName + "\\SQLEXPRESS;Initial Catalog=ALPRDataBase;"
        //        + "Persist Security Info=True;User ID=sa;Password=123456");// IKA-FDF3AA55734
               
        //        sw.Close();
        //    }
        //}

        //public void loadSettings()
        //{
            
        //    if (File.Exists(fileLoc))
        //    {
        //        using (TextReader tr = new StreamReader(fileLoc))
        //        {
        //            string str;
        //            while (true)
        //            {
        //                str = tr.ReadLine();
        //                if (str == null)
        //                {
        //                    break;
        //                }
        //                if (str.Substring(0, GetNthIndex(str, '=', 1)) == "ServerIP")
        //                {
        //                    int symbpos = GetNthIndex(str,'=',1);
        //                    IP = str.Substring(symbpos + 1, str.Length - symbpos - 1);
                            
        //                }
        //                if (str.Substring(0, GetNthIndex(str, '=', 1)) == "User")
        //                {
        //                    int symbpos = GetNthIndex(str, '=', 1);
        //                    User =  str.Substring(symbpos + 1, str.Length - symbpos - 1);
        //                  //  Form1.SynListener.setPort(System.Convert.ToInt32(str.Substring(symbpos + 1, str.Length - symbpos - 1)));
        //                }
        //                if (str.Substring(0, GetNthIndex(str, '=', 1)) == "Password")
        //                {
        //                    int symbpos = GetNthIndex(str, '=', 1);
        //                    Password =  str.Substring(symbpos + 1, str.Length - symbpos - 1);
        //                    //Form1.SQLConnectionString = str.Substring(symbpos + 1, str.Length - symbpos - 1);
        //                }
        //                if (str.Substring(0, GetNthIndex(str, '=', 1)) == "conString")
        //                {
        //                    int symbpos = GetNthIndex(str, '=', 1);
        //                    ConString = str.Substring(symbpos + 1, str.Length - symbpos - 1);
        //                    //Form1.SQLConnectionString = str.Substring(symbpos + 1, str.Length - symbpos - 1);
        //                }
                       
                       
        //            }
        //        } 
        //    }
        //    else
        //    {
        //        createDefaultConfFile();   
        //    }

          
        //}

        //public void saveConfig()
        //{
        //    if (File.Exists(fileLoc))
        //    {
        //        using (StreamWriter sw = new StreamWriter(fileLoc))
        //        {
        //            sw.WriteLine("ServerIP="+textBox_IP.Text);
        //            sw.WriteLine("User="+textBox_User.Text);
        //            sw.WriteLine("Password="+textBox_Pass.Text);
        //            sw.WriteLine("conString=" + textBox_connectionString.Text);//Data Source=IKA-FDF3AA55734\\SQLEXPRESS;Initial Catalog=Point;Persist Security Info=True;User ID=sa;Password=12345");
                    
                  

        //            sw.Close();
        //        }
        //    }
        //}


        public void SaveSettings()
        {
            settings.COMPortBaudRate = int.Parse(cmbBaudRate.Text);
            settings.COMPortDataBits = int.Parse(cmbDataBits.Text);
            settings.COMPortParity = (Parity)Enum.Parse(typeof(Parity), cmbParity.Text);
            settings.COMPortStopBits = (StopBits)Enum.Parse(typeof(StopBits), cmbStopBits.Text);
            settings.COMPortName = cmbPortName.Text;

            settings.ACTiPAssword = textBox_Pass.Text;
            settings.ACTiServerIP = textBox_IP.Text;
            settings.ACTiUser = textBox_User.Text;

            settings.DBConnectionString = textBox_connectionString.Text;

            settings.Save();
        }
        public void LoadSettingsGlobal()
        {
            this.IP = settings.ACTiServerIP;
            this.User = settings.ACTiUser;
            this.Password = settings.ACTiPAssword;

            if (settings.DBConnectionString == "")
            {
                this.ConString = "Data Source=" + System.Environment.MachineName + "\\SQLEXPRESS;Initial Catalog=ALPRDataBase;"
                + "Persist Security Info=True;User ID=sa;Password=123456";
            }
            else

            {
                this.ConString = settings.DBConnectionString;
            }



        }


        private void RefreshComPortList()
        {
            // Determain if the list of com port names has changed since last checked
            string selected = RefreshComPortList(cmbPortName.Items.Cast<string>(), cmbPortName.SelectedItem as string, comport.IsOpen());

            // If there was an update, then update the control showing the user the list of port names
            if (!String.IsNullOrEmpty(selected))
            {
                cmbPortName.Items.Clear();
                cmbPortName.Items.AddRange(OrderedPortNames());
                cmbPortName.SelectedItem = selected;
            }
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

        private string[] OrderedPortNames()
        {
            // Just a placeholder for a successful parsing of a string to an integer
            int num;

            // Order the serial port names in numberic order (if possible)
            return SerialPort.GetPortNames().OrderBy(a => a.Length > 3 && int.TryParse(a.Substring(3), out num) ? num : 0).ToArray();
        }

        public void fillConfForm()
        {


            cmbStopBits.Items.Clear();
            cmbStopBits.Items.AddRange(Enum.GetNames(typeof(StopBits)));

            cmbParity.Items.Clear(); 
            cmbParity.Items.AddRange(Enum.GetNames(typeof(Parity)));

            textBox_IP.Text = this.IP;
            textBox_User.Text = this.User;
            textBox_Pass.Text = this.Password;
            textBox_connectionString.Text = this.ConString;

            textBox_ACTiControlPort.Text =  settings.ACTiServerControlPort.ToString();
            textBox_ACTiHttpPort.Text = settings.ACTiServerHttpPort.ToString();
            textBox_ACTiMultIP.Text = settings.ACTiServerMulticastIP;
            textBox_ACTiMultPort.Text = settings.ACTiServerMulticastPort.ToString();
            textBox_ACTiRegPort.Text = settings.ACTiServerRegisterPort.ToString();
            textBox_ACTiStreamngPort.Text = settings.ACTiServerStreamingPort.ToString();
            textBox_ATCiID.Text = settings.ACTiServerID.ToString();
            textBox_MediaType.Text = settings.ACTiServerMediaType.ToString();


            comboBox_ACTiBaudRate.Text = settings.ACTiServerBaudRate.ToString();

            cmbBaudRate.Text = settings.COMPortBaudRate.ToString();
            cmbDataBits.Text = settings.COMPortDataBits.ToString();
            cmbParity.Text = settings.COMPortParity.ToString();
            cmbStopBits.Text = settings.COMPortStopBits.ToString();

            RefreshComPortList();

            if (cmbPortName.Items.Contains(settings.COMPortName)) cmbPortName.Text = settings.COMPortName;
            else if (cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = cmbPortName.Items.Count - 1;
            else
            {
                MessageBox.Show(this, "There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.", "No COM Ports Installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ConfigurationsForm_Load(object sender, EventArgs e) //load form
        {
            //loadSettings();
            LoadSettingsGlobal();
            fillConfForm();
            conStringChanged = false;
        }

        private void button2_Click(object sender, EventArgs e) //save
        {

           // saveConfig();
            SaveSettings();
            if (conStringChanged == true)
            {
                MessageBox.Show("Connection string changed. Restart program to changes take effect");
                conStringChanged = false;
            }
            LoadSettingsGlobal();
           // loadSettings();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e) //cancel
        {
            this.Hide();
        }

        private void ConfigurationsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void textBox_connectionString_TextChanged(object sender, EventArgs e)
        {
            conStringChanged = true;
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        public string getcmbPortName()
        {
            return cmbPortName.Text;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void lblComPort_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            comport.Open();
        }
    }
}
