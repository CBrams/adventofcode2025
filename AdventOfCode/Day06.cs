using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    public class Day06 : BaseDay
    {
        private readonly string _input;

        public Day06()
        {
            _input = File.ReadAllText(InputFilePath);
        }


        public override ValueTask<string> Solve_1()
        {
            var columns = ReadColumnsOfNumbersAndOperators(_input);
            var results = new List<Int64>();
            foreach (var column in columns)
            {
                var result = PerformOperationOnColumns(column);
                results.Add(result);
            }
            return new($"Results: {results.Sum()}");
        }

        public override ValueTask<string> Solve_2()
        {
            var columns = ReadColumnsFromWeirdDataFormatString(_input);
            var groupedColumns = GroupColumnsSeperatedByEmptyColumns(columns);
            var results = new List<Int64>();
            foreach (var group in groupedColumns)
            {
                var operatorAndNumbers = GetOperatorAndStringsFromColumns(group);
                var result = PerformOperation(operatorAndNumbers.Item1, operatorAndNumbers.Item2);
                results.Add(result);
            }
            return new($"Results: {results.Sum()}");
        }

        public static List<List<string>> ReadColumnsOfNumbersAndOperators(string data)
        {
            string[] lines = data.Split(new[] { "\r\n" }, StringSplitOptions.None);
            List<List<string>> columns = new();
            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for(int j = 0; j < parts.Length; j++)
                {
                    if (columns.Count < j+1)
                    {
                        columns.Add(new List<string>());
                    }
                    columns[j].Add(parts[j].Trim());
                }
            }

            return columns;
        }

        public static Int64 PerformOperationOnColumns(List<string> column)
        {
            string operatorString = column.Last();
            var numbers = column.Take(column.Count - 1).ToList();
            return PerformOperation(operatorString, numbers);
        }


        public static List<List<string>> ReadColumnsFromWeirdDataFormatString(string weirdData)
        {
            string[] lines = weirdData.Split(new[] { "\r\n" }, StringSplitOptions.None);

            string[][] columnsPerLine = lines.Select(line => line.ToCharArray().Select(c => c.ToString()).ToArray()).ToArray();

            int columnCount = columnsPerLine[0].Length;

            List<List<string>> columns = new List<List<string>>();
            for (int i = 0; i < columnCount; i++)
            {
                columns.Add(new List<string>());
            }

            foreach (var line in columnsPerLine)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    columns[i].Add(line[i]);
                }
            }

            return columns;
        }


        public List<List<List<string>>> GroupColumnsSeperatedByEmptyColumns(List<List<string>> columns)
        {
            List<List<List<string>>> groupedColumns = new List<List<List<string>>>();
            List<List<string>> currentGroup = new List<List<string>>();
            foreach (var column in columns)
            {
                if (column.All(c => string.IsNullOrWhiteSpace(c)))
                {
                    if (currentGroup.Count > 0)
                    {
                        groupedColumns.Add(new List<List<string>>(currentGroup));
                        currentGroup.Clear();
                    }
                }
                else
                {
                    currentGroup.Add(column);
                }
            }
            if (currentGroup.Count > 0)
            {
                groupedColumns.Add(new List<List<string>>(currentGroup));
            }
            return groupedColumns;
        }

        public static Tuple<string, List<string>> GetOperatorAndStringsFromColumns(List<List<string>> columns)
        {

            var operatorColumn = columns[0];
            string operatorChar = operatorColumn.Last();
            List<string> numberStrings = new List<string>();
            for (int i = 0; i < columns.Count; i++)
            {
                numberStrings.Add(String.Join("", columns[i].Take(columns[i].Count - 1)));
            }

            return new Tuple<string, List<string>>(operatorChar, numberStrings);
        }

        public static Int64 PerformOperation(string operatorChar, List<string> numbers)
        {
            var parsedNumbers = numbers.Select(n => Int64.Parse(n)).ToList();
            return operatorChar switch
            {
                "+" => parsedNumbers.Sum(),
                "*" => parsedNumbers.Aggregate(1L, (acc, val) => acc * val),
                _ => throw new ArgumentException($"Unsupported operator: {operatorChar}"),
            };
        }
    }
}
