using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day13
{
    // Puzzle Description: https://adventofcode.com/2021/day/13

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 13");

            //var dotCoordinatesRaw = File.ReadLines(@".\DotCoordinates-test.txt").ToList();
            //var foldInstructions = File.ReadLines(@".\FoldInstructions-test.txt").ToList().Select(i => i.Replace("fold along ", "")).ToList();

            var dotCoordinatesRaw = File.ReadLines(@".\DotCoordinates-full.txt").ToList();
            var foldInstructions = File.ReadLines(@".\FoldInstructions-full.txt").ToList().Select(i => i.Replace("fold along ", "")).ToList();

            PartA(dotCoordinatesRaw, foldInstructions);
            PartB(dotCoordinatesRaw, foldInstructions);
        }

        static void PartA(List<string> dotCoordinatesRaw, List<string> foldInstructions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            FoldUpThePaper(dotCoordinatesRaw, foldInstructions, 'A');
        }

        static void PartB(List<string> dotCoordinatesRaw, List<string> foldInstructions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            FoldUpThePaper(dotCoordinatesRaw, foldInstructions, 'B');
        }

        static void FoldUpThePaper(List<string> dotCoordinatesRaw, List<string> foldInstructions, char puzzlePart)
        {
            var performAllFolds = (puzzlePart == 'B');
            var dotCoordinates = new List<int[]>();

            foreach (var dotCoordinate in dotCoordinatesRaw)
                dotCoordinates.Add(dotCoordinate.Split(',').Select(c => int.Parse(c)).ToArray());

            var dotCols = dotCoordinates.Max(c => c[0]) + 1;
            dotCols += (dotCols % 2) == 1 ? 0 : 1;

            var dotRows = dotCoordinates.Max(c => c[1]) + 1;
            dotRows += (dotRows % 2) == 1 ? 0 : 1;

            Console.WriteLine($"* Paper is {dotCols:N0} cols x {dotRows:N0} rows");

            var paperDots = new bool[dotCols, dotRows];

            foreach (var dotCoordinate in dotCoordinates)
                paperDots[dotCoordinate[0], dotCoordinate[1]] = true;

            foreach (var foldInstruction in foldInstructions)
            {
                Console.WriteLine($"* Folding on {foldInstruction}");

                var foldInstructionParts = foldInstruction.Split('=').ToArray();

                switch (foldInstructionParts[0])
                {
                    case "x":
                        paperDots = FoldPaperOnCol(paperDots, int.Parse(foldInstructionParts[1]), dotRows, dotCols);
                        dotCols = paperDots.GetLength(0);
                        break;

                    case "y":
                        paperDots = FoldPaperOnRow(paperDots, int.Parse(foldInstructionParts[1]), dotRows, dotCols);
                        dotRows = paperDots.GetLength(1);
                        break;

                    default:
                        break;
                }

                Console.WriteLine($"** Paper is now {dotCols:N0} cols x {dotRows:N0} rows");

                // if we just need to perform the first fold (for puzzle part A), break out
                if (!performAllFolds)
                    break;
            }

            // if we're performing all the folds, print out the final set of visible dots
            if (performAllFolds)
                Console.WriteLine($"** Final visible dots (paper is now {dotCols:N0} cols x {dotRows:N0} rows):");

            var dotCount = 0;

            for (int row = 0; row < dotRows; row++)
            {
                for (int col = 0; col < dotCols; col++)
                {
                    if (performAllFolds)
                        Console.Write($"{(paperDots[col, row] ? "#" : ".")}");

                    dotCount += paperDots[col, row] ? 1 : 0;
                }

                if (performAllFolds)
                    Console.Write("\r\n");
            }

            Console.WriteLine($"*** # of visible dots: {dotCount:N0}");
        }

        static bool[,] FoldPaperOnRow(bool[,] paperDots, int foldRow, int rows, int cols)
        {
            var topRows = foldRow;

            if (topRows * 2 == rows)
                Console.WriteLine($"** Sheet does not fold with an empty row: rows = {rows:N0} / fold @ {foldRow:N0}; assuming no blank row between halves");

            var foldedPaper = new bool[cols, topRows];

            for (int row = 0; row < topRows; row++)
            {
                var bottomRow = (rows - 1) - row;

                if (bottomRow >= rows)
                    Console.WriteLine($"** Row is out of bounds: {bottomRow:N0}");

                for (int col = 0; col < cols; col++)
                    foldedPaper[col, row] = paperDots[col, row] || paperDots[col, bottomRow];
            }

            return foldedPaper;
        }

        static bool[,] FoldPaperOnCol(bool[,] paperDots, int foldCol, int rows, int cols)
        {
            var leftCols = foldCol;

            if (leftCols * 2 == cols)
                Console.WriteLine($"** Sheet does not fold with an empty col: cols = {cols:N0} / fold @ {foldCol:N0}; assuming no blank col between halves");

            var foldedPaper = new bool[leftCols, rows];

            for (int col = 0; col < leftCols; col++)
            {
                var rightCol = (cols - 1) - col;

                if (rightCol >= cols)
                    Console.WriteLine($"** Col is out of bounds: {rightCol:N0}");

                for (int row = 0; row < rows; row++)
                    foldedPaper[col, row] = paperDots[col, row] || paperDots[rightCol, row];
            }

            return foldedPaper;
        }
    }
}
