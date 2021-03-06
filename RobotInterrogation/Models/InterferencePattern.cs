﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace RobotInterrogation.Models
{
    public class InterferencePattern
    {
        public InterferencePattern(int width, int height)
        {
            Width = width;
            Height = height;
            Connections = new Direction[width, height];
        }

        public int Width { get; }

        public int Height { get; }

        public Direction[,] Connections { get; }

        [Flags]
        public enum Direction
        {
            None = 0,
            North = 1 << 0,
            South = 1 << 1,
            East = 1 << 2,
            West = 1 << 3,
        }

        public List<Point> Markers { get; } = new List<Point>();

        public List<int> MarkerSequence { get; } = new List<int>();

        public List<string> SolutionSequence => MarkerSequence
            .Select(index => ((char)('A' + index)).ToString())
            .ToList();

        public List<Tuple<Point, Direction>> Arrows = new List<Tuple<Point, Direction>>();

        public string[,] CellContents
        {
            get
            {
                var results = new string[Width, Height];

                foreach (var arrow in Arrows)
                    results[arrow.Item1.X, arrow.Item1.Y] = GetArrowForDirection(arrow.Item2).ToString();

                for (int index = 0; index < Markers.Count; index++)
                {
                    var marker = Markers[index];
                    results[marker.X, marker.Y] = GetSymbolForMarker(index).ToString();
                }

                return results;
            }
        }

        private static char GetArrowForDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    return '↑';
                case Direction.South:
                    return '↓';
                case Direction.East:
                    return '→';
                case Direction.West:
                    return '←';
                default:
                    return ' ';
            }
        }

        private static char GetSymbolForMarker(int index)
        {
            return (char)('A' + index);
        }

        #region serialization
        public override string ToString()
        {
            var output = new StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                var rowTop = new StringBuilder();
                var rowMid = new StringBuilder();

                for (int x = 0; x < Width; x++)
                {
                    var thisCell = Connections[x, y];

                    var prevRowPrevCell = y == 0
                        ? x > 0
                            ? Direction.East | Direction.West
                            : Direction.East | Direction.South
                        : x > 0
                            ? Connections[x - 1, y - 1]
                            : Direction.North | Direction.South;

                    rowTop.Append(DetermineCornerCharacter(prevRowPrevCell, thisCell));
                    rowTop.Append(DetermineVerticalCharacter(thisCell));

                    rowMid.Append(DetermineHorizontalCharacter(thisCell));
                    rowMid.Append(DetermineContentCharacter(x, y));
                }

                var aboveLastCell = y > 0
                    ? Connections[Width - 1, y - 1]
                    : Direction.East | Direction.West;

                rowTop.Append(DetermineCornerCharacter(aboveLastCell, Direction.North | Direction.South));
                rowMid.Append(DetermineHorizontalCharacter(Direction.North | Direction.South));

                output.AppendLine(rowTop.ToString());
                output.AppendLine(rowMid.ToString());
            }

            {
                var rowTop = new StringBuilder();

                int y = Height - 1;
                for (int x = 0; x < Width; x++)
                {
                    var prevRowPrevCell = x > 0
                        ? Connections[x - 1, y]
                        : Direction.North | Direction.South;

                    var fakeRow = Direction.East | Direction.West;

                    rowTop.Append(DetermineCornerCharacter(prevRowPrevCell, fakeRow));
                    rowTop.Append(DetermineVerticalCharacter(fakeRow));
                }

                rowTop.Append(DetermineCornerCharacter(Direction.None, Direction.North | Direction.West));

                output.AppendLine(rowTop.ToString());
            }

            return output.ToString();
        }

        private static char DetermineVerticalCharacter(Direction connections)
        {
            return connections.HasFlag(Direction.North)
                ? ' '
                : '─';
        }

        private static char DetermineHorizontalCharacter(Direction connections)
        {
            return connections.HasFlag(Direction.West)
                ? ' '
                : '│';
        }

        private static char DetermineCornerCharacter(Direction topLeft, Direction bottomRight)
        {
            if (topLeft.HasFlag(Direction.East))
                if (topLeft.HasFlag(Direction.South))
                {
                    if (bottomRight.HasFlag(Direction.West))
                    {
                        if (bottomRight.HasFlag(Direction.North))
                            return ' ';
                        else
                            return '─'; // right-only-dash?
                    }
                    else if (bottomRight.HasFlag(Direction.North))
                        return '│'; // down-only dash?
                    else
                        return '┌';
                }
                else
                {
                    if (bottomRight.HasFlag(Direction.West))
                    {
                        /*
                        if (bottomRight.HasFlag(Direction.North))
                            return '─'; // left-only dash?
                        else
                        */
                            return '─';
                    }
                    else if (bottomRight.HasFlag(Direction.North))
                        return '┐';
                    else
                        return '┬';
                }
            else if (topLeft.HasFlag(Direction.South))
            {
                if (bottomRight.HasFlag(Direction.West))
                {
                    if (bottomRight.HasFlag(Direction.North))
                        return '│'; // up-only dash?
                    else
                        return '└';
                }
                else if (bottomRight.HasFlag(Direction.North))
                    return '│';
                else
                    return '├';
            }
            else
            {
                if (bottomRight.HasFlag(Direction.West))
                {
                    if (bottomRight.HasFlag(Direction.North))
                        return '┘';
                    else
                        return '┴';
                }
                else if (bottomRight.HasFlag(Direction.North))
                    return '┤';
                else
                    return '┼';
            }
        }
        private char DetermineContentCharacter(int x, int y)
        {
            var point = new Point(x, y);

            var index = Markers.IndexOf(point);
            if (index != -1)
                return GetSymbolForMarker(index);

            var matchingArrow = Arrows.FirstOrDefault(arr => arr.Item1 == point);

            if (matchingArrow != null)
                return GetArrowForDirection(matchingArrow.Item2);

            return ' ';
        }
        #endregion serialization
    }
}
