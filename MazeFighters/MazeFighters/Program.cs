using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// ----------Description:
/// MAZE-FIGHTERS! A multithreaded, autonomous 'game'.
/// A maze is generated out of a text file (see MazeGenerator class).
/// There are a number of objects in the maze, these can be equipped to boost dps stats.
/// Equipped objects expire and are redistributed every time the innerVoice ticks.
/// A number of fighters are 'parachuted' inside the maze.
/// These fighters try to exit the maze but may also be willing to fight.
/// The game ends when a fighter escapes the maze.
/// The fighters move on intelligence (personal algorithm).
/// They remember where they've been and update their paths so that when they
/// back-track they rearange their paths (see GameManager class' WatchStep method, 2nd part).
/// 
/// ----------Noticed bugs:
/// -Sometimes passing through enemies without damaging them.
/// -When starting to backtrack, dissapearing for 1 frame.
/// 
/// </summary>

namespace MazeFighters
{
    class Program
    {
        static void Main(string[] args)
        {
            // Default Settings
            int mazeRows = 16; // this one and the next one is automatic for now.
            int mazeCols = 38;
            int speed = 1000; // gamespeed, in miliseconds
            int innerVoice = 30000; // 30sec. This is the timer for when fighters' objects expire.
            int fighterCount = 3; // number of fighters
            int equipmentCount = 60; // number of equipments
            
            // Customized Settings
            if (args.Length > 1) // accepting between 2 and 6 arguments
            {
                try
                {
                    mazeRows = Int32.Parse(args[0]);
                    mazeCols = Int32.Parse(args[1]);
                    if (args.Length > 2) speed = Int32.Parse(args[2]);
                    if (args.Length > 3) innerVoice = Int32.Parse(args[3]);
                    if (args.Length > 4) fighterCount = Int32.Parse(args[4]);
                    if (args.Length > 5) equipmentCount = Int32.Parse(args[5]);
                    Console.WriteLine("{0} custom settings have been applied.", args.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            // Here we launch launch thy game.
            GameManager game = new GameManager(mazeRows, mazeCols, speed, innerVoice, 
                fighterCount, equipmentCount);

            game.Init(); // Good ol' init.
            
        }
    }
}


/*

███╗   ███╗ █████╗ ███████╗███████╗    ███████╗██╗ ██████╗ ██╗  ██╗████████╗███████╗██████╗ ███████╗
████╗ ████║██╔══██╗╚══███╔╝██╔════╝    ██╔════╝██║██╔════╝ ██║  ██║╚══██╔══╝██╔════╝██╔══██╗██╔════╝
██╔████╔██║███████║  ███╔╝ █████╗      █████╗  ██║██║  ███╗███████║   ██║   █████╗  ██████╔╝███████╗
██║╚██╔╝██║██╔══██║ ███╔╝  ██╔══╝      ██╔══╝  ██║██║   ██║██╔══██║   ██║   ██╔══╝  ██╔══██╗╚════██║
██║ ╚═╝ ██║██║  ██║███████╗███████╗    ██║     ██║╚██████╔╝██║  ██║   ██║   ███████╗██║  ██║███████║
╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝╚══════╝    ╚═╝     ╚═╝ ╚═════╝ ╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝╚══════╝
*/
