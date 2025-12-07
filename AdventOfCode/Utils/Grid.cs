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
            int newX = x;
            int newY = y;
            if (wrap)
            {
                if (newX < 0) newX = nodes.GetLength(0) - 1;
                if (newX >= nodes.GetLength(0)) newX = 0;
                if (newY < 0) newY = nodes.GetLength(1) - 1;
                if (newY >= nodes.GetLength(1)) newY = 0;
                return nodes[newX, newY];
            }
            if (x < 0 || x >= nodes.GetLength(0) || y < 0 || y >= nodes.GetLength(1))
            {
                return null;
            }
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
                        var node = GetNode(x+i, y+j);
                        if(node != null)
                        {
                            yield return node;
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
                        var node = GetNode(x + i, y + j);
                        if (node != null)
                        {
                            yield return node;
                        }
                    }
                }
            }
        }

        public IEnumerable<GridNode<T>> GetAllNeighbours(int x, int y)
        {
            return GetDirectNeighbours(x, y).Concat(GetCrossNeighbours(x, y));
        }

        public GridNode<T> GetNeighbour(int x, int y, Direction direction)
        {
            switch (direction)
            {
                case Direction.StraightUp:
                    return GetNode(x, y - 1);
                case Direction.StraightDown:
                    return GetNode(x, y + 1);
                case Direction.StraightLeft:
                    return GetNode(x - 1, y);
                case Direction.StraightRight:
                    return GetNode(x + 1, y);
                case Direction.UpRight:
                    return GetNode(x + 1, y - 1);
                case Direction.UpLeft:
                    return GetNode(x - 1, y - 1);
                case Direction.DownRight:
                    return GetNode(x + 1, y + 1);
                case Direction.DownLeft:
                    return GetNode(x - 1, y + 1);
                default:
                    throw new ArgumentException("Invalid direction");
            }
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
        public int Width
        {
            get { return nodes.GetLength(0); }
        }
        public int Height
        {
            get { return nodes.GetLength(1); }
        }

        public void PrintGrid()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Console.Write(nodes[x, y].Value);
                }
                Console.WriteLine();
            }
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

        public override bool Equals(object obj)
        {
            return obj is GridNode<T> node &&
                   EqualityComparer<T>.Default.Equals(Value, node.Value) &&
                   X == node.X &&
                   Y == node.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, X, Y);
        }
    }

    public enum Direction
    {
        StraightUp,
        StraightDown,
        StraightLeft,
        StraightRight,
        UpRight,
        UpLeft,
        DownRight,
        DownLeft
    }
}
