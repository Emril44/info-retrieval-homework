using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir1_khomenko
{
    public class PermutationGenerator
    {
        public static List<string> GeneratePermutations(string word)
        {
            List<string> permutations = new List<string>();
            char[] chars = word.ToCharArray();
            int n = chars.Length;

            int[] indices = new int[n];
            for(int i = 0; i < n; i++)
            {
                indices[i] = 0;
            }

            permutations.Add(new string(chars));

            int j = 0;
            while (j < n)
            {
                if (indices[j] < j)
                {
                    // locate wildcard
                    if (chars[j] == '*')
                    {
                        // generate permutations
                        for (char c = 'a'; c <= 'z'; c++)
                        {
                            chars[j] = c;
                            permutations.Add(new string(chars));
                        }

                        // reset wildcard after generation
                        chars[j] = '*';
                    }

                    j++;
                }
                else if (indices[j] == j)
                {
                    Swap(ref chars[j % 2 == 1 ? indices[j] : 0], ref chars[j]);
                    permutations.Add(new string(chars));
                    indices[j]++;
                    j = 0;
                }
                else
                {
                    indices[j] = 0;
                    j++;
                }
            }

            return permutations;
        }

        private static void Swap(ref char a, ref char b)
        {
            (b, a) = (a, b);
        }
    }
}
