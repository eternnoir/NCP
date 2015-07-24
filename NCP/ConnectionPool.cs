using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCP
{
    public class ConnectionPool
    {
        private int maxConnectionNumber = 4;
        private int minConnectionNumber = 1;
        private string hostAddress = string.Empty;
        private int hostPort = 0;
        private int socketNumber = 0;
        private Queue<NcpClient> socketsQueue =  new Queue<NcpClient>();

        public ConnectionPool(string hostIPAddress, int hostPort, int minConnections, int maxConnections)
        {
            if (minConnectionNumber > maxConnectionNumber)
            {
                //TODO Throw Exception
            }
            this.hostAddress = hostIPAddress;
            this.hostPort = hostPort; 
            this.maxConnectionNumber = maxConnections;
            this.minConnectionNumber = minConnections;
            this.InitConnections();
        }

        public NcpClient GetSocket()
        {
            lock (this.socketsQueue)
            {
                NcpClient socket = null;
                while (this.socketsQueue.Count > 0)
                {
                    socket = this.socketsQueue.Dequeue();
                    if (socket.Connected)
                    {
                        return socket;
                    }
                    else
                    {
                        socket.Close();
                        System.Threading.Interlocked.Decrement(ref socketNumber);
                    }
                }
            }
            return CreateSocket();
        }

        public void PutSocket(NcpClient socket)
        {
            lock (this.socketsQueue)
            {
                if (this.socketsQueue.Count < this.maxConnectionNumber)
                {
                    if (socket != null)
                    {
                        if (socket.Connected)
                        {
                            this.socketsQueue.Enqueue(socket);
                        }
                        else
                        {
                            socket.Close();
                        }
                    }
                }
                else
                {
                    socket.Close();
                }
            }
        }

        private void InitConnections()
        {
            for (int i = 0; i < minConnectionNumber; i++)
            {
                var socket = CreateSocket();
                this.PutSocket(socket);
            }
        }

        private NcpClient CreateSocket()
        {
            if (socketNumber <= maxConnectionNumber)
            {
                System.Threading.Interlocked.Increment(ref socketNumber);
                return new NcpClient(this.hostAddress, this.hostPort);
            }
            throw new Exception("Connection Pool reached its limit");
        }
    }
}
