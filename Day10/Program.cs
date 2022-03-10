using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
{
    // Puzzle Description: https://adventofcode.com/2021/day/10

    class Program
    {
        private static readonly List<string> ValidPairs = "(),[],{},<>".Split(',').ToList();
        private static readonly List<string> CorruptPairs = "(],(},(>,[),[},[>,{),{],{>,<),<],<}".Split(',').ToList();
        const string PairEnds = ")]}>";

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 10");

            //var navigationSubsystemLines = File.ReadLines(@".\NavigationSubsystem-test.txt").ToList();
            var navigationSubsystemLines = File.ReadLines(@".\NavigationSubsystem-full.txt").ToList();

            Console.WriteLine($"* Navigation subsystem entries: {navigationSubsystemLines.Count:N0}");

            PartA(navigationSubsystemLines);
            PartB(navigationSubsystemLines);
        }

        static void PartA(List<string> navigationSubsystemLines)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var corruptLines = new List<string>();
            var firstCorruptPairInLines = new List<string>();
            var incompleteLines = new List<string>();

            ProcessTheLines(navigationSubsystemLines, corruptLines, firstCorruptPairInLines, incompleteLines);

            Console.WriteLine($"** Corrupted lines found: {firstCorruptPairInLines.Count:N0}");

            var pairEndScores = new int[] { 3, 57, 1197, 25137 };
            var points = 0;

            foreach (var pair in firstCorruptPairInLines)
                points += pairEndScores[PairEnds.IndexOf(pair[1])];

            Console.WriteLine($"*** Total points: {points:N0}");
        }

        static void PartB(List<string> navigationSubsystemLines)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var corruptLines = new List<string>();
            var firstCorruptPairInLines = new List<string>();
            var incompleteLines = new List<string>();

            ProcessTheLines(navigationSubsystemLines, corruptLines, firstCorruptPairInLines, incompleteLines);

            Console.WriteLine($"* Incomplete lines found: {incompleteLines.Count:N0}");

            var linePoints = new List<long>();

            foreach (var line in incompleteLines)
            {
                long points = 0;
                var endOfLine = String.Concat(line.Reverse()).Replace('(', ')').Replace('[', ']').Replace('{', '}').Replace('<', '>');

                foreach (var pairEnd in endOfLine)
                {
                    points = (points * 5) + (PairEnds.IndexOf(pairEnd) + 1);
                }

                linePoints.Add(points);

                Console.WriteLine($"* Starting/closing characters: {line}/{endOfLine} - Score: {points:N0}");
            }

            linePoints.Sort();
            var middleScore = linePoints[linePoints.Count / 2];

            Console.WriteLine($"*** Middle score: {middleScore:N0}");
        }

        static void ProcessTheLines(List<string> navigationSubsystemLines, List<string> corruptLines, List<string> firstCorruptPairInLines, List<string> incompleteLines)
        {
            // General process flow:
            // remove all valid pairs repeatedly until no more pairs remain
            // search for corrupted pairs, if any are found, stop and mark the line as corrupted
            // an incomplete line has no corruption, but still has characters
            // a valid line has no corruption, and no remaining characters

            foreach (var line in navigationSubsystemLines)
            {
                var remainingLine = line;
                var validPairsFound = true;
                //Console.WriteLine($"** Starting line: {remainingLine}");

                // remove all the valid pairs in the line,
                // which may take multiple passes while pairs collapse
                while (remainingLine.Length > 0 && validPairsFound)
                {
                    validPairsFound = false;

                    foreach (var pair in ValidPairs)
                    {
                        if (remainingLine.Contains(pair))
                        {
                            validPairsFound = true;
                            remainingLine = remainingLine.Replace(pair, "");
                        }
                    }
                }

                //Console.WriteLine($"** All valid pairs removed: {remainingLine}");

                // check for corrupt pairs
                var corruptPairsFound = false;
                var corruptPairInLine = new List<string>();

                foreach (var pair in CorruptPairs)
                {
                    if (remainingLine.Contains(pair))
                    {
                        if (!corruptPairsFound)
                            corruptLines.Add(line);

                        corruptPairsFound = true;

                        var pairPosition = remainingLine.IndexOf(pair);
                        corruptPairInLine.Add($"{pairPosition:D3}:{pair}");

                        //Console.WriteLine($"** Found corrupt pair at {pairPosition:D3}: {pair}");
                    }
                }

                if (corruptPairsFound)
                {
                    firstCorruptPairInLines.Add(corruptPairInLine.OrderBy(p => p).First().Split(':')[1]);
                    continue;
                }

                if (remainingLine.Length > 0)
                    incompleteLines.Add(remainingLine);
            }
        }
    }
}
