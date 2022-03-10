using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day22
{
    // Puzzle Description: https://adventofcode.com/2021/day/22

    class Program
    {
        // Part A classes/variables
        public class RebootStep
        {
            public bool TurnOnCuboid { get; set; }
            public bool CuboidAffectsMainArea { get; set; }
            public int XRangeStart { get; set; }
            public int XRangeEnd { get; set; }
            public int YRangeStart { get; set; }
            public int YRangeEnd { get; set; }
            public int ZRangeStart { get; set; }
            public int ZRangeEnd { get; set; }
        }

        private const int CoordinateOffset = 50;    // adjust all defined ranges by this amount
        private const int AxisRange = 101;          // going from -50 to +50 is 101 elements

        // Part B classes/variables
        public class Cuboid
        {
            public bool TurnOnCuboid { get; set; }
            public int XRangeStart { get; set; }
            public int XRangeEnd { get; set; }
            public int YRangeStart { get; set; }
            public int YRangeEnd { get; set; }
            public int ZRangeStart { get; set; }
            public int ZRangeEnd { get; set; }
        }

        private static int XRangeMin;
        private static int XRangeMax;
        private static int XRange;
        private static int XRangeOffset;
        private static int YRangeMin;
        private static int YRangeMax;
        private static int YRange;
        private static int YRangeOffset;
        private static int ZRangeMin;
        private static int ZRangeMax;
        private static int ZRange;
        private static int ZRangeOffset;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 22");

            //var rebootStepsPartARaw = File.ReadLines(@".\RebootSteps-test-part-a.txt").ToList();
            //var rebootStepsPartBRaw = File.ReadLines(@".\RebootSteps-test-part-b.txt").ToList();
            var rebootStepsPartARaw = File.ReadLines(@".\RebootSteps-full.txt").ToList();
            var rebootStepsPartBRaw = rebootStepsPartARaw.ToList();

            PartA(rebootStepsPartARaw);
            PartB(rebootStepsPartBRaw);
        }

        static void PartA(List<string> rebootStepsRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var rebootSteps = DecodeRawRebootStepsPartA(rebootStepsRaw);
            var reactor = new bool[AxisRange, AxisRange, AxisRange];
            var poweredOnCubes = 0;
            var currentStep = 0;

            foreach (var rebootStep in rebootSteps)
            {
                if (rebootStep.CuboidAffectsMainArea)
                {
                    // limit processing to the area of the reactor we're concerned about
                    var xRangeStart = rebootStep.XRangeStart > 0 ? Math.Min(AxisRange - 1, rebootStep.XRangeStart) : Math.Max(0, rebootStep.XRangeStart);
                    var xRangeEnd = rebootStep.XRangeEnd > 0 ? Math.Min(AxisRange - 1, rebootStep.XRangeEnd) : Math.Max(0, rebootStep.XRangeEnd);
                    var yRangeStart = rebootStep.YRangeStart > 0 ? Math.Min(AxisRange - 1, rebootStep.YRangeStart) : Math.Max(0, rebootStep.YRangeStart);
                    var yRangeEnd = rebootStep.YRangeEnd > 0 ? Math.Min(AxisRange - 1, rebootStep.YRangeEnd) : Math.Max(0, rebootStep.YRangeEnd);
                    var zRangeStart = rebootStep.ZRangeStart > 0 ? Math.Min(AxisRange - 1, rebootStep.ZRangeStart) : Math.Max(0, rebootStep.ZRangeStart);
                    var zRangeEnd = rebootStep.ZRangeEnd > 0 ? Math.Min(AxisRange - 1, rebootStep.ZRangeEnd) : Math.Max(0, rebootStep.ZRangeEnd);

                    for (int x = xRangeStart; x < xRangeEnd + 1; x++)
                    {
                        for (int y = yRangeStart; y < yRangeEnd + 1; y++)
                        {
                            for (int z = zRangeStart; z < zRangeEnd + 1; z++)
                            {
                                if (rebootStep.TurnOnCuboid)
                                {
                                    if (!reactor[x, y, z])
                                        poweredOnCubes++;
                                }
                                else
                                {
                                    if (reactor[x, y, z])
                                        poweredOnCubes--;
                                }

                                reactor[x, y, z] = rebootStep.TurnOnCuboid;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"** Skipping step {currentStep + 1}: {rebootStepsRaw[currentStep]}");
                }

                currentStep++;
            }

            Console.WriteLine($"*** Powered on cubes: {poweredOnCubes:N0}");
        }

        private static List<RebootStep> DecodeRawRebootStepsPartA(List<string> rebootStepsRaw)
        {
            var rebootSteps = new List<RebootStep>();

            foreach (var rebootStepRaw in rebootStepsRaw)
            {
                var initialParse = rebootStepRaw.Split(' ');
                var secondaryParse = initialParse[1].Replace("x=", "").Replace("y=", "").Replace("z=", "").Split(',');
                var xRange = secondaryParse[0].Split("..");
                var yRange = secondaryParse[1].Split("..");
                var zRange = secondaryParse[2].Split("..");

                var rebootStep = new RebootStep()
                {
                    TurnOnCuboid = initialParse[0] == "on",
                    // adjust all the coordinates by the offset,
                    // so that they can be checked against the
                    // reactor array more easily
                    XRangeStart = int.Parse(xRange[0]) + CoordinateOffset,
                    XRangeEnd = int.Parse(xRange[1]) + CoordinateOffset,
                    YRangeStart = int.Parse(yRange[0]) + CoordinateOffset,
                    YRangeEnd = int.Parse(yRange[1]) + CoordinateOffset,
                    ZRangeStart = int.Parse(zRange[0]) + CoordinateOffset,
                    ZRangeEnd = int.Parse(zRange[1]) + CoordinateOffset
                };

                // order the axis ranges to be low->high
                // this makes later checks easier
                // reassignments use tuples
                if (rebootStep.XRangeStart > rebootStep.XRangeEnd)
                    (rebootStep.XRangeStart, rebootStep.XRangeEnd) = (rebootStep.XRangeEnd, rebootStep.XRangeStart);

                if (rebootStep.YRangeStart > rebootStep.YRangeEnd)
                    (rebootStep.YRangeStart, rebootStep.YRangeEnd) = (rebootStep.YRangeEnd, rebootStep.YRangeStart);

                if (rebootStep.ZRangeStart > rebootStep.ZRangeEnd)
                    (rebootStep.ZRangeStart, rebootStep.ZRangeEnd) = (rebootStep.ZRangeEnd, rebootStep.ZRangeStart);

                // determine if the cuboid is going to affect
                // the area of the reactor we're concerned about
                rebootStep.CuboidAffectsMainArea =
                    (rebootStep.XRangeStart < AxisRange && rebootStep.XRangeEnd >= 0) &&
                    (rebootStep.YRangeStart < AxisRange && rebootStep.YRangeEnd >= 0) &&
                    (rebootStep.ZRangeStart < AxisRange && rebootStep.ZRangeEnd >= 0);

                rebootSteps.Add(rebootStep);
            }

            return rebootSteps;
        }

        static void PartB(List<string> rebootStepsRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var rebootSteps = DecodeRawRebootStepsPartB(rebootStepsRaw);
            CalculateRanges(rebootSteps);
            UpdateRebootStepsBasedOnRanges(rebootSteps);

            var reactorCuboids = new List<Cuboid>();

            foreach (var cuboid in rebootSteps)
            {
                // If any existing cuboids overlap the cuboid for
                // the current reboot step, add a negating cuboid.
                //
                // This has to be done for all cuboids, regardless
                // of whether they are lit or not.
                //
                // Cuboids in this list that are unlit will only be
                // from previous overlaps, and so are valid for
                // negating.
                reactorCuboids.AddRange(
                    reactorCuboids
                        .Select(rc => CuboidOverlap(cuboid, rc))
                        .Where(olc => OverlapIsValid(olc))    // only include cuboid overlaps that are validly defined
                        .ToList()
                    );

                // only add cuboids that are lit
                if (cuboid.TurnOnCuboid)
                    reactorCuboids.Add(cuboid);
            }

            var poweredOnCubes = reactorCuboids.Sum(rc => CuboidSize(rc) * (rc.TurnOnCuboid ? 1 : -1));

            Console.WriteLine($"*** Powered on cubes: {poweredOnCubes:N0}");
        }

        private static List<Cuboid> DecodeRawRebootStepsPartB(List<string> rebootStepsRaw)
        {
            var rebootSteps = new List<Cuboid>();

            foreach (var rebootStepRaw in rebootStepsRaw)
            {
                var initialParse = rebootStepRaw.Split(' ');
                var secondaryParse = initialParse[1].Replace("x=", "").Replace("y=", "").Replace("z=", "").Split(',');
                var xRange = secondaryParse[0].Split("..");
                var yRange = secondaryParse[1].Split("..");
                var zRange = secondaryParse[2].Split("..");

                var rebootStep = new Cuboid()
                {
                    TurnOnCuboid = initialParse[0] == "on",
                    XRangeStart = int.Parse(xRange[0]),
                    XRangeEnd = int.Parse(xRange[1]),
                    YRangeStart = int.Parse(yRange[0]),
                    YRangeEnd = int.Parse(yRange[1]),
                    ZRangeStart = int.Parse(zRange[0]),
                    ZRangeEnd = int.Parse(zRange[1])
                };

                // order the axis ranges to be low->high
                // this makes later checks easier
                // reassignments use tuples
                if (rebootStep.XRangeStart > rebootStep.XRangeEnd)
                    (rebootStep.XRangeStart, rebootStep.XRangeEnd) = (rebootStep.XRangeEnd, rebootStep.XRangeStart);

                if (rebootStep.YRangeStart > rebootStep.YRangeEnd)
                    (rebootStep.YRangeStart, rebootStep.YRangeEnd) = (rebootStep.YRangeEnd, rebootStep.YRangeStart);

                if (rebootStep.ZRangeStart > rebootStep.ZRangeEnd)
                    (rebootStep.ZRangeStart, rebootStep.ZRangeEnd) = (rebootStep.ZRangeEnd, rebootStep.ZRangeStart);

                rebootSteps.Add(rebootStep);
            }

            return rebootSteps;
        }

        private static void CalculateRanges(List<Cuboid> rebootSteps)
        {
            XRangeMin = rebootSteps.Min(s => s.XRangeStart);
            XRangeMax = rebootSteps.Max(s => s.XRangeEnd);
            YRangeMin = rebootSteps.Min(s => s.YRangeStart);
            YRangeMax = rebootSteps.Max(s => s.YRangeEnd);
            ZRangeMin = rebootSteps.Min(s => s.ZRangeStart);
            ZRangeMax = rebootSteps.Max(s => s.ZRangeEnd);

            XRange = XRangeMax - XRangeMin + 1;
            YRange = YRangeMax - YRangeMin + 1;
            ZRange = ZRangeMax - ZRangeMin + 1;

            XRangeOffset = -XRangeMin;
            YRangeOffset = -YRangeMin;
            ZRangeOffset = -ZRangeMin;

            Console.WriteLine($"* X Range: min = {XRangeMin:N0}; max = {XRangeMax:N0}; range = {XRange:N0}; offset = {XRangeOffset:N0}");
            Console.WriteLine($"* Y Range: min = {YRangeMin:N0}; max = {YRangeMax:N0}; range = {YRange:N0}; offset = {YRangeOffset:N0}");
            Console.WriteLine($"* Z Range: min = {ZRangeMin:N0}; max = {ZRangeMax:N0}; range = {ZRange:N0}; offset = {ZRangeOffset:N0}");
        }

        private static void UpdateRebootStepsBasedOnRanges(List<Cuboid> rebootSteps)
        {
            // these updates ensure ranges are within 0 - <n>Range, where <n> is X, Y, or Z
            for (int s = 0; s < rebootSteps.Count; s++)
            {
                rebootSteps[s].XRangeStart += XRangeOffset;
                rebootSteps[s].XRangeEnd += XRangeOffset;
                rebootSteps[s].YRangeStart += YRangeOffset;
                rebootSteps[s].YRangeEnd += YRangeOffset;
                rebootSteps[s].ZRangeStart += ZRangeOffset;
                rebootSteps[s].ZRangeEnd += ZRangeOffset;
            }
        }

        private static Cuboid CuboidOverlap(Cuboid firstCuboid, Cuboid secondCuboid)
        {
            return new Cuboid()
            {
                TurnOnCuboid = !secondCuboid.TurnOnCuboid,
                XRangeStart = Math.Max(firstCuboid.XRangeStart, secondCuboid.XRangeStart),
                XRangeEnd = Math.Min(firstCuboid.XRangeEnd, secondCuboid.XRangeEnd),
                YRangeStart = Math.Max(firstCuboid.YRangeStart, secondCuboid.YRangeStart),
                YRangeEnd = Math.Min(firstCuboid.YRangeEnd, secondCuboid.YRangeEnd),
                ZRangeStart = Math.Max(firstCuboid.ZRangeStart, secondCuboid.ZRangeStart),
                ZRangeEnd = Math.Min(firstCuboid.ZRangeEnd, secondCuboid.ZRangeEnd)
            };
        }

        private static long CuboidSize(Cuboid cuboid)
        {
            return (cuboid.XRangeEnd - cuboid.XRangeStart + 1L) *
                (cuboid.YRangeEnd - cuboid.YRangeStart + 1L) *
                (cuboid.ZRangeEnd - cuboid.ZRangeStart + 1L);
        }

        private static bool OverlapIsValid(Cuboid cuboid)
        {
            return cuboid.XRangeStart <= cuboid.XRangeEnd &&
                cuboid.YRangeStart <= cuboid.YRangeEnd &&
                cuboid.ZRangeStart <= cuboid.ZRangeEnd;
        }
    }
}
