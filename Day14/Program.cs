using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day14
{
    // Puzzle Description: https://adventofcode.com/2021/day/14

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 14");

            //var polymerInstructionsRaw = File.ReadLines(@".\PolymerInstructions-test.txt").ToList();
            var polymerInstructionsRaw = File.ReadLines(@".\PolymerInstructions-full.txt").ToList();

            PartA(polymerInstructionsRaw);
            PartB(polymerInstructionsRaw);
        }

        static void PartA(List<string> polymerInstructionsRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            ProcessPolymerInstructions(polymerInstructionsRaw, 10);
        }

        static void PartB(List<string> polymerInstructionsRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            ProcessPolymerInstructions(polymerInstructionsRaw, 40);
        }

        static void ProcessPolymerInstructions(List<string> polymerInstructionsRaw, int stepsToRun)
        {
            var polymerTemplate = polymerInstructionsRaw.First();

            var instructionStepsBase = polymerInstructionsRaw
                .Where(i => i.Contains("->")).ToList()
                .Select(i => i.Split("->", StringSplitOptions.TrimEntries).ToArray()).ToList();

            var instructionSteps = new Dictionary<string, string>();

            foreach (var step in instructionStepsBase)
                instructionSteps.Add(step[0], step[1]);

            var templatePairs = BuildTemplatePairs(polymerTemplate);
            Console.WriteLine($"** Unique template pairs found: {templatePairs.Count:N0}");

            for (int s = 0; s < stepsToRun; s++)
            {
                var postRulePairs = new Dictionary<string, long>();

                foreach (var pair in templatePairs)
                {
                    if (instructionSteps.ContainsKey(pair.Key))
                    {
                        var leftPair = pair.Key[0] + instructionSteps[pair.Key];
                        var rightPair = instructionSteps[pair.Key] + pair.Key[1];

                        AddPostRulePair(postRulePairs, leftPair, pair.Value);
                        AddPostRulePair(postRulePairs, rightPair, pair.Value);
                    }
                    else
                    {
                        AddPostRulePair(postRulePairs, pair.Key, pair.Value);
                    }
                }

                templatePairs = postRulePairs;

                Console.WriteLine($"** Step {s:N0}: pair count: {templatePairs.Count:N0}; polymer size: {templatePairs.Sum(p => p.Value) + 1:N0}");
            }

            var polymerElements = new Dictionary<char, long>();

            foreach (var pair in templatePairs)
            {
                foreach (var element in pair.Key)
                {
                    if (!polymerElements.TryAdd(element, pair.Value))
                        polymerElements[element] += pair.Value;
                }
            }

            foreach (var element in polymerElements.OrderBy(e => e.Key))
            {
                var elementCount = GetElementCount(polymerTemplate, element);
                Console.WriteLine($"** {element.Key} => {elementCount:N0}");
            }

            var orderedElements = polymerElements.OrderBy(e => e.Value);
            var leastAbundantElement = orderedElements.First();
            var leastAbundantElementCount = GetElementCount(polymerTemplate, leastAbundantElement);
            var mostAbundantElement = orderedElements.Last();
            var mostAbundantElementCount = GetElementCount(polymerTemplate, mostAbundantElement);

            Console.WriteLine($"** Least abundant element: {leastAbundantElement.Key} - {leastAbundantElementCount:N0}");
            Console.WriteLine($"**  Most abundant element: {mostAbundantElement.Key} - {mostAbundantElementCount:N0}");
            Console.WriteLine($"*** Difference: {mostAbundantElementCount - leastAbundantElementCount:N0}");
        }

        private static long GetElementCount(string polymerTemplate, KeyValuePair<char, long> element)
        {
            return (element.Value + ((polymerTemplate[0] == element.Key || polymerTemplate[polymerTemplate.Length - 1] == element.Key) ? 1 : 0)) / 2;
        }

        static Dictionary<string, long> BuildTemplatePairs(string PolymerTemplate)
        {
            var templatePairs = new Dictionary<string, long>();

            for (int i = 0; i < PolymerTemplate.Length - 1; i++)
            {
                var pair = PolymerTemplate.Substring(i, 2);

                if (!templatePairs.TryAdd(pair, 1))
                    templatePairs[pair]++;
            }

            return templatePairs;
        }

        static void AddPostRulePair(Dictionary<string, long> postRulePairs, string key, long value)
        {
            if (!postRulePairs.TryAdd(key, value))
                postRulePairs[key] += value;
        }
    }
}
