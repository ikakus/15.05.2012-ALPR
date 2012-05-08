using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using AForge.Video;
using AForge.Video.DirectShow;


namespace myALPR1
{
    class BT878CaptureClass
    {

        private AForge.Controls.VideoSourcePlayer videoSourcePlayer;
        private VideoCaptureDevice videoSource;


        public  BT878CaptureClass()
        {
            videoSourcePlayer = new AForge.Controls.VideoSourcePlayer();
            videoSourcePlayer.Size = new System.Drawing.Size(640, 480);
            FilterInfoCollection videoDevices;



            string device;
            try
            {
                // enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                // add all devices to combo

                device = videoDevices[0].MonikerString;
                videoSource = new VideoCaptureDevice(device);
                
               
            }
            catch (ApplicationException)
            {
                MessageBox.Show("No local capture devices");

            }

        }


        public void StartPlay()
        {
            // stop current video source
            videoSourcePlayer.SignalToStop();
            videoSourcePlayer.WaitForStop();

            // start new video source
            videoSourcePlayer.VideoSource = videoSource;
            videoSourcePlayer.Start();
        }


        public  void StopPlay()
        {

            if (videoSourcePlayer.VideoSource != null)
            {
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
            }
        }

        public Bitmap getBMP()
        {
            
              return   videoSourcePlayer.GetCurrentVideoFrame();
            
        }

    }
}
