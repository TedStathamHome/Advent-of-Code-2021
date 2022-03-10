using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day24
{
    // Puzzle Description: https://adventofcode.com/2021/day/24

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 24");

            var monadProgram = File.ReadLines(@".\MONAD-program-test1.txt").ToList();
            //var monadProgram = File.ReadLines(@".\MONAD-program-test2.txt").ToList();
            //var monadProgram = File.ReadLines(@".\MONAD-program-test3.txt").ToList();
            //var monadProgram = File.ReadLines(@".\MONAD-program-full.txt").ToList();

            PartA(monadProgram);
            PartB(monadProgram);
        }

        static void PartA(List<string> monadProgram)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

        }

        static void PartB(List<string> monadProgram)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

        }
    }
}
