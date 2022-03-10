using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11
{
    // Puzzle Description: https://adventofcode.com/2021/day/11

    class Program
    {
        class ProcessResults
        {
            public int StepsRun { get; set; }
            public int FlashesSeen { get; set; }
        }

        const int MaxStepsToProcess = 100;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 11");

            //var octopusGridRaw = File.ReadLines(@".\OctopusGrid-test.txt").ToList();
            var octopusGridRaw = File.ReadLines(@".\OctopusGrid-full.txt").ToList();
            var gridCols = octopusGridRaw.First().Length;
            var gridRows = octopusGridRaw.Count;

            PartA(octopusGridRaw, gridCols, gridRows);
            PartB(octopusGridRaw, gridCols, gridRows);
        }

        static void PartA(List<string> octopusGridRaw, int gridCols, int gridRows)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var octopusGrid = ParseOctopusGridRaw(octopusGridRaw);
            var processResults = WatchOctopuses(octopusGrid, gridCols, gridRows, true);

            Console.WriteLine($"*** Flashes seen after {processResults.StepsRun:N0} steps: {processResults.FlashesSeen:N0}");
        }

        static void PartB(List<string> octopusGridRaw, int gridCols, int gridRows)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var octopusGrid = ParseOctopusGridRaw(octopusGridRaw);
            var processResults = WatchOctopuses(octopusGrid, gridCols, gridRows, false);

            Console.WriteLine($"*** All octopuses flashed on step {processResults.StepsRun:N0}; flashes seen so far: {processResults.FlashesSeen:N0}");
        }

        static List<List<int>> ParseOctopusGridRaw(List<string> octopusGridRaw)
        {
            var octopusGrid = new List<List<int>>();

            foreach (var row in octopusGridRaw)
                octopusGrid.Add(row.ToCharArray().Select(col => int.Parse(col.ToString())).ToList());

            return octopusGrid;
        }

        static ProcessResults WatchOctopuses(List<List<int>> octopusGrid, int gridCols, int gridRows, bool limitSteps)
        {
            var processResults = new ProcessResults()
            { 
                FlashesSeen = 0, 
                StepsRun = 0
            };

            while (true)
            {
                processResults.StepsRun++;
                var stepFlashes = 0;

                var octopusHasFlashed = new bool[gridRows, gridCols];
                var flashes = new List<int[]>();

                // increment all the octopuses, and detect initial flashes
                for (int r = 0; r < gridRows; r++)
                {
                    for (int c = 0; c < gridCols; c++)
                    {
                        octopusGrid[r][c]++;

                        if (octopusGrid[r][c] > 9)
                            flashes.Add(new int[] { r, c });
                    }
                }

                // process flashes and following flashes until none remain
                var currentFlash = 0;

                while (currentFlash < flashes.Count)
                {
                    var flash = flashes[currentFlash];
                    var flashRow = flash[0];
                    var flashCol = flash[1];

                    if (!octopusHasFlashed[flashRow, flashCol])
                    {
                        octopusHasFlashed[flashRow, flashCol] = true;
                        processResults.FlashesSeen++;
                        stepFlashes++;

                        for (int r = (flashRow - 1); r < (flashRow + 2); r++)
                        {
                            for (int c = (flashCol - 1); c < (flashCol + 2); c++)
                            {
                                // if we are within the bounds of the grid, and not on our starting spot
                                if (r >= 0 && r < gridRows && c >= 0 && c < gridCols)
                                {
                                    if (!(r == flashRow && c == flashCol))
                                    {
                                        if (!octopusHasFlashed[r, c])
                                        {
                                            octopusGrid[r][c]++;

                                            if (octopusGrid[r][c] > 9)
                                                flashes.Add(new int[] { r, c });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    currentFlash++;
                }

                // clear all energy levels > 9
                for (int r = 0; r < gridRows; r++)
                {
                    for (int c = 0; c < gridCols; c++)
                    {
                        if (octopusGrid[r][c] > 9)
                            octopusGrid[r][c] = 0;
                    }
                }

                // if we're supposed to limit the number of steps, break out when we hit the maximum number of steps,
                // otherwise, process until we reach a step where all octopi flash at the same time and then break out
                if (limitSteps 
                    ? (processResults.StepsRun >= MaxStepsToProcess) 
                    : (stepFlashes == gridRows * gridCols))
                    break;
            }

            return processResults;
        }
    }
}
