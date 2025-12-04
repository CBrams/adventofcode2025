using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    public class Day02 : BaseDay
    {
        private readonly string _input;

        public Day02()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            List<string> ranges = _input.Split(",").ToList();
            List<(Int64, Int64)> parsedRanges = ranges.Select(r => ParseRange(r)).ToList();

            List<Int64> numbersWithRepeats = new();

            foreach(var (start, end) in parsedRanges)
            {
                var numbers = GetNumbersWithRepeatsInRange(start, end);
                numbersWithRepeats.AddRange(numbers);
            }

            return new ValueTask<string>(numbersWithRepeats.Sum().ToString());
        }

        public override ValueTask<string> Solve_2()
        {
            List<string> ranges = _input.Split(",").ToList();
            List<(Int64, Int64)> parsedRanges = ranges.Select(r => ParseRange(r)).ToList();

            List<Int64> numbersWithRepeats = new();

            foreach (var (start, end) in parsedRanges)
            {
                var numbers = GetNumbersWithRepeatsInRange(start, end, onlyRepeatTwice: false);
                numbersWithRepeats.AddRange(numbers);
            }

            return new ValueTask<string>(numbersWithRepeats.Sum().ToString());
        }

        private (Int64, Int64) ParseRange(string range)
        {
            var parts = range.Split("-");
            return (Int64.Parse(parts[0]), Int64.Parse(parts[1]));
        }

        private List<Int64> GetNumbersWithRepeatsInRange(Int64 start, Int64 end, bool onlyRepeatTwice = true)
        {
            List<Int64> numbers = new();
            for(Int64 i = start; i <= end; i++)
            {
                if(onlyRepeatTwice ? IsRepeatedTwice(i) : ConsistsOnlyOfRepeatingParts(i))
                {
                    numbers.Add(i);
                }
            }
            return numbers;
        }

        private bool IsRepeatedTwice(Int64 number)
        {
            string numStr = number.ToString();
            if(numStr.Length % 2 != 0)
            {
                return false;
            }
            var firstHalf = numStr.Substring(0, numStr.Length / 2);
            var secondHalf = numStr.Substring(numStr.Length / 2);
            if(firstHalf == secondHalf)
            {
                return true;
            }
            return false;
        }

        private bool ConsistsOnlyOfRepeatingParts(Int64 number)
        {  
            string numStr = number.ToString();
            for(int partLength = 1; partLength <= numStr.Length / 2; partLength++)
            {
                if(numStr.Length % partLength != 0)
                {
                    continue;
                }
                string part = numStr.Substring(0, partLength);
                StringBuilder sb = new StringBuilder();
                int repeatCount = numStr.Length / partLength;
                for(int i = 0; i < repeatCount; i++)
                {
                    sb.Append(part);
                }
                if(sb.ToString() == numStr)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
