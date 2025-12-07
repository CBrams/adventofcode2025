using AdventOfCode.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    internal class Day07 : BaseDay
    {
        private readonly string _input;

        public Day07()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            var grid = Grid<char>.ParseGrid(_input);
            var startNodes = grid.GetAllNodes().Where(n => n.Value.Equals('S')).ToList();
            var currentNodes = new List<GridNode<char>>(startNodes);
            int splits = 0;
            while (currentNodes.Count > 0)
            {
                var nextNodes = currentNodes.Select(n => grid.GetNeighbour(n.X, n.Y,Direction.StraightDown)).ToList();
                var splitterNodes = nextNodes.Where(n => n != null && n.Value.Equals('^')).ToList();
                var nonSplitterNodes = nextNodes.Where(n => n != null && !n.Value.Equals('^')).ToList();

                var splitToNodes = splitterNodes.SelectMany(n => new List<GridNode<char>> { grid.GetNeighbour(n.X, n.Y, Direction.StraightLeft),
                    grid.GetNeighbour(n.X, n.Y, Direction.StraightRight)
                }).Where(n => n != null).ToList();

                var allNextNodes = nonSplitterNodes.Union(splitToNodes).ToList();
                splits = splits + splitterNodes.Count;
                currentNodes = allNextNodes;
            }

            return new($"Total splits: {splits}");
        }

        public override ValueTask<string> Solve_2()
        {
            var grid = Grid<char>.ParseGrid(_input);
            var valueGrid = grid.TransformGrid(c => Int64.Parse("0")); //Why is this the easiest way to initialize Int64 as zero
            var startNodes = grid.GetAllNodes().Where(n => n.Value.Equals('S')).ToList();
            valueGrid.GetNode(startNodes[0].X, startNodes[0].Y).Value = 1;
            var currentNodes = new List<GridNode<char>>(startNodes);
            while (currentNodes.Count > 0)
            {
                var nextNodes = currentNodes.Select(n => grid.GetNeighbour(n.X, n.Y, Direction.StraightDown)).ToList();
                var splitterNodes = nextNodes.Where(n => n != null && n.Value.Equals('^')).ToList();
                var nonSplitterNodes = nextNodes.Where(n => n != null && !n.Value.Equals('^')).ToList();

                foreach (var n in nonSplitterNodes)
                {
                    valueGrid.GetNode(n.X, n.Y).Value += valueGrid.GetNeighbour(n.X, n.Y, Direction.StraightUp).Value;
                }

                var splitLeftNodes = splitterNodes.Select(n => grid.GetNeighbour(n.X, n.Y, Direction.StraightLeft)).Where(n => n != null).ToList();
                foreach (var n in splitLeftNodes)
                {
                    var splitterNode = grid.GetNeighbour(n.X, n.Y, Direction.StraightRight);
                    var originatorNode = grid.GetNeighbour(splitterNode.X, splitterNode.Y, Direction.StraightUp);
                    valueGrid.GetNode(n.X, n.Y).Value += valueGrid.GetNode(originatorNode.X,originatorNode.Y).Value;
                }

                var splitRightNodes = splitterNodes.Select(n => grid.GetNeighbour(n.X, n.Y, Direction.StraightRight)).Where(n => n != null).ToList();
                foreach (var n in splitRightNodes)
                {
                    var splitterNode = grid.GetNeighbour(n.X, n.Y, Direction.StraightLeft);
                    var originatorNode = grid.GetNeighbour(splitterNode.X, splitterNode.Y, Direction.StraightUp);
                    valueGrid.GetNode(n.X, n.Y).Value += valueGrid.GetNode(originatorNode.X, originatorNode.Y).Value;
                }




                var allNextNodes = nonSplitterNodes.Union(splitLeftNodes).Union(splitRightNodes).ToList();
                currentNodes = allNextNodes;
            }

            var bottomNodes = valueGrid.GetAllNodes().Where(n => n.Y == valueGrid.Height - 1).ToList();

            return new($"Total timelines: {bottomNodes.Select(bottomNode => bottomNode.Value).Sum()}");
        }



    }
}
