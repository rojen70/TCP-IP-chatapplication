using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using System.Linq.Expressions;
using Windows_Forms_CORE_CHAT_UGH;
using System.Xml.Linq;
using System.Data.SQLite;

//https://github.com/AbleOpus/NetworkingSamples/blob/master/MultiServer/Program.cs
namespace Windows_Forms_Chat
{
    public class TCPChatServer : TCPChatBase
    {
        // create new instance of database class
        public database db = new database();
        public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //connected clients
        public List<ClientSocket> clientSockets = new List<ClientSocket>();
        public List<string> events = new List<string>(); // a list to store events happening in the server
        // List of players playing TicTacToe
        public List<ClientSocket> players =new List<ClientSocket>();
        // Creating connection for sql manipulation
        SQLiteConnection con = new SQLiteConnection("Data Source= Users.db;vesion=3;New=True;Compress=True;");
        SQLiteCommand cmd;
        SQLiteDataReader reader;
        public static TCPChatServer createInstance(int port, TextBox chatTextBox)
        {
            TCPChatServer tcp = null;
            //setup if port within range and valid chat box given
            if (port > 0 && port < 65535 && chatTextBox != null)
            {
                tcp = new TCPChatServer();
                tcp.port = port;
                tcp.chatTextBox = chatTextBox;

            }


            //return empty if user not enter useful details
            return tcp;
        }

