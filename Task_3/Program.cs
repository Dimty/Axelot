using System;
using System.Collections;

namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            var chars = new[] {'a', 'b', 'c'};

            foreach (var item in GetPermutationWithoutRepetition(chars))
            {
                Console.WriteLine(item);
            }
        }

        static IEnumerable<string> GetPermutationWithoutRepetition(IEnumerable<char> chars)
        {
            List<char> list = new(chars);
            foreach (var item in GetPermutationWithoutRepetitionRecursion(list))
            {
                yield return item;
            }
        }

        static IEnumerable<string> GetPermutationWithoutRepetitionRecursion(List<char> list)
        {
            if (list.Count == 0)
            {
                yield return string.Empty;
            }

            foreach (var item in list.ToList())
            {
                list.Remove(item);
                foreach (var prevRecursionResult in GetPermutationWithoutRepetitionRecursion(list))
                {
                    yield return item + prevRecursionResult;
                }

                list.Add(item);
            }
        }
    }
}