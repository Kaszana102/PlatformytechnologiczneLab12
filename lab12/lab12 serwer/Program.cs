using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;



namespace lab12_serwer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Server turned on!");

            UdpClient udpServer = new UdpClient(11000);
            ConcurrentQueue<(double,double)> queue = new ConcurrentQueue<(double, double)>();

            int clientPort = 11001;
            byte clientID = 0;

            //utworz watek propagujacy

            //

            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, 11000);
                var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                if (data.Length == 1 && data[0] == 1)
                {
                    Console.WriteLine("new connection!");
                    Task listenTask = Task.Factory.StartNew(() =>
                    {                        
                        Listener listener = new Listener(udpServer, queue, clientPort, remoteEP,clientID);
                        listener.Listen();
                    });

                    //dodaj do watku propagujacego
                    //port i id


                    clientPort++;
                    clientID++;

                }


                //udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
            }
        }
    }
}