        public void SetupServer()
        {
            AddToChat ("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(0);
            //kick off thread to read connecting clients, when one connects, it'll call out AcceptCallback function
            serverSocket.BeginAccept(AcceptCallback, this);
            AddToChat ("Server setup complete");
        }



        public void CloseAllSockets()
        {
            foreach (ClientSocket clientSocket in clientSockets)
            {
                clientSocket.socket.Shutdown(SocketShutdown.Both);
                clientSocket.socket.Close();
            }
            clientSockets.Clear();
            serverSocket.Close();
        }

        public void AcceptCallback(IAsyncResult AR)
        {
            Socket joiningSocket;

            try
            {
                joiningSocket = serverSocket.EndAccept(AR);
            }
            catch (ObjectDisposedException) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
                return;
            }

            ClientSocket newClientSocket = new ClientSocket();
            newClientSocket.socket = joiningSocket;

            clientSockets.Add(newClientSocket);
            //start a thread to listen out for this new joining socket. Therefore there is a thread open for each client
            joiningSocket.BeginReceive(newClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, newClientSocket);
            AddToChat("Client connected, waiting for request...");

            //we finished this accept thread, better kick off another so more people can join
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        public void ReceiveCallback(IAsyncResult AR)
        {
            ClientSocket currentClientSocket = (ClientSocket)AR.AsyncState;
            
            int received;

            try
            {
                received = currentClientSocket.socket.EndReceive(AR);
            }
            catch (SocketException) 
            {
                AddToChat("Client forcefully disconnected");
                // Don't shutdown because the socket may be disposed and its disconnected anyway.
                currentClientSocket.socket.Close();
                clientSockets.Remove(currentClientSocket);
                return;
            }
            catch (ObjectDisposedException) // Had to add another catch because would pop up everytime a user was kicked out
            {
                
               
                return;

            }

            byte[] recBuf = new byte[received];
            Array.Copy(currentClientSocket.buffer, recBuf, received);
            bool check=true ; // a variable to be check if the username is unique or not
            string text = Encoding.ASCII.GetString(recBuf);
            // Tokenizing the received text into an array of string on the basis of the whitespace in order to extract various parts of commands sent from user
            string[] tokens = text.Split(' ');
            // Specifically tokenizing the text with blank spaces only upto 2 string
            string[] game_token = text.Split(new[] { ' ' }, 2);
           
           AddToChat( text );
          

            if (text.ToLower() == "!commands") // Client requested time
            {
                byte[] data = Encoding.ASCII.GetBytes("Server: Commands are \r\n!commands------> Get a list of Commands \r\n" +
                                                                            "!about------->Information about the server\r\n" +
                                                                            " !who--------->List of members of the server\r\n" +
                                                                            " !whisper-----> Send a private text to a member of the server\r\n" +
                                                                            " !username-----> Set or Change username" +
                                                                            "!exit---------> Leave the server\r\n" +
                                                                            " !events--------> Display the ongoing or upcoming events");
                currentClientSocket.socket.Send(data);
                AddToChat("Commands sent to client");
            }
            else if (text.ToLower() == "!exit") // Client wants to exit gracefully
            {
                // Always Shutdown before closing
                currentClientSocket.socket.Shutdown(SocketShutdown.Both);
                currentClientSocket.socket.Close();
                clientSockets.Remove(currentClientSocket);
                AddToChat("Client disconnected");
                return;
            }
            else if (text.ToLower() == "!about")
            {
                byte[] data = Encoding.ASCII.GetBytes("Server: A simple chat system developed by Mathew and "+
                                                      "modified by Rojen");
                currentClientSocket.socket.Send(data);
                AddToChat("Sent to the Client");
            }
            else if (tokens[0].ToLower() == "!username")
            {
                check = db.checkuser(tokens[1]);
                if (check == false) // if the username is unique
                {
                    db.update_user(currentClientSocket.username, tokens[1]);
                    SendToAll(currentClientSocket.username + " changed their username to " + tokens[1], null);
                    currentClientSocket.username = tokens[1];

                    AddToChat("Sent to the Client");
                }
                else // if the username matches with any existing users, give out an error message
                {
                    byte[] data = Encoding.ASCII.GetBytes("Server: Failed to change username as it already exists");
                    currentClientSocket.socket.Send(data);
                    AddToChat("Sent to the Client");
                }
            }
            else if (text.ToLower() == "!who")
            {
                byte[] data1 = Encoding.ASCII.GetBytes("The members of the chat are:");
                currentClientSocket.socket.Send(data1);
                
                foreach (ClientSocket c in clientSockets)
                {
                    byte[] data = Encoding.ASCII.GetBytes(c.username + "\r\n");
                    currentClientSocket.socket.Send(data);
                    
                }
                AddToChat("Sent to client");
            }
            else if (tokens[0].ToLower() == "!whisper")
            {
                //merge the array of single string into one string starting from the second index of the array
                string message = string.Join("", tokens, 2, (tokens.Count() - 2));
                SendToOne(message, currentClientSocket, tokens[1]);
                AddToChat(message);
            }
            else if (tokens[0].ToLower() == "!kick")
            {
                if (currentClientSocket.is_mod == true) // check if the current client is a mod
                {
                    foreach (ClientSocket s in clientSockets)
                    {
                        if (tokens[1] == s.username) 
                        {
                            
                            SendToAll(s.username + " has been kicked from the server", null);
                       
                            clientSockets.Remove(s);
                        
                            s.socket.Shutdown(SocketShutdown.Both);
                            s.socket.Close();
                            AddToChat("Client disconnected");
                            break;

                        }
                        else // if the user does not exist
                        {
                            byte[] data = Encoding.ASCII.GetBytes("The user does not exist");
                            currentClientSocket.socket.Send(data);
                        }
                    }
                }
                
                else
                {
                    byte[] data = Encoding.ASCII.GetBytes("You do not have kicking priviledges");
                    currentClientSocket.socket.Send(data);
                    AddToChat("Sent to Client");
                }
            }
            // this would be a new feature where it displays the upcoming events
            else if (text.ToLower() == "!events")
            {
                foreach (String e in events) 
                {
                    byte[]data = Encoding.ASCII.GetBytes(e);
                    currentClientSocket.socket.Send(data);
                    AddToChat("Sent to Client");

                }
            }
            // text received from the form to store the logging username to the username of the current client socket
            else if (tokens[0] == "!user_info")
            {
                currentClientSocket.username = tokens[1];
            }
            // Either start the game or queue up for the game
            else if (tokens[0] == "!play")
            {
                if (players.Count <= 0 || players.Count <= 2) // if there are at least 2 players in the list 'players'
                {
                    players.Add(currentClientSocket);
                    byte[] player_ann = Encoding.ASCII.GetBytes("Added as a player");
                    currentClientSocket.socket.Send(player_ann);
                    // check for players in queue are a pair
                    if (players.Count == 2)
                    {
                        byte[] player1_data = Encoding.ASCII.GetBytes("!SetupPlayer1"); //setup for player 1
                        byte[] player2_data = Encoding.ASCII.GetBytes("!SetupPlayer2"); // setup for player 2
                        players[0].socket.Send(player1_data);
                        players[1].socket.Send(player2_data);
                        
                        
                        

                    }
                }
            }
            //update Wins counter in the database
            else if (text == "!UpdateWins")
            {
               
                db.update_wins(currentClientSocket.username);
                if (players != null)
                {
                    players.Clear();
                }
            }
            //update loss counter in the database
            else if (text == "!UpdateLosses")
            {
                db.update_losses(currentClientSocket.username);
                if (players != null)
                {
                    players.Clear();
                }
            }
            //update draw counter in the database
            else if (text == "!UpdateDraws")
            {
                db.update_draws(currentClientSocket.username);
                if (players != null)
                {
                    players.Clear();
                }
            }
              
                
                
            //Change the tictactoe grid to a string
            else if (tokens[0]== "!GridToString")
            {
                SendToAll("!UpdateGrid "+ game_token[1], null); // update the board for all the chat members

            }
            // Change the player turn to true
            else if (text == "!SetTurnToTrue")
            {
            
                byte[] data = Encoding.ASCII.GetBytes("!Set_Player_Turn_True");
                /* Check if the current client sent request to change player turn to true
                 * if it is true then change the next player's turn to true
                 * if it is false then change the current client's turn to true
                  */
                if ( currentClientSocket == players[0])
                {
                    players[1].socket.Send(data);
                }
                else
                {
                    players[0].socket.Send(data);
                }
            }
            // display the current scoreboard to the client in descending order based on number of wins
            else if (text.ToLower() == "!stats")
            {
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT Username,COALESCE(Wins,0),COALESCE(Losses,0),COALESCE(Draws,0) FROM Members ORDER BY Wins DESC";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string result = reader.GetValue(0).ToString() + ", " + reader.GetValue(1).ToString() + ", " + reader.GetValue(2).ToString() + ", " + reader.GetValue(3).ToString()+ "\r\n";
                    byte[] data = Encoding.ASCII.GetBytes(result);
                    currentClientSocket.socket.Send(data);
                }
            }

            else
            {
                //normal message broadcast out to all clients
                SendToAll(text, currentClientSocket);
            }
            //we just received a message from this socket, better keep an ear out with another thread for the next one
             currentClientSocket.socket.BeginReceive(currentClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, currentClientSocket);
        }

