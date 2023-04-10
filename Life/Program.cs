using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.Json;
using System.Data.Common;
using System.Xml;
using System.ComponentModel.Design;
using System.IO;

namespace cli_life
{
    public class Figure
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string mask { get; set; }
        public int Type { get; set; }
        public int count = 0;
    }
    public class Settings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CellSize { get; set; }
        public double LiveDensity { get; set; }
    }
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
        public int CountIsAlive()
        {
            int count = 0;
            for (int x = 0; x < Columns; x++)

                for (int y = 0; y < Rows; y++)
                    if (Cells[x, y].IsAlive == true)
                        count++;

            return count;
        }

        public bool CheckSimmetriHorizontal()
        {
            bool sim = true;

            for (int y = 0; y < Rows / 2; y++)

                for (int x = 0; x < Columns; x++)
                {
                    if (Cells[x, y].IsAlive != Cells[x, Rows - y - 1].IsAlive)
                    {
                        sim = false;
                        break;
                    }
                }

            return sim;
        }

        public bool CheckSimmetriVertical()
        {
            bool sim = true;

            for (int y = 0; y < Rows / 2; y++)

                for (int x = 0; x < Columns / 2; x++)
                {
                    if (Cells[x, y].IsAlive != Cells[Columns - x - 1, y].IsAlive)
                    {
                        sim = false;
                        break;
                    }
                }

            return sim;
        }
    }
    public class Program
    {
        static Board board;
        
        static public Figure MackMask(string file_name, Figure figure)
        {
            file_name = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\\..\\..\\"), file_name);
            StreamReader file = new StreamReader(file_name);

            figure.Height = File.ReadAllLines(file_name).Length;
            figure.Width = file.ReadLine().Length;
            file.DiscardBufferedData();
            file.BaseStream.Seek(0, SeekOrigin.Begin);
            string s;
            while ((s = file.ReadLine()) != null)
            {
                figure.mask += s;
            }
            if (figure.mask[figure.Width + 1] == ' ')
                figure.Type = 1;
            else figure.Type = 2;
            file.Close();
            return figure;
        }

        static public Figure[] MackAllMasks()
        {
            Figure barge = new Figure();
            Figure block = new Figure();
            Figure boat = new Figure();
            Figure box = new Figure();
            Figure hive = new Figure();
            Figure loaf = new Figure();
            Figure long_boat = new Figure();
            Figure long_ship = new Figure();
            Figure pond = new Figure();
            Figure ship = new Figure();
            Figure snake = new Figure();
            barge = MackMask("barge.txt", barge);
            block = MackMask("block.txt", block);
            boat = MackMask("boat.txt", boat);
            box = MackMask("box.txt", box);
            hive = MackMask("hive.txt", hive);
            loaf = MackMask("loaf.txt", loaf);
            long_boat = MackMask("long_boat.txt", long_boat);
            long_ship = MackMask("long_ship.txt", long_ship);
            pond = MackMask("pond.txt", pond);
            ship = MackMask("ship.txt", ship);
            snake = MackMask("snake.txt", snake);
            Figure[] figures = { barge, block, boat, box, hive, loaf, long_boat, long_ship, pond, ship, snake };
            return figures;
        }

        static public Figure[] AllCheck(Board b, Figure[] fig)
        {
            for (int x = 0; x < b.Columns; x++)
            {
                for (int y = 0; y < b.Rows; y++)
                {
                    if (b.Cells[x, y].IsAlive == true)
                    {
                        for(int i = 0; i < 11; i++)
                        {
                            if(PartToCheck(fig[i], x, y, b))
                            {
                                fig[i].count ++;
                                break;
                            }
                        }
                    }
                }
            }
            return fig;
        }
        static public bool PartToCheck(Figure figure, int corX, int corY,Board b)
        {
            bool IsFigure = false;
            int xR = 0;
            int yT = 0;
            string s = "";
            int startX = 0;
            int startY = 0;
            if (figure.Type == 1)
            {
                startX = corX - 2;
                startY = corY - 1;
            }
            else if (figure.Type == 2)
            {
                startX = corX - 1;
                startY = corY - 1;
            }
            for (int y = startY; y < startY + figure.Height; y++)
            {
                if (y < 0)
                {
                    yT = b.Height + y;
                }
                else if (y > b.Height - 1)
                {
                    yT = y - b.Height;
                }
                else
                {
                    yT = y;
                }

                for (int x = startX; x < startX + figure.Width; x++)
                {
                    if (x < 0)
                    {
                        xR = b.Width + x;
                    }
                    else if (x > b.Width - 1)
                    {
                        xR = x - b.Width;
                    }
                    else
                    {
                        xR = x;
                    }
                    
                    if (b.Cells[xR, yT].IsAlive)
                        s += "*";
                    else s += " ";
                }
            }
            if (s == figure.mask)
            {
                for (int y = startY; y < startY + figure.Height; y++)
                {
                    if (y < 0)
                    {
                        yT = b.Height + y;
                    }
                    else if (y > b.Height - 1)
                    {
                        yT = yT - b.Height;
                    }
                    else
                    {
                        yT = y;
                    }

                    for (int x = startX; x < startX + figure.Width; x++)
                    {
                        if (x < 0)
                        {
                            xR = b.Width + x;
                        }
                        else if (x > b.Width - 1)
                        {
                            xR = xR - b.Width;
                        }
                        else
                        {
                            xR = x;
                        }
                        b.Cells[xR, yT].IsAlive = false;
                    }
                }
                IsFigure=true;
            }
            return IsFigure;
        }
        static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0.5);
        }

        public static Board ChangeSetting(string file)
        {
            file = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\\..\\..\\"), file);

            string jsonFile = File.ReadAllText(file);

            var set = JsonSerializer.Deserialize<Settings>(jsonFile);
            board = new Board(set.Width, set.Height, set.CellSize, set.LiveDensity);
            return board;
        }

        public static void SaveFile(Board b)
        {
            string file_name = "save.txt";

            file_name = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\\..\\..\\"), file_name);

            StreamWriter file = new StreamWriter(file_name, false);

            for (int row = 0; row < b.Rows; row++)
            {
                for (int column = 0; column < b.Columns; column++)
                {
                    if (b.Cells[column, row].IsAlive)
                    {
                        file.Write("*");
                    }
                    else file.Write(" ");
                }
                file.WriteLine();
            }

            file.Close();
        }

        public static Board ReadFile(string file_name)
        {
            file_name = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"..\\..\\..\\"), file_name);
            StreamReader file = new StreamReader(file_name);

            int numStr = File.ReadAllLines(file_name).Length;
            int numSymb = file.ReadLine().Length;

            file.DiscardBufferedData();
            file.BaseStream.Seek(0, SeekOrigin.Begin);
            Board b = new Board(numSymb, numStr, 1, 0.5);

            for (int j = 0; j < numStr; j++)
            {
                string line = file.ReadLine();
                for (int i = 0; i < numSymb; i++)
                {
                    if (line[i] == '*')
                        b.Cells[i, j].IsAlive = true;
                    else
                        b.Cells[i, j].IsAlive = false;
                }
            }
            file.Close();
            return b;
        }

        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Main(string[] args)
        {
            Figure[] figures = new Figure[11];
            Reset();
            board = ReadFile("file1.txt");
            
            figures = MackAllMasks();
            figures = AllCheck(board, figures);
            while (true)
            {
                Console.Clear();
                Render();
                board.Advance();
                Thread.Sleep(500);
            }
        }
    }
}
