using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day12
{
    // Puzzle Description: https://adventofcode.com/2021/day/12

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 12");

            //var caveMapRaw = File.ReadLines(@".\CaveMap-test1.txt").ToList();
            //var caveMapRaw = File.ReadLines(@".\CaveMap-test2.txt").ToList();
            //var caveMapRaw = File.ReadLines(@".\CaveMap-test3.txt").ToList();
            var caveMapRaw = File.ReadLines(@".\CaveMap-full.txt").ToList();

            PartA(caveMapRaw);
            PartB(caveMapRaw);
        }

        static void PartA(List<string> caveMapRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var pathsFound = new CaveSystem(caveMapRaw, 'A').CountPaths();
            Console.WriteLine($"*** Paths found: {pathsFound:N0}");
        }

        static void PartB(List<string> caveMapRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var pathsFound = new CaveSystem(caveMapRaw, 'B').CountPaths();
            Console.WriteLine($"*** Paths found: {pathsFound:N0}");
        }
    }

    class CaveSystem
    {
        Dictionary<string, List<string>> _from = new();
        Dictionary<string, List<int>> _pastVisits = new();
        int _pathCount;
        readonly char _solutionPart;

        public CaveSystem(List<string> caveMap, char solutionPart)
        {
            foreach (var edge in caveMap)
            {
                var caveNames = edge.Split('-');
                AddEdge(caveNames);
            }

            var caves = _from.Keys.Where(IsSmallCave);

            foreach (var cave in caves)
                _pastVisits.Add(cave, new());

            _solutionPart = solutionPart;
        }

        bool IsSmallCave(string cave) => char.IsLower(cave[0]);

        private void AddEdge(string[] caves)
        {
            FromTo(caves[0], caves[1]);
            FromTo(caves[1], caves[0]);

            void FromTo(string caveFrom, string caveTo)
            {
                if (caveTo == "start") return;
                if (caveFrom == "end") return;

                _from.AddToListUnique(caveFrom, caveTo);
            }
        }

        public int CountPaths()
        {
            SearchFrom("start", steps: 1);
            return _pathCount;
        }

        private void SearchFrom(string cave, int steps)
        {
            if (IsSmallCave(cave))
                _pastVisits[cave].Add(steps);

            foreach (var nextCave in _from[cave])
            {
                if (nextCave == "end")
                {
                    _pathCount++;
                    continue;
                }

                if (IsSmallCave(nextCave))
                {
                    if (_pastVisits[nextCave].Any())
                    {
                        if (_solutionPart == 'A')
                            continue;

                        if (_pastVisits.Any(cave => cave.Value.Count > 1))
                            continue;
                    }
                }

                SearchFrom(nextCave, steps + 1);

                foreach (var key in _pastVisits.Keys)
                {
                    _pastVisits[key] = _pastVisits[key].Where(s => s <= steps).ToList();
                }
            }
        }
    }

    public static class Extensions
    {
        public static void AddToListUnique<T, U>(this IDictionary<T, List<U>> dict, T key, U value)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, new List<U>() { value });
            else if (dict[key].Contains(value))
                return;
            else
                dict[key].Add(value);
        }
    }
}
