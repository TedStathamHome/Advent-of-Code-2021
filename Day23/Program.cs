using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day23
{
    // Puzzle Description: https://adventofcode.com/2021/day/23

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 23");

            var situationDiagramRaw = File.ReadLines(@".\SituationDiagram-test.txt").ToList();
            //var situationDiagramRaw = File.ReadLines(@".\SituationDiagram-full.txt").ToList();

            PartA(situationDiagramRaw);
            PartB(situationDiagramRaw);
        }

        static void PartA(List<string> situationDiagramRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

        }

        static void PartB(List<string> situationDiagramRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

        }
    }
}
