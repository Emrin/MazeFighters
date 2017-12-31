using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// This is where the magic happens.
/// In the maze we have 3 numbers,
/// 0: the spot is empty
/// 1: a wall is present
/// 2: an exit
/// 3: an equipment
/// 4: a player
/// Comments might have gotten a bit out of hand after spending some time on this.
/// </summary>


namespace MazeFighters
{
    class GameManager
    {
        private int mazeRows;
        private int mazeCols;
        private int speed;
        private int innerVoice;
        private int fighterCount;
        private int equipmentCount;
        private MazeGenerator MazeGen;
        private List<Fighter> Fighters;
        private List<Thread> Threads;
        private bool GameOver;
        private bool Announced;

        // Construct a new game.
        public GameManager(int mazeRows, int mazeCols, int speed, int innerVoice, int fighterCount, int equipmentCount)
        {
            //this.mazeRows = mazeRows; // this is set automatically for now.
            //this.mazeCols = mazeCols; // this too.
            this.speed = speed;
            this.innerVoice = innerVoice;
            this.fighterCount = fighterCount;
            this.equipmentCount = equipmentCount;
            MazeGen = new MazeGenerator(); // Initialized a new maze.
            this.mazeRows = MazeGen.MazeRows;
            this.mazeCols = MazeGen.MazeCols;
            // If you want to make equipment count 10% of the free space, uncomment this next part.
            double freeSpace = 0; // count free space
            for (int i = 0; i < MazeGen.MazeRows; i++)
            {
                for (int j = 0; j < MazeGen.MazeCols; j++)
                {
                    if (MazeGen.Maze[i, j] == 0)
                    {
                        freeSpace += 1;
                    }
                }
            }
            // now set the number of equipments to 10% of the free space
            this.equipmentCount = Convert.ToInt32(Math.Round(freeSpace*0.1));

            Fighters = new List<Fighter>();
            Threads = new List<Thread>();
            GameOver = false;
            Announced = false;
        }
        
        // Initialise a new game.
        public void Init()
        {
            // Create the fighters and place them randomly.
            for (int i = 0; i < fighterCount; i++)
            {
                Fighter solider76 = new Fighter(i);
                int[] spot = GetSpecificSpot(0); // some random empty spot
                solider76.Move(spot[0], spot[1]);
                MazeGen.Maze[spot[0], spot[1]] = 4;
                Fighters.Add(solider76);
                Console.WriteLine("Fighter {0} is in position!", i+1);
                Thread.Sleep(11); // to refresh the random
            }
            // Drop care packages into the maze, randomly.
            for (int i = 0; i < equipmentCount; i++)
            {
                DropCarePackage();
                Thread.Sleep(42); // refresh random
            }
            Console.WriteLine("Care packages have been dropped around the map!");
            // Show initial game state.
            MazeGen.Display(Fighters);
            // Begin AI sequences for each fighter.
            for (int i = 0; i < fighterCount; i++)
            {
                int k = i; // Still no idea why this is necessary.
                Thread AI = new Thread(() => Play(Fighters[k]));
                Threads.Add(AI);
            }
            Console.WriteLine("Press any key when ready...");
            Console.ReadKey();
            for (int i = 0; i < fighterCount; i++)
            {
                Threads[i].Start();
                Thread.Sleep(200);
            }
            // Launch a thread that displays the game.
            Thread DisplayThread = new Thread(() => AutoRefresh());
            DisplayThread.Start();
            // Launch a thread that resets equipments.
            Thread VoiceThread = new Thread(() => TheVoice());
            VoiceThread.Start();
        }

        // Drop a piece of equipment to a random empty spot.
        public void DropCarePackage()
        {
            int[] spot = GetSpecificSpot(0);
            MazeGen.Maze[spot[0], spot[1]] = 3;
        }

        // Returns a spot in the maze that is occupied by the specified criteria.
        public int[] GetSpecificSpot(int criteria)
        {
            int posRow;
            int posCol;
            Random rnd = new Random();
            //Determine an empty position.
            do
            {
                posRow = rnd.Next(1, mazeRows);
                posCol = rnd.Next(1, mazeCols);
            } while (MazeGen.Maze[posRow, posCol] != criteria);
            int[] result = new int[] { posRow, posCol };
            return (result);
        }

