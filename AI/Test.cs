// By DaweiX

using System.Collections.Generic;
using System.Diagnostics;

namespace UAV_with_AI.AI
{
    static class Test
    {
        public static void GoSim()
        {
            if (Nums.Nodes == null) Nums.Nodes = new List<Node>();

            int[,] board = new int[3, 3]
            {
            { -1, 0 ,0 },
            { 0, 1, 0 },
            { -1, 0, 0 },
            };

            int[,] board_zero = new int[3, 3];
            GameState state = new GameState(board_zero, 1);
            var search = new Search(new Node(state, null));
            Node best = search.BestAction(50);

            Debug.WriteLine("**********************");
            Log.PrintMat(best.State.Board);
            var a = Nums.Steps;
            var result = best.Results;
            Debug.WriteLine((double)(result.O + result.Draw) / (double)(result.O + result.X + result.Draw));
            Nums.IsReady = true;
        }

        public static Node GiveTheBestStep(int[,] board, int next)
        {
            if (Nums.Tree == null)
            {
                Debug.WriteLine("I can't give an intelligence step because I've not been trained.");
            }
            var tree = Nums.Tree;
            Node root = null;
            foreach (var node in tree.Nodes)
            {
                if (node.State.Board == board && node.State.Next == next)
                {
                    root = node;
                    break;
                }
            }
            if (root == null)
            {
                Debug.WriteLine("This step is not trained yet. Will take a random step.");
                return null;
            }
            else
            {
                var result = Method.SelectBest(root, 0);
                Log.PrintMat(result.State.Board, "THE RESULT");
                return result;
            }
        }
    }
}
