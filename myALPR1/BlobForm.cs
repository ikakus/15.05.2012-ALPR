using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;

namespace myALPR1
{
    public partial class BlobForm : Form
    {
       
        
         int picsCount1 = 0;
         int picsCount2 = 0;
         int picsCount3 = 0;
         int lettersCount = 0;
         int lettersCount2 = 0;
         int lettersCount3 = 0;
        public BlobForm()
        {
            InitializeComponent();
        }

        public void setText1(string str)
        {
            this.label1.Text = str;
        }
        public void setText2(string str)
        {
            this.label2.Text = str;
        }

        public void loadPicture(Bitmap bmp)
        {
            
            this.pictureBox1.Image = bmp;
            //this.pictureBox1.Width = bmp.Width;
            //this.pictureBox1.Height = bmp.Height;
           
        }


        public void addPicture(Bitmap bmp)
        {

            PictureBox pic = new PictureBox();
           
            pic.Size = new Size(bmp.Width, bmp.Height);
            pic.Location = new Point( picsCount1 * 40 + 10,100);
            pic.Image = bmp;
            pic.Visible = true;
            //pic.BorderStyle = BorderStyle.FixedSingle;
           // pic.BackColor = Color.Black;
           // pic.
            this.Controls.Add(pic);
            picsCount1++;
          

        }
        public void addPicture2(Bitmap bmp)
        {

            PictureBox pic = new PictureBox();

            pic.Size = new Size(bmp.Width, bmp.Height);
            pic.Location = new Point(picsCount2 * 40 + 10, 150);
            pic.Image = bmp;
            pic.Visible = true;
            //pic.BorderStyle = BorderStyle.FixedSingle;
            // pic.BackColor = Color.Black;
            // pic.
            this.Controls.Add(pic);
            picsCount2++;


        }
        public void addPicture3(Bitmap bmp)
        {

            PictureBox pic = new PictureBox();

            pic.Size = new Size(bmp.Width, bmp.Height);
            pic.Location = new Point(picsCount3 * 40 + 10, 200);
            pic.Image = bmp;
            pic.Visible = true;
            
            this.Controls.Add(pic);
            picsCount3++;


        }

        public void addLetter(String str)
        {
            Label la = new Label();
            
            la.Text = str;
            la.AutoSize = true;
            la.Location = new Point(lettersCount * 40 + 20, 250);
          
            this.Controls.Add(la);
            lettersCount++;
        }


        public void addLetter2(String str)
        {
            Label la = new Label();

            la.Text = str;
            la.AutoSize = true;
            la.Location = new Point(lettersCount2 * 40 + 20, 300);

            this.Controls.Add(la);
            lettersCount2++;
        }


        public void addLetter3(String str)
        {
            Label la = new Label();

            la.Text = str;
            la.AutoSize = true;
            la.Location = new Point(lettersCount3 * 40 + 20, 350);

            this.Controls.Add(la);
            lettersCount3++;
        }

        public void loadPicture2(Bitmap bmp)
        {

            ResizeNearestNeighbor Resizefilter = new ResizeNearestNeighbor(200, 40);
            Bitmap resized = Resizefilter.Apply(bmp);

            this.pictureBox2.Image = resized;
        }
        private void BlobForm_Load(object sender, EventArgs e)
        {

        }
    }
}
