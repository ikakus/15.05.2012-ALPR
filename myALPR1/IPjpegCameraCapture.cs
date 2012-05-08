using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Net;

namespace myALPR1
{
    class IPjpegCameraCapture
    {

        private string User;
        private string Password;
        private string URL;

        public void setURL(string s)
        {
            this.URL = s;
        }

        public void setUsername(string s)
        {
            this.User = s;
        }

        public void setPassword(string s)
        {
            this.Password = s;
        }

        public IPjpegCameraCapture(string Username, string Password, string sourceURL)
        {
            setPassword(Password);
            setUsername(Username);
            setURL(sourceURL);
        }

        public Bitmap capture()
        {


            Bitmap bmp;


            try
            {
                string sourceURL = this.URL;// = "http://192.168.1.162:80/goform/video2";

                byte[] buffer = new byte[100000];
                int read, total = 0;
                // create HTTP request

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);
                // get response
                req.Credentials = new NetworkCredential(this.User, this.Password); //("admin", "pataraia");

                WebResponse resp = req.GetResponse();
                // get response stream

                Stream stream = resp.GetResponseStream();
                // read data from stream                                             

                while ((read = stream.Read(buffer, total, 1000)) != 0)
                {
                    total += read;
                }
                // get bitmap

                bmp = new Bitmap((Bitmap)Bitmap.FromStream(new MemoryStream(buffer, 0, total)));

                return bmp;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return new Bitmap(640,480);
        }

    }
}
