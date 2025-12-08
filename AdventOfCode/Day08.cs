using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode
{
    public class Day08 : BaseDay
    {
        private readonly string[] _input;

        public Day08()
        {
            _input = _input = File.ReadAllLines(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var coordinates = ParseCoordinates(_input);
            var junctionBoxes = coordinates.Select(c => new JunctionBox { Position = c }).ToList();
            var connections = CalculateConnections(junctionBoxes);
            connections.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            var graphSpace = new GraphSpace
            {
                Boxes = junctionBoxes,
            };
            for(int i = 0; i < (junctionBoxes.Count > 20 ? 1000 : 10); i++)
            {
                var connection = connections[i];
                graphSpace.JoinGraphs(connection.BoxA, connection.BoxB);
            }
            var groupsOfJunctionBoxes = junctionBoxes.GroupBy(box => graphSpace.FindAncestor(box)).OrderByDescending(g => g.Count()).ToList();

            return new($"Product of the 3 biggest sizes: ({groupsOfJunctionBoxes[0].Count()} * {groupsOfJunctionBoxes[1].Count()} * {groupsOfJunctionBoxes[2].Count()}) = {groupsOfJunctionBoxes[0].Count() * groupsOfJunctionBoxes[1].Count() * groupsOfJunctionBoxes[2].Count()}");
        }

        public override ValueTask<string> Solve_2()
        {
            var coordinates = ParseCoordinates(_input);
            var junctionBoxes = coordinates.Select(c => new JunctionBox { Position = c }).ToList();
            var connections = CalculateConnections(junctionBoxes);
            connections.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            var graphSpace = new GraphSpace
            {
                Boxes = junctionBoxes,
            };
            var numberOfCircuits = junctionBoxes.Count;
            int i = 0;
            JunctionBox lastBoxA = null;
            JunctionBox lastBoxB = null;
            while (numberOfCircuits > 1 && i < connections.Count)
            {
                var connection = connections[i];
                var joinedCircuit = graphSpace.JoinGraphs(connection.BoxA, connection.BoxB);
                if(joinedCircuit)
                    numberOfCircuits--;
                i++;
                if(numberOfCircuits == 1)
                {
                    lastBoxA = connection.BoxA;
                    lastBoxB = connection.BoxB;
                    break;
                }
            }
            return new($"Product of the X coordinate of the last boxes: ({lastBoxA.Position.Item1},{lastBoxA.Position.Item2},{lastBoxA.Position.Item3}) and ({lastBoxB.Position.Item1},{lastBoxB.Position.Item2},{lastBoxB.Position.Item3}) gives {lastBoxA.Position.Item1} * {lastBoxB.Position.Item1}) = {lastBoxA.Position.Item1 * lastBoxB.Position.Item1}");

        }

        public HashSet<Tuple<Int64,Int64,Int64>> ParseCoordinates(string[] data)
        {
            var coordinates = new HashSet<Tuple<Int64, Int64, Int64>>();
            foreach (var line in data)
            {
                var parts = line.Split(',');
                var x = Int64.Parse(parts[0]);
                var y = Int64.Parse(parts[1]);
                var z = Int64.Parse(parts[2]);
                coordinates.Add(new Tuple<Int64, Int64, Int64>(x, y, z));
            }
            return coordinates;
        }

        public Int64 CalculateEuclidianDistance(Tuple<Int64, Int64, Int64> pointA, Tuple<Int64, Int64, Int64> pointB)
        {
            var deltaX = pointA.Item1 - pointB.Item1;
            var deltaY = pointA.Item2 - pointB.Item2;
            var deltaZ = pointA.Item3 - pointB.Item3;
            return (Int64)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        public List<Connection> CalculateConnections(List<JunctionBox> boxes)
        {
            var connections = new List<Connection>();
            var boxesCovered = new HashSet<JunctionBox>();
            foreach (var boxA in boxes)
            {
                boxesCovered.Add(boxA);
                foreach (var boxB in boxes)
                {
                    if(boxesCovered.Contains(boxB))
                        continue;
                    var distance = CalculateEuclidianDistance(boxA.Position, boxB.Position);
                    connections.Add(new Connection
                    {
                        BoxA = boxA,
                        BoxB = boxB,
                        Distance = distance
                    });
                }
            }
            return connections;
        }

        public class JunctionBox
        {
            public Tuple<Int64, Int64, Int64> Position { get; set; }
            public override bool Equals(object obj)
            {
                return obj is JunctionBox box &&
                       Position.Item1 == box.Position.Item1 && Position.Item2 == box.Position.Item2 && Position.Item3 == box.Position.Item3;
            }
        }

        public class Connection
        {
            public JunctionBox BoxA { get; set; }
            public JunctionBox BoxB { get; set; }
            public Int64 Distance { get; set; }
        }

        public class GraphSpace
        {
            public List<JunctionBox> Boxes { get; set; } = new List<JunctionBox>();

            public Dictionary<JunctionBox, JunctionBox> AncestorMap { get; set; } = new();

            public JunctionBox FindAncestor(JunctionBox box)
            {
                if (!AncestorMap.ContainsKey(box))
                {
                    AncestorMap[box] = box;
                    return box;
                }
                if (!AncestorMap[box].Equals(box))
                {
                    AncestorMap[box] = FindAncestor(AncestorMap[box]);
                    return AncestorMap[box];
                }
                return box;
            }

            public bool JoinGraphs(JunctionBox boxA, JunctionBox boxB)
            {
                var ancestorA = FindAncestor(boxA);
                var ancestorB = FindAncestor(boxB);
                if (EqualityComparer<JunctionBox>.Default.Equals(ancestorA, ancestorB))
                    return false; //We didn't join anything, they are already connected
                AncestorMap[ancestorB] = ancestorA;
                return true; //We joined them
            }
        }
    }
}
