﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NCP
{
    public class NcpClient : TcpClient
    {

        public NcpClient(string host, int port)
            : base(host, port)
        {
        }
    }
}
