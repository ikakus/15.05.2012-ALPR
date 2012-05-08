using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace myALPR1
{
    public partial class SecBase : Form
    {
        public SecBase(LicensePlatesDBConnectionClass Db)
        {
            InitializeComponent();
            DatabaseConnector = Db;
            con.ConnectionString = DatabaseConnector.getConnectionString();
            con.Open();
            
        }

        int count;
        delegate void RenewQuerryListDelegate();
        delegate void ClearListDelegate();
        delegate void QuerryListAdd(ListViewItem itm);
        delegate string GetListText(int i,int j);
        delegate void SetListItemTextDelagate(int i, int j ,string str);
        public System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();

        public LicensePlatesDBConnectionClass DatabaseConnector;

        public int getCount()
        {
            return count;
        }
        public void SetListItemText(int i, int j, string str)
        {


            if (this.listView1.InvokeRequired)
                this.listView1.Invoke(new SetListItemTextDelagate(SetListItemText), new object[] { i,j,str });
            else
            {
                this.listView1.Items[i].SubItems[j].Text = str;

            }
        }
        public string GetText(int i, int j)
        {
            if (this.listView1.InvokeRequired)
               return (string) this.listView1.Invoke(new GetListText(GetText), new object[] { i ,j});
            else
            {
                return listView1.Items[i].SubItems[j].Text;

            }
            
        }
        public void Scheduler_FormClosing(object sender, FormClosingEventArgs e)
        {


            this.Hide();
            e.Cancel = true; // this cancels the close event.

        }
        public void AddToQuerry(ListViewItem itm)
        {
            if (this.listView1.InvokeRequired)
                this.listView1.Invoke(new QuerryListAdd(AddToQuerry), new object[] { itm });
            else
            {
               
                this.listView1.Items.Add(itm);

            }
 
        }
        public void ClearList()
        {
            if (this.listView1.InvokeRequired)
                this.listView1.Invoke(new ClearListDelegate(ClearList), new object[] { });
            else
            {
                this.listView1.Items.Clear();
               
            }
        }
        //public void RenewQuerryList()
        //{

        //    if (this.listView1.InvokeRequired)
        //        this.listView1.Invoke(new RenewQuerryListDelegate(RenewQuerryList), new object[] { });
        //    else
        //    {
        //       // this.listView1.Items.Clear();
        //        this.UpdateScheduleGrid();
        //    }
        //}
        public void  UpdateScheduleGrid()
        {
            //System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
            //con.ConnectionString = Form1.SQLConnectionString;
            System.Data.SqlClient.SqlDataReader myReader = null;
            string[] str = new string[6];
            //ClearList(); 
            ListViewItem itm;
            string comm = "SELECT * FROM [MessageSchedule]";   
             //con.Open();
            System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(comm, con);
            myReader = myCommand.ExecuteReader();
            int i = 0;
            while (myReader.Read())
            {
                str[0] = myReader["FKUnit_ID"].ToString();
                str[1] = myReader["message1"].ToString();
                str[2] = myReader["message2"].ToString();
                str[3] = myReader["message3"].ToString();
                str[4] = myReader["message4"].ToString();
                str[5] = myReader["message5"].ToString();

               // listView1.Items[0].SubItems.Clear();

                itm = new ListViewItem(str);
                itm.Name = str[0];

                if(GetText(i,0)==str[0])//if (listView1.Items[i].SubItems[0].Text == str[0])
                {
                    //listView1.Items[i].SubItems[1].Text = str[1];
                    //listView1.Items[i].SubItems[2].Text = str[2];
                    //listView1.Items[i].SubItems[3].Text = str[3];
                    //listView1.Items[i].SubItems[4].Text = str[4];
                    //listView1.Items[i].SubItems[5].Text = str[5];
                    SetListItemText(i, 0, str[0]);
                    SetListItemText(i, 1, str[1]);
                    SetListItemText(i, 2, str[2]);
                    SetListItemText(i, 3, str[3]);
                    SetListItemText(i, 4, str[4]);
                    SetListItemText(i, 5, str[5]);

                }

                i++;

            }
            myReader.Close();
            myReader.Dispose();
        }
        public void fillScheduleGrid()
        {
           // //System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
           // con.ConnectionString = Form1.SQLConnectionString;
            System.Data.SqlClient.SqlDataReader myReader = null;
            string[] str = new string[3];
            ClearList(); 
            ListViewItem itm;
            string comm = "SELECT * FROM [Plates]";
           // con.Open();
            
            System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(comm, con);
            myReader = myCommand.ExecuteReader();
            
            while (myReader.Read())
            {
                str[0] = myReader["ID"].ToString();
                str[1] = myReader["LicenseNumber"].ToString();
                str[2] = myReader["Owner"].ToString();
               

                //listView1.Items[0].SubItems.Clear();
               
                itm = new ListViewItem(str);
                itm.Name = str[0];
                
                AddToQuerry(itm);

                count++;
               
            }

            myReader.Dispose();

           
             //con.Close();

        }
        private void CheckAll()
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Checked = true;
            }
            
        }
        private void UncheckAll()
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                listView1.Items[i].Checked = false;
            }
 
        }
        public void Scheduler_Load(object sender, EventArgs e)
        {

            //con.ConnectionString = Form1.SQLConnectionString;
            fillScheduleGrid();
          //  CheckAll();


        }

        public void RemoveSelected()
        {
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (listView1.Items[i].Checked == true)
                {
                    DatabaseConnector.DeleteByID(Convert.ToInt32 (listView1.Items[i].SubItems[0].Text));
                }
            }
            ClearList();
            fillScheduleGrid();
        }


        public void Scheduler_SizeChanged(object sender, EventArgs e)
        {
            //this.Width = 642;
            this.listView1.Width = this.Width - 30;
            this.listView1.Height = this.Height - 130;
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }
        //private void RemoveAll()
        //{
        //    System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        //  //  con.ConnectionString = Form1.SQLConnectionString;
        //    System.Data.SqlClient.SqlDataReader myReader = null;
        //    string comm = "SELECT * FROM [MessageSchedule]"; 
        //    con.Open();
        //    System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(comm, con);
        //    myReader = myCommand.ExecuteReader();

        //    string str;
        //    int i = 0;
        //    while (myReader.Read())
        //    {
        //        str = myReader["FKUnit_ID"].ToString();
        //        if (listView1.Items[i].Checked == true)
        //        {
        //            for (int co = 1; co <= 5; co++)
        //            {
        //                RemoveMessage(str, "message" + co.ToString());
        //            }
        //        } i++;
        //    }
        //    con.Close();
        //    UpdateScheduleGrid();
        //}
        //public void RemoveMesageFromQuerry(string id)
        //{
        //    System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        //   // con.ConnectionString = Form1.SQLConnectionString;
        //    System.Data.SqlClient.SqlDataReader myReader = null;
        //    string comm = "SELECT * FROM [MessageSchedule] WHERE FKUnit_ID=" +"'" +id+"'"; 
        //     con.Open();
        //    System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(comm, con);
        //    myReader = myCommand.ExecuteReader();


        //    string[] str = new string[6];

        //    while (myReader.Read())
        //    {

        //        str[0] = myReader["FKUnit_ID"].ToString();
        //        str[1] = myReader["message1"].ToString();
        //        str[2] = myReader["message2"].ToString();
        //        str[3] = myReader["message3"].ToString();
        //        str[4] = myReader["message4"].ToString();
        //        str[5] = myReader["message5"].ToString();

        //    }

        //    myReader.Close();
        //    myReader.Dispose();
        //    for (int i = 1; i <= str.Length - 1; i++)
        //    {
        //        if (str[1] == "")
        //        {
        //            break;
        //        }else
        //        if (i == 5)
        //        {
        //            string comm2 = "UPDATE [MessageSchedule] SET message" + i.ToString() + "=NULL WHERE FKUnit_ID = " + id;
        //            myCommand = new System.Data.SqlClient.SqlCommand(comm2, con);
        //            myCommand.ExecuteNonQuery();

        //        }else
        //        if (str[i] != "")
        //        {
                    
        //            string comm1 = "UPDATE [MessageSchedule] SET message" + i.ToString() + "= " + "'" + str[i+1]  + "'" + "WHERE FKUnit_ID = " + id;
        //            myCommand = new System.Data.SqlClient.SqlCommand(comm1, con);
        //            myCommand.ExecuteNonQuery();
                  
                   
        //        }
        //    }
        //    myCommand.Dispose();
        ////   UpdateScheduleGrid();
        //     //con.Close();

           
        //}
        ////private void addMessageToAll()
        ////{
        ////    System.Data.SqlClient.SqlDataReader myReader1 = null;
        ////    if (this.textBox_ScheduleMessage.Text != "")
        ////    {
        ////        System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        ////     //   con.ConnectionString = Form1.SQLConnectionString;

        ////        string Message,text;
        ////        text = this.textBox_ScheduleMessage.Text;

               


        ////        int i = 0;

        ////        string comm1 = "SELECT * FROM [MessageSchedule] "; // "INSERT INTO [Prov] (pr)" + "Values(" + count.ToString() + ")";   UPDATE [Prov] SET pr=
        ////        con.Open();
        ////        System.Data.SqlClient.SqlCommand myCommand1 = new System.Data.SqlClient.SqlCommand(comm1, con);

        ////        myReader1 = myCommand1.ExecuteReader();

        ////        string[] str1 = new string[6];

        ////        while (myReader1.Read())
        ////        {
                    

        ////            str1[0] = myReader1["FKUnit_ID"].ToString();
        ////            str1[1] = myReader1["message1"].ToString();
        ////            str1[2] = myReader1["message2"].ToString();
        ////            str1[3] = myReader1["message3"].ToString();
        ////            str1[4] = myReader1["message4"].ToString();
        ////            str1[5] = myReader1["message5"].ToString();

        ////            if (listView1.Items[i].Checked == true)
        ////            {
        ////                for (int ind = 1; ind <= str1.Length - 1; ind++)
        ////                {
        ////                    if (str1[ind] == "")
        ////                    {
        ////                        if (text.Length >= 8)
        ////                        {
        ////                            if (text.Substring(0, 8) == "$ST+UNPM" && str1[0].Substring(0, 4) == "1010")
        ////                            {
        ////                                //MessageBox.Show("lol");
        ////                                Message = text.Substring(0, 14) + str1[0] + text.Substring(3 + str1[0].Length);
        ////                                //MessageBox.Show(Message);
        ////                            }
        ////                            else
        ////                                Message = text;
        ////                        }
        ////                        else
        ////                            Message = text;


        ////                        System.Data.SqlClient.SqlConnection con2 = new System.Data.SqlClient.SqlConnection();
        ////                     //   con2.ConnectionString = Form1.SQLConnectionString; //"Data Source=IKA-FDF3AA55734\\SQLEXPRESS;Initial Catalog=Point;Persist Security Info=True;User ID=sa;Password=12345";
        ////                        string comm2 = "UPDATE [MessageSchedule] SET message" + ind.ToString() + "= " + "'" + Message + "'" + "WHERE FKUnit_ID=" + "'"+ str1[0]+"'";
        ////                        System.Data.SqlClient.SqlCommand myCommand2 = new System.Data.SqlClient.SqlCommand(comm2, con2);
        ////                        con2.Open();
        ////                        myCommand2.ExecuteNonQuery();
 
        ////                        con2.Close();
        ////                        break;
        ////                    }

        ////                }
        ////            }
        ////            i++;


        ////        }


        ////        con.Close();
        ////        myReader1.Close();
        ////        UpdateScheduleGrid();




        ////    }
            


        ////}
        ////private void addMessageToSend(string id)
        ////{
        ////    if (this.textBox_ScheduleMessage.Text != "")
        ////    {
        ////        //System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        ////        //con.ConnectionString = Form1.SQLConnectionString;
        ////        System.Data.SqlClient.SqlDataReader myReader1 = null;

        ////        string comm1 = "SELECT * FROM [MessageSchedule] WHERE FKUnit_ID=" + id; // "INSERT INTO [Prov] (pr)" + "Values(" + count.ToString() + ")";   UPDATE [Prov] SET pr=
        ////       //  //con.Open();
        ////        System.Data.SqlClient.SqlCommand myCommand1 = new System.Data.SqlClient.SqlCommand(comm1, con);
        ////        myReader1 = myCommand1.ExecuteReader();

        ////        string Message, subStrText;
        ////        subStrText = this.textBox_ScheduleMessage.Text;
        ////        string[] str = new string[6];

        ////        while (myReader1.Read())
        ////        {

        ////            str[0] = myReader1["FKUnit_ID"].ToString();
        ////            str[1] = myReader1["message1"].ToString();
        ////            str[2] = myReader1["message2"].ToString();
        ////            str[3] = myReader1["message3"].ToString();
        ////            str[4] = myReader1["message4"].ToString();
        ////            str[5] = myReader1["message5"].ToString();

        ////        }

        ////        myReader1.Close();
        ////        for (int i = 1; i <= str.Length - 1; i++)
        ////        {
        ////            if (str[i] == "")
        ////            {
        ////                if (subStrText.Substring(0, 8) == "$ST+UNPM")
        ////                {
        ////                    //MessageBox.Show("lol");
        ////                    Message = subStrText.Substring(0, 14) + id + subStrText.Substring(3 + id.Length);
        ////                    //MessageBox.Show(Message);
        ////                }
        ////                else
        ////                    Message = subStrText;

        ////                string comm2 = "UPDATE [MessageSchedule] SET message" + i.ToString() + "= " + "'" + this.textBox_ScheduleMessage.Text.ToString() + "'" + "WHERE FKUnit_ID = " + id;
        ////                myCommand1 = new System.Data.SqlClient.SqlCommand(comm2, con);
        ////                myCommand1.ExecuteNonQuery();
        ////                //MessageBox.Show(str[i]);
        ////                break;
        ////            }
        ////        }

        ////         //con.Close();

        ////        UpdateScheduleGrid();
        ////    }


        ////}
        //public void addMessageToSend(string id, string message)
        //{
        //    //if (this.textBox_ScheduleMessage.Text != "")
        //    {
        //        //System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        //        System.Data.SqlClient.SqlCommand myCommand;
        //        System.Data.SqlClient.SqlDataReader myReader = null;
        //        try
        //        {

        //            /////System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        //            //con.ConnectionString = Form1.SQLConnectionString;
                   
        //            string comm = "SELECT * FROM [MessageSchedule] WHERE FKUnit_ID=" + id; // "INSERT INTO [Prov] (pr)" + "Values(" + count.ToString() + ")";   UPDATE [Prov] SET pr=
        //             //con.Open();
        //            myCommand = new System.Data.SqlClient.SqlCommand(comm, con);
        //            myReader = myCommand.ExecuteReader();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }

        //        string[] str = new string[6];

                
        //        while (myReader.Read())
        //        {

        //            str[0] = myReader["FKUnit_ID"].ToString();
        //            str[1] = myReader["message1"].ToString();
        //            str[2] = myReader["message2"].ToString();
        //            str[3] = myReader["message3"].ToString();
        //            str[4] = myReader["message4"].ToString();
        //            str[5] = myReader["message5"].ToString();

        //        }

        //        myReader.Close();
        //        for (int i = 1; i <= str.Length - 1; i++)
        //        {
        //            if (str[i] == "")
        //            {
                        
        //                string comm1 = "UPDATE [MessageSchedule] SET message" + i.ToString() + "= " + "'" + message + "'" + "WHERE FKUnit_ID = " + id;
        //                myCommand = new System.Data.SqlClient.SqlCommand(comm1, con);
        //                myCommand.ExecuteNonQuery();
        //                //MessageBox.Show(str[i]);
        //                break;
        //            }
        //        }

        //         //con.Close();

        //        UpdateScheduleGrid();
        //    }


        //}
        //private void RemoveMessage(string id, string mesNo)
        //{
        //    System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection();
        //  //  con.ConnectionString = Form1.SQLConnectionString;
        //    string comm = "UPDATE [MessageSchedule] SET " + mesNo + "=NULL WHERE FKUnit_ID="+ id; 
        //    con.Open();
        //    System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(comm, con);
        //    myCommand.ExecuteNonQuery();
        //    con.Close();
        //    myCommand.Dispose();

        //  //    UpdateScheduleGrid
        //}
        ////private void button_scheduleMessage_Click(object sender, EventArgs e)
        ////{
        ////    addMessageToAll();
        ////    this.textBox_ScheduleMessage.Text = "";
        ////}
        private void button_select_Click(object sender, EventArgs e)
        {
            CheckAll();
        }
        private void button_deselect_Click(object sender, EventArgs e)
        {
            UncheckAll();
        }
        private void button2_Click(object sender, EventArgs e)
        {
          //  ImportIDBase impB = new ImportIDBase();
            //impB.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {

         //   AddNewID newID = new AddNewID();
            //newID.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            AddPlate ad = new AddPlate(DatabaseConnector);
            ad.Show();
          //  AddNewID AddID = new AddNewID();
           // AddID.Show();
        }//

       
    }
}
