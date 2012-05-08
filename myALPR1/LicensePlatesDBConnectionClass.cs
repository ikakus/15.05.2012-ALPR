using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;


namespace myALPR1

{
    public class LicensePlatesDBConnectionClass
    {
        private System.Data.SqlClient.SqlConnection SQLConnection;

        private string connectionStr;
        public LicensePlatesDBConnectionClass(string connStr)
        {
            SQLConnection = new System.Data.SqlClient.SqlConnection();
            connectionStr = connStr;

            SQLConnection.ConnectionString = connectionStr;
            try
            {

                SQLConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string getConnectionString()
        {
            return connectionStr;
        }

        public LicensePlatesDBConnectionClass(string ServerName,string  DatabaseNAme,string PSI, string UserID, string Password)
        {
           SQLConnection = new System.Data.SqlClient.SqlConnection();
           connectionStr = "Data Source=" + ServerName +
                   "\\SQLEXPRESS;Initial Catalog=" + DatabaseNAme +
                   ";Persist Security Info=" + PSI +
                   ";User ID=" + UserID +
                   ";Password=" + Password;

           SQLConnection.ConnectionString = connectionStr;
           SQLConnection.Open();
        }

        public void Connect()
        {
            try
            {
                SQLConnection.Open();
                
                MessageBox.Show("Connected");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                if (SQLConnection != null)
                {
                    SQLConnection.Dispose();
                }
            }

            
        }

        public int Search(string Plate)
        {
            if (SQLConnection.State == System.Data.ConnectionState.Open)
            {
                string[] str = new string[3];
                using (SqlCommand command = new SqlCommand("SELECT Id FROM Plates WHERE LicenseNumber =   @Ident", SQLConnection))
                {
                    //
                    // Add new SqlParameter to the command.
                    //
                    command.Parameters.Add(new SqlParameter("Ident", Plate));
                    //
                    // Read in the SELECT results.
                    //
                    SqlDataReader myReader = command.ExecuteReader();
                    while (myReader.Read())
                    {

                        str[0] = myReader["ID"].ToString();
                        //str[1] = myReader["LIcenseNumber"].ToString();
                       // str[2] = myReader["Owner"].ToString();

                    }
                    myReader.Dispose();

                    if (str[0] != "")
                    {
                       // MessageBox.Show(str[0]);
                        return Convert.ToInt32(str[0]);
                    }
                    else return -1;
                }
            }
            else return -2;
        }

        public void AddPlate(int id, string LicenseNum, string Owner)
        {
            using (SqlCommand command = new SqlCommand("INSERT INTO Plates (ID, LicenseNumber, Owner)  VALUES (@ID, @LicenseNumber, @Owner)", SQLConnection))
            {
                command.Parameters.Add(new SqlParameter("ID", id.ToString()));
                command.Parameters.Add(new SqlParameter("LicenseNumber", LicenseNum));
                command.Parameters.Add(new SqlParameter("Owner", Owner));


                command.ExecuteNonQuery();
            }
        }   

        public void UpdatePlateByID(int ID,string LicenseNum, string Owner)
        {
            using (SqlCommand command = new SqlCommand("UPDATE Plates SET LicenseNumber= @LicenseNumber, Owner= @Owner  WHERE Id =" + ID.ToString(), SQLConnection))
            {
                
                command.Parameters.Add(new SqlParameter("LicenseNumber", LicenseNum));
                command.Parameters.Add(new SqlParameter("Owner", Owner));


                command.ExecuteNonQuery();
            }
        }

        public void DeleteByID(int ID)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM Plates WHERE Id =  " + ID.ToString(), SQLConnection))
            {
            command.ExecuteNonQuery();
            }
        }

        public void DeleteByIDFromLog(int ID)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM PlatesLog WHERE Id =  " + ID.ToString(), SQLConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public string GetInfoById(int Id)
        {
            string str= "";
                using (SqlCommand command = new SqlCommand("SELECT Owner FROM Plates WHERE Id =   @Ident", SQLConnection))
                {
                    command.Parameters.Add(new SqlParameter("Ident", Id.ToString()));
                    
                    SqlDataReader myReader = command.ExecuteReader();
                    while (myReader.Read())
                    {

                       
                         str = myReader["Owner"].ToString();

                    }
                    myReader.Dispose();
                    return str;
                }
            
        }



        private byte[] BmpToBytes_MemStream(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            // Save to memory using the Jpeg format
            bmp.Save(ms, ImageFormat.Jpeg);

            // read to end
            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();

            return bmpBytes;
        }


        private Image BytesToImg(byte[] bmpBytes)
        {
            MemoryStream ms = new MemoryStream(bmpBytes);
            Image img = Image.FromStream(ms);
            // Do NOT close the stream!

            return img;
        }

        public Image GetPictureFromBaase(int ID)
        {
            try
            {

                using (SqlCommand command = new SqlCommand("select Picture from PlatesLog where ID=@ID", SQLConnection))
                {
                    command.Parameters.Add(new SqlParameter("ID", ID.ToString()));



                    byte[] barrImg = (byte[])command.ExecuteScalar();
                    return BytesToImg(barrImg);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        public void StorePictureToLogBase(int ID, Bitmap bitmap, string Plate, DateTime Date)
        {
            try
            {
                byte[] Image = BmpToBytes_MemStream(bitmap);

                using (SqlCommand command = new SqlCommand("INSERT INTO PlatesLog (ID,Picture, LicensePlate, DateTime)  VALUES (@id,@Bmp, @LPlate, @Date)", SQLConnection))
                {
                    command.Parameters.Add(new SqlParameter("id", ID.ToString()));
                    command.Parameters.Add(new SqlParameter("Bmp", Image));
                    command.Parameters.Add(new SqlParameter("LPlate", Plate));
                    command.Parameters.Add(new SqlParameter("Date", Date));


                    command.ExecuteNonQuery();
                }
                //  this.sqlConnection1.Open();
                //if (sqlCommand1.Parameters.Count == 0)
                //{
                //    this.sqlCommand1.CommandText = "INSERT INTO tblImgData(ID,Name,Picture) values(@ID,@Name,@Picture)";
                //    this.sqlCommand1.Parameters.Add("@ID", System.Data.SqlDbType.Int, 4);
                //    this.sqlCommand1.Parameters.Add("@Name", System.Data.SqlDbType.VarChar, 50);
                //    this.sqlCommand1.Parameters.Add("@Picture", System.Data.SqlDbType.Image);
                //}
                //this.sqlCommand1.Parameters["@ID"].Value = this.editID.Text;
                //this.sqlCommand1.Parameters["@Name"].Value = this.editName.Text;
                //this.sqlCommand1.Parameters["@Picture"].Value = this.m_barrImg;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int getRowsCount()
        {
            try
            {
                System.Data.SqlClient.SqlDataReader myReader = null;

                string comm = "SELECT * FROM [PlatesLog]";
                // con.Open();
                int count = 0;
                System.Data.SqlClient.SqlCommand myCommand = new System.Data.SqlClient.SqlCommand(comm, SQLConnection);
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {

                    count++;


                }

                myReader.Dispose();

                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
            
            //con.Close();

        }
    }
}
