using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    // Puzzle Description: https://adventofcode.com/2021/day/15

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 15");

            //var riskLevelMapRaw = File.ReadLines(@".\RiskLevelMap-test.txt").ToList();
            var riskLevelMapRaw = File.ReadLines(@".\RiskLevelMap-full.txt").ToList();

            PartA(riskLevelMapRaw);
            PartB(riskLevelMapRaw);
        }

        static void PartA(List<string> riskLevelMapRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            short gridColumns = (short)riskLevelMapRaw[0].Length;
            short gridRows = (short)riskLevelMapRaw.Count;
            var riskLevelMap = new short[gridColumns, gridRows];

            Console.WriteLine($"* Grid is {gridColumns:N0} columns x {gridRows:N0} rows");

            for (short row = 0; row < gridRows; row++)
            {
                var rowRiskLevels = riskLevelMapRaw[row].ToCharArray().Select(c => short.Parse(c.ToString())).ToArray();

                for (short col = 0; col < rowRiskLevels.Length; col++)
                {
                    riskLevelMap[col, row] = rowRiskLevels[col];
                    Console.Write($"{riskLevelMap[col, row]} ");
                }

                Console.Write("\r\n");
            }

            Console.Write("\r\n");

            var grid = new AStar.SquareGrid(gridColumns, gridRows, riskLevelMap);

            var aStarSearch = new AStar.Search(grid, new AStar.Location(0, 0), new AStar.Location((short)(gridColumns - 1), (short)(gridRows - 1)));

            DrawGrid(grid, aStarSearch, riskLevelMap);
            var pathCost = GetPathCost(grid, aStarSearch, riskLevelMap);

            Console.WriteLine($"*** Path cost: {pathCost:N0}");
        }

        static void PartB(List<string> riskLevelMapRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            short baseGridColumns = (short)riskLevelMapRaw[0].Length;
            short baseGridRows = (short)riskLevelMapRaw.Count;
            short gridColumns = (short)(baseGridColumns * 5);
            short gridRows = (short)(baseGridRows * 5);
            var riskLevelMap = new short[gridColumns, gridRows];

            Console.WriteLine($"* Grid is {gridColumns:N0} columns x {gridRows:N0} rows");

            for (short row = 0; row < baseGridRows; row++)
            {
                var rowRiskLevels = riskLevelMapRaw[row].ToCharArray().Select(c => short.Parse(c.ToString())).ToArray();

                for (short col = 0; col < rowRiskLevels.Length; col++)
                {
                    for (short subMapCol = 0; subMapCol < 5; subMapCol++)
                    {
                        for (int subMapRow = 0; subMapRow < 5; subMapRow++)
                        {
                            var riskLevelMapCellValue = (short)(rowRiskLevels[col] + subMapCol + subMapRow);

                            if (riskLevelMapCellValue > 9)
                                riskLevelMapCellValue -= 9;

                            riskLevelMap[col + (subMapCol * baseGridColumns), row + (subMapRow * baseGridRows)] = riskLevelMapCellValue;
                        }
                    }
                }
            }

            var grid = new AStar.SquareGrid(gridColumns, gridRows, riskLevelMap);

            var aStarSearch = new AStar.Search(grid, new AStar.Location(0, 0), new AStar.Location((short)(gridColumns - 1), (short)(gridRows - 1)));

            // don't attempt to draw the grid, as it's too huge for display
            var pathCost = GetPathCost(grid, aStarSearch, riskLevelMap);

            Console.WriteLine($"*** Path cost: {pathCost:N0}");
        }

        static void DrawGrid(AStar.SquareGrid grid, AStar.Search search, short[,] riskLevelMap)
        {
            for (short row = 0; row < grid.Rows; row++)
            {
                for (short col = 0; col < grid.Columns; col++)
                {
                    var location = new AStar.Location(col, row);
                    var pointer = location;

                    if (!search.PriorLocation.TryGetValue(location, out pointer))
                        pointer = location;

                    var riskLevel = riskLevelMap[col, row];

                    if (pointer.Column == col + 1) { Console.Write($"{riskLevel}\u2192"); }         // → right arrow
                    else if (pointer.Column == col - 1) { Console.Write($"{riskLevel}\u2190"); }    // ← left arrow
                    else if (pointer.Row == row + 1) { Console.Write($"{riskLevel}\u2193"); }       // ↓ down arrow
                    else if (pointer.Row == row - 1) { Console.Write($"{riskLevel}\u2191"); }       // ↑ up arrow
                    else { Console.Write($"{riskLevel} "); }
                }

                Console.WriteLine();
            }
        }

        static int GetPathCost(AStar.SquareGrid grid, AStar.Search search, short[,] riskLevelMap)
        {
            int pathCost = 0;
            short currentColumn = (short)(grid.Columns - 1);
            short currentRow = (short)(grid.Rows - 1);

            while (currentColumn > 0 || currentRow > 0)   // loop until we hit the starting point
            {
                var currentLocation = new AStar.Location(currentColumn, currentRow);    // grab the end location
                var mapCellRisk = riskLevelMap[currentLocation.Column, currentLocation.Row];
                pathCost += mapCellRisk;

                //Console.WriteLine($"** Risk @ {currentColumn}:{currentRow} = {mapCellRisk}");

                var pointer = currentLocation;

                if (!search.PriorLocation.TryGetValue(currentLocation, out pointer))    // if there's no prior location (but there should be)
                    pointer = currentLocation;

                currentColumn = pointer.Column;
                currentRow = pointer.Row;
            }

            return pathCost;
        }
    }
}
