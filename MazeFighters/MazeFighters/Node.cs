using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeFighters
{
    /// <summary>
    /// Basicly a spot in the maze.
    /// </summary>
    
    class Node
    {
        private int posRow;
        private int posCol;

        public int PosRow { get => posRow; set => posRow = value; }
        public int PosCol { get => posCol; set => posCol = value; }

        public Node(int posRow, int posCol)
        {
            PosRow = posRow;
            PosCol = posCol;
        }

        

        /*
         * Broken hopes and dreams
        private int[] position; // position of current node [row, column]
        private List<Node> choices = new List<Node>(); // towards the choises not yet visited
        private List<Node> visited = new List<Node>(); // list of nodes already visited when at this node
        // visited list includes parent node and child node.

        // Constructor
        public Node(int[] position)
        {
            this.position = position;
        }

        // Getters and setters
        public int[] Position { get => position; set => position = value; }
        public List<Node> Choices { get => choices; set => choices = value; } // what's up with internal?
        internal List<Node> Visited { get => visited; set => visited = value; }

        /// Methods
        */



    }
}
