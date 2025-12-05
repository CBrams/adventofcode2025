using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode
{
    public class Day05 : BaseDay
    {

        private readonly string[] _input;

        public Day05()
        {
            _input = File.ReadAllLines(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var parseResults = ParseInventoryManagementSystem(_input);
            var ingredients = parseResults.Item2;
            var ranges = parseResults.Item1;

            var freshIngredients = ingredients.Where(i => IsNumberInAnyRange(ranges, i));
            return new($"Fresh ingredients: {freshIngredients.Count()}");
        }

        public override ValueTask<string> Solve_2()
        {
            var parseResults = ParseInventoryManagementSystem(_input);
            var ranges = parseResults.Item1;

            var mergedRanges = MergeRanges(ranges);
           
            var numberOfFreshIngredients = CountCoveredByNonOverlappingRanges(mergedRanges);

            return new($"Fresh ingredients: {numberOfFreshIngredients}");

        }

        private Int64 CountCoveredByNonOverlappingRanges(List<Tuple<Int64, Int64>> ranges)
        {
            var lengths = ranges.Select(r =>  r.Item2 - r.Item1 + 1).ToList();
            var totalCountCovered = lengths.Sum();
            return totalCountCovered;
        }

        public static List<Tuple<Int64, Int64>> MergeRanges(List<Tuple<Int64, Int64>> ranges)
        {
            if (ranges == null || ranges.Count == 0)
                return new ();

            ranges.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            List<Tuple<Int64, Int64>> merged = new();
            merged.Add(ranges[0]);

            foreach (var current in ranges.Skip(1))
            {
                var last = merged.Last();
                if (current.Item1 <= last.Item2)
                {
                    Int64 newStart = last.Item1;
                    Int64 newEnd = Math.Max(last.Item2, current.Item2);
                    merged[merged.Count - 1] = Tuple.Create(newStart, newEnd);
                }
                else
                {
                    merged.Add(current);
                }
            }

            return merged;
        }

        private bool IsNumberInAnyRange(List<Tuple<Int64,Int64>> ranges, Int64 number)
        {
            var rangesThatStartBefore = ranges.Where(r => r.Item1 <= number);
            var rangesThatEndAfter = rangesThatStartBefore.Where(r => r.Item2 >= number);
            return rangesThatEndAfter.Any();
        }

        private Tuple<List<Tuple<Int64,Int64>>,List<Int64>> ParseInventoryManagementSystem(string[] inputLines)
        {
            List<Tuple<Int64, Int64>> freshRanges = new();
            List<Int64> ingredients = new();
            foreach(var inputLine in inputLines)
            {
                var rangeRegex = new Regex(@"^(\d+)-(\d+)$");
                var ingredientRegex = new Regex(@"^(\d+)$");

                var rangeMatch = rangeRegex.Match(inputLine);
                if (rangeMatch.Success)
                {
                    Int64 startRange = Int64.Parse(rangeMatch.Groups[1].Value); //0 index is whole matched string
                    Int64 endRange = Int64.Parse(rangeMatch.Groups[2].Value);

                    freshRanges.Add(new Tuple<Int64, Int64>(startRange, endRange));
                    continue;
                }

                var ingredientMatch = ingredientRegex.Match(inputLine);
                if (ingredientMatch.Success)
                {
                    Int64 ingredient = Int64.Parse(ingredientMatch.Groups[1].Value);
                    ingredients.Add(ingredient);
                    continue;
                }
            }

            return new Tuple<List<Tuple<Int64, Int64>>, List<Int64>>(freshRanges, ingredients);
        }
    }
}
