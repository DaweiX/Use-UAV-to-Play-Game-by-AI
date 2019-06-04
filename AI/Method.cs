/* -------------------------------------
 *      The Util Methods of MCTS
 *              By DaweiX
 *         Based on GitHub Repo:
 *        monte-carlo-tree-search
 * ------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;


namespace UAV_with_AI.AI
{
    public static class Method
    {
        public static Node SelectBest(Node node, double c = 1.4)
        {
            // Selecting best child by UCT. C = sqrt(2)
            List<double> weights = new List<double>();
            foreach (var item in node.Children)
            {
                // The UCT formula
                weights.Add((item.Q / item.N) + c * Math.Sqrt(2 * Math.Log(Nums.N / item.N)));
            }
            double[] array = new double[weights.Count];
            // NOTE: shallow copy used
            weights.CopyTo(array);
            var sort = array.ToList();
            sort.Sort();
            var best = node.Children[weights.IndexOf(weights.Max())];
            return best;
        }

        public static GameState.Move Rollout_Rand(GameState.Move[] moves)
        {
            var roll = moves[new Random().Next(0, moves.Length - 1)];
            return roll;
        }

        public static Node Expand(Node node)
        {
            var actions = node.UntriedNodes;
            var action = actions[new Random().Next(0, actions.Count)];
            node.UntriedNodes.Remove(action);
            var state = new GameState(node.State.Board, node.State.Next);
            GameState nextState = state.TakeStep(new GameState.Move(action.X, action.Y, action.Side));
            //Debug.WriteLine("EXPAND");
            //Log.PrintMat(nextState.Board);
            var child = new Node(nextState, node);
            node.Children.Add(child);
            Nums.Nodes.Add(child);
            Nums.Steps.Add(new Step
            {
                Move = action,
                Type = StepType.EXPAND,
                CurrentBoard = nextState.Board
            });
            return child;
        }

        public static void BackPropagate(Node node, int result)
        {
            node.N++;
            // Nums.N is static
            Nums.N++;

            if (result == 1) node.Results.O++;
            if (result == -1) node.Results.X++;
            if (result == 0) node.Results.Draw++;

            if (node.Parent != null)
            {
                // Recursion to the root node
                BackPropagate(node.Parent, result);

                // Max-min principle
                // BackPropagate(node.Parent, -result);
            }
        }

        public static int Rollout(Node node)
        {
            var currentState = node.State;
            if (currentState.IsOver())
            {
                //Log.PrintMat(node.State.Board);
            }
            while (!currentState.IsOver())
            {
                var moves = currentState.GetLegalActions();
                List<GameState.Move> list = new List<GameState.Move>();
                foreach (var item in moves)
                {
                    if (item[2] == '-')
                        list.Add(new GameState.Move(item[0] - '0', item[1] - '0', -1));
                    else
                        list.Add(new GameState.Move(item[0] - '0', item[1] - '0', 1));
                }
                var step = Rollout_Rand(list.ToArray());
                currentState = currentState.TakeStep(step);
                //Debug.WriteLine("ROLLOUT");
                //Log.PrintMat(currentState.Board);
                Nums.Steps.Add(new Step
                {
                    Move = step,
                    Type = StepType.ROLLOUT,
                    CurrentBoard = currentState.Board
                });
            }
            int result = currentState.GetResult();
            Nums.i++;
            // Debug.WriteLine("----------" + Nums.i.ToString() + "----------:" + result.ToString());
            Nums.Steps.Add(new Step
            {
                Type = StepType.OUTCOME,
                Result = result,
                CurrentBoard = currentState.Board
            });
            return result;
        }

        public static bool IsLeafNode(Node node)
        {
            return node.State.IsOver();
        }

        public static bool HasChild(Node node)
        {
            return node.Children.Count != 0;
        }

        public static bool IsFullyExpanded(Node node)
        {
            // This can also be implemented by Hash-Table
            if (node.UntriedNodes == null)
            {
                return false;
            }
            return node.UntriedNodes.Count == 0;
        }
    }
}
