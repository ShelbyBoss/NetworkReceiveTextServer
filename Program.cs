using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkReceiveTextServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = new IPAddress(new byte[] { 192, 168, 1, 224 });
            TcpListener serverSocket = new TcpListener(ip, 8888);
            int requestCount = 0;
            TcpClient clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");

            while (!Console.KeyAvailable)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> Accept connection from client");
                requestCount = 0;


                try
                {
                    requestCount = requestCount + 1;
                    NetworkStream networkStream = clientSocket.GetStream();
                    byte[] bytesFrom = new byte[10025];
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);

                    Console.WriteLine("ASCII:" + ToString(Encoding.ASCII, bytesFrom));
                    Console.WriteLine("BEU  :" + ToString(Encoding.BigEndianUnicode, bytesFrom));
                    Console.WriteLine("Defau:" + ToString(Encoding.Default, bytesFrom));
                    Console.WriteLine("Unico:" + ToString(Encoding.Unicode, bytesFrom));
                    Console.WriteLine("UTF32:" + ToString(Encoding.UTF32, bytesFrom));
                    Console.WriteLine("UTF7 :" + ToString(Encoding.UTF7, bytesFrom));
                    Console.WriteLine("UTF8 :" + ToString(Encoding.UTF8, bytesFrom));

                    string serverResponse = "Last Message from client: "+DateTime.Now.Millisecond;
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine(" >> " + serverResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                clientSocket.Close();
            }

            serverSocket.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }

        private static string ToString(Encoding encoding, byte[] bytes)
        {
            string dataFromClient = encoding.GetString(bytes);
            return new string(dataFromClient.Where(c => c != '\0').ToArray());
        }
    }
}
