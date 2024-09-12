using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Windows_Forms_CORE_CHAT_UGH
{
    public class database
    {
        // variables for connection, command and read
        SQLiteConnection con;
        SQLiteCommand cmd;
        SQLiteDataReader reader;
        public database()
        {
            //create new connection to database and create the database if it doesn't exist
            con = new SQLiteConnection("Data Source=D:\\New folder\\Windows Forms core chat\\bin\\Debug\\netcoreapp3.1\\Users.db;vesion=3;New=True;Compress=True;");
            //Open database
            con.Open();
            cmd = con.CreateCommand();
            //Create Table
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Members( ID INTEGER PRIMARY KEY AUTOINCREMENT,Username VARCHAR(255),Password VARCHAR(20),Wins INT ,Losses INT ,Draws INT);";
            cmd.ExecuteNonQuery();
            
           

        }
        // Insert data into the table
        public void Insert_into_Db(SQLiteConnection con, string command)
        {
            
            cmd = con.CreateCommand(); 
            cmd.CommandText = command; 
            cmd.ExecuteNonQuery();
            
        }
        // check if the username exists already in the database
        public bool checkuser(string name)
        {
            //con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Username FROM Members";
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string current_read=reader.GetString(0);
                if (current_read == name)
                {
                    con.Close();
                    return true;    
                }
            }
            //con.Close();
            return false;
        }
        // update the username in the database to a new one
        public void update_user( string i_name,string f_name)
        {
            
            cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Members SET Username=@fname WHERE Username=@iname";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@fname", f_name);
            cmd.Parameters.AddWithValue("iname", i_name);
            cmd.ExecuteNonQuery();
            
        }
        // increment wins of a particular user by 1
        public void update_wins(string name)
        {
            //con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Members SET Wins = COALESCE(Wins,0) + 1 WHERE Username=@name";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@name", name);
            
            cmd.ExecuteNonQuery();
            //dcon.Close();
        }
        // increment loss of a user by 1
        public void update_losses(string name)
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Members SET Losses =  COALESCE(Losses,0) + 1 WHERE Username=@name";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@name", name);

            cmd.ExecuteNonQuery();
           
        }
        // increment draws of a user in table by 1
        public void update_draws(string name)
        {
      
            cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE Members SET Draws =  COALESCE(Draws,0) + 1 WHERE Username=@name";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@name", name);

            cmd.ExecuteNonQuery();
         
        }

        
    }
}
