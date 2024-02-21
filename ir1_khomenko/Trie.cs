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

            return prefix.Depth == str.Length && prefix.FindChildNode('$') != null;
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
            var commonPrefix = Prefix(str);
            var current = commonPrefix;

            for(var i = current.Depth; i < str.Length; i++)
            {
                var newNode = new Node(str[i], current, current.Depth + 1);
                
                current.Children.Add(newNode);
                current = newNode;
            }

            current.Children.Add(new Node('$', current, current.Depth + 1));
        }

        public void Delete(string str)
        {
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
    }
}
