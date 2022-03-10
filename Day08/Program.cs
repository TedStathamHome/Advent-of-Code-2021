using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day08
{
    // Puzzle Description: https://adventofcode.com/2021/day/8

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 8");

            //var signalDataEntriesRaw = File.ReadLines(@".\SignalData-test.txt").ToList();
            var signalDataEntriesRaw = File.ReadLines(@".\SignalData-full.txt").ToList();
            Console.WriteLine($"* # of signal data entries: {signalDataEntriesRaw.Count:N0}");

            PartA(signalDataEntriesRaw);
            PartB(signalDataEntriesRaw);
        }

        static void PartA(List<string> signalDataEntriesRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var outputValuesRaw = signalDataEntriesRaw.Select(d => d.Split('|', StringSplitOptions.TrimEntries)[1]).ToList();
            var outputValues = new List<string>();

            foreach (var outputValue in outputValuesRaw)
            {
                outputValues.AddRange(outputValue.Split(' '));
            }

            Console.WriteLine($"* # of output values: {outputValues.Count:N0}");

            // size of output value strings for numbers 1, 7, 4, and 8, respectively
            int[] outputValueLengths = { 2, 3, 4, 7 };

            var digitCount = outputValues.Where(v => outputValueLengths.Contains(v.Length)).Count();

            Console.WriteLine($"*** The digits 1, 4, 7, and 8 appear a total of {digitCount:N0} times.");
        }

        static void PartB(List<string> signalDataEntriesRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var signalDataEntries = signalDataEntriesRaw.Select(d => new
            {
                SignalPatterns = d.Split('|', StringSplitOptions.TrimEntries)[0].Split(' ').ToList(),
                OutputValues = d.Split('|', StringSplitOptions.TrimEntries)[1].Split(' ').ToList()
            }).ToList();

            // Processing notes on how to deduce how the display is wired

            // unique configurations
            // 1 = 0010010                          0010010
            // 4 = 0111010      diff from 1       - 0101000
            // 7 = 1010010      diff from 1, 7    - 1000000
            // 8 = 1111111      diff from 1, 4, 7 - 0000101

            // digits using 5 elements of the seven element display
            // 2 = 1011101
            // 3 = 1011011
            // 5 = 1101011

            // digits using 6 elements of the seven element display
            // 0 = 1110111      0 has only 1 element of 4 diff from 1
            // 6 = 1101111      6 has only 1 element of 1
            // 9 = 1111011      9 has only 1 element of 8 diff from 1, 4, 7

            // find 7 length -> #8
            // find 2 length -> #1; indicators 3 and 6
            // find 6 length items
            //      find one which contains only 1 character of #1; this is #6
            //      found character is indicator 6 (index 5), unfound is indicator 3 (index 2)
            // find 3 length -> #7
            //      character that is not in #1 is indicator 1 (index 0)
            // indicators known: 1, 3, 6
            // find 4 length -> #4
            //      the characters not in #1 are indicators 2 and 4
            // #s known: 1, 4, 6, 7, 8
            // find 5 length items
            //      find the one which contains both characters from #4 diff; this is #5
            //      find which character is missing from the other 2
            //          missing character is indicator 2
            //          found character is indicator 4
            // indicators known: 1, 2, 3, 4, 6
            // find 6 length item missing indicator 4 -> #0
            // find 5 length items
            //      find the one which contains two unknown indicators; this is #2, indicators 5 and 7
            //      find which character is missing from the other 2
            //          missing character is indicator 5
            //          found character is indicator 7
            // all indicators now known
            // #s known: 0, 1, 2, 4, 5, 6, 7, 8
            // find 6 length missing indicator 5 -> #9
            // remaining 5 length -> #3

            var totalOutputValues = 0;

            foreach (var signalDataEntry in signalDataEntries)
            {
                var signalDigits = signalDataEntry.SignalPatterns.ToList();
                signalDigits.AddRange(signalDataEntry.OutputValues.ToList());

                for (var i = 0; i < signalDigits.Count; i++)
                {
                    var digits = signalDigits[i].ToCharArray();
                    Array.Sort(digits);
                    signalDigits[i] = new string(digits);
                }

                var uniqueSignalDigits = signalDigits.Distinct().ToList();

                //Console.WriteLine($"* Unique signal digits: {uniqueSignalDigits.Count:N0}");

                var outputValues = signalDataEntry.OutputValues.ToList();
                for (int i = 0; i < outputValues.Count; i++)
                {
                    var digits = outputValues[i].ToCharArray();
                    Array.Sort(digits);
                    outputValues[i] = new string(digits);
                }

                var remainingIndicators = "abcdefg";

                // dictionary => signal digit, real digit (0 - 9)
                var digitMap = new string[10];

                // add the easy #s (1, 4, 7, 8) based on their unique lengths
                digitMap[1] = uniqueSignalDigits.Where(d => d.Length == 2).First();
                digitMap[4] = uniqueSignalDigits.Where(d => d.Length == 4).First();
                digitMap[7] = uniqueSignalDigits.Where(d => d.Length == 3).First();
                digitMap[8] = uniqueSignalDigits.Where(d => d.Length == 7).First();

                //Console.WriteLine($"* 1 = {digitMap[1]}, 4 = {digitMap[4]}, 7 = {digitMap[7]}, 8 = {digitMap[8]}");

                var signalDigits5 = uniqueSignalDigits.Where(d => d.Length == 5).ToList();
                var signalDigits6 = uniqueSignalDigits.Where(d => d.Length == 6).ToList();

                var indicator = new char[8];
                var indicators3and6 = digitMap[1];

                // find the 6 length entry which contains only 1 of indicators 3 and 6
                // this is #6
                var digit6 = signalDigits6
                    .Where(d => !(d.Contains(indicators3and6[0]) && d.Contains(indicators3and6[1])))
                    .ToList().First();
                digitMap[6] = digit6;
                signalDigits6.Remove(digit6);
                //Console.WriteLine($"* 6 = {digitMap[6]}");

                // if the first character for #1 is found in #6, it is indicator 6, otherwise indicator 3
                var isIndicator6 = digit6.Contains(indicators3and6[0]);
                indicator[3] = isIndicator6 ? indicators3and6[1] : indicators3and6[0];
                indicator[6] = isIndicator6 ? indicators3and6[0] : indicators3and6[1];

                remainingIndicators = remainingIndicators
                    .Replace(indicator[3].ToString(), "")
                    .Replace(indicator[6].ToString(), "");

                // remove the characters for #1 from #7, and the remaining character is indicator 1
                var indicator1 = digitMap[7].Replace(indicator[3].ToString(), "").Replace(indicator[6].ToString(), "");
                indicator[1] = indicator1[0];

                remainingIndicators = remainingIndicators.Replace(indicator[1].ToString(), "");

                // pull the two unknown indicators out of the characters for #4
                var indicators2and4 = digitMap[4].Replace(indicator[3].ToString(), "").Replace(indicator[6].ToString(), "");

                // find the 5 length entry which contains both indicators 2 and 4
                // this is #5
                var digit5 = signalDigits5
                    .Where(d => d.Contains(indicators2and4[0]) && d.Contains(indicators2and4[1]))
                    .First();
                digitMap[5] = digit5;
                signalDigits5.Remove(digit5);
                //Console.WriteLine($"* 5 = {digitMap[5]}");

                // from the remaining 5 length entries, pull one of them; it doesn't matter which one
                var a5LengthDigit = signalDigits5.First();

                // whichever of indicators 2 or 4 we can find in the 5 length entry,
                // it is indicator 4 and the other is indicator 2
                var isIndicator4 = a5LengthDigit.Contains(indicators2and4[0]);
                indicator[2] = isIndicator4 ? indicators2and4[1] : indicators2and4[0];
                indicator[4] = isIndicator4 ? indicators2and4[0] : indicators2and4[1];

                remainingIndicators = remainingIndicators
                    .Replace(indicator[2].ToString(), "")
                    .Replace(indicator[4].ToString(), "");

                // find the 6 length entry missing indicator 4, this is #0
                var digit0 = signalDigits6
                    .Where(d => !d.Contains(indicator[4])).First();

                digitMap[0] = digit0;
                signalDigits6.Remove(digit0);
                //Console.WriteLine($"* 0 = {digitMap[0]}");

                // at this point, we should have indicators 1, 2, 3, 4, and 6

                // find the 5 length entry which contains both remaining indicators
                // this is #2
                var digit2 = signalDigits5
                    .Where(d => d.Contains(remainingIndicators[0]) && d.Contains(remainingIndicators[1]))
                    .First();
                digitMap[2] = digit2;
                signalDigits5.Remove(digit2);
                //Console.WriteLine($"* 2 = {digitMap[2]}");

                // the remaining 5 length entry is #3
                var digit3 = signalDigits5.First();
                digitMap[3] = digit3;
                //Console.WriteLine($"* 3 = {digitMap[3]}");

                // whichever of the remaining indicators we can find in #3,
                // it is indicator 7 and the other is indicator 5
                var isIndicator7 = digit3.Contains(remainingIndicators[0]);
                indicator[5] = isIndicator7 ? remainingIndicators[1] : remainingIndicators[0];
                indicator[7] = isIndicator7 ? remainingIndicators[0] : remainingIndicators[1];

                // all indicators are now known

                // the remaining 6 length entry is #9
                var digit9 = signalDigits6.First();
                digitMap[9] = digit9;
                //Console.WriteLine($"* 9 = {digitMap[9]}");

                var codeMap = digitMap.ToList();

                // all digits are now known

                var signalOutput = 1000 * codeMap.IndexOf(outputValues[0])
                    + 100 * codeMap.IndexOf(outputValues[1])
                    + 10 * codeMap.IndexOf(outputValues[2])
                    + codeMap.IndexOf(outputValues[3]);

                totalOutputValues += signalOutput;

                Console.Write("* Output values:");
                foreach (var value in outputValues)
                {
                    Console.Write($" {value}");
                }

                Console.WriteLine($": {signalOutput:N0}");
            }

            Console.WriteLine($"*** Total of output values: {totalOutputValues:N0}");
        }
    }
}
