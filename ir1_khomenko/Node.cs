using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    public class Node
    {
        public char Value { get; set; }
        public Dictionary<char, Node> Children { get; set; }
        public Node Parent { get; set; }
        public int Depth { get; set; }

        public Node(char value, Node parent, int depth)
        {
            Value = value;
            Children = new();
            Parent = parent;
            Depth = depth;
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public Node FindChildNode(char c)
        {
            if(Children.ContainsKey(c)) return Children[c];

            return null;
        }

        public void DeleteChildNode(char c)
        {
            if (Children.ContainsKey(c))
                Children.Remove(c);
        }
    }
}
