using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day02
{
    // Puzzle Description: https://adventofcode.com/2021/day/2

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 2");

            //var submarineInstructions = File.ReadLines(@".\SubmarineInstructions-test.txt").ToList();
            var submarineInstructions = File.ReadLines(@".\SubmarineInstructions-full.txt").ToList();
            Console.WriteLine($"* Number of submarine instructions to analyze: {submarineInstructions.Count:N0}");

            PartA(submarineInstructions);
            PartB(submarineInstructions);
        }

        static void PartA(List<string> submarineInstructions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var finalHorizontalPosition = submarineInstructions.Where(dm => dm.StartsWith("forward")).Select(dm => int.Parse(dm.Split(' ')[1])).Sum();
            var depthChangeUp = submarineInstructions.Where(dm => dm.StartsWith("up")).Select(dm => int.Parse(dm.Split(' ')[1])).Sum();
            var depthChangeDown = submarineInstructions.Where(dm => dm.StartsWith("down")).Select(dm => int.Parse(dm.Split(' ')[1])).Sum();
            var finalDepth = depthChangeDown - depthChangeUp;

            Console.WriteLine($"** Final horizontal position:     {finalHorizontalPosition:N0}");
            Console.WriteLine($"** Depth change down:             {depthChangeDown:N0}");
            Console.WriteLine($"** Depth change up:               {depthChangeUp:N0}");
            Console.WriteLine($"** Final depth:                   {finalDepth:N0}");
            Console.WriteLine($"\r\n*** Depth x horizontal position: {finalDepth * finalHorizontalPosition:N0}");
        }

        static void PartB(List<string> submarineInstructions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var finalHorizontalPosition = submarineInstructions.Where(dm => dm.StartsWith("forward")).Select(dm => int.Parse(dm.Split(' ')[1])).Sum();
            int aim = 0;
            int depth = 0;

            foreach (var instruction in submarineInstructions)
            {
                var details = instruction.Split(' ');
                switch (details[0])
                {
                    case "forward":
                        depth += int.Parse(details[1]) * aim;
                        break;

                    case "up":
                        aim -= int.Parse(details[1]);
                        break;

                    case "down":
                        aim += int.Parse(details[1]);
                        break;

                    default:
                        break;
                }
            }

            Console.WriteLine($"** Final horizontal position:     {finalHorizontalPosition:N0}");
            Console.WriteLine($"** Final depth:                   {depth:N0}");
            Console.WriteLine($"\r\n*** Depth x horizontal position: {depth * finalHorizontalPosition:N0}");
        }
    }
}
