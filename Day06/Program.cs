using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day06
{
    // Puzzle Description: https://adventofcode.com/2021/day/6

    class Program
    {
        const int baseFishCycleLength = 6;
        const int newFishCycleLength = baseFishCycleLength + 2;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 6");

            //var lanternfish = File.ReadLines(@".\Lanternfish-test.txt").ToList().First().Split(',').Select(f => int.Parse(f)).ToList();
            var lanternfish = File.ReadLines(@".\Lanternfish-full.txt").ToList().First().Split(',').Select(f => int.Parse(f)).ToList();
            Console.WriteLine($"* Number of lanternfish: {lanternfish.Count:N0}");

            PartA(lanternfish);
            PartB(lanternfish);
        }

        static void PartA(List<int> lanternfish)
        {
            const int lifetime = 80;

            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            RunTheBreedingProgram(lanternfish, lifetime);
        }

        static void PartB(List<int> lanternfish)
        {
            const int lifetime = 256;

            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            RunTheBreedingProgram(lanternfish, lifetime);
        }

        static void RunTheBreedingProgram(List<int> lanternfish, int lifetime)
        {
            var currentFishState = GetInitialFishStateCounts(lanternfish);

            Console.WriteLine($"** Initial fish state counts: {string.Join(',', currentFishState.ToList())}");

            BreedTheFish(ref currentFishState, lifetime);

            Console.WriteLine($"** Final fish state counts:   {string.Join(',', currentFishState.ToList())}");
            Console.WriteLine($"*** Total lanternfish: {currentFishState.Sum():N0}");
        }

        static long[] GetInitialFishStateCounts(List<int> lanternfish)
        {
            var initialFishState = new long[newFishCycleLength + 1];

            for (int i = 0; i < (newFishCycleLength + 1); i++)
                initialFishState[i] = lanternfish.Where(f => f == i).Count();

            return initialFishState;
        }

        static void BreedTheFish(ref long[] currentFishState, int lifetime)
        {
            for (int day = 0; day < lifetime; day++)
            {
                // remember the number of fish at the zero state,
                // as they're going to create new fish at 1:1 ratio
                var newFish = currentFishState[0];

                // update all the fish's lifecycles
                var newFishState = new long[newFishCycleLength + 1];                    // set up a temporary holding array
                Array.Copy(currentFishState, 1, newFishState, 0, newFishCycleLength);   // shift the fish state left
                newFishState[baseFishCycleLength] += newFish;                           // add the fish that are creating new fish back into the breeding pool
                newFishState[newFishCycleLength] = newFish;                             // add the new fish into the state
                Array.Copy(newFishState, currentFishState, newFishCycleLength + 1);     // pull the updated state back into the current state
            }
        }
    }
}
