using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace SeaWars
{
    public partial class Form1 : Form
    {
        public const int mapSize = 10;
        public int cellSize = 30;
        public string alphabet = "АБВГДЕЖЗИК";
        public int shipCount = 0;
        public Label turnLabel = new Label();
        public Label shipLabel = new Label();
        public Button startButton = new Button();
        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];



        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public bool isPlaying = false;

        public Bot bot;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Сервер";
            Init();
        }

        public void Init()
        {
            isPlaying = false;
            CreateMaps();
            //bot = new Bot(enemyMap, myMap, enemyButtons, myButtons);
            //enemyMap = bot.ConfigureShips();
        }

        public void CreateMaps()
        {
            this.Width = mapSize * 2 * cellSize + 50;
            this.Height = (mapSize + 3) * cellSize + 200;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0)
                            button.Text = alphabet[j - 1].ToString();
                        if (j == 0 && i > 0)
                            button.Text = i.ToString();
                    }
                    else
                    {
                        button.Click += new EventHandler(ConfigureShips);
                    }
                    myButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    myMap[i, j] = 0;
                    enemyMap[i, j] = 0;

                    Button button = new Button();
                    button.Location = new Point(320 + j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.BackColor = Color.White;
                    if (j == 0 || i == 0)
                    {
                        button.BackColor = Color.Gray;
                        if (i == 0 && j > 0)
                            button.Text = alphabet[j - 1].ToString();
                        if (j == 0 && i > 0)
                            button.Text = i.ToString();
                    }
                    else
                    {
                        button.Click += new EventHandler(PlayerShoot);
                    }
                    enemyButtons[i, j] = button;
                    this.Controls.Add(button);
                }
            }
            Label map1 = new Label();
            map1.Text = "Карта игрока";
            map1.Font = new Font("Consolas", 18f, FontStyle.Bold);
            map1.Location = new Point(0, mapSize * cellSize + 10);
            map1.Width = 200;
            map1.Height = 30;
            this.Controls.Add(map1);

            Label map2 = new Label();
            map2.Text = "Карта противника";
            map2.Font = new Font("Consolas", 18f, FontStyle.Bold);
            map2.Location = new Point(350, mapSize * cellSize + 10);
            map2.Width = 250;
            map2.Height = 30;
            this.Controls.Add(map2);


            startButton.Text = "Создать игру";
            startButton.Font = new Font("Consolas", 18f, FontStyle.Bold);
            startButton.Click += new EventHandler(Start);
            startButton.Location = new Point(0, mapSize * cellSize + 50);
            startButton.Width = 200;
            startButton.Height  = 50;
            this.Controls.Add(startButton);

            turnLabel.Text = "Ваш ход";
            turnLabel.Width = 800;
            turnLabel.Height = 30;
            turnLabel.Font = new Font("Consolas", 18f, FontStyle.Bold);
            turnLabel.Location = new Point(0, mapSize * cellSize + 50);

            shipLabel.Text = "Разместите на своём поле корабли:" + shipCount.ToString();
            shipLabel.Width = 800;
            shipLabel.Height = 30;
            shipLabel.Font = new Font("Consolas", 18f, FontStyle.Bold);
            shipLabel.Location = new Point(0, mapSize * cellSize + 50);
            //this.Controls.Add(shipLabel);

            CreateShip();
        }

        public void Start(object sender, EventArgs e)
        {
            string strMap = "";
            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    strMap += myMap[i, j];
                }
                strMap += "\n";
            }
            
            isPlaying = true;
            Server server = new Server(strMap);
            Thread server_thread = new Thread(server.Start);
            server_thread.Name = "server_thread";
            server_thread.Start();
            Thread reset_Map = new Thread(ResetMap);
            reset_Map.Name = "reset_Map";
            reset_Map.Start();
            Button pressedButton = sender as Button;
            pressedButton.Text = "Игра создана";
            pressedButton.Enabled = false;
            Thread.Sleep(1000);
            this.Controls.Remove(startButton);
            this.Controls.Add(turnLabel);
        }

        public bool CheckIfMapIsNotEmpty()
        {
            bool isEmpty1 = true;
            bool isEmpty2 = true;
            for (int i = 1; i < mapSize; i++)
            {
                for (int j = 1; j < mapSize; j++)
                {
                    if (myMap[i, j] != 0)
                        isEmpty1 = false;
                    if (enemyMap[i, j] != 0)
                        isEmpty2 = false;
                }
            }
            if (isEmpty1 || isEmpty2)
                return false;
            else return true;
        }

        public void ConfigureShips(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            if (!isPlaying)
            {
                if (myMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] == 0 && shipCount > 0)
                {
                    pressedButton.BackColor = Color.Red;
                    myMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 1;
                    shipCount--;
                    Console.WriteLine(shipCount.ToString());
                    shipLabel.Text = "Разместите на своём поле корабли:" + shipCount.ToString();
                }
                else
                {
                    pressedButton.BackColor = Color.White;
                    myMap[pressedButton.Location.Y / cellSize, pressedButton.Location.X / cellSize] = 0;
                    shipCount++;
                    shipLabel.Text = "Разместите на своём поле корабли:" + shipCount.ToString();
                }
            }
        }
        public void ResetMap()
        {
            while (true)
            {
                Client client = new Client("mps");
                string result = client.Send();
                Console.WriteLine(result);
                for (int i = 1; i < mapSize; i++)
                {
                    for (int j = 1; j < mapSize; j++)
                    {
                        if (result[(j - 1) + (i - 1) * 9] == '2')
                        {
                            myButtons[i, j].BackColor = Color.Black;
                        }
                        else if (result[(j - 1) + (i - 1) * 9] == '3')
                        {
                            this.Invoke((MethodInvoker)delegate// делегируем отрисовку GUI основному потоку, в котором обрабатывается
                            {
                                myButtons[i, j].BackColor = Color.Blue;
                                myButtons[i, j].Text = "X";
                            });
                            
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }
        public void PlayerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;
            int x = (pressedButton.Location.X - 320) / cellSize - 1;
            int y = pressedButton.Location.Y / cellSize - 1;

            Client client = new Client(x.ToString()+y.ToString()+'1');
            //Thread client_thread = new Thread(client.Send);
            //client_thread.Name = "client_thread";
            //client_thread.Start();
            string result = client.Send();
            Console.WriteLine(result);
            if (result != "Error" && result != "Сейчас не ваш ход")
            {
                int res = result[result.Length - 1] - '0';
                Console.WriteLine(res);
                if (res == 3)
                {
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = "X";
                }
                else if (res == 2)
                {
                    pressedButton.BackColor = Color.Black;
                }
                turnLabel.Text = "Ваш ход";
            }
            else 
            {
                turnLabel.Text = "Сейчас ходит другой игрок";
            }
        }


        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;
            if (isPlaying)
            {
                int delta = 0;
                if (pressedButton.Location.X > 320)
                    delta = 320;
                if (map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] != 0)
                {
                    hit = true;
                    map[pressedButton.Location.Y / cellSize, (pressedButton.Location.X - delta) / cellSize] = 0;
                    pressedButton.BackColor = Color.Blue;
                    pressedButton.Text = "X";
                }
                else
                {
                    hit = false;

                    pressedButton.BackColor = Color.Black;
                }
            }
            return hit;
        }
        public void CreateShip()
        {
            int[,] newMap = new int[mapSize, mapSize];
            int tries = 1;
            Random rnd = new Random();
            for (int length = 4; length >= 1; length--)
            {
                for (int k = 0; k < 5 - length; k++)
                {
                    int orientation = rnd.Next(0, 2);
                    Console.WriteLine(orientation);
                    // 0 - вертикально 1 - горизонтально
                    if (orientation == 0)
                    {
                        int x = rnd.Next(1, 10);
                        int y = rnd.Next(1, 11 - length);
                        bool stop = false;
                        int max_y = y + length;
                        int max_x = x + 1;
                        if (y == 10 - length)
                            max_y = 9;
                        if (x == 9)
                            max_x = 9;

                        for (int i = y - 1; i <= max_y; i++)
                        {
                            for (int j = x - 1; j <= max_x; j++)
                            {
                                if (newMap[i, j] == 1)
                                {
                                    stop = true;
                                    break;
                                }
                            }
                        }
                        if (stop)
                        {
                            k--;
                            tries++;
                            continue;
                        }

                        for (int i = 0; i < length; i++)
                        {
                            newMap[y + i, x] = 1;
                            myButtons[y + i, x].BackColor = Color.Red;
                            if (x != 9)
                            {
                                newMap[y + i, x + 1] = -1;
                                //myButtons[y + i, x + 1].BackColor = Color.Green;
                            }
                            if (x != 1)
                            {
                                newMap[y + i, x - 1] = -1;
                                //myButtons[y + i, x - 1].BackColor = Color.Green;
                            }
                        }
                        if (x != 9)
                        {
                            if (y != 1)
                            {
                                newMap[y - 1, x + 1] = -1;
                                //myButtons[y - 1, x + 1].BackColor = Color.Green;
                            }
                            if (y != 10 - length)
                            {
                                newMap[y + length, x + 1] = -1;
                                //myButtons[y + length, x + 1].BackColor = Color.Green;
                            }
                        }
                        if (x != 1)
                        {
                            if (y != 1)
                            {
                                newMap[y - 1, x - 1] = -1;
                                //myButtons[y - 1, x - 1].BackColor = Color.Green;
                            }
                            if (y != 10 - length)
                            {
                                newMap[y + length, x - 1] = -1;
                                //myButtons[y + length, x - 1].BackColor = Color.Green;
                            }
                        }
                        if (y != 1)
                        {
                            newMap[y - 1, x] = -1;
                            //myButtons[y - 1, x].BackColor = Color.Green;
                        }
                        if (y != 10 - length)
                        {
                            newMap[y + length, x] = -1;
                            //myButtons[y + length, x].BackColor = Color.Green;
                        }
                    }
                    else
                    {
                        int x = rnd.Next(1, 11 - length);
                        int y = rnd.Next(1, 10);

                        bool stop = false;
                        int max_y = y + 1;
                        int max_x = x + length;
                        if (y == 9)
                            max_y = 9;
                        if (x == 10 - length)
                            max_x = 9;

                        for (int i = y - 1; i <= max_y; i++)
                        {
                            for (int j = x - 1; j <= max_x; j++)
                            {
                                if (newMap[i, j] == 1)
                                {
                                    stop = true;
                                    break;
                                }
                            }
                        }
                        if (stop)
                        {
                            k--;
                            tries++;
                            continue;
                        }

                        for (int i = 0; i < length; i++)
                        {

                            if (y != 1)
                            {
                                newMap[y - 1, x + i] = -1;
                                //myButtons[y - 1, x + i].BackColor = Color.Green;
                            }
                            if (y != 9)
                            {
                                newMap[y + 1, x + i] = -1;
                                //myButtons[y + 1, x + i].BackColor = Color.Green;
                            }
                            newMap[y, x + i] = 1;
                            myButtons[y, x + i].BackColor = Color.Red;
                        }

                        if (x != 10 - length)
                        {
                            if (y != 1)
                            {
                                newMap[y - 1, x + length] = -1;
                                //myButtons[y - 1, x + length].BackColor = Color.Green;
                            }
                            if (y != 9)
                            {
                                newMap[y + 1, x + length] = -1;
                                //myButtons[y + 1, x + length].BackColor = Color.Green;
                            }
                            newMap[y, x + length] = -1;
                            //myButtons[y, x + length].BackColor = Color.Green;
                        }
                        if (x != 1)
                        {
                            if (y != 1)
                            {
                                newMap[y - 1, x - 1] = -1;
                                //myButtons[y - 1, x - 1].BackColor = Color.Green;
                            }
                            if (y != 9)
                            {
                                newMap[y + 1, x - 1] = -1;
                                //myButtons[y + 1, x - 1].BackColor = Color.Green;
                            }
                            newMap[y, x - 1] = -1;
                            //myButtons[y, x - 1].BackColor = Color.Green;
                        }
                    }
                }

                for (int i = 0; i < mapSize; i++)
                {
                    for (int j = 0; j < mapSize; j++)
                    {
                        if (newMap[i, j] == 1)
                        {
                            myMap[i, j] = 1;
                        }
                    }
                } 
            }
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Console.Write(myMap[i, j]);
                }
                Console.WriteLine("\n");
            }
            Console.WriteLine(tries);
        }
    }
}
