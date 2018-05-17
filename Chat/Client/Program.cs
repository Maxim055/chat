using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static Random rnd = new Random();
        private static int port1 = 3000;
        private const string server = "127.0.0.1";
        public static IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public static async void receive(UdpClient udpClient)
        {
            while (true)
            {
                await udpClient.ReceiveAsync().ContinueWith(t => Console.WriteLine(Encoding.ASCII.GetString(t.Result.Buffer)));
            }
        }
        
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient();

            client.Connect(IPAddress.Parse(server), port1);
            IPEndPoint ipEndPoint = (IPEndPoint)client.Client.LocalEndPoint;
            var ipAddress = ipEndPoint.Address;
            Console.WriteLine("Cleients Port" + ipEndPoint.ToString());

            byte[] dataread = new byte[256];
            byte[] datawrite = new byte[256];
            StringBuilder response = new StringBuilder();
            NetworkStream stream = client.GetStream();

            string me = Console.ReadLine();
            
            datawrite = Encoding.UTF8.GetBytes(me);
            stream.Write(datawrite, 0, datawrite.Length);
            
            do
            {
                int bytes = stream.Read(dataread, 0, dataread.Length);
                response.Append(Encoding.UTF8.GetString(dataread, 0, bytes));
            }
            while (stream.DataAvailable); // while data is available  
            stream.Close();
            Console.WriteLine(response.ToString());
            client.Close();

            var ipandport = response.ToString().Split(':');
            IPAddress ip = IPAddress.Parse(ipandport[0]);
            int port;
            int.TryParse(ipandport[1], out port);

            UdpClient udpClient = new UdpClient(ipEndPoint);
            
            /*udpClient.ReceiveAsync().ContinueWith(t => Console.WriteLine(Encoding.ASCII.GetString(t.Result.Buffer)));
            Byte[] sendBytes = Encoding.ASCII.GetBytes(Console.ReadLine());
            udpClient.SendAsync(sendBytes, sendBytes.Length, new IPEndPoint(ip, port));*/
            Thread f = new Thread(() => { receive(udpClient); });
            
            f.IsBackground = false;
            
            f.Start();
            //s.Start();

            while (true)
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(Console.ReadLine());
                udpClient.SendAsync(sendBytes, sendBytes.Length, new IPEndPoint(ip, port));
            }
        }
    }
}
