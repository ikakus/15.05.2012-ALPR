using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace myALPR1
{
    public partial class AddPlate : Form
    {
        public AddPlate(LicensePlatesDBConnectionClass DB)
        {
            DBClass = DB;
            InitializeComponent();
        }

        LicensePlatesDBConnectionClass DBClass;
        private void button1_Click(object sender, EventArgs e)//OK
        {
            if (maskedTextBox2.Text != "" && maskedTextBox1.Text.Length == 7)
            {
                DBClass.AddPlate(Convert.ToInt32(maskedTextBox2.Text), maskedTextBox1.Text, textBox2.Text);

                this.Close();
            }
        }

        private void maskedTextBox1_LostFocus(object sender, EventArgs e)
        {
            maskedTextBox1.Text = maskedTextBox1.Text.ToUpper();

            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
