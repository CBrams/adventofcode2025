using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Utils
{
    public class Grid<T>
    {
        private GridNode<T>[,] nodes;
        private bool wrap = false;
        public Grid(int width, int height)
        {
            nodes = new GridNode<T>[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new GridNode<T>(default(T),x,y);
                }
            }
        }

        public GridNode<T> GetNode(int x, int y)
        {
            return nodes[x, y];
        }

        public IEnumerable<GridNode<T>> GetDirectNeighbours(int x, int y)
        {
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if(Math.Abs(i) != Math.Abs(j))
                    {
                        int newX = x + i;
                        int newY = y + j;
                        if(newX >= 0 && newX < nodes.GetLength(0) && newY >= 0 && newY < nodes.GetLength(1))
                        {
                            yield return nodes[newX, newY];
                        }
                        if(wrap)
                        {
                            if(newX < 0) newX = nodes.GetLength(0) - 1;
                            if(newX >= nodes.GetLength(0)) newX = 0;
                            if(newY < 0) newY = nodes.GetLength(1) - 1;
                            if(newY >= nodes.GetLength(1)) newY = 0;
                            yield return nodes[newX, newY];
                        }
                    }
                }
            }
        }
        public IEnumerable<GridNode<T>> GetCrossNeighbours(int x, int y)
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (Math.Abs(i) == Math.Abs(j) && i != 0)
                    {
                        int newX = x + i;
                        int newY = y + j;
                        if (newX >= 0 && newX < nodes.GetLength(0) && newY >= 0 && newY < nodes.GetLength(1))
                        {
                            yield return nodes[newX, newY];
                        }
                        if (wrap)
                        {
                            if (newX < 0) newX = nodes.GetLength(0) - 1;
                            if (newX >= nodes.GetLength(0)) newX = 0;
                            if (newY < 0) newY = nodes.GetLength(1) - 1;
                            if (newY >= nodes.GetLength(1)) newY = 0;
                            yield return nodes[newX, newY];
                        }
                    }
                }
            }
        }

        public IEnumerable<GridNode<T>> GetAllNeighbours(int x, int y)
        {
            return GetDirectNeighbours(x, y).Concat(GetCrossNeighbours(x, y));
        }



        public IEnumerable<GridNode<T>> GetAllNodes()
        {
            for(int x = 0; x < nodes.GetLength(0); x++)
            {
                for(int y = 0; y < nodes.GetLength(1); y++)
                {
                    yield return nodes[x, y];
                }
            }
        }

        public static Grid<char> ParseGrid(string input)
        {
            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var grid = new Grid<char>(lines[0].Length, lines.Length);
            for(int y = 0; y < lines.Length; y++)
            {
                for(int x = 0; x < lines[y].Length; x++)
                {
                    grid.GetNode(x, y).Value = lines[y][x];
                }
            }
            return grid;
        }

        public Grid<S> TransformGrid<S>(Func<T,S> transformElementFunction)
        {
            var newGrid = new Grid<S>(nodes.GetLength(0), nodes.GetLength(1));
            for(int x = 0; x < nodes.GetLength(0); x++)
            {
                for(int y = 0; y < nodes.GetLength(1); y++)
                {
                    newGrid.GetNode(x, y).Value = transformElementFunction(nodes[x, y].Value);
                }
            }
            return newGrid;
        }
}

    public class GridNode<T>
    {
        public GridNode(T value, int x, int y)
        {
            Value = value;
            X = x;
            Y = y;
        }

        public T Value { get; set;  } = default(T);
        public int X { get; set; }
        public int Y { get; set; }

    }
}
