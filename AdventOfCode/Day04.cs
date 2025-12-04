using AdventOfCode.Utils;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    public class Day04 : BaseDay
    {
        private readonly string _input;

        public Day04()
        {
            _input = File.ReadAllText(InputFilePath);
        }

        public override ValueTask<string> Solve_1()
        {
            var grid = Grid<char>.ParseGrid(_input).TransformGrid(c => c.Equals('@') ? true : false);
            var spacesWithRolls = grid.GetAllNodes().Where(node => node.Value).ToList();
            var spacesWithRollsWithLessThanFourNeighboursWithRools = spacesWithRolls.Where(node =>
            {
                var neighbours = grid.GetAllNeighbours(node.X, node.Y).ToList();
                var neighboursWithRolls = neighbours.Where(n => n.Value).Count();
                return neighboursWithRolls < 4;
            }).ToList();

            return new($"Number of rolls with fewer than 4 neighbours: {spacesWithRollsWithLessThanFourNeighboursWithRools.Count}");
        }

        public override ValueTask<string> Solve_2()
        {
            var removedRolls = 0;

            var grid = Grid<char>.ParseGrid(_input).TransformGrid(c => c.Equals('@') ? true : false);
            var spacesWithLessThanFourNeighbours = GetRollsWithLessThatFourNeighbours(grid, grid.GetAllNodes());
            while(spacesWithLessThanFourNeighbours.Count > 0)
            {
                removedRolls = removedRolls + RemoveRollsFromSpaces(spacesWithLessThanFourNeighbours);
                spacesWithLessThanFourNeighbours = GetRollsWithLessThatFourNeighbours(grid, grid.GetAllNodes());
            }
            return new($"Total number of removed rolls: {removedRolls}");
        }

        private List<GridNode<bool>> GetRollsWithLessThatFourNeighbours(Grid<bool>grid, IEnumerable<GridNode<bool>> nodes)
        {
            var spacesWithRolls = nodes.Where(node => node.Value).ToList();
            var spacesWithRollsWithLessThanFourNeighboursWithRools = spacesWithRolls.Where(node =>
            {
                var neighbours = grid.GetAllNeighbours(node.X, node.Y).ToList();
                var neighboursWithRolls = neighbours.Where(n => n.Value).Count();
                return neighboursWithRolls < 4;
            }).ToList();
            return spacesWithRollsWithLessThanFourNeighboursWithRools;
        }

        private int RemoveRollsFromSpaces(List<GridNode<bool>> nodes)
        {
            int removedRolls = 0;
            foreach (var node in nodes)
            {
                node.Value = false;
                removedRolls++;
            }
            return removedRolls;
        }

    }
}
