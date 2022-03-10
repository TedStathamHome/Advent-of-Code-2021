using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day03
{
    // Puzzle Description: https://adventofcode.com/2021/day/3

    class Program
    {
        const int FromBinary = 2;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 3");

            //var diagnosticReport = File.ReadLines(@".\DiagnosticReport-test.txt").ToList();
            var diagnosticReport = File.ReadLines(@".\DiagnosticReport-full.txt").ToList();
            var numberOfEntries = diagnosticReport.Count;
            var numberOfBits = diagnosticReport.First().Length;

            Console.WriteLine($"* # of diagnostic report entries: {numberOfEntries:N0}");
            Console.WriteLine($"* # of bits in each entry: {numberOfBits}");

            PartA(diagnosticReport, numberOfEntries, numberOfBits);
            PartB(diagnosticReport, numberOfBits);
        }

        static void PartA(List<string> diagnosticReport, int numberOfEntries, int numberOfBits)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var highCountBits = new char[numberOfBits];

            for (int bit = 0; bit < numberOfBits; bit++)
            {
                var zeroBitCount = diagnosticReport.Where(dr => dr[bit] == '0').Count();
                highCountBits[bit] = (zeroBitCount > (numberOfEntries - zeroBitCount)) ? '0' : '1';
            }

            var gammaRateRaw = "";
            var epsilonRateRaw = "";

            for (int bit = 0; bit < numberOfBits; bit++)
            {
                gammaRateRaw += highCountBits[bit];
                epsilonRateRaw += highCountBits[bit] == '0' ? '1' : '0';
            }

            Console.WriteLine($"** Raw gamma rate:   {gammaRateRaw}");
            Console.WriteLine($"** Raw epsilon rate: {epsilonRateRaw}");

            var gammaRate = Convert.ToInt32(gammaRateRaw, FromBinary);
            var epsilonRate = Convert.ToInt32(epsilonRateRaw, FromBinary);

            Console.WriteLine($"** Integer gamma rate:       {gammaRate:N0}");
            Console.WriteLine($"** Integer epsilon rate:     {epsilonRate:N0}");
            Console.WriteLine($"*** Power consumption rate: {gammaRate * epsilonRate:N0}");
        }

        static void PartB(List<string> diagnosticReport, int numberOfBits)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var filteredReportEntries = new List<string>();
            filteredReportEntries.AddRange(diagnosticReport);

            // Determine the oxygen generator rating
            for (int bit = 0; bit < numberOfBits; bit++)
            {
                var filteredZeroEntries = filteredReportEntries.Where(e => e[bit] == '0').ToList();
                var filteredOneEntries = filteredReportEntries.Where(e => e[bit] == '1').ToList();

                filteredReportEntries.Clear();

                if (filteredOneEntries.Count >= filteredZeroEntries.Count)
                {
                    filteredReportEntries.AddRange(filteredOneEntries);
                }
                else
                {
                    filteredReportEntries.AddRange(filteredZeroEntries);
                }

                if (filteredReportEntries.Count == 1)
                    break;
            }

            var oxygenGeneratorRating = Convert.ToInt32(filteredReportEntries.First(), FromBinary);
            Console.WriteLine($"** Oxygen generator rating (binary / decimal): {filteredReportEntries.First()} / {oxygenGeneratorRating:N0}");

            filteredReportEntries.Clear();
            filteredReportEntries.AddRange(diagnosticReport);

            // Determine the CO2 scrubber rating
            for (int bit = 0; bit < numberOfBits; bit++)
            {
                var filteredZeroEntries = filteredReportEntries.Where(e => e[bit] == '0').ToList();
                var filteredOneEntries = filteredReportEntries.Where(e => e[bit] == '1').ToList();

                filteredReportEntries.Clear();

                if (filteredZeroEntries.Count <= filteredOneEntries.Count)
                {
                    filteredReportEntries.AddRange(filteredZeroEntries);
                }
                else
                {
                    filteredReportEntries.AddRange(filteredOneEntries);
                }

                if (filteredReportEntries.Count == 1)
                    break;
            }

            var co2ScrubberRating = Convert.ToInt32(filteredReportEntries.First(), FromBinary);
            Console.WriteLine($"** CO2 scrubber rating (binary / decimal):     {filteredReportEntries.First()} / {co2ScrubberRating:N0}");

            Console.WriteLine($"*** Life support rating: {oxygenGeneratorRating * co2ScrubberRating:N0}");
        }
    }
}