        // Actualize every X miliseconds the maze in the console.
        public void AutoRefresh()
        {
            while (!GameOver)
            {
                Console.Clear(); // Great comment-to-debug line heh
                MazeGen.Display(Fighters);
                Thread.Sleep(speed);
            }
        }

        // Thread that resets equipments, and also the morals.
        public void TheVoice()
        {
            Thread.Sleep(innerVoice);
            Random randy = new Random();
            int k;

            while (!GameOver)
            {
                Console.WriteLine("The weapons have expired!");
                for (int i = 0; i < Fighters.Count; i++) // for each player
                {
                    k = Fighters[i].Bonuses.Count;
                    for (int j = 1; j < k; j++) // for each gathered piece of equipment
                    {
                        Fighters[i].Bonuses.RemoveAt(1); // expire the piece of equipment
                        DropCarePackage(); // drop an equipment
                        // They just lost their equipments, now they might want to flee :/
                        if (randy.Next(1, 11) > 3)
                        {
                            Fighters[i].Defensive = true;
                            Thread.Sleep(10); // refresh random
                        }
                    }
                }
                Thread.Sleep(innerVoice);
            }
        }

        // Thread that takes care of individual fights.
        // another thread to check healths and replace objects
        public void Meele(Fighter hunter, Fighter target, bool dead)
        {
            target.Hp -= hunter.Bonuses.Sum(); // pew pew
            if (target.Hp <= 0) // check pulse
            {
                MazeGen.Maze[target.PosRow, target.PosCol] = 0; // delete from the maze
                Fighters.Remove(target); // you no longer exist to me
                fighterCount -= 1;
                dead = true;
                Console.WriteLine("Another one bites the dust!");
            }
        }
        
        // Moving happens here
        public void MoveOnce(Fighter fighter, List<Node> Path, bool dead)
        {
            bool moved = false;
            int direction;
            Random rnd = new Random();
            int[] targetLoc;

            // preferances loop checks what's around and has a preferance for certain types
            while (true)
            {
                // MAIN PREFERANCE IS EXIT: DETECT IF NEAR AND GO FOR IT
                if (MazeGen.Maze[fighter.PosRow + 1, fighter.PosCol] == 2)
                {
                    targetLoc = new int[] { fighter.PosRow + 1, fighter.PosCol };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow - 1, fighter.PosCol] == 2)
                {
                    targetLoc = new int[] { fighter.PosRow - 1, fighter.PosCol };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow, fighter.PosCol + 1] == 2)
                {
                    targetLoc = new int[] { fighter.PosRow, fighter.PosCol + 1 };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow, fighter.PosCol - 1] == 2)
                {
                    targetLoc = new int[] { fighter.PosRow, fighter.PosCol - 1 };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }

