using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day20
{
    // Puzzle Description: https://adventofcode.com/2021/day/20

    class Program
    {
        private static bool VoidIsDark = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 20");

            //var imageEnhancementRaw = File.ReadLines(@".\ImageEnhancement-test.txt").ToList();
            var imageEnhancementRaw = File.ReadLines(@".\ImageEnhancement-full.txt").ToList();

            // grab the enhancement algorithm (first line), and then
            // remove the algorithm and the blank line which follows it
            var enhancementAlgorithm = imageEnhancementRaw[0];
            imageEnhancementRaw.RemoveRange(0, 2);

            PartA(imageEnhancementRaw, enhancementAlgorithm);
            PartB(imageEnhancementRaw, enhancementAlgorithm);
        }

        static void PartA(List<string> imageEnhancementRaw, string enhancementAlgorithm)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            PerformEnhancement(imageEnhancementRaw, enhancementAlgorithm, 2);
        }

        static void PartB(List<string> imageEnhancementRaw, string enhancementAlgorithm)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            PerformEnhancement(imageEnhancementRaw, enhancementAlgorithm, 50);
        }

        static void PerformEnhancement(List<string> imageEnhancementRaw, string enhancementAlgorithm, int enhancementPasses)
        {
            VoidIsDark = true;
            var enhancementGrid = imageEnhancementRaw.Select(r => r).ToList();

            Console.WriteLine($"* Starting grid is {enhancementGrid[0].Length}x{enhancementGrid.Count} pixels");

            for (int i = 0; i < enhancementPasses; i++)
            {
                enhancementGrid = PrepareEnhancementGrid(enhancementGrid, enhancementAlgorithm);
                enhancementGrid = EnhanceImage(enhancementGrid, enhancementAlgorithm);
            }

            Console.WriteLine($"*** Final grid is {enhancementGrid[0].Length}x{enhancementGrid.Count} pixels");
            var litPixels = enhancementGrid.Sum(g => g.Replace(".", "").Length);
            Console.WriteLine($"\r\n*** # of lit pixels in grid: {litPixels:N0}");
        }

        private static List<string> PrepareEnhancementGrid(List<string> originalImage, string enhancementAlgorithm)
        {
            var startingImageCols = originalImage[0].Length;
            var enhancementGridCols = startingImageCols + 4;
            var emptyGridLine = new string((VoidIsDark ? '.' : '#'), enhancementGridCols);
            var leftRightPadding = new string((VoidIsDark ? '.' : '#'), 2);
            var enhancementGrid = new List<string>();

            // The enhancement grid is built from the original image,
            // with a margin all around of 2 pixels of whatever the void
            // color is. The void color is determined by looking at the
            // first entry in the image enhancement algorithm, as it
            // represents what a sub-grid of all "unlit" pixels is.
            //
            // This means that the grid starts with two "empty" lines
            // where the pixels are all whatever the void color is,
            // followed by all the lines of the original image which have
            // two pixels of the void color added at the start and end of
            // the lines, followed by two more "empty" lines. This
            // structure allows each pixel in the original image to be
            // referenced in a full 3x3 grid of pixels.

            // For example, an original image of, with a void color of ".":
            //   ####
            //   #..#
            //   #..#
            //   ####
            //
            // results in an enhancement grid of:
            //   ........
            //   ........
            //   ..####..
            //   ..#..#..
            //   ..#..#..
            //   ..####..
            //   ........
            //   ........
            //
            // Then, using a zero-based index on the lines, the grid is
            // processed starting with the pixel at 1,1 (from the top-left), 
            // which pulls a sub-grid of:
            //   ...
            //   ...
            //   ..#
            //
            // That sub-grid can then be combined into a 9-bit value of
            // ........#, which translates to 000000001, which is 1 in
            // decimal. This points at the index 1 in the image
            // enhancement algorithm.

            // start with 2 empty rows
            enhancementGrid.Add(emptyGridLine);
            enhancementGrid.Add(emptyGridLine);

            // add padding on the left/right of all the lines of the original image
            enhancementGrid.AddRange(originalImage.Select(r => $"{leftRightPadding}{r}{leftRightPadding}"));

            // end with 2 empty rows
            enhancementGrid.Add(emptyGridLine);
            enhancementGrid.Add(emptyGridLine);

            return enhancementGrid;
        }

        private static List<string> EnhanceImage(List<string> enhancementGrid, string enhancementAlgorithm)
        {
            // An incoming enhancement grid will look something like this,
            // where the #s indicate the area of the original image to be
            // enhanced (though the real image does not have a solid
            // border like this):
            //   ........
            //   ........
            //   ..####..
            //   ..#..#..
            //   ..#..#..
            //   ..####..
            //   ........
            //   ........
            //
            // Processing starts from the top-left, at a zero-based index
            // of 1,1, meaning the first pixel processed is centered there.
            // The sub-grid of 3x3 pixels centered at 1,1 looks like:
            //   ...
            //   ...
            //   ..#
            //
            // This sub-grid can then be combined into a 9-bit value of
            // ........#, which translates to 000000001, which is 1 in
            // decimal. This points at the index 1 in the image
            // enhancement algorithm.

            //Console.WriteLine("\r\n* Enhancement grid:");
            //foreach (var row in enhancementGrid)
            //{
            //    Console.WriteLine($"** {row}");
            //}

            var enhancedImage = new List<string>();
            var enhancedImageCols = enhancementGrid[0].Length - 2;
            var enhancedImageRows = enhancementGrid.Count - 2;

            for (int row = 0; row < enhancedImageRows; row++)
            {
                var enhancedRowPixels = new StringBuilder();

                for (int col = 0; col < enhancedImageCols; col++)
                {
                    var pixelData = enhancementGrid[row].Substring(col, 3) + enhancementGrid[row + 1].Substring(col, 3) + enhancementGrid[row + 2].Substring(col, 3);
                    var algorithmIndex = Convert.ToInt32(pixelData.Replace('.', '0').Replace('#', '1'), 2);
                    enhancedRowPixels.Append(enhancementAlgorithm[algorithmIndex]);
                }

                enhancedImage.Add(enhancedRowPixels.ToString());
            }

            if ((VoidIsDark && enhancementAlgorithm[0] == '#') || (!VoidIsDark && enhancementAlgorithm[^1] == '.'))
                VoidIsDark = !VoidIsDark;

            //Console.WriteLine("\r\n* Enhanced image:");
            //foreach (var row in enhancedImage)
            //{
            //    Console.WriteLine($"** {row}");
            //}

            return enhancedImage;
        }
    }
}
