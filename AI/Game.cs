/* ------------------------
 *   The AI (MCTS) Method
 *       By DaweiX
 *  Based on GitHub Repo:
 *  monte-carlo-tree-search
 * ------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace UAV_with_AI.AI
{
    public class GameState
    {
        int[,] _board;

        public GameState(int[,] board, int next)
        {
            Board = board;
            Next = next;
        }

        // x = -1 while o = 1
        
        public int[,] Board
        {
            get { return _board; }
            set
            {
                if (value.Rank != 2)
                    throw new FormatException("The Input Array Must Be 2D.");
                if (value.GetLength(0) != value.GetLength(1))
                    throw new FormatException("The Input Array Must Be a Square.");
                _board = value;
            }
        }

        public int Next { get; set; }

        public int GetResult()
        {
            // Caculate if the game is finished
            int[] sum = new int[8];
            for (int i = 0; i < 3; i++)
            {
                sum[i] = Board[i, 0] + Board[i, 1] + Board[i, 2];
            }
            for (int i = 3; i < 6; i++)
            {
                sum[i] = Board[0, i - 3] + Board[1, i - 3] + Board[2, i - 3];
            }
            sum[6] = Board[0, 0] + Board[1, 1] + Board[2, 2];
            sum[7] = Board[0, 2] + Board[1, 1] + Board[2, 0];

            if (sum.Contains(3)) return 1;        // O WINs
            if (sum.Contains(-3)) return -1;      // X WINs
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Board[i, j] == 0)
                        // We make 100 represent Unfinished here.
                        return 100;
                }
            }
            return 0;                             // Draw
        }

        internal bool IsOver()
        {
            return !(GetResult() == 100);
        }

        public bool IsMoveLegal(Move move)
        {
            if (move.Side != Next) return false;
            if (move.X < 0 || move.X > 2) return false;
            if (move.Y < 0 || move.Y > 2) return false;
            return Board[move.X, move.Y] == 0;
        }

        public GameState TakeStep(Move move)
        {
            if (!IsMoveLegal(move))
                throw new Exception("Move is Not Legal.");
            int[,] newBoard = (int[,])Board.Clone();
            newBoard[move.X, move.Y] = move.Side;
            int next = Next == 1 ? -1 : 1;
            return new GameState(newBoard, next);
        }

        public List<string> GetLegalActions()
        {
            List<string> list = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // The grid is in the board, and it's empty, then it's legal
                    if (Board[i, j] == 0)
                    {
                        list.Add(string.Format("{0}{1}{2}", i, j, Next));
                    }
                }
            }
            return list;
        }

        public class Move
        {
            public int Side { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public Move(int x, int y, int side)
            {
                Side = side;
                X = x;
                Y = y;
            }
        }
    }
}
