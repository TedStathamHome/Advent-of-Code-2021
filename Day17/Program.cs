using System;
using System.Collections.Generic;
using System.Linq;

namespace Day17
{
    // Puzzle Description: https://adventofcode.com/2021/day/17

    class Program
    {
        private static short xAxisTargetRangeStart;
        private static short xAxisTargetRangeEnd;
        private static short yAxisTargetRangeStart;
        private static short yAxisTargetRangeEnd;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 17");

            //var targetAreaRaw = "target area: x=20..30, y=-10..-5";
            var targetAreaRaw = "target area: x=230..283, y=-107..-57";

            PartsAandB(targetAreaRaw);
        }

        static void PartsAandB(string targetAreaRaw)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Parts A and B");

            var targetRangesRaw = targetAreaRaw.Replace("target area: ", "").Replace(" ", "").Split(",");
            var xAxisTargetRange = targetRangesRaw[0].Replace("x=", "").Split("..").Select(v => short.Parse(v)).ToArray();
            var yAxisTargetRange = targetRangesRaw[1].Replace("y=", "").Split("..").Select(v => short.Parse(v)).ToArray();

            xAxisTargetRangeStart = xAxisTargetRange.Min();
            xAxisTargetRangeEnd = xAxisTargetRange.Max();
            yAxisTargetRangeStart = yAxisTargetRange.Max();
            yAxisTargetRangeEnd = yAxisTargetRange.Min();

            Console.WriteLine($"** Raw target range: {targetAreaRaw}");
            Console.WriteLine($"** Target range: {xAxisTargetRangeStart:N0}:{yAxisTargetRangeStart:N0} to {xAxisTargetRangeEnd:N0}:{yAxisTargetRangeEnd:N0}");

            short minimumStartingXVelocity = 0;
            short totalXVelocityDelta = 0;

            // X-axis velocity can only change so much, cumulatively, and
            // ranges from what can reach the left side of the target area to
            // what can reach the right side of the target area.
            while (totalXVelocityDelta < xAxisTargetRangeStart)
            {
                minimumStartingXVelocity++;
                totalXVelocityDelta += minimumStartingXVelocity;
            }

            minimumStartingXVelocity--;

            short maximumStartingXVelocity = 0;
            totalXVelocityDelta = 0;

            while (totalXVelocityDelta < xAxisTargetRangeEnd)
            {
                maximumStartingXVelocity++;
                totalXVelocityDelta += maximumStartingXVelocity;
            }

            maximumStartingXVelocity++;

            Console.WriteLine($"** Minimum/maximum starting X velocity: {minimumStartingXVelocity:N0}/{maximumStartingXVelocity:N0}");

            // array is structured as startingXVelocity, startingYVelocity, maximumHeightReached
            // entries are only added if a step lands in the target zone
            var velocityAndHeight = new List<short[]>();
            short yAxisRangeSize = (short)((Math.Abs(yAxisTargetRangeEnd) - Math.Abs(yAxisTargetRangeStart) + 1) * 4);

            // for (short x = minimumStartingXVelocity; x < maximumStartingXVelocity; x++)
            for (short x = minimumStartingXVelocity; x <= xAxisTargetRangeEnd; x++)
            {
                for (short y = yAxisTargetRangeEnd; y < yAxisRangeSize; y++)
                {
                    var maximumHeightReached = y;
                    //Console.WriteLine($"\r\n** Starting velocity is [{x}, {y}]");

                    if (ProbeLandedInTarget(x, y, ref maximumHeightReached))
                    {
                        velocityAndHeight.Add(new short[] { x, y, maximumHeightReached });
                    }
                }
            }

            var maxHeightReached = velocityAndHeight.Max(e => e[2]);
            var vahEntry = velocityAndHeight.Where(e => e[2] == maxHeightReached).First();
            
            Console.WriteLine($"\r\n*** Found {velocityAndHeight.Count:N0} possible starting velocities.");
            Console.WriteLine($"*** Maximum height of {maxHeightReached:N0} with starting velocity of [{vahEntry[0]}, {vahEntry[1]}]");
        }

        static bool ProbeLandedInTarget(short startingXVelocity, short startingYVelocity, ref short maximumHeightReached)
        {
            bool probeLandedInTarget = false;
            var currentXVelocity = startingXVelocity;
            var currentYVelocity = startingYVelocity;
            short currentPositionX = 0;
            short currentPositionY = 0;

            while (currentPositionX < xAxisTargetRangeEnd && currentPositionY > yAxisTargetRangeEnd)
            {
                currentPositionX += currentXVelocity;
                currentPositionY += currentYVelocity;

                currentXVelocity -= (short)((currentXVelocity > 0) ? 1 : (currentXVelocity < 0 ? -1 : 0));
                currentYVelocity -= 1;

                if (currentPositionY > maximumHeightReached)
                    maximumHeightReached = currentPositionY;

                // if the current position is in the target area
                if ((currentPositionX >= xAxisTargetRangeStart && currentPositionX <= xAxisTargetRangeEnd) &&
                    (currentPositionY >= yAxisTargetRangeEnd && currentPositionY <= yAxisTargetRangeStart))
                {
                    //Console.WriteLine($"** Hit target zone at [{currentPositionX}, {currentPositionY}]");
                    //Console.WriteLine($"** Maximum height reached: {maximumHeightReached}");
                    probeLandedInTarget = true;
                    break;
                }
            }

            //if (!probeLandedInTarget)
            //    Console.WriteLine($"** Missed target zone!");

            return probeLandedInTarget;
        }
    }
}
