using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace lab12_serwer
{
    internal class Listener
    {
        ConcurrentQueue<(double, double)> queue;
        IPEndPoint remoteEP;
        UdpClient udpServer;
        byte clientID;
        public Listener(UdpClient udpServer, ConcurrentQueue<(double, double)> queue,int port, IPEndPoint lastPort, byte id)
        {
            this.queue = queue;
            remoteEP = new IPEndPoint(IPAddress.Any, port);
            byte[] bytePort = BitConverter.GetBytes(port);

            udpServer.Send(bytePort, bytePort.Length, lastPort); // reply new port
            udpServer.Send(BitConverter.GetBytes(id), 1, lastPort); // reply id
            this.udpServer = new UdpClient(port);

            clientID = id;

            Console.WriteLine("client new port:" + port + " id: "+id);


           
        }

        public void Listen()
        {
            while (true)
            {
                var data = udpServer.Receive(ref remoteEP);

                double x = BitConverter.ToDouble(data, 0);
                double y = BitConverter.ToDouble(data, 8);

                queue.Append((x, y));
                

            }
            
        }
    }
}
