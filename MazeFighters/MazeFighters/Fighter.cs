using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Dynamic fighter object.
/// </summary>

namespace MazeFighters
{
    class Fighter
    {
        private int id;
        private int hp;
        private int posRow;
        private int posCol;
        private List<int> bonuses; // when he picks up an item
        private bool defensive;
        private int backtrack; // if backtracking, how many times he has done so

        public Fighter(int id) // Constructor with custom args.
        {
            this.id = id;
            hp = 100;
            bonuses = new List<int>();
            bonuses.Add(10);
            defensive = true;
            Random rnd = new Random();
            if (rnd.Next(1,11) > 3)
            {
                defensive = false;
            }
            backtrack = 0;
        }

        // Getters and setters.
        public int Id { get => id; set => id = value; }
        public int Hp { get => hp; set => hp = value; }
        public int PosRow { get => posRow; set => posRow = value; }
        public int PosCol { get => posCol; set => posCol = value; }
        public List<int> Bonuses { get => bonuses; set => bonuses = value; }
        public bool Defensive { get => defensive; set => defensive = value; }
        public int Backtrack { get => backtrack; set => backtrack = value; }

        /// Methods

        // Moves the fighter to his new position.
        public void Move(int posRow, int posCol)
        {
            this.posRow = posRow;
            this.posCol = posCol;
        }
    }
}
