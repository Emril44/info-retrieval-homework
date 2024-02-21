using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    public class Trie
    {
        private readonly Node _root;

        public Trie()
        {
            _root = new Node('^', null, 0);
        }

        public Node Prefix(string str)
        {
            var currentNode = _root;
            var result = currentNode;

            foreach (var chr in str)
            {
                currentNode = currentNode.FindChildNode(chr);
                if(currentNode == null)
                {
                    break;
                }

                result = currentNode;
            }

            return result;
        }

        public bool Search(string str)
        {
            var prefix = Prefix(str);

            return prefix.Depth == str.Length && prefix.Children.ContainsKey('$');
        }

        public void InsertRange(List<string> strings)
        {
            for(int i = 0; i < strings.Count; i++)
            {
                Insert(strings[i]);
            }
        }

        public void Insert(string str)
        {
            Console.WriteLine($"Inserting {str}");
            Node current = _root;

            for(var i = current.Depth; i < str.Length; i++)
            {
                Node newNode = new(str[i], current, current.Depth + 1);
                current.Children[str[i]] = newNode;
                current = newNode;
            }

            current.Children['$'] = new Node('$', current, current.Depth + 1);

            PrintTrie(_root, "");
        }

        public void Delete(string str)
        {
            Console.WriteLine($"Deleting {str}");
            if(Search(str))
            {
                var node = Prefix(str).FindChildNode('$');

                node.Parent.DeleteChildNode('$');

                while(node.IsLeaf() && node.Parent != _root)
                {
                    var parent = node.Parent;
                    parent.DeleteChildNode(node.Value);
                    node = parent;
                }
            }
        }

        public void PrintTrie(Node node, string prefix)
        {
            Console.WriteLine($"{prefix} - {node.Value}");
            foreach (var child in node.Children)
            {
                PrintTrie(child.Value, prefix + node.Value);
            }
        }
    }
}
