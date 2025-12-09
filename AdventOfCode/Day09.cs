using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AdventOfCode
{
    public class Day09 : BaseDay
    {

        private readonly string[] _input;

        public Day09()
        {
            _input = _input = File.ReadAllLines(InputFilePath);
        }
        public override ValueTask<string> Solve_1()
        {
            var vertices = ParseVertices(_input);
            int noOfVertices = vertices.Count;
            Int64 maxArea = -1;
            for (int i = 0; i < noOfVertices; i++)
            {
                var v1 = vertices[i];
                for(int j = i + 1; j < noOfVertices; j++)
                {
                    var v2 = vertices[j];
                    var area = CalculateArea(v1, v2);
                    if(area > maxArea)
                    {
                        maxArea = area;
                    }
                }
            }

            return new($"Max area: {maxArea}");
        }

        public override ValueTask<string> Solve_2()
        {
            var vertices = ParseVertices(_input);
            int noOfVertices = vertices.Count;
            var polygon = new Polygon(vertices);
            Int64 maxArea = -1;
            for (int i = 0; i < noOfVertices; i++)
            {
                var v1 = vertices[i];
                for (int j = i + 1; j < noOfVertices; j++)
                {
                    var v2 = vertices[j];
                    var rectangle = new Rectangle(v1, v2);
                    if (polygon.ContainsRectangleFormedByPoints(rectangle))
                    {
                        var area = CalculateArea(v1, v2);
                        if (area > maxArea)
                        {
                            maxArea = area;
                        }
                    }
                    
                }
            }

            return new($"Max area: {maxArea}");
        }

        public List<Tuple<Int64,Int64>> ParseVertices(string[] data)
        {
            List<Tuple<Int64, Int64>> vertices = new List<Tuple<Int64, Int64>>();
            foreach(var line in data)
            {
                var parts = line.Split(',');

                vertices.Add(new Tuple<Int64, Int64>(Int64.Parse(parts[0]), Int64.Parse(parts[1])));
            }
            return vertices;
        }

        public Int64 CalculateArea(Tuple<Int64, Int64> v1, Tuple<Int64, Int64> v2)
        {
            Int64 width = Math.Abs(v1.Item1 - v2.Item1) + 1;
            Int64 height = Math.Abs(v1.Item2 - v2.Item2) + 1;
            return height * width;
        }

        public class Rectangle
        {
            public Rectangle(Tuple<Int64,Int64> corner1, Tuple<Int64, Int64> corner2)
            {
                X1 = Math.Min(corner1.Item1, corner2.Item1);
                X2 = Math.Max(corner1.Item1, corner2.Item1);
                Y1 = Math.Min(corner1.Item2, corner2.Item2);
                Y2 = Math.Max(corner1.Item2, corner2.Item2);
            }

            public Int64 X1 { get; }
            public Int64 X2 { get; }
            public Int64 Y1 { get; }
            public Int64 Y2 { get; }

            public Tuple<Int64, Int64> X1Y1() => new Tuple<Int64, Int64>(X1, Y1);
            public Tuple<Int64, Int64> X1Y2() => new Tuple<Int64, Int64>(X1, Y2);
            public Tuple<Int64, Int64> X2Y1() => new Tuple<Int64, Int64>(X2, Y1);
            public Tuple<Int64, Int64> X2Y2() => new Tuple<Int64, Int64>(X2, Y2);
        }

        // https://en.wikipedia.org/wiki/Rectilinear_polygon
        // Since this says (and I trust wikipedia a little here) that the maximum rectangle in a Rectilinear polygon must intersect with the edges (and that makes sense)
        // We want to check if a point is on the edges
        // How do we figure out if a point is inside though?
        public class Polygon
        {

            public List<Tuple<Int64, Int64>> Vertices { get; }
            public List<Int64> HorizontalEdgeExistsHere { get; }
            public List<Int64> VerticalEdgeExistsHere { get; }
            public Dictionary<Tuple<Int64, Int64>, bool> VerticesInsidePolygonMap = new();

            public Polygon(List<Tuple<Int64, Int64>> Vertices)
            {
                this.Vertices = Vertices;
                HorizontalEdgeExistsHere = Vertices.Select(v => v.Item1).Distinct().Order().ToList();
                VerticalEdgeExistsHere = Vertices.Select(v => v.Item2).Distinct().Order().ToList();
            }

            public bool ContainsRectangleFormedByPoints(Rectangle rectangle)
            {
                // If one of the corners is not inside the polygon, obviously it is not inside the polygon. 
                // This can happen, since we are defining each rectangle from just two corners
                if(   !ContainsPoint(rectangle.X1Y1())
                   || !ContainsPoint(rectangle.X1Y2()) 
                   || !ContainsPoint(rectangle.X2Y1())
                   || !ContainsPoint(rectangle.X2Y2())
                    )
                {
                    return false;
                }

                //If we think we are inside, check all the possible points that COULD be outside. This means all points where there is a an edge that might be bounded by the rectangle
                foreach(var horizontalEdge in HorizontalEdgeExistsHere.Where(x => x>=rectangle.X1 && x <= rectangle.X2))
                {
                    //Create new points to check for all the intersections possible
                    if (!ContainsPoint(new Tuple<Int64,Int64>(horizontalEdge,rectangle.Y1)) || !ContainsPoint(new Tuple<Int64, Int64>(horizontalEdge, rectangle.Y2)))
                    {
                        return false;
                    }
                }

                //Repeat for vertical edges
                foreach (var verticalEdge in VerticalEdgeExistsHere.Where(y => y >= rectangle.Y1 && y <= rectangle.Y2))
                {
                    //Create new points to check for all the intersections possible
                    if (!ContainsPoint(new Tuple<Int64, Int64>(rectangle.X1, verticalEdge)) || !ContainsPoint(new Tuple<Int64, Int64>(rectangle.X2, verticalEdge)))
                    {
                        return false;
                    }
                }

                //If nothing is outside, we are inside
                return true;
            }

            public bool ContainsPoint(Tuple<Int64,Int64> point)
            {
                var memoFound = VerticesInsidePolygonMap.TryGetValue(point, out bool containsPoint);
                if (memoFound)
                {
                    return containsPoint;
                }
                bool calculatedContainsPoint = IsPointOnBoundaryOrInside(point);
                VerticesInsidePolygonMap.Add(point, calculatedContainsPoint);
                return calculatedContainsPoint;
            }

            public bool IsPointOnBoundaryOrInside(Tuple<Int64,Int64> point) 
            {
                var noOfPoints = Vertices.Count;
                int noOfIntersections = 0;
                for(int i = 0; i<noOfPoints; i++)
                {
                    var startVertice = Vertices[i];
                    var endVertice = Vertices[(i + 1) % noOfPoints];

                    if(startVertice.Item1 == endVertice.Item1) //Horizontal edge
                    {
                        if (point.Item1 == startVertice.Item1 && IsValueBetween(point.Item2, startVertice.Item2, endVertice.Item2)) // We are on the edge of two adjacent points, thus by definition inside the polygon
                        {
                            return true;
                        }
                    }
                    //By definition, since the polygon is Rectiliniear, if we are not an horizontal edge, we will need to be a vertical one.
                    //And thus, we can just check if the point is on this edge.
                    else if (point.Item2 == startVertice.Item2 && IsValueBetween(point.Item1,startVertice.Item1,endVertice.Item1)) 
                    {
                        return true;
                    }

                    //If we count the number of edges we intersect until our point when entering from the top, we can figure out if we are inside or outside the polygon or outside, despite "lommer"
                    //Every time we cross an edge we are either entering or exiting. So if we cross an odd number of points, then we are inside
                    if(startVertice.Item1 == endVertice.Item1) //Make sure we are on a horizontal edge again
                    {
                        if(point.Item1 > startVertice.Item1 && IsValueBetween(point.Item2,startVertice.Item2, endVertice.Item2))
                        {
                            //Don't count intersections with corners as double intersections? Only count them on one of the edges
                            if(point.Item2 != endVertice.Item2)
                            {
                                noOfIntersections++;
                            } 
                        }
                    }
                }
                return noOfIntersections % 2 != 0; //Again, if we haven't short circuted because we are on the edge, then we can figure it out by counting the number of enters from the top
            }
        }

        public static bool IsValueBetween(Int64 value, Int64 start, Int64 end)
        {
            var max = start > end ? start : end;
            var min = start < end ? start : end;
            return value >= min && value <= max;
        }


    }
}