        public void SendToAll(string str, ClientSocket from)
        {
            string[] tokens = str.Split(' '); // tokenize string in order to work on commands sent by the server
            if (from == null)
            {
                if (tokens[0].ToLower() == "!event_add") // add event to the server
                {
                    string new_event = string.Join("", tokens, 1, (tokens.Count() - 1)); // extract the event from the message sent by the server
                    events.Add(new_event);
                    AddToChat("New event has been added");
                    return;
                }
                else if (tokens[0].ToLower() == "!event_remove") // remove event from the server based on its index
                {

                    //covert the index from string into integer and remove it from the array events
                    try
                    {
                        events.RemoveAt(int.Parse(tokens[1]));
                        AddToChat("Old event has been removed");
                        return;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        AddToChat("The index was not valid");
                        return;
                    }
                }
            }
                foreach (ClientSocket c in clientSockets)
                {
                // if the message was sent by the server
                if (from == null)
                {
                    if (tokens[0].ToLower() == "!mod" && tokens.Length == 1)
                    {
                        byte[] mod_list = Encoding.ASCII.GetBytes("Mods are:");
                        c.socket.Send(mod_list);
                        foreach (ClientSocket md in clientSockets)

                        {
                            if (md.is_mod == true)
                            {
                                byte[] data2 = Encoding.ASCII.GetBytes(md.username + "\r\n");
                                c.socket.Send(data2);

                            }

                        }
                    }
                    else if (tokens[0].ToLower() == "!mod")
                    {
                        if (c.username == tokens[1])
                        {
                            if (c.is_mod == false)
                            {
                                c.is_mod = true;
                                SendToAll(c.username + " has been added as a mod", null);
                            }
                        }
                        else if (c.is_mod == true)
                        {
                            c.is_mod = false;
                            SendToAll(c.username + " has been removed as a mod", null);
                        }
                    }

                    else
                    {
                        byte[] data = Encoding.ASCII.GetBytes("Server: " + str);
                        c.socket.Send(data);
                    }
                }

                else if (!from.socket.Equals(c))
                {

                    byte[] data = Encoding.ASCII.GetBytes(from.username + ": " + str);
                    c.socket.Send(data);

                }
            }
        }
        // function for the whisper feature
        
        public void SendToOne(string str, ClientSocket from, string receiver_user_name)
        {
            bool found_user = false; // to check if the user the whisper is being sent to exists
            foreach (ClientSocket c in clientSockets)
            {
                if (c.username == receiver_user_name)
                {
                    byte[] data= Encoding.ASCII.GetBytes("Private message sent by " + from.username + ": "+ str);
                    c.socket.Send(data); // sends the private message to the receiver
                    found_user = true;
                    byte[] data1 = Encoding.ASCII.GetBytes("Private message was sent");
                    from.socket.Send(data1); // sends the confirmation of the message being sent to the sender
                    break;
                }
            }
                if (found_user == false) // if the user the message is being sent to does not exist
                {
                    byte[] data2 = Encoding.ASCII.GetBytes("The user was not found");
                    from.socket.Send(data2);
                }
        }
       
        
    }
}
