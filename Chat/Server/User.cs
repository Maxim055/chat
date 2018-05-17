using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class User
    {
        public string me;
        public string to;
        public string IP;
        public NetworkStream stream;

        public User(string me, string to, string ip, NetworkStream str)
        {
            this.me = me;
            this.to = to;
            this.stream = str;
            this.IP = ip;
        }
    }
}
