using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Windows_Forms_Chat;

namespace Windows_Forms_CORE_CHAT_UGH
{
    public partial class Form2 : Form
    {
        SQLiteConnection con = new SQLiteConnection("Data Source= Users.db;vesion=3;New=True;Compress=True;");
        SQLiteCommand cmd;
        SQLiteDataReader reader;
        public Form2()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, EventArgs e)
        {
            // check if fields are empty
            if (Username.Text != "" && Password.Text != "")
            {
                //check if the chosen username already exists
                int v = Check_forduplicate(Username.Text);
                if (v != 1)
                {
                    con.Open();
                    string command = "INSERT INTO Members(Username, Password) VALUES (@username,@password)";
                    cmd = con.CreateCommand();
                    cmd.CommandText = command;
                    cmd.Parameters.AddWithValue("@username", Username.Text);
                    cmd.Parameters.AddWithValue("@password", Password.Text);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("New User registerd Successfully");
                    Username.Text = "";
                    Password.Text = "";

                }
                else
                {
                    MessageBox.Show("User already Registered!!");
                    Username.Text = "";
                    Password.Text = "";

                }
            }
            else
            {
                MessageBox.Show("Fields are empty!!");
                Username.Text = "";
                Password.Text = "";

            }
        }
        // function to check for duplicate username
        public int Check_forduplicate(string username)
        {
            con.Open();
            string command = "SELECT COUNT (*) FROM Members WHERE Username='" + username + "'";
            cmd = con.CreateCommand();
            cmd.CommandText = command;
            int v = (int)(long)cmd.ExecuteScalar();
            con.Close();

            return v;
        }
        // if Back to home button is pressed
        private void BackToHome_Click(object sender, EventArgs e)
        {
            this.Hide(); // hide this form
            Form1 f1 = new Form1(); // create new form 1
            f1.Show(); // show new form
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
