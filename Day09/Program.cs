using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day09
{
    // Puzzle Description: https://adventofcode.com/2021/day/9

    class Program
    {
        class LowPoint
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public int PointValue { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 9");

            //var heightMapRaw = File.ReadLines(@".\HeightMap-test.txt").ToList();
            var heightMapRaw = File.ReadLines(@".\HeightMap-full.txt").ToList();
            var heightMap = heightMapRaw.Select(r => r.ToCharArray().Select(v => int.Parse(v.ToString())).ToList()).ToList();
            var mapRows = heightMapRaw.Count;
            var mapCols = heightMapRaw[0].Length;

            Console.WriteLine($"* Height map grid is {mapRows:N0} rows x {mapCols:N0} columns");

            PartA(heightMap, mapRows, mapCols);
            PartB(heightMap, mapRows, mapCols);
        }

        static void PartA(List<List<int>> heightMap, int mapRows, int mapCols)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var lowPoints = FindLowPoints(heightMap, mapRows, mapCols);
            var totalOfRiskLevels = lowPoints.Select(lp => lp.PointValue + 1).Sum();

            Console.WriteLine($"*** Total of risk levels: {totalOfRiskLevels:N0}");
        }

        static void PartB(List<List<int>> heightMap, int mapRows, int mapCols)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var lowPoints = FindLowPoints(heightMap, mapRows, mapCols);

            Console.WriteLine($"* Count of low points: {lowPoints.Count:N0}");

            var basinSizes = new List<int>();

            foreach (var lowPoint in lowPoints)
            {
                var lastPointChecked = 0;
                var basinPoints = new List<string>();
                var basinPointsRowCol = new List<LowPoint>();
                basinPoints.Add($"{lowPoint.Row}:{lowPoint.Col}");
                basinPointsRowCol.Add(lowPoint);

                // since we don't include 9s in the basin, our edge values can be 9
                var edgeValue = 9;

                while (lastPointChecked < basinPoints.Count)
                {
                    var row = basinPointsRowCol[lastPointChecked].Row;
                    var col = basinPointsRowCol[lastPointChecked].Col;

                    var pointValue = heightMap[row][col];
                    var valueUp = (row == 0) ? edgeValue : heightMap[row - 1][col];
                    var valueDown = (row == (mapRows - 1)) ? edgeValue : heightMap[row + 1][col];
                    var valueLeft = (col == 0) ? edgeValue : heightMap[row][col - 1];
                    var valueRight = (col == (mapCols - 1)) ? edgeValue : heightMap[row][col + 1];

                    if (valueUp < edgeValue && valueUp >= pointValue)
                        AddBasinPoint(basinPoints, basinPointsRowCol, row - 1, col);

                    if (valueDown < edgeValue && valueDown >= pointValue)
                        AddBasinPoint(basinPoints, basinPointsRowCol, row + 1, col);

                    if (valueLeft < edgeValue && valueLeft >= pointValue)
                        AddBasinPoint(basinPoints, basinPointsRowCol, row, col - 1);

                    if (valueRight < edgeValue && valueRight >= pointValue)
                        AddBasinPoint(basinPoints, basinPointsRowCol, row, col + 1);

                    lastPointChecked++;
                }

                basinSizes.Add(basinPoints.Count);
                Console.WriteLine($"* Basin at {lowPoint.Row}:{lowPoint.Col}, size: {basinPoints.Count:N0}");
            }

            basinSizes.Sort();
            var productOfSizes = 0;

            Console.Write("** Top 3 basin sizes:");
            foreach (var basin in basinSizes.TakeLast(3))
            {
                Console.Write($" {basin:N0}");
                productOfSizes = basin * (productOfSizes == 0 ? 1 : productOfSizes);
            }

            Console.WriteLine($"\r\n*** Product of top 3 sizes: {productOfSizes:N0}");
        }

        private static List<LowPoint> FindLowPoints(List<List<int>> heightMap, int mapRows, int mapCols)
        {
            var lowPoints = new List<LowPoint>();

            for (int r = 0; r < mapRows; r++)
            {
                for (int c = 0; c < mapCols; c++)
                {
                    var pointValue = heightMap[r][c];

                    // make this 1 higher than the point value so the later comparison can detect a low point at an edge/corner
                    var edgeValue = pointValue + 1;

                    var valueUp = (r == 0) ? edgeValue : heightMap[r - 1][c];
                    var valueDown = (r == (mapRows - 1)) ? edgeValue : heightMap[r + 1][c];
                    var valueLeft = (c == 0) ? edgeValue : heightMap[r][c - 1];
                    var valueRight = (c == (mapCols - 1)) ? edgeValue : heightMap[r][c + 1];

                    if (pointValue < valueUp && pointValue < valueDown && pointValue < valueLeft && pointValue < valueRight)
                    {
                        lowPoints.Add(new LowPoint() { Row = r, Col = c, PointValue = pointValue });
                        Console.WriteLine($"** Low point at {r}:{c} -> {pointValue}; risk level {pointValue + 1}");
                    }
                }
            }

            return lowPoints;
        }

        private static void AddBasinPoint(List<string> BasinPoints, List<LowPoint> BasinPointsRowCol, int Row, int Col)
        {
            var basinPoint = $"{Row}:{Col}";

            if (!BasinPoints.Contains(basinPoint))
            {
                BasinPoints.Add(basinPoint);
                BasinPointsRowCol.Add(new LowPoint() { Row = Row, Col = Col, PointValue = -1 });
            }
        }
    }
}
