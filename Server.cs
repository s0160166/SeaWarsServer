using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace SeaWars
{
    class Server
    {
        TcpListener listener;
        string strMap;
        public int[,] myMap = new int[9,9];
        public int[,] enemyMap = new int[9, 9];
        public int turn = 1;
        public Server(string strMap)
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 1302);
            this.listener = listener;
            this.listener.Start();
            this.strMap = strMap;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(strMap[j+i*10]-'0');
                }
                Console.WriteLine("");
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    myMap[i, j] = strMap[j + i * 10]-'0';
                }
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(myMap[i, j]);
                }
                Console.WriteLine("");
            }
        }
        public void Start() { 
            while (true)
            {
                Console.WriteLine("Waiting for a connection.");
                TcpClient client = this.listener.AcceptTcpClient();
                Console.WriteLine("Client accepted.");
                NetworkStream stream = client.GetStream();
                StreamReader sr = new StreamReader(client.GetStream());
                StreamWriter sw = new StreamWriter(client.GetStream());
                try
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    int recv = 0;
                    foreach (byte b in buffer)
                    {
                        if (b != 0)
                        {
                            recv++;
                        }
                    }
                    string request = Encoding.UTF8.GetString(buffer, 0, recv);
                    Console.WriteLine(strMap);
                    Console.WriteLine("request received: " + request);

                    if (request.Length > 3)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                enemyMap[i, j] = request[j + i * 10] - '0';
                            }
                        }
                        sw.WriteLine("Client map getted");
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                Console.Write(enemyMap[i, j]);
                            }
                            Console.WriteLine("");
                        }
                    }else if (request == "mps")
                    {
                        string strMap = "";
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                strMap += myMap[i, j];
                            }
                        }
                        sw.WriteLine(strMap);
                    }else if (request == "mpc")
                    {
                        string strMap = "";
                        for (int i = 0; i < 9; i++)
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                strMap += enemyMap[i, j];
                            }
                        }
                        sw.WriteLine(strMap);
                    }
                    else
                    {
                        int x = request[0] - '0';
                        int y = request[1] - '0';
                        int code = request[2] - '0';
                        if (code == 1 && turn == 1)
                        {
                            if (this.enemyMap[y,x] == 1)
                            {
                                this.enemyMap[y, x] = 3;
                            }
                            else
                            {
                                this.enemyMap[y, x] = 2;
                                turn = 2;
                            }
                            sw.WriteLine(request + " is " + this.enemyMap[y, x]);
                            
                        }
                        else if (code == 2 && turn == 2)
                        {
                            if (this.myMap[y, x] == 1)
                            {
                                this.myMap[y, x] = 3;
                            }
                            else
                            {
                                this.myMap[y, x] = 2;
                                turn = 1;
                            }
                            sw.WriteLine(request + " is " + this.myMap[y, x]);

                        }
                        else
                        {
                            sw.WriteLine("Сейчас не ваш ход");
                        }
                    }

                    sw.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong.");
                    sw.WriteLine(e.ToString());
                }
            }
        }
        public void Send(StreamWriter sw, string str)
        {
            sw.WriteLine(str);
        }
    }
}