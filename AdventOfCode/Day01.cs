using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string _input;

    public Day01()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1() {
        var rotations = _input.Split("\n").Select(l => ParseLineIntoRotation(l)).ToList();
        var positions = GetPositionList(50, rotations);
        var positionAtZeroCount = positions.Where(p => p == 0).Count();
        return new($"Number of times position was at 0: {positionAtZeroCount}");
    }

    public override ValueTask<string> Solve_2()
    {
        var rotations = _input.Split("\n").Select(l => ParseLineIntoRotation(l)).ToList();
        var positions = GetPositionAndPassesList(50, rotations); ;
        var positionPasses = positions.Select(p => p.Item2).Sum();
        return new($"Number of times position was at 0: {positionPasses}");
    }


    public static List<int> GetPositionList(int startRotation, List<Rotation> rotations)
    {
        List<int> positions = new List<int>() { startRotation };
        var currentPosition = startRotation;
        foreach (Rotation rotation in rotations)
        {
            var newPosition = GetNewPositionAfterRotation(currentPosition, rotation.Distance, rotation.Direction);
            positions.Add(newPosition);
            currentPosition = newPosition;
        }
        return positions;
    }

    public static List<Tuple<int, int>> GetPositionAndPassesList(int startRotation, List<Rotation> rotations)
    {
        List<Tuple<int, int>> positions = new List<Tuple<int, int>>() { new Tuple<int, int>(startRotation, 0) };
        var currentPosition = startRotation;
        foreach (Rotation rotation in rotations)
        {
            var newPosition = GetNewPositionAfterRotationAndNumberOfPassesFor0(currentPosition, rotation.Distance, rotation.Direction);
            positions.Add(newPosition);
            currentPosition = newPosition.Item1;
        }
        return positions;
    }

    public static int GetNewPositionAfterRotation(int startRotation, int movement, RotationDirection direction)
    {
        switch (direction)
        {
            case RotationDirection.Left:
                return (startRotation - movement + 100) % 100;
            case RotationDirection.Right:
                return (startRotation + movement + 100) % 100;
            default:
                throw new ApplicationException($"Rotation {direction.ToString()} not supported");
        }
    }

    public static Tuple<int, int> GetNewPositionAfterRotationAndNumberOfPassesFor0(int startRotation, int movement, RotationDirection direction)
    {

        var actualMovement = movement % 100;
        var numberOfPassesToZero = (int)movement / 100;

        int endposition;

        switch (direction)
        {
            case RotationDirection.Left:
                endposition = (startRotation - actualMovement + 100) % 100;
                break;
            case RotationDirection.Right:
                endposition = (startRotation + actualMovement + 100) % 100;
                break;
            default:
                throw new ApplicationException($"Rotation {direction.ToString()} not supported");
        }

        if (endposition == 0)
        {
            return new Tuple<int, int>(0, numberOfPassesToZero + 1);
        }

        if (startRotation == 0)
        {
            return new Tuple<int, int>(endposition, numberOfPassesToZero); ;
        }

        if (direction.Equals(RotationDirection.Left) && endposition > startRotation)
        {
            return new Tuple<int, int>(endposition, numberOfPassesToZero + 1);
        }

        if (direction.Equals(RotationDirection.Right) && endposition < startRotation)
        {
            return new Tuple<int, int>(endposition, numberOfPassesToZero + 1);
        }
        return new Tuple<int, int>(endposition, numberOfPassesToZero);


    }

    private static Rotation ParseLineIntoRotation(string line)
    {
        var match = Regex.Match(line, @"([a-zA-Z]+)(\d+)");
        if (match.Success)
        {
            string letterPart = match.Groups[1].Value;
            string numberPart = match.Groups[2].Value;
            RotationDirection direction;

            switch (letterPart)
            {
                case "L":
                    direction = RotationDirection.Left; break;
                case "R":
                    direction = RotationDirection.Right; break;
                default:
                    throw new ApplicationException($"Direction {letterPart} not recognized");
            }
            return new Rotation
            {
                Direction = direction,
                Distance = int.Parse(numberPart),
            };
        }
        return null;
    }

    public class Rotation
    {
        public RotationDirection Direction { get; set; }
        public int Distance { get; set; }
    }

    public enum RotationDirection
    {
        Left,
        Right
    };
}
