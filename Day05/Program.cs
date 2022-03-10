using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day05
{
    // Puzzle Description: https://adventofcode.com/2021/day/5

    class Program
    {
        //const int gridSize = 10;      // test
        const int gridSize = 1000;    // full

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 5");

            //var ventLines = File.ReadLines(@".\VentLines-test.txt").ToList();
            var ventLines = File.ReadLines(@".\VentLines-full.txt").ToList();

            Console.WriteLine($"* Vent lines to inspect: {ventLines.Count:N0}");

            PartA(ventLines);
            PartB(ventLines);
        }

        static void PartA(List<string> ventLines)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var ventGrid = new int[gridSize, gridSize];
            var straightLines = 0;

            foreach (var ventLine in ventLines)
            {
                var coordinates = ventLine.Split("->", StringSplitOptions.TrimEntries);

                var lineStart = coordinates[0].Split(',');
                var lineStartX = int.Parse(lineStart[0]);
                var lineStartY = int.Parse(lineStart[1]);

                var lineEnd = coordinates[1].Split(',');
                var lineEndX = int.Parse(lineEnd[0]);
                var lineEndY = int.Parse(lineEnd[1]);

                // check for a straight line
                if ((lineStartX == lineEndX) || (lineStartY == lineEndY))
                {
                    straightLines++;

                    // plot the lines in the grid

                    // drawing on the Y-axis
                    if (lineStartX == lineEndX)
                    {
                        // if the start is past the end
                        if (lineStartY > lineEndY)
                            // swap the start and end using a tuple assignment
                            (lineStartY, lineEndY) = (lineEndY, lineStartY);

                        for (int y = lineStartY; y <= lineEndY; y++)
                            ventGrid[lineStartX, y]++;
                    }
                    // drawing on the X-axis
                    else
                    {
                        // if the start is past the end
                        if (lineStartX > lineEndX)
                            // swap the start and end using a tuple assignment
                            (lineStartX, lineEndX) = (lineEndX, lineStartX);

                        for (int x = lineStartX; x <= lineEndX; x++)
                            ventGrid[x, lineStartY]++;
                    }
                }
            }

            Console.WriteLine($"** Straight lines found: {straightLines:N0}");

            DisplayGridIfSmall(ventGrid);

            var ventOverlaps = CountOfVentOverlaps(ventGrid);

            Console.WriteLine($"*** Points of overlap: {ventOverlaps:N0}");
        }

        static void PartB(List<string> ventLines)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var ventGrid = new int[gridSize, gridSize];
            var straightLines = 0;

            foreach (var ventLine in ventLines)
            {
                var coordinates = ventLine.Split("->", StringSplitOptions.TrimEntries);

                var lineStart = coordinates[0].Split(',');
                var lineStartX = int.Parse(lineStart[0]);
                var lineStartY = int.Parse(lineStart[1]);

                var lineEnd = coordinates[1].Split(',');
                var lineEndX = int.Parse(lineEnd[0]);
                var lineEndY = int.Parse(lineEnd[1]);

                // check for a straight line
                if ((lineStartX == lineEndX) || (lineStartY == lineEndY))
                {
                    straightLines++;

                    // plot the lines in the grid

                    // drawing on the Y-axis
                    if (lineStartX == lineEndX)
                    {
                        // if the start is past the end
                        if (lineStartY > lineEndY)
                            // swap the start and end using a tuple assignment
                            (lineStartY, lineEndY) = (lineEndY, lineStartY);

                        for (int y = lineStartY; y <= lineEndY; y++)
                            ventGrid[lineStartX, y]++;
                    }
                    // drawing on the X-axis
                    else
                    {
                        // if the start is past the end
                        if (lineStartX > lineEndX)
                            // swap the start and end using a tuple assignment
                            (lineStartX, lineEndX) = (lineEndX, lineStartX);

                        for (int x = lineStartX; x <= lineEndX; x++)
                            ventGrid[x, lineStartY]++;
                    }
                }
                else  // must be a diagonal line
                {
                    var lineY = lineStartY;
                    var verticalDirection = (lineStartY < lineEndY) ? 1 : -1;

                    if (lineStartX < lineEndX)
                    {
                        for (int x = lineStartX; x <= lineEndX; x++)
                        {
                            ventGrid[x, lineY]++;
                            lineY += verticalDirection;
                        }
                    }
                    else
                    {
                        for (int x = lineStartX; x >= lineEndX; x--)
                        {
                            ventGrid[x, lineY]++;
                            lineY += verticalDirection;
                        }
                    }
                }
            }

            Console.WriteLine($"* Straight lines found: {straightLines:N0}");

            DisplayGridIfSmall(ventGrid);

            var ventOverlaps = CountOfVentOverlaps(ventGrid);

            Console.WriteLine($"*** Points of overlap: {ventOverlaps:N0}");
        }

        static void DisplayGridIfSmall(int[,] ventGrid)
        {
            if (gridSize > 20)
                return;

            for (int y = 0; y < gridSize; y++)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    Console.Write(ventGrid[x, y] == 0 ? "." : $"{ventGrid[x, y]}");
                }
                Console.Write("\r\n");
            }
        }

        static int CountOfVentOverlaps(int[,] ventGrid)
        {
            var ventOverlaps = 0;

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    if (ventGrid[x, y] > 1)
                        ventOverlaps++;
                }
            }

            return ventOverlaps;
        }
    }
}
