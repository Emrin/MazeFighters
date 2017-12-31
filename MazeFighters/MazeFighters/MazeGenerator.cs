using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// The maze's logic.
/// You can modify maze file's name and location here.
/// </summary>

namespace MazeFighters
{
    class MazeGenerator
    {
        private string pathMaze;
        private int mazeRows;
        private int mazeCols;
        private int[,] maze;

        // Maze constructor
        public MazeGenerator()
        {
            ReadMaze();
        }

        // Getters and setters
        public string PathMaze { get => pathMaze; set => pathMaze = value; }
        public int MazeRows { get => mazeRows; set => mazeRows = value; }
        public int MazeCols { get => mazeCols; set => mazeCols = value; }
        public int[,] Maze { get => maze; set => maze = value; }

        
        /// Methods
        
        // Reads the maze from a text file named ShapeOfYou.txt
        public void ReadMaze()
        {
            bool initialised = false;
            int counter = 0;
            string line;

            // Set the location of the maze file.
            string pathApp = Directory.GetCurrentDirectory();
            PathMaze = pathApp.Substring(0, pathApp.Length - 35);
            PathMaze += "ShapeOfYou.txt";

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(PathMaze);
            while ((line = file.ReadLine()) != null)
            {
                if (!initialised) // Initialise the maze's settings.
                {
                    initialised = true;
                    mazeRows = File.ReadLines(PathMaze).Count();
                    mazeCols = line.Count();
                    maze = new int[mazeRows, mazeCols];
                }
                for (int i = 0; i < line.Count(); i++)
                {
                    maze[counter, i] = line[i] - '0'; // Interesting way to convert char to int
                }
                counter++;
            }

            file.Close();
            System.Console.WriteLine("There were {0} rows and {1} columns in the read maze text file.", mazeRows, mazeCols);
        }

        // Displays the current maze in the console.
        public void Display(List<Fighter> Fighters)
        {
            string state;
            for(int i = 0; i < maze.GetLength(0); i++)
            {
                for(int j = 0; j < maze.GetLength(1); j++)
                {
                    //Console.Write(maze[i, j]);
                    if (maze[i, j] == 0) Console.Write(" "); // Empty spot
                    if (maze[i, j] == 1) Console.Write("█"); // Wall
                    if (maze[i, j] == 2) Console.Write("░"); // Exit door
                    if (maze[i, j] == 3) Console.Write("."); // Loot crate
                    //if (maze[i, j] == 4) Console.Write("X"); // Player
                    if (maze[i, j] == 4) // display player id
                    {
                        for (int k = 0; k < Fighters.Count; k++)
                        {
                            if (Fighters[k].PosRow == i && Fighters[k].PosCol == j)
                            {
                                Console.Write(Fighters[k].Id + 1);
                                break;
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            for (int i = 0; i < Fighters.Count; i++)
            {
                state = "╩ offensive ╩";
                if (Fighters[i].Defensive) state = "▀▄ defensive ▄▀";
                Console.WriteLine("Player {0}: {1} health | {2} damage | {3}", Fighters[i].Id+1, Fighters[i].Hp, Fighters[i].Bonuses.Sum(), state);
            }
        }
       /* 
        // Update the maze's logic.
        public void Update(int currentX, int currentY, int targetX, int targetY)
        {
            maze[targetX, targetY] = 4;
            maze[currentX, currentY] = 0;
        }
        */
        public void Update(int[] current, int[] target)
        {
            maze[target[0], target[1]] = 4;
            maze[current[0], current[1]] = 0;
        }

    }
}
