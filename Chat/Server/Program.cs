using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private static List<User> users = new List<User>();

        static void serv(NetworkStream stream, TcpClient client)
        {
            StringBuilder name = new StringBuilder();

            byte[] dataread = new byte[256];

            do
            {
                int bytes = stream.Read(dataread, 0, dataread.Length);
                name.Append(Encoding.UTF8.GetString(dataread, 0, bytes));
            }
            while (stream.DataAvailable);

            char[] charSeparators = new char[] { ' ' };

            var spltd = name.ToString().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(name);

            IPEndPoint ipEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            Console.WriteLine("Cleients point" + ipEndPoint.ToString());
            bool found = false;
            User founded = null;

            lock (users)
            {
                foreach (var str in users)
                {
                    if (spltd[0] == str.to && spltd[2] == str.me)
                    {
                        byte[] datawrite = new byte[256];
                        datawrite = Encoding.UTF8.GetBytes(str.IP);
                        stream.Write(datawrite, 0, datawrite.Length);
                        datawrite = Encoding.UTF8.GetBytes(ipEndPoint.ToString());
                        str.stream.Write(datawrite, 0, datawrite.Length);
                        found = true;
                        founded = str;
                    }
                }
                if (!found)
                {
                    users.Add(new User(spltd[0], spltd[2], ipEndPoint.ToString(),stream));
                }
                else
                {
                    users.Remove(founded);
                }

            }
        }

        static void Main(string[] args)
        {
            
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            int port = 3000;
            TcpListener server = new TcpListener(localAddr,port);
            server.Start();
            while (true)
            {
                Console.WriteLine("Waiting for connects ");

                // Connecting
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected ");

                // stream for reading and writing
                NetworkStream stream = client.GetStream();
                

                Task.Run(() => { serv(stream, client); });
                
            }
        }
    }
}
