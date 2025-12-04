using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    public class Day03 : BaseDay
    {
        private readonly string[] _input;

        public Day03()
        {
            _input = File.ReadAllLines(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var pairs = _input.Select(line => GetLargestOrderedPair(line)).ToList();
            var sum = pairs.Select(p => p.Item1 * 10 + p.Item2).Sum();
            return new($"Output joltage: {sum}");
        }

        public override ValueTask<string> Solve_2()
        {
            var lists = _input.Select(line => GetLargestOrderedInteger(line,12)).ToList();
            var sum = lists.Select(l => ParseListOfIntegersAsInteger(l)).Sum();
            return new($"Output joltage: {sum}");

        }

        private Int64 ParseListOfIntegersAsInteger(List<int> integers)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var integer in integers)
            {
                sb.Append(integer.ToString());
            }
            return Int64.Parse(sb.ToString());
        }

        private List<int> GetLargestOrderedInteger(string integerListAsAString, int lengthOfInteger)
        {
            int[] integers = integerListAsAString.ToCharArray().Select(i => int.Parse(i.ToString())).ToArray();
            List<int> finalListOfIntegers = Enumerable.Repeat(0,lengthOfInteger).ToList();
            for (var i = 0; i < integers.Length; i++)
            {
                var largestIndexToCheck = Math.Min(integers.Length - i, lengthOfInteger);
                var startIndexInList = lengthOfInteger - largestIndexToCheck;
                
                for(var j = 0; j < largestIndexToCheck; j++)
                {
                    //Check all positions and replace if any is strictly larger
                    if (finalListOfIntegers.ElementAt(startIndexInList+j) < integers[i+j])
                    {

                        //Once we have a hit, replace all subsequent positions
                        for (var k = j; k < largestIndexToCheck; k++)
                        {
                            finalListOfIntegers[startIndexInList + k] = integers[i + k];
                        }
                        continue; //No need to check further positions until next integer
                    }
                }
            }
            return finalListOfIntegers;

        }

        private Tuple<int, int> GetLargestOrderedPair(string integerListAsAString)
        {
            int[] integers = integerListAsAString.ToCharArray().Select(i => int.Parse(i.ToString())).ToArray();
            int tenner = -1;
            int single = -1;

            for(var i = 0; i < integers.Length-1; i++)
            {
                if(integers[i] > tenner)
                {
                    tenner = integers[i];
                    single = integers[i+1];
                } else if(integers[i] > single)
                {
                    single = integers[i];
                }
            }
            //Off-by-one check
            if (integers.Last() > single)
            {
                single = integers.Last();
            }
            return new(tenner, single);
        }
    }
}
