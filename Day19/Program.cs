using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day19
{
    // Puzzle Description: https://adventofcode.com/2021/day/19

    class Program
    {
        private class Beacon
        {
            public int XPosition { get; set; }
            public int YPosition { get; set; }
            public int ZPosition { get; set; }
            public List<double> NearbyBeaconDistances { get; set; } = new();
        }

        private class Scanner
        {
            public List<Beacon> NearbyBeacons { get; set; } = new();
            public List<int> OverlappingScanners { get; set; } = new();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 19");

            var scannerAndBeaconDataRaw = File.ReadLines(@".\ScannerAndBeaconData-test.txt").ToList();
            //var scannerAndBeaconDataRaw = File.ReadLines(@".\ScannerAndBeaconData-full.txt").ToList();

            PartA(scannerAndBeaconDataRaw);
            PartB();
        }

        static void PartA(List<string> scannerAndBeaconDataRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var scannerAndBeaconData = ParseScannerAndBeaconData(scannerAndBeaconDataRaw);
            var overlappingScanners = FindOverlappingScanners(scannerAndBeaconData);

            foreach (var scanner in overlappingScanners.Keys)
            {
                Console.WriteLine($"** Scanner {scanner} overlaps with scanner(s) {string.Join(", ", overlappingScanners[scanner].Select(s => s.ToString()))}");
            }
        }

        static void PartB()
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

        }

        static private Dictionary<int, Scanner> ParseScannerAndBeaconData(List<string> scannerAndBeaconDataRaw)
        {
            var scannerAndBeaconData = new Dictionary<int, Scanner>();
            var currentScanner = -1;

            foreach (var line in scannerAndBeaconDataRaw)
            {
                // skip blank lines
                if (line.Trim().Length == 0)
                {
                    if (currentScanner >= 0)
                    {
                        Console.WriteLine($"** Scanner {currentScanner} can see {scannerAndBeaconData[currentScanner].NearbyBeacons.Count} beacons.");
                    }

                    continue;
                }

                // set up a new scanner
                if (line.StartsWith("---"))
                {
                    currentScanner = int.Parse(line.Replace("---", "").Replace("scanner", "").Trim());
                    scannerAndBeaconData.Add(currentScanner, new Scanner());
                    continue;
                }

                // all other lines are beacons associated with the current scanner
                var beaconCoordinates = line.Split(',').Select(c => int.Parse(c)).ToArray();

                scannerAndBeaconData[currentScanner].NearbyBeacons.Add(new Beacon()
                {
                    XPosition = beaconCoordinates[0],
                    YPosition = beaconCoordinates[1],
                    ZPosition = beaconCoordinates[2]
                });
            }

            Console.WriteLine($"** Scanner {currentScanner} can see {scannerAndBeaconData[currentScanner].NearbyBeacons.Count} beacons.");

            CalculateBeaconDistances(scannerAndBeaconData);
            // use .Intersect() LINQ method to find matching distances

            return scannerAndBeaconData;
        }

        /// <summary>
        /// Calculates all the distances between the beacons associated with
        /// each scanner. These distances are used later on to help determine
        /// how/if a scanner's beacons overlap with other scanners.
        /// </summary>
        /// <param name="scannerAndBeaconData">The list of scanners and their nearby beacons.</param>
        static private void CalculateBeaconDistances(Dictionary<int, Scanner> scannerAndBeaconData)
        {
            // loop through the scanners
            foreach (var scanner in scannerAndBeaconData.Keys)
            {
                // loop through the scanner's nearby beacons
                for (int cb = 0; cb < scannerAndBeaconData[scanner].NearbyBeacons.Count; cb++)
                {
                    // loop through the scanner's nearby beacons a second time,
                    // leaving out the current beacon, and calculate the distance
                    // between each pair
                    for (int nb = 0; nb < scannerAndBeaconData[scanner].NearbyBeacons.Count; nb++)
                    {
                        // don't calculate distance if the "nearby" beacon is the current beacon
                        if (cb != nb)
                        {
                            scannerAndBeaconData[scanner].NearbyBeacons[cb].NearbyBeaconDistances.Add(DistanceBetweenBeacons(
                                scannerAndBeaconData[scanner].NearbyBeacons[cb],  // the current beacon
                                scannerAndBeaconData[scanner].NearbyBeacons[nb]   // the nearby beacon
                                ));
                        }
                    }

                    scannerAndBeaconData[scanner].NearbyBeacons[cb].NearbyBeaconDistances.Sort();
                }
            }
        }

        /// <summary>
        /// Calculates the 3D straight-line distance between two beacons, 
        /// regardless of their orientation from their associated scanner.
        /// See: https://www.calculatorsoup.com/calculators/geometry-solids/distance-two-points.php
        /// </summary>
        /// <param name="beacon1">The first beacon.</param>
        /// <param name="beacon2">The second beacon.</param>
        /// <returns>The 3D straight-line distance between the first and second beacons.</returns>
        static private double DistanceBetweenBeacons(Beacon beacon1, Beacon beacon2)
        {
            return Math.Sqrt(Math.Pow(beacon2.XPosition - beacon1.XPosition, 2)
                + Math.Pow(beacon2.YPosition - beacon1.YPosition, 2)
                + Math.Pow(beacon2.ZPosition - beacon1.ZPosition, 2));
        }

        static private Dictionary<int, List<int>> FindOverlappingScanners(Dictionary<int, Scanner> scannerAndBeaconData)
        {
            var overlappingScanners = new Dictionary<int, List<int>>();

            foreach (var scanner1 in scannerAndBeaconData.Keys)
            {
                foreach (var scanner2 in scannerAndBeaconData.Keys)
                {
                    if (scanner1 != scanner2)
                    {
                        if (ScannersHaveOverlappingBeacons(scannerAndBeaconData[scanner1], scannerAndBeaconData[scanner2]))
                        {
                            if (!overlappingScanners.ContainsKey(scanner1))
                                overlappingScanners[scanner1] = new();

                            overlappingScanners[scanner1].Add(scanner2);
                        }
                    }
                }
            }

            return overlappingScanners;
        }

        static private bool ScannersHaveOverlappingBeacons(Scanner scanner1, Scanner scanner2)
        {
            const int minimumMatchingDistances = 6;
            const int minimumMatchingBeacons = 12;

            var similarDistances = new Dictionary<int, List<int>>();

            for (int s1b = 0; s1b < scanner1.NearbyBeacons.Count; s1b++)
            {
                for (int s2b = 0; s2b < scanner2.NearbyBeacons.Count; s2b++)
                {
                    var matchingDistancesCount = scanner1.NearbyBeacons[s1b].NearbyBeaconDistances
                        .Intersect(scanner2.NearbyBeacons[s2b].NearbyBeaconDistances)
                        .Count();

                    if (matchingDistancesCount >= minimumMatchingDistances)
                    {
                        if (!similarDistances.ContainsKey(s1b))
                            similarDistances[s1b] = new();

                        similarDistances[s1b].Add(matchingDistancesCount);
                    }
                }
            }

            return similarDistances.Count >= minimumMatchingBeacons;
        }
    }
}
