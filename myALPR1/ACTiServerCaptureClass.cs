using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AxnvUnifiedControlLib;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace myALPR1
{
    class ACTiServerCaptureClass
    {
        private AxnvUnifiedControl oMedia;
        public ACTiServerCaptureClass(Form form, string IP, string User, String Password)
        {


            oMedia = new AxnvUnifiedControl();

            ((System.ComponentModel.ISupportInitialize)(oMedia)).BeginInit();
            form.Controls.Add(this.oMedia);
            ((System.ComponentModel.ISupportInitialize)(oMedia)).EndInit();
          
            oMedia.Hide();

            oMedia.MediaType = 1;
            oMedia.ID = 1;
            oMedia.MediaSource = IP;//"192.168.1.162";
            oMedia.MulticastIP = "";
            oMedia.MediaChannel = 1;
            oMedia.MediaUsername = User;
            oMedia.MediaPassword = Password;
            oMedia.HttpPort = 80;
            oMedia.RegisterPort = 6000;
            oMedia.ControlPort = 6001;
            oMedia.StreamingPort = 6002;
            oMedia.MulticastPort = 5000;
            oMedia.EnableCurrentImageBuffer();
            oMedia.Connect(0);
            oMedia.DisplayTitleBar(0);

            oMedia.BaudRate = 2400;
            oMedia.AddressID = 002;
            
            oMedia.PTZPanSpeed = 9;
            oMedia.PTZTiltSpeed = 9;

            
            
            
           
        }


        public void DO1on()
        {
            if (oMedia.NetworkStatus > 0)
            {
                oMedia.SetDigitalOutput(1, 1);
            }
        }

        public void DO1off()
        {
            if (oMedia.NetworkStatus > 0)
            {
                oMedia.SetDigitalOutput(1, 0);
            }
        }


        public void DO2on()
        {
            if (oMedia.NetworkStatus > 0)
            {
                oMedia.SetDigitalOutput(2, 1);
            }
        }

        public void DO2off()
        {
            if (oMedia.NetworkStatus > 0)
            {
                oMedia.SetDigitalOutput(2, 0);
            }
        }


        public bool IsConnected()
        
        {
            if (oMedia.NetworkStatus > 0)
            {
                return true;
            }
            else return false;

        }

        public void Disconnect()
        {
            oMedia.Disconnect();
        }
        public void Play()
        {
            if (oMedia.NetworkStatus > 0)
            {
                oMedia.Play();

                oMedia.EnableOnNewImageEvent(1);
            }         
        }
        public void Stop()
        { 
            oMedia.Disconnect();
        }
        //public void PTZup()
        //{
        //    oMedia.PTZFile = "C:\\Pelco_Pelco-D.ptz";
        //    //oMedia.Parity = "N81";
           
        //    long i = oMedia.EnablePTZ();
        //    //oMedia.PTZMove("UP");
        //    oMedia.SendPTZCmd("TILTUP", 3, 0);
            
        //    //oMedia.PTZMove("STOP");
        //}

        //public void PTZdown()
        //{
        //    oMedia.PTZFile = "C:\\Pelco_Pelco-D.ptz";
        //    //oMedia.Parity = "N81";
        //    oMedia.AddressID = 002;
        //    oMedia.BaudRate = 2400;
        //    oMedia.PTZPanSpeed = 3;
        //    oMedia.PTZTiltSpeed = 3;
        //    long i = oMedia.EnablePTZ();
        //    // MessageBox.Show(i.ToString());

        //    int a = oMedia.SendPTZCmd("TILTDOWN", 3, 0);
        //    //MessageBox.Show(a.ToString());
        //    //oMedia.PTZMove("POWN");
        //    //oMedia.PTZMove("STOP"); 
        //   // oMedia.PTZMove("HOME");
        //}

        //public void PTZStop()
        //{
        //    oMedia.PTZFile = "C:\\Pelco_Pelco-D.ptz";
        //    //oMedia.Parity = "N81";
        //    oMedia.AddressID = 002;
        //    oMedia.BaudRate = 2400;
        //    oMedia.PTZPanSpeed = 3;
        //    oMedia.PTZTiltSpeed = 3;
        //    long i = oMedia.EnablePTZ();
        //    // MessageBox.Show(i.ToString());

        //    int a = oMedia.SendPTZCmd("STOP", 3, 0);
        //}


        //public void PTZZoomIn()
        //{
        //    oMedia.PTZFile = "C:\\Pelco_Pelco-D.ptz";
        //    //oMedia.Parity = "N81";
        //    oMedia.AddressID = 002;
        //    oMedia.BaudRate = 2400;
        //    oMedia.PTZPanSpeed = 3;
        //    oMedia.PTZTiltSpeed = 3;
        //    oMedia.EnablePTZ();
        //    oMedia.PTZZoom("IN");
            
            
        //}

        //public void PTZZoomOut()
        //{
        //    oMedia.PTZFile = "C:\\Pelco_Pelco-D.ptz";
        //    //oMedia.Parity = "N81";
        //    oMedia.AddressID = 002;
        //    oMedia.BaudRate = 2400;
        //    oMedia.PTZPanSpeed = 3;
        //    oMedia.PTZTiltSpeed = 3;
        //    oMedia.EnablePTZ();
        //    oMedia.PTZZoom("OUT");


        //}

        public Bitmap getCurrentFrame()
        {
            Bitmap bMap = new Bitmap(640, 480);
            oMedia.EnableCurrentImageBuffer();

            const int size = 0xFFFFFFF;
            var ptr = Marshal.AllocHGlobal(size);
            var i = ptr.ToInt32();
            try
            {
                var len = oMedia.GetCurrentImageEx(i, size);
                if (len < 1) return bMap;
                var buffer = new byte[len + 14];
                Marshal.Copy(ptr, buffer, 14, len);
                buffer[0] = 66;
                buffer[1] = 77;
                buffer[10] = 54;
                BitConverter.GetBytes(len + 14).CopyTo(buffer, 2);
                using (var stream = new MemoryStream(buffer))
                {
                    using (Image bmp = Bitmap.FromStream(stream))
                    {//Image.FromStream(stream)) {
                        bMap = new Bitmap(bmp);

                        return bMap;
                        
                    }
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
                return bMap;
            }
            finally
            {                                                                    
                Marshal.FreeHGlobal(ptr);
            }
        }
        public void Snapshot()
        {
            oMedia.SnapShot(1, "C:\\test.jpg", 0, 0, 0, 0);
        }


    }
}
