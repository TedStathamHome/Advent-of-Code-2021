using System;
using System.IO;
using System.Linq;

namespace Day01
{
    // Puzzle Description: https://adventofcode.com/2021/day/1

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 1");

            //var depthMeasurementRaw = File.ReadLines(@".\DepthMeasurements-test.txt").ToList();
            var depthMeasurementRaw = File.ReadLines(@".\DepthMeasurements-full.txt").ToList();
            var depthMeasurementEntries = depthMeasurementRaw.Select(e => int.Parse(e)).ToArray();

            Console.WriteLine($"* Number of depth measurements to analyze: {depthMeasurementEntries.Length:N0}");

            PartA(depthMeasurementEntries);
            PartB(depthMeasurementEntries);
        }

        static void PartA(int[] depthMeasurementEntries)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var entriesWithIncreasingValues = 0;
            var lastEntryValue = -1;

            foreach (var entry in depthMeasurementEntries)
            {
                if (lastEntryValue > -1)
                {
                    if (entry > lastEntryValue)
                        entriesWithIncreasingValues++;
                }

                lastEntryValue = entry;
            }

            if (entriesWithIncreasingValues == 0)
            {
                Console.WriteLine($"*** There were NO depth increases detected.");
            }
            else
            {
                Console.WriteLine($"*** A total of {entriesWithIncreasingValues:N0} increases in depth were detected.");
            }
        }

        static void PartB(int[] depthMeasurementEntries)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            const int measurementWindowSize = 3;
            var measurementWindows = depthMeasurementEntries.Length - measurementWindowSize + 1;
            var windowSums = new int[measurementWindows];

            Console.WriteLine($"* Number of measurement windows to analyze: {measurementWindows:N0}");

            for (int i = 0; i < measurementWindows; i++)
            {
                for (int j = 0; j < measurementWindowSize; j++)
                {
                    windowSums[i] += depthMeasurementEntries[i + j];
                }
            }

            var entriesWithIncreasingValues = 0;
            var lastEntryValue = -1;

            foreach (var entry in windowSums)
            {
                if (lastEntryValue > -1)
                {
                    if (entry > lastEntryValue) entriesWithIncreasingValues++;
                }

                lastEntryValue = entry;
            }

            if (entriesWithIncreasingValues == 0)
            {
                Console.WriteLine($"*** There were NO depth window increases detected.");
            }
            else
            {
                Console.WriteLine($"*** A total of {entriesWithIncreasingValues:N0} increases in depth windows were detected.");
            }
        }
    }
}
