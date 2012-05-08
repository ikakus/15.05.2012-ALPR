using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Net;
using AForge.Imaging.Filters;

namespace myALPR1
{
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
           
            //ACTi.Play();
            // _ocr = new Tesseract("tessdata", "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_CUBE_COMBINED);
        }


        //private string ConStr;
        //private string User;
        //private string Passwrd;
        //private string IP;

        Thread CamraCaptureThread;

        ALPRclass myALPR = new ALPRclass();
        // IPjpegCameraCapture MyCapture = new IPjpegCameraCapture("admin", "pataraia", "http://192.168.1.162:80/goform/video2");
        Bitmap globalBMPtoALPR = new Bitmap(640, 480);

        //  BT878CaptureClass BTCapture = new BT878CaptureClass();
        ACTiServerCaptureClass ACTi; //= new ACTiServerCaptureClass(this, "192.168.1.162", "Admin", "123456");// = new ACTiServerCaptureClass(this, "192.168.1.162", "Admin", "123456");

        // ACTi = new ACTiServerCaptureClass(this, "192.168.1.162", "Admin", "123456");

        ResizeNearestNeighbor ResizeNearestNeighbor = new ResizeNearestNeighbor(640, 480);
      //  ResizeBilinear ResizeBilinearFilter = new ResizeBilinear(640, 480);
       // ResizeBicubic ResizeBiBubic = new ResizeBicubic(640, 480);
        EdgNBlobALPRClass Edg = new EdgNBlobALPRClass();
        ConfigurationsForm ConForm;
        delegate void UpdateListViewDelegate(ListViewItem itm, ListView listv, ImageList BmList);

        LicensePlatesDBConnectionClass BaseConnection;
        SecBase sch;
        CapturedPlatesLog plLog;

        int CORRELATION = 4600;

        //public void UpdateListView(ListViewItem itm, ListView listv, ImageList BmList)
        //{
        //    if (listv.InvokeRequired)
        //        try
        //        {
        //            listv.Invoke(new UpdateLabelDelegate(UpdateLabel), new object[] { itm, listv, BmList });
        //        }
        //        catch (Exception)
        //        { }
        //    else
        //    {
        //        listView1.LargeImageList = BmList;
        //        this.listView1.Items.Add(itm);//.AppendText(msg);
        //    }
        //}
        bool started = false;

        int countRows = 0 ;
        delegate void UpdateLabelDelegate(String str, Label la);       
        public void UpdateLabel(String str, Label la)
        {

            if (la.InvokeRequired)
                try
                {
                    la.Invoke(new UpdateLabelDelegate(UpdateLabel), new object[] { str, la });
                }
                catch (Exception)
                { }
            else
                la.Text = str;//.AppendText(msg);

        }
        delegate void setLabelColorDelegate(Label la, Color col);
        public void setLabelColor(Label label, Color col)
        {
            if (label.InvokeRequired)
             try
            {
                label.Invoke(new setLabelColorDelegate(setLabelColor), new object[] { label, col });
            }
            catch (Exception)
            {}
            else
            {
               label.ForeColor = col;
            }
        }
        bool requestStop = false;      
        private void cap()
        {

            ACTi.Play();
            Bitmap TempBMP = new Bitmap(640, 480);
            List<Bitmap> imgList;

           // listView1.View = View.LargeIcon;
            ImageList bmpLIST = new ImageList();
            ALPRclass.ReturnedValue returnedVal = new ALPRclass.ReturnedValue();

            Bitmap bmpEdgeDetectedRedSquare = new Bitmap(640, 480);

            while (!requestStop)
            {
              

                 Monitor.Enter(this);

                bool EdgeReturned = false;
                bool VerticalReturned = false;
                TempBMP = ACTi.getCurrentFrame();//BTCapture.getBMP();
                if (TempBMP != null)
                {
                    globalBMPtoALPR = ResizeNearestNeighbor.Apply(TempBMP);
                }
                myALPR.setSumCorrelation(0);
                Bitmap image = globalBMPtoALPR;


                #region Edge Detection based ALPR

              
                BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, image.PixelFormat);
                UnmanagedImage img = new UnmanagedImage(bitmapData);
                 string EdgStr=".";
               // IFilter grayfilter = new GrayscaleBT709();
                IFilter contrast = new ContrastCorrection(3);
                // IFilter brightnes = new BrightnessCorrection(0.13f);

                // img = brightnes.Apply(img);
                if (checkBox1.Checked == true)
                {
                    img = contrast.Apply(img);
                }

                if (checkBox3.Checked == false && checkBox2.Checked == false)
                {
                    pictureBox_image.Image = img.ToManagedImage();
                }

                imgList = Edg.findPlates(img);
                image.UnlockBits(bitmapData);
               // Bitmap bmpEdgeDetectedRedSquare = new Bitmap(640, 480);
                if (checkBox3.Checked == true)
                {
                    

                    foreach (Bitmap bmp in imgList)
                    {
                        if (bmp.Width == 640 & bmp.Height == 480)
                        {
                            pictureBox_image.Image = bmp;
                            bmpEdgeDetectedRedSquare = bmp;
                        }
                        else
                        {
                            bmpLIST.Images.Add(bmp);
                            Edg.setSumCorrelation(0);
                            EdgStr = Edg.OCR(bmp);
                            if (EdgStr.Length > 0)
                            {
                                if (Edg.getSumCorrealtion() > CORRELATION || EdgStr!="" && EdgStr!="")
                                {
                                    UpdateLabel(EdgStr, label4);
                                    EdgeReturned = true;

                                }
                                else
                                {
                                    UpdateLabel("too low", label4);
                                    EdgeReturned = false;
                                }
                               
                                UpdateLabel(Edg.getSumCorrealtion().ToString(), label5);

                            }



                        }
                    }

                    if (!requestStop && EdgeReturned == false)
                    {
                        UpdateLabel(".", label4);
                    }
                }
                #endregion


                #region Vertical lines detection ALPR
                //if (TempBMP != null)
                //{
                //    globalBMPtoALPR = ResizeNearestNeighbor.Apply(TempBMP);
                //}
                //myALPR.setSumCorrelation(0);
                //Bitmap image = globalBMPtoALPR;

             

                if (checkBox2.Checked == true)
                {
                    returnedVal = myALPR.doTheShit(globalBMPtoALPR);//img.ToManagedImage());


                    pictureBox_graph.Image = returnedVal.GraphBMP;
                    if (returnedVal.RecognizedPlates[0].Length == 7)
                    {
                        if (myALPR.getSumCorrealtion() > CORRELATION  )
                        {
                            UpdateLabel(returnedVal.RecognizedPlates[0], label1);
                            VerticalReturned = true;

                        }
                        else
                        {
                            UpdateLabel("too low", label1);

                        }

                        UpdateLabel(myALPR.getSumCorrealtion().ToString(), label2);
                    }

                    else if (!requestStop)
                    {
                        UpdateLabel(".", label1);
                    }

                    if (checkBox3.Checked == false)
                    {
                        pictureBox_image.Image = returnedVal.BMap;

                    }

                }
                #endregion


                if (EdgeReturned == true && VerticalReturned == true)
                {
                    setLabelColor(label6, Color.Green);
                    UpdateLabel(returnedVal.RecognizedPlates[0], label6); //MessageBox.Show("Olol");
                    //MessageBox.Show(BaseConnection.GetInfoById( BaseConnection.Search(returnedVal.RecognizedPlates[0])));
                    Thread.Sleep(10);
                    BaseConnection.StorePictureToLogBase(countRows + 1, globalBMPtoALPR, returnedVal.RecognizedPlates[0], DateTime.Now);
                    countRows++;
                }
                else
                {

                    if (EdgeReturned == true)
                    {
                        setLabelColor(label6, Color.DarkGoldenrod);
                        UpdateLabel(EdgStr, label6);
                        //if (BaseConnection.Search(EdgStr)>0)
                        //{
                        //MessageBox.Show(BaseConnection.GetInfoById(BaseConnection.Search(EdgStr)));
                        //}
                        Thread.Sleep(20);
                        if (EdgStr != "")
                        {

                            BaseConnection.StorePictureToLogBase(countRows + 1, bmpEdgeDetectedRedSquare, EdgStr, DateTime.Now);
                            countRows++;
                        }
                
                    }

                   // else
                    
                        if (VerticalReturned == true)
                        {
                            setLabelColor(label6, Color.DarkGoldenrod);
                            UpdateLabel(returnedVal.RecognizedPlates[0], label6);
                            Thread.Sleep(10);
                            BaseConnection.StorePictureToLogBase(countRows + 1, globalBMPtoALPR, returnedVal.RecognizedPlates[0], DateTime.Now);
                            countRows++;
                        }
                    
                }


                
                Thread.Sleep(60);


                 Monitor.Exit(this);
            }

        }
        public void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                // "192.168.1.162", "Admin", "123456");
                COMPortConnectionClass COMPortConection = new COMPortConnectionClass();

                ConForm = new ConfigurationsForm(COMPortConection);
                // ConForm.getConString();
               // ACTi = new ACTiServerCaptureClass(this, ConForm.getIP(), ConForm.getUser(), ConForm.getPass());

                if (connected == true)
                {
                    button1.Enabled = true;
                    
                }
                else {
                    button1.Enabled = false;
                }
                
                BaseConnection = new LicensePlatesDBConnectionClass(ConForm.getConString());//"IKAKUS", "ALPRDataBase", "True", "sa", "123456");//("IKA-FDF3AA55734", "ALPRDataBase", "True", "sa", "12345");
                countRows = BaseConnection.getRowsCount();
                sch = new SecBase(BaseConnection);
                plLog = new CapturedPlatesLog(BaseConnection);
                //ConForm.createDefaultConfFile();
                myALPR.mOcr.fillEtalonBase();
                Edg.EdgemOcr.fillEtalonBase();
            //    CamraCaptureThread = new Thread(cap);
               // CamraCaptureThread.IsBackground = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        public void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (connected == true)
            {
                ACTi.Stop();

                requestStop = true;

                if (started == true )
                {
                    if (CamraCaptureThread.ThreadState == System.Threading.ThreadState.Background)
                        CamraCaptureThread.Join();
                }

                if (suspended == true)
                {
                    CamraCaptureThread.Resume();
                    if (CamraCaptureThread.ThreadState == System.Threading.ThreadState.Background)
                        CamraCaptureThread.Join();
                }
            }


        } 
        private void button_Open_Click(object sender, EventArgs e)
        {

            // EdgNBlobALPRClass Edg = new EdgNBlobALPRClass();
            OpenFileDialog ofd = new OpenFileDialog();
            List<Bitmap> imgList;
            if (ofd.ShowDialog() == DialogResult.OK)
            {


                try
                {
                    Stopwatch watch = new Stopwatch();

                    ImageList bmpLIST = new ImageList();

                    ALPRclass.ReturnedValue returnedVal = new ALPRclass.ReturnedValue();
                    Bitmap image = new Bitmap(ofd.FileName);
                    // create image document
                    BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
                    UnmanagedImage img = new UnmanagedImage(bitmapData);

                    watch.Start();
                    IFilter contrast = new ContrastCorrection(3);
                    IFilter brightnes = new BrightnessCorrection(0.13f);

                    // img = brightnes.Apply(img);
                    //img = contrast.Apply(img);

                    //Bitmap b = Edg.findPlates(img);


                    //listView1.Clear();
                    imgList = Edg.findPlates(img);

                    image.UnlockBits(bitmapData);

                   // listView1.View = View.LargeIcon;
                   // listView1.LargeImageList = bmpLIST;
                    bmpLIST.ImageSize = new Size(256, 64);
                    foreach (Bitmap bmp in imgList)
                    {
                        if (bmp.Width == 640 & bmp.Height == 480)
                        {
                            pictureBox_image.Image = bmp;
                        }
                        else
                        {
                            bmpLIST.Images.Add(bmp);

                            String str = Edg.OCR(bmp);
                            if (str.Length > 6)
                            {

                                UpdateLabel(str, label1);
                                MessageBox.Show(Edg.OCR(bmp));
                            }

                        }
                    }



                    for (int j = 0; j < bmpLIST.Images.Count; j++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.ImageIndex = j;
                       // this.listView1.Items.Add(item);
                    }


                    this.Text = ofd.FileName;
                    watch.Stop();




                  //  label_maxSpike.Text = watch.Elapsed.TotalSeconds.ToString();


                }
                catch (ApplicationException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



            }
        }
        bool suspended = false;
        private void button1_Click(object sender, EventArgs e)
        {
           
            // ACTi = new ACTiServerCaptureClass(this,ConForm.getIP(),ConForm.getUser(),ConForm.getPass());

                if (started == false)
                {
                    // ACTi = new ACTiServerCaptureClass(this, "192.168.1.162", "Admin", "123456");
                    button1.Text = "Start";
                    Thread.Sleep(1000);
                    CamraCaptureThread = new Thread(cap);
                    CamraCaptureThread.IsBackground = true;
                    CamraCaptureThread.Start();



                    started = true;
                    
                }
                if (suspended == true)
                {
                    CamraCaptureThread.Resume();

                    suspended = false;
                    
                }

                
            
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (started == true)
            {
                CamraCaptureThread.Suspend();

                // ACTi.Stop();
                // started = false;
                suspended = true;

            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
           // ACTi.Snapshot();
          //  ACTi.PTZup();
        }
        private void button4_Click(object sender, EventArgs e)
        {
          //  ACTi.PTZdown();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            //ACTi.PTZStop();
        }
        private void button6_Click(object sender, EventArgs e)
        {
           // ACTi.PTZZoomIn();
        }
        private void button7_Click(object sender, EventArgs e)
        {
           // ACTi.PTZZoomOut();
        }
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConForm.Show();
        }

        private void baseToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }

        private void secBaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sch.Show();
            sch.fillScheduleGrid();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plLog.Show();
            plLog.fillScheduleGrid();
        }


        bool connected = false;
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (connected == false)// Connect
            {
                try
                {
                    ACTi = new ACTiServerCaptureClass(this, ConForm.getIP(), ConForm.getUser(), ConForm.getPass());


                    if (ACTi.IsConnected())
                    {
                        button1.Enabled = true;
                        toolStripDropDownButton1.Text = "Connected";
                        toolStripDropDownButton1.Image = Properties.Resources.Bitmap_Green;

                        connectToolStripMenuItem.Text = "Disconnect";
                        connectToolStripMenuItem.Image = Properties.Resources.Bitmap_Red;
                        connected = true;
                    }
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (connected == true)//Disconnect
            {
                //ACTi.Stop();
                try
                {
                    ACTi.Disconnect();
                    
                    button1.Enabled = false;
                    toolStripDropDownButton1.Text = "Disconnected";
                    toolStripDropDownButton1.Image = Properties.Resources.Bitmap_Red;

                    connectToolStripMenuItem.Text = "Connect";
                    connectToolStripMenuItem.Image = Properties.Resources.Bitmap_Green;

                    connected = false;
                    started = false;

                    ACTi.Stop();
                    requestStop = true;

                    if (CamraCaptureThread.ThreadState == System.Threading.ThreadState.Background)
                        CamraCaptureThread.Join();
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {

        }

     


    }
}