                // IF OFFENSIVE AND A PLAYER IS NEAR: GET EM' BOIS
                if (MazeGen.Maze[fighter.PosRow + 1, fighter.PosCol] == 4 && !fighter.Defensive)
                {
                    targetLoc = new int[] { fighter.PosRow + 1, fighter.PosCol };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow - 1, fighter.PosCol] == 4 && !fighter.Defensive)
                {
                    targetLoc = new int[] { fighter.PosRow - 1, fighter.PosCol };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow, fighter.PosCol + 1] == 4 && !fighter.Defensive)
                {
                    targetLoc = new int[] { fighter.PosRow, fighter.PosCol + 1 };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow, fighter.PosCol - 1] == 4 && !fighter.Defensive)
                {
                    targetLoc = new int[] { fighter.PosRow, fighter.PosCol - 1 };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }

                // Last preference is loot crates
                if (MazeGen.Maze[fighter.PosRow + 1, fighter.PosCol] == 3)
                {
                    targetLoc = new int[] { fighter.PosRow + 1, fighter.PosCol };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow - 1, fighter.PosCol] == 3)
                {
                    targetLoc = new int[] { fighter.PosRow - 1, fighter.PosCol };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow, fighter.PosCol + 1] == 3)
                {
                    targetLoc = new int[] { fighter.PosRow, fighter.PosCol + 1 };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }
                if (MazeGen.Maze[fighter.PosRow, fighter.PosCol - 1] == 3)
                {
                    targetLoc = new int[] { fighter.PosRow, fighter.PosCol - 1 };
                    moved = WatchStep(fighter, targetLoc, Path, dead);
                    break;
                }

                break;
            }

            // If the above isn't the case, just pick a random direction.
            while (!moved)
            {
                direction = rnd.Next(0, 4); // 0 is up, 1 is right, 2 is down, 3 is right.
                switch (direction)
                {
                    case 0: // up
                        targetLoc = new int[] { fighter.PosRow - 1, fighter.PosCol };
                        moved = WatchStep(fighter, targetLoc, Path, dead); // for some reason this has to be called here
                        break;
                    case 1: // right
                        targetLoc = new int[] { fighter.PosRow, fighter.PosCol + 1 };
                        moved = WatchStep(fighter, targetLoc, Path, dead);
                        break;
                    case 2: // down
                        targetLoc = new int[] { fighter.PosRow + 1, fighter.PosCol };
                        moved = WatchStep(fighter, targetLoc, Path, dead);
                        break;
                    case 3: // left
                        targetLoc = new int[] { fighter.PosRow, fighter.PosCol - 1 };
                        moved = WatchStep(fighter, targetLoc, Path, dead);
                        break;
                    default:
                        break;
                }
            }
        }
        
        // Returns a list of all available nodes near a fighter
        public List<Node> GetChoices(int row, int col)
        {
            // fill a list of choises that are available
            List<Node> Choices = new List<Node>();


            int[] targetLoc = new int[] { row - 1, col }; // check up
            if (MazeGen.Maze[targetLoc[0], targetLoc[1]] != 1) // as long as it's not a wall it's visitable
            {
                Node upper = new Node(targetLoc[0], targetLoc[1]);
                Choices.Add(upper);
            }

            targetLoc = new int[] { row, col + 1 }; // check right
            if (MazeGen.Maze[targetLoc[0], targetLoc[1]] != 1)
            {
                Node upper = new Node(targetLoc[0], targetLoc[1]);
                Choices.Add(upper);
            }

            targetLoc = new int[] { row + 1, col }; // check down
            if (MazeGen.Maze[targetLoc[0], targetLoc[1]] != 1)
            {
                Node upper = new Node(targetLoc[0], targetLoc[1]);
                Choices.Add(upper);
            }

            targetLoc = new int[] { row, col - 1 }; // check left
            if (MazeGen.Maze[targetLoc[0], targetLoc[1]] != 1)
            {
                Node upper = new Node(targetLoc[0], targetLoc[1]);
                Choices.Add(upper);
            }

            return Choices;
        }
           
        // used to detect dead ends
        public bool nothingToExplore(int X, int Y, List<Node> Path)
        {
            bool nothingToExplore = false;
            List<Node> Choices = GetChoices(X, Y);
            int nbChoices = Choices.Count;
            int explored = 0;
            // check if ALL of the choices we have have been visited
            for (int m = 0; m < nbChoices; m++)
            {
                for (int n = 0; n < Path.Count; n++)
                {
                    if (Choices[m].PosRow == Path[n].PosRow && Choices[m].PosCol == Path[n].PosCol)
                    {
                        explored += 1;
                    }
                }
            }
            if (explored == nbChoices)
            {
                nothingToExplore = true;
            }
            return nothingToExplore;
        }
        
        // Watch where you're goin', pal.
        public bool WatchStep(Fighter fighter, int[] targetLoc, List<Node> Path, bool dead)
        {
            bool result = false; // aka moved
            Random randy = new Random();
            int[] currentLoc = new int[] { fighter.PosRow, fighter.PosCol };
            int targetType = MazeGen.Maze[targetLoc[0], targetLoc[1]];
            int bonusDmg = 0;
            Node T;
            // later optimize this part
            // Check if we've already been there
            bool alreadySeen = false;
            for (int k = 0; k < Path.Count; k++)
            {
                if (Path[k].PosRow == targetLoc[0] && Path[k].PosCol == targetLoc[1])
                {
                    alreadySeen = true;
                    break;
                }
            }

            // if there's no wall and it's unexplored we can go there
            if (targetType != 1 && !alreadySeen)
            {
                fighter.Backtrack = 0;
                
                result = true;
                if (targetType == 2) // check if exit
                {
                    GameOver = true;
                }
                if (targetType == 3) // check if loot
                {
                    bonusDmg = randy.Next(1, 11);
                    fighter.Bonuses.Add(bonusDmg);
                    // There's a good chance that the fighter becomes offensive after picking up a loot crate.
                    if (randy.Next(1, 11) > 3)
                    {
                        fighter.Defensive = false;
                    }
                }
                if (targetType == 4) // check if enemy
                {
                    if (fighter.Defensive)
                    { // this can be optimized
                        result = false; // move somewhere else
                    }
                    else // offensive
                    {
                        // find which enemy it is
                        int enemyNumber = 0;
                        for (int k = 0; k < Fighters.Count; k++)
                        {
                            if (Fighters[k].PosRow == targetLoc[0] && Fighters[k].PosCol == targetLoc[1])
                            {
                                enemyNumber = k;
                            }
                        }
                        // DESTROY HIM!
                        Meele(fighter, Fighters[enemyNumber], dead);
                    }
                }
                MazeGen.Update(currentLoc, targetLoc); // update the maze
                fighter.Move(targetLoc[0], targetLoc[1]); // update the fighter
                Node thisNode = new Node(fighter.PosRow, fighter.PosCol); // complete path
                Path.Add(thisNode);
            }

            if (!result) // if we still havent moved, check if we're cornered
            {
                // Begin backtracking.
                // while nothing to explore go back in path, reorderng path each time, only add after u've gotten a new thing
                while (targetType != 1 && nothingToExplore(fighter.PosRow, fighter.PosCol, Path))
                {
                    try
                    {
                        //fighter.Backtrack += 1; // tell fighter he has backtracked
                        targetLoc[0] = Path[Path.Count - 1].PosRow; // backpeddle once
                        targetLoc[1] = Path[Path.Count - 1].PosCol;

                        // change currentLoc
                        currentLoc[0] = fighter.PosRow;
                        currentLoc[1] = fighter.PosCol;
                        
                        MazeGen.Update(currentLoc, targetLoc); // update the maze
                        fighter.Move(targetLoc[0], targetLoc[1]); // update the fighter


                        // now also reposition last node of path to front
                        T = Path[Path.Count - 1];
                        Path.RemoveAt(Path.Count - 1);
                        Path.Insert(0, T);

                        result = true;
                        Thread.Sleep(speed);
                    }
                    catch
                    {
                        Console.WriteLine("The maze has no exit! IT'S A TRAP!");
                        GameOver = true;
                        break;
                    }
                }
                fighter.Backtrack = 0;
            }
            return result;
        }
        
        // "Run, you fools!" -Gandalf The Dev
        public void Play(Fighter fighter)
        {
            List<Node> Path = new List<Node>();
            bool dead = false;

            // Add current position to path.
            Node curNode = new Node(fighter.PosRow, fighter.PosCol);
            Path.Add(curNode);

            while (!GameOver && !dead)
            {
                // Move once, every second.
                MoveOnce(fighter, Path, dead);

                Thread.Sleep(speed);
            }
            Announcement();
        }
        
        // When game == very over
        public void Announcement()
        {
            if (!Announced)
            {
                Announced = true;
                Console.WriteLine("*Spectators applaud while tears glide down their cheeks as they blissfully watch in awe*");
                Console.WriteLine("Game Over! What an amazing performance!");
                Console.WriteLine("-=Disclaimer: No pixels were harmed during the animation of this application.=-");
                Console.ReadKey();
            }
        }

        

    }
}
