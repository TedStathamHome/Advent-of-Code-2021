using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day25
{
    // Puzzle Description: https://adventofcode.com/2021/day/25

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 25");

            //var seaCucumberMap = File.ReadLines(@".\SeaCucumberMap-test.txt").ToList();
            var seaCucumberMap = File.ReadLines(@".\SeaCucumberMap-full.txt").ToList();

            PartA(seaCucumberMap);
            PartB();
        }

        static void PartA(List<string> seaCucumberMap)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var stepNumber = 0;

            Console.WriteLine("** Original map:");
            foreach (var mapLine in seaCucumberMap)
            {
                Console.WriteLine(mapLine);
            }

            while (true)
            {
                var countOfSeaCucumberMovements = 0;
                stepNumber++;

                // process west to east movement
                for (int l = 0; l < seaCucumberMap.Count; l++)
                {
                    var lineToCheck = seaCucumberMap[l] + seaCucumberMap[l][0];     // tack the first space onto the end, to detect a wraparound movement
                    var seaCucumbersThatCanMove = IndexesOfSeaCucumbersThatCanMove(lineToCheck, '>');

                    if (seaCucumbersThatCanMove.Count > 0)
                    {
                        countOfSeaCucumberMovements += seaCucumbersThatCanMove.Count;
                        var mapLine = seaCucumberMap[l].ToCharArray();

                        foreach (var seaCucumberPosition in seaCucumbersThatCanMove)
                        {
                            mapLine[seaCucumberPosition] = '.';

                            // if the sea cucumber was at the end of the line, wrap it to the start
                            mapLine[(seaCucumberPosition + 1 == mapLine.Length) ? 0 : seaCucumberPosition + 1] = '>';
                        }

                        seaCucumberMap[l] = new string(mapLine);
                    }
                }

                // process north to south movement
                for (int c = 0; c < seaCucumberMap[0].Length; c++)
                {
                    var columnToCheck = new string(seaCucumberMap.Select(mapLine => mapLine[c]).ToArray()) + seaCucumberMap[0][c];
                    var seaCucumbersThatCanMove = IndexesOfSeaCucumbersThatCanMove(columnToCheck, 'v');

                    if (seaCucumbersThatCanMove.Count > 0)
                    {
                        countOfSeaCucumberMovements += seaCucumbersThatCanMove.Count;

                        foreach (var seaCucumberPosition in seaCucumbersThatCanMove)
                        {
                            var mapLine = seaCucumberMap[seaCucumberPosition].ToCharArray();
                            mapLine[c] = '.';
                            seaCucumberMap[seaCucumberPosition] = new string(mapLine);

                            var nextLine = (seaCucumberPosition + 1 == seaCucumberMap.Count) ? 0 : seaCucumberPosition + 1;
                            mapLine = seaCucumberMap[nextLine].ToCharArray();
                            mapLine[c] = 'v';
                            seaCucumberMap[nextLine] = new string(mapLine);
                        }
                    }
                }

                if (countOfSeaCucumberMovements == 0)
                    break;
            }

            Console.WriteLine($"\r\n*** No sea cucumber movement detected during step {stepNumber}");

            foreach (var mapLine in seaCucumberMap)
                Console.WriteLine(mapLine);
        }

        static void PartB()
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

        }

        private static List<int> IndexesOfSeaCucumbersThatCanMove(string stringToCheck, char seaCucumberMarker)
        {
            var indexes = new List<int>();
            var lastIndex = -1;
            string markerToFind = $"{seaCucumberMarker}.";

            while (lastIndex < stringToCheck.Length)
            {
                lastIndex = stringToCheck.IndexOf(markerToFind, lastIndex + 1);

                // if we can't find the marker, break out of the loop
                if (lastIndex == -1)
                    break;

                indexes.Add(lastIndex);
            }

            return indexes;
        }
    }
}
