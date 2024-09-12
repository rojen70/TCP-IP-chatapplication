using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

//reference: https://github.com/AbleOpus/NetworkingSamples/blob/master/MultiClient/Program.cs
namespace Windows_Forms_Chat
{
    public class TCPChatClient : TCPChatBase
    {
        //public static TCPChatClient tcpChatClient;
        public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public ClientSocket clientSocket = new ClientSocket();
        public TicTacToe game = new TicTacToe();

        public int serverPort;
        public string serverIP;


        public static TCPChatClient CreateInstance(int port, int serverPort, string serverIP, TextBox chatTextBox)
        {
            TCPChatClient tcp = null;
            //if port values are valid and ip worth attempting to join
            if (port > 0 && port < 65535 && 
                serverPort > 0 && serverPort < 65535 && 
                serverIP.Length > 0 &&
                chatTextBox != null)
            {
                tcp = new TCPChatClient();
                tcp.port = port;
                tcp.serverPort = serverPort;
                tcp.serverIP = serverIP;
                tcp.chatTextBox = chatTextBox;
                tcp.clientSocket.socket = tcp.socket;
             

            }

            return tcp;
        }

        public void ConnectToServer()
        {
            int attempts = 0;

            while (!socket.Connected)
            {
                try
                {
                    attempts++;
                    SetChat("Connection attempt " + attempts);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    socket.Connect(serverIP, serverPort);
                }
                catch (SocketException)
                {
                    chatTextBox.Text = "";
                }
            }

            //Console.Clear();
            AddToChat("Connected");
            //keep open thread for receiving data
            clientSocket.socket.BeginReceive(clientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, clientSocket);
        }

        public void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
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
                return;
            }
            //read bytes from packet
            byte[] recBuf = new byte[received];
            Array.Copy(currentClientSocket.buffer, recBuf, received);
            //convert to string so we can work with it
            string text = Encoding.ASCII.GetString(recBuf);
            Console.WriteLine("Received Text: " + text);
            // tokenize the string into an string array by spaces
            string[] tokens = text.Split(' ');
            // tokenize the string into an array of size 3 by spaces
            string[] game_tokens = text.Split(new[] { ' ' }, 3);
            // setup player 1
            if (text == "!SetupPlayer1")
            {
                
                game.myTurn = true;
                game.playerTileType = TileType.cross;
                AddToChat("You are X");
            }
            // setup player 2
            else if (text == "!SetupPlayer2")
            {
               
                game.myTurn = true;
                game.playerTileType = TileType.naught;
                AddToChat("You are O");
            }
            // change turn of player to true
            else if (text== "!Set_Player_Turn_True")
            {
                game.myTurn = true;
                AddToChat("Your Turn");
            }
            // check if there are at least 2 string in the game_tokens 
            else if (game_tokens.Length >= 2)
            {
                // check if the second string is !UpdateGrid
                if(game_tokens[1] == "!UpdateGrid")
                {
                    game.StringToGrid(game_tokens[2]); // convert the string to the grid of the game
                    // check the state of game for possible win loss or draw
                    GameState gs = game.GetGameState(); 
                    if (gs == GameState.crossWins)
                    {
                        chatTextBox.AppendText("X wins!");
                        chatTextBox.AppendText(Environment.NewLine);
                        game.ResetBoard();
                        if (game.playerTileType == TileType.cross)
                        {
                            SendString("!UpdateWins");
                        }
                        else
                        {
                            SendString("!UpdateLosses");
                        }
                                               
                    }
                    if (gs == GameState.naughtWins)
                    {
                        chatTextBox.AppendText("O wins!");
                        chatTextBox.AppendText(Environment.NewLine);
                        game.ResetBoard();
                        if (game.playerTileType == TileType.naught)
                        {
                            SendString("!UpdateWins");

                        }
                        else
                        {
                            SendString("!UpdateLosses");
                        }
                    }
                    if (gs == GameState.draw)
                    {
                        chatTextBox.AppendText("Draw!");
                        chatTextBox.AppendText(Environment.NewLine);
                        game.ResetBoard();
                        SendString("!UpdateDraws");
                    }

                }
                else
                {

                    AddToChat(text);
                }
            }
            

            
            else
            {
                //text is from server but could have been broadcast from the other clients
                AddToChat(text);
                
            }
            
            //we just received a message from this socket, better keep an ear out with another thread for the next one
            currentClientSocket.socket.BeginReceive(currentClientSocket.buffer, 0, ClientSocket.BUFFER_SIZE, SocketFlags.None, ReceiveCallback, currentClientSocket);
        }
        public void Close()
        {
            socket.Close();
        }

        public void setBoard( ref TicTacToe board)
        {
            game = board;
        }
    }

}
