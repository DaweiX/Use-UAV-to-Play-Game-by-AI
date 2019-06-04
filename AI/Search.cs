/* -------------------------------------
 *      The Search Mehtod of MCTS
 *              By DaweiX
 * ------------------------------------*/

using System;

namespace UAV_with_AI.AI
{
    public class Search
    {
        public Node Root { get; }

        public Search(Node node)
        {
            Root = node;
        }

        public Node FindCurrentNode()
        {
            // Find a leaf node and expand it
            var current = Root;
            while (!Method.IsLeafNode(current))
            {
                if (!Method.IsFullyExpanded(current))
                {
                    return Method.Expand(current);
                }
                else
                {
                    current = Method.SelectBest(current);
                }
            }
            return current;
        }

        public Node BestAction(int sim)
        {
            for (int i = 0; i < sim; i++)
            {
                var node = FindCurrentNode();
                var reward = Method.Rollout(node);
                if (reward == 100) throw new Exception("Rollout Ended with Game Unfinished.");
                Method.BackPropagate(node, reward);
            }

            Nums.Tree = Root;

            // When get the result, don't explore
            return Method.SelectBest(Root, c: 0);
        }
    }
}
