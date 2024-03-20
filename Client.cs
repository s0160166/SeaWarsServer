using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace SeaWars
{
    class Client
    {
        TcpClient client;
        string messageToSend;
        public Client(string messageToSend)
        {
            TcpClient client = new TcpClient("127.0.0.1", 1302);
            this.client = client;
            this.messageToSend = messageToSend;
        }
        public string Send() {
            try
            {
                int byteCount = Encoding.ASCII.GetByteCount(this.messageToSend + 1);
                byte[] sendData = Encoding.ASCII.GetBytes(this.messageToSend);
                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
                Console.WriteLine("sending data to server...");

                StreamReader sr = new StreamReader(stream);
                string response = sr.ReadLine();
                Console.WriteLine("Responce get: "+response);

                stream.Close();
                client.Close();
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to connect...");
                Console.WriteLine(e.ToString());
                return "Error";
            }
        }
    }
}