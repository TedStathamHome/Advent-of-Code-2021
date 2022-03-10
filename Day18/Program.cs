using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day18
{
    // Puzzle Description: https://adventofcode.com/2021/day/18

    class Program
    {
        private static string SnailfishNumberStructure;
        private static List<int[]> LeftBracketIndexes = new();
        private static List<int[]> RightBracketIndexes = new();
        private static List<int> BracketIndexes = new();
        private static List<int[]> RegularNumbers = new();
        private static List<int[]> CommaIndexes = new();

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 18");

            //var valuesToAdd = File.ReadLines(@".\ValuesToAdd-test.txt").ToList();
            var valuesToAdd = File.ReadLines(@".\ValuesToAdd-full.txt").ToList();

            PartA(valuesToAdd);
            PartB(valuesToAdd);
        }

        static void PartA(List<string> valuesToAdd)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var currentNumber = valuesToAdd.First();

            for (var n = 1; n < valuesToAdd.Count; n++)
            {
                //Console.WriteLine($"*   {currentNumber}");
                //Console.WriteLine($"* + {valuesToAdd[n]}");
                currentNumber = $"[{currentNumber},{valuesToAdd[n]}]";
                //Console.WriteLine($"* = {currentNumber}");

                currentNumber = ReducedSnailfishNumber(currentNumber);
            }

            //Console.WriteLine("\r\n****************************************\r\n");

            var magnitude = CalculateMagnitude(currentNumber);
            Console.WriteLine($"\r\n*** Magnitude is: {magnitude:N0}");
        }

        static void PartB(List<string> valuesToAdd)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var magnitudes = new List<int>();

            for (int l = 0; l < valuesToAdd.Count; l++)
            {
                for (int r = 0; r < valuesToAdd.Count; r++)
                {
                    if (l != r)
                    {
                        var currentNumber = $"[{valuesToAdd[l]},{valuesToAdd[r]}]";
                        currentNumber = ReducedSnailfishNumber(currentNumber);
                        var magnitude = CalculateMagnitude(currentNumber);
                        magnitudes.Add(magnitude);
                    }
                }
            }

            magnitudes.Sort();
            var highestMagnitude = magnitudes.Last();

            Console.WriteLine($"\r\n*** Highest magnitude is: {highestMagnitude:N0}");
        }

        static string ReducedSnailfishNumber(string startingSnailfishNumber)
        {
            var currentSnailfishNumber = startingSnailfishNumber;
            const string pairMarker = "[]";
            var lastIndexChecked = 0;

            while (true)
            {
                PrepareToReduceSnailfishNumber(currentSnailfishNumber);

                var pairIndexes = new List<int>();
                var lastIndex = 0;
                var explodeOccurred = false;
                var splitOccurred = false;

                while (lastIndex < SnailfishNumberStructure.Length)
                {
                    var pairIndex = SnailfishNumberStructure.IndexOf(pairMarker, lastIndex);

                    if (pairIndex == -1)
                        break;

                    pairIndexes.Add(pairIndex);
                    lastIndex = pairIndex + pairMarker.Length;
                }

                if (pairIndexes.Count > 0)     // we found one or more pairs
                {
                    foreach (var pairIndex in pairIndexes)
                    {
                        var leftBracketIndex = BracketIndexes[pairIndex];
                        var rightBracketIndex = BracketIndexes[pairIndex + 1];

                        // To determine if the pair is nested at least 4 deep,
                        // subtract the number of preceding left brackets from
                        // the number of leading right brackets. If it is 4 or
                        // greater, then an explode needs to be performed.

                        var leadingLeftBrackets = LeftBracketIndexes.Count(b => b[1] < leftBracketIndex);
                        var leadingRightBrackets = RightBracketIndexes.Count(b => b[1] < leftBracketIndex);

                        if (leadingLeftBrackets - leadingRightBrackets >= 4)
                        {
                            //Console.WriteLine("* EXPLODING");
                            lastIndexChecked += pairMarker.Length;
                            var indexOfFirstRegularNumber = leftBracketIndex + 1;
                            //Console.WriteLine($"** index of first regular number: {indexOfFirstRegularNumber}");
                            var leftRegularNumber = RegularNumbers.Where(r => r[0] == indexOfFirstRegularNumber).First();
                            var rightRegularNumber = RegularNumbers.Where(r => r[0] > indexOfFirstRegularNumber).First();
                            //Console.WriteLine($"* Regular numbers in pair @ {indexOfFirstRegularNumber - 1:N0}: [{leftRegularNumber[1]},{rightRegularNumber[1]}]");

                            var explodedSnailfishNumber = new StringBuilder();

                            // add the left value to the regular number to the left, if any
                            if (RegularNumbers.Where(r => r[0] < indexOfFirstRegularNumber).Any())
                            {
                                var regularNumberDetailsToLeftOfExplode = RegularNumbers.Where(r => r[0] < indexOfFirstRegularNumber).Last();
                                var textBeforeTheRegularNumber = currentSnailfishNumber.Substring(0, regularNumberDetailsToLeftOfExplode[0]);
                                var indexAfterTheRegularNumber = regularNumberDetailsToLeftOfExplode[0] + $"{regularNumberDetailsToLeftOfExplode[1]}".Length;
                                var numberOfCharactersAfterRegularNumberUpToPair = leftBracketIndex - indexAfterTheRegularNumber;
                                var textAfterTheRegularNumberUpToPair = currentSnailfishNumber.Substring(indexAfterTheRegularNumber, numberOfCharactersAfterRegularNumberUpToPair);
                                explodedSnailfishNumber.Append(textBeforeTheRegularNumber);
                                explodedSnailfishNumber.Append($"{regularNumberDetailsToLeftOfExplode[1] + leftRegularNumber[1]}");
                                explodedSnailfishNumber.Append(textAfterTheRegularNumberUpToPair);
                            }
                            else    // there was no regular number to the left, so just take everthing before the pair started
                            {
                                var textBeforeThePair = currentSnailfishNumber.Substring(0, leftBracketIndex);
                                explodedSnailfishNumber.Append(textBeforeThePair);
                            }

                            explodedSnailfishNumber.Append("0");    // replace the pair with a zero

                            // add the right value to the regular number to the right, if any
                            if (RegularNumbers.Where(r => r[0] > rightRegularNumber[0]).Any())
                            {
                                var regularNumberDetailsToRightOfExplode = RegularNumbers.Where(r => r[0] > rightRegularNumber[0]).First();
                                var textBeforeTheRegularNumber = currentSnailfishNumber.Substring(rightBracketIndex + 1, regularNumberDetailsToRightOfExplode[0] - rightBracketIndex - 1);
                                var indexAfterTheRegularNumber = regularNumberDetailsToRightOfExplode[0] + $"{regularNumberDetailsToRightOfExplode[1]}".Length;
                                explodedSnailfishNumber.Append(textBeforeTheRegularNumber);
                                explodedSnailfishNumber.Append($"{regularNumberDetailsToRightOfExplode[1] + rightRegularNumber[1]}");
                                explodedSnailfishNumber.Append(currentSnailfishNumber.Substring(indexAfterTheRegularNumber));
                            }
                            else    // there was no regular number to the right, so just take everything after the pair ended
                            {
                                var textAfterThePair = currentSnailfishNumber.Substring(rightBracketIndex + 1);
                                explodedSnailfishNumber.Append(textAfterThePair);
                            }

                            //Console.WriteLine($"* Partial new number: {explodedSnailfishNumber}");
                            // add the right value to the regular number to the right, if any
                            // replace the pair with the regular number 0

                            // only process the first explode
                            explodeOccurred = true;
                            currentSnailfishNumber = explodedSnailfishNumber.ToString();
                            break;
                        }
                    }

                    if (explodeOccurred)
                        continue;
                }

                // if no explodes occurred, look for the first regular number that is 10+
                if (!explodeOccurred)
                {
                    if (RegularNumbers.Where(r => r[1] >= 10).Any())
                    {
                        //Console.WriteLine("* SPLITTING");
                        var firstSplittableRegularNumber = RegularNumbers.Where(r => r[1] >= 10).First();
                        //Console.WriteLine($"** index of first splittable regular number: {firstSplittableRegularNumber[0]}");
                        //Console.WriteLine($"** splittable number is: {firstSplittableRegularNumber[1]}");
                        var splitSnailfishNumber = new StringBuilder();
                        splitSnailfishNumber.Append(currentSnailfishNumber.Substring(0, firstSplittableRegularNumber[0]));

                        var leftRegularNumber = firstSplittableRegularNumber[1] / 2;
                        var rightRegularNumber = firstSplittableRegularNumber[1] - leftRegularNumber;
                        splitSnailfishNumber.Append($"[{leftRegularNumber},{rightRegularNumber}]");

                        var indexAfterTheSplittableRegularNumber = firstSplittableRegularNumber[0] + $"{firstSplittableRegularNumber[1]}".Length;
                        splitSnailfishNumber.Append(currentSnailfishNumber.Substring(indexAfterTheSplittableRegularNumber));

                        //Console.WriteLine($"* Partial new number: {splitSnailfishNumber}");
                        splitOccurred = true;
                        currentSnailfishNumber = splitSnailfishNumber.ToString();
                    }
                }

                if (!explodeOccurred && !splitOccurred)
                    break;
            }

            return currentSnailfishNumber;
        }

        static void PrepareToReduceSnailfishNumber(string snailfishNumber)
        {
            SnailfishNumberStructure = "";
            LeftBracketIndexes.Clear();
            RightBracketIndexes.Clear();
            BracketIndexes.Clear();
            RegularNumbers.Clear();
            CommaIndexes.Clear();
            var regularNumber = "";
            var regularNumberStart = 0;
            var leftBracketCount = 0;
            var rightBracketCount = 0;
            var commaCount = 0;

            for (int i = 0; i < snailfishNumber.Length; i++)
            {
                switch (snailfishNumber[i])
                {
                    case '[':
                        SnailfishNumberStructure += snailfishNumber[i];
                        BracketIndexes.Add(i);
                        LeftBracketIndexes.Add(new int[] { leftBracketCount, i });
                        leftBracketCount++;
                        CheckForRegularNumber(ref regularNumber, ref regularNumberStart);
                        break;

                    case ']':
                        SnailfishNumberStructure += snailfishNumber[i];
                        BracketIndexes.Add(i);
                        RightBracketIndexes.Add(new int[] { rightBracketCount, i });
                        rightBracketCount++;
                        CheckForRegularNumber(ref regularNumber, ref regularNumberStart);
                        break;

                    case ',':
                        CommaIndexes.Add(new int[] { commaCount, i });
                        commaCount++;
                        CheckForRegularNumber(ref regularNumber, ref regularNumberStart);
                        break;

                    default:
                        if (regularNumberStart == 0)
                            regularNumberStart = i;
                        regularNumber += snailfishNumber[i];
                        break;
                }
            }

            //Console.WriteLine($"\r\n* Structure is: {SnailfishNumberStructure}");

            static void CheckForRegularNumber(ref string regularNumber, ref int regularNumberStart)
            {
                if (regularNumberStart > 0)
                {
                    RegularNumbers.Add(new int[] { regularNumberStart, int.Parse(regularNumber) });
                    regularNumberStart = 0;
                    regularNumber = "";
                }
            }
        }

        private static int CalculateMagnitude(string startingSnailfishNumber)
        {
            var magnitude = 0;
            var currentSnailfishNumber = startingSnailfishNumber;
            const string pairMarker = "[]";
            //var lastIndexChecked = 0;

            while (true)
            {
                PrepareToReduceSnailfishNumber(currentSnailfishNumber);

                var pairIndexes = new List<int>();
                var lastIndex = 0;

                while (lastIndex < SnailfishNumberStructure.Length)
                {
                    var pairIndex = SnailfishNumberStructure.IndexOf(pairMarker, lastIndex);

                    if (pairIndex == -1)
                        break;

                    pairIndexes.Add(pairIndex);
                    lastIndex = pairIndex + pairMarker.Length;
                }

                if (pairIndexes.Count > 0)
                {
                    // loop through the pairs in reverse order, as this
                    // lets us rebuild the snailfish number right-to-left,
                    // replacing pairs with their calculated magnitude
                    for (int p = (pairIndexes.Count - 1); p >= 0; p--)
                    {
                        var leftBracketIndex = BracketIndexes[pairIndexes[p]];
                        var rightBracketIndex = BracketIndexes[pairIndexes[p] + 1];
                        var indexOfFirstRegularNumber = leftBracketIndex + 1;
                        var leftRegularNumber = RegularNumbers.Where(r => r[0] == indexOfFirstRegularNumber).First();
                        var rightRegularNumber = RegularNumbers.Where(r => r[0] > indexOfFirstRegularNumber).First();
                        //Console.WriteLine($"* Regular numbers in pair @ {leftBracketIndex:N0}: [{leftRegularNumber[1]},{rightRegularNumber[1]}]");

                        magnitude = leftRegularNumber[1] * 3 + rightRegularNumber[1] * 2;

                        if (leftBracketIndex > 0)
                        {
                            currentSnailfishNumber = currentSnailfishNumber.Substring(0, leftBracketIndex) + $"{magnitude}" + currentSnailfishNumber.Substring(rightBracketIndex + 1);
                            //Console.WriteLine($"* Magnitude updated:\r\n** {currentSnailfishNumber}");
                        }
                    }

                    if (pairIndexes.Count == 1)
                        break;
                }
            }

            return magnitude;
        }
    }
}
