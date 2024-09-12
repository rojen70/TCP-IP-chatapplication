﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Data.SQLite;

namespace Windows_Forms_Chat
{
    public class ClientSocket
    {
        //add other attributes to this, e.g username, what state the client is in etc
        public Socket socket;
        public bool is_mod = false;
        public string username;
        
        public const int BUFFER_SIZE = 2048;
        public byte[] buffer = new byte[BUFFER_SIZE];
    }
}
