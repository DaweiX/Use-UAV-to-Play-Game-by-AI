/* -------------------------------------
 *      The Classes Def of MCTS
 *              By DaweiX
 * ------------------------------------*/

using System.Collections.Generic;

namespace UAV_with_AI.AI
{
    public enum StepType
    {
        ROLLOUT,
        EXPAND,
        TAKESTEP,
        OUTCOME,        // when a game is finished
    }

    public class Step
    {
        public GameState.Move Move { get; set; }

        public StepType Type { get; set; }

        public int Result { get; set; }

        public int[,] CurrentBoard { get; set; }
    }

    public class Result
    {
        public int X { get; set; }
        public int O { get; set; }
        public int Draw { get; set; }
    }

    public class Node
    {
        List<GameState.Move> _UntriedNodes = null;

        public List<Node> Nodes { get; set; }

        public GameState State { get; set; }

        public Node Parent { get; set; }

        public List<Node> Children { get; set; }

        public Result Results { get; set; }

        public List<GameState.Move> UntriedNodes
        {
            get
            {
                if (_UntriedNodes == null)
                {
                    List<GameState.Move> list = new List<GameState.Move>();
                    var array = State.GetLegalActions();
                    foreach (var item in array)
                    {
                        if (item[2] == '-')
                            list.Add(new GameState.Move(item[0] - '0', item[1] - '0', -1));
                        else
                            list.Add(new GameState.Move(item[0] - '0', item[1] - '0', 1));
                    }
                    _UntriedNodes = list;
                }
                return _UntriedNodes;
            }
            set
            {
                _UntriedNodes = value;
            }
        }

        public double Q
        {
            // Here, we make Q a readonly property
            get
            {
                int next = Parent.State.Next;
                int sign = next == 1 ? 1 : -1;
                return sign * (Results.O - Results.X);
            }
        }

        public int N { get; set; } = 0;

        public Node(GameState state, Node parent)
        {
            State = state;
            Parent = parent;
            Children = new List<Node>();
            Results = new Result();
            Nodes = new List<Node>();
        }

    }
}
