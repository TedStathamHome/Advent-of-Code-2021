using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day07
{
    // Puzzle Description: https://adventofcode.com/2021/day/7

    class Program
    {
        class PositionSummary
        {
            public int Position { get; set; }
            public int SubCount { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 7");

            //var crabSubPositions = File.ReadLines(@".\CrabSubPositions-test.txt").ToList().First().Split(',').Select(f => int.Parse(f)).ToList();
            //bool inTestMode = true;

            var crabSubPositions = File.ReadLines(@".\CrabSubPositions-full.txt").ToList().First().Split(',').Select(f => int.Parse(f)).ToList();
            bool inTestMode = false;
            
            Console.WriteLine($"* Number of crab submarines: {crabSubPositions.Count:N0}");

            PartA(crabSubPositions);
            PartB(crabSubPositions, inTestMode);
        }

        static void PartA(List<int> crabSubPositions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var positionSummaries = GetNumberOfCrabSubsAtEachPosition(crabSubPositions);
            var positionCosts = new Dictionary<int, int>();

            foreach (var positionSummary in positionSummaries)
            {
                var fuelCost = positionSummaries.Select(ps => Math.Abs(ps.Position - positionSummary.Position) * ps.SubCount).Sum();
                positionCosts.Add(positionSummary.Position, fuelCost);
            }

            var lowestFuelCost = positionCosts.Where(pc => pc.Value == positionCosts.Min(pc => pc.Value)).First();
            Console.WriteLine($"*** Lowest fuel cost of {lowestFuelCost.Value:N0} is for position {lowestFuelCost.Key:N0}");
        }

        static void PartB(List<int> crabSubPositions, bool inTestMode)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var positionSummaries = GetNumberOfCrabSubsAtEachPosition(crabSubPositions);
            var highestPosition = positionSummaries.Max(pc => pc.Position);

            var fuelCosts = new int[highestPosition + 1];
            var currentCost = 0;

            for (int i = 0; i < highestPosition; i++)
            {
                currentCost += i;
                fuelCosts[i] = currentCost;
            }

            var positionCosts = new Dictionary<int, int>();

            // when running with their test data, this loop must start at 1
            for (int i = (inTestMode ? 1 : 0); i <= highestPosition; i++)
            {
                var fuelCost = positionSummaries.Select(pc => fuelCosts[Math.Abs(pc.Position - i)] * pc.SubCount).Sum();
                positionCosts.Add(i, fuelCost);

                //Console.WriteLine($"** Position: {i:N0} - Cost: {fuelCost:N0}");
            }

            var lowestFuelCost = positionCosts.Where(pc => pc.Value == positionCosts.Min(pc => pc.Value)).First();
            Console.WriteLine($"*** Lowest fuel cost of {lowestFuelCost.Value:N0} is for position {lowestFuelCost.Key:N0}");
        }

        static List<PositionSummary> GetNumberOfCrabSubsAtEachPosition(List<int> crabSubPositions)
        {
            return crabSubPositions
                .GroupBy(s => s)
                .Select(g => new PositionSummary() { Position = g.Key, SubCount = g.Count() })
                .OrderBy(r => r.Position)
                .ToList();
        }
    }
}
