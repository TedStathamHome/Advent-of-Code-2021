using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day04
{
    // Puzzle Description: https://adventofcode.com/2021/day/4

    class Program
    {
        const int boardSize = 5;

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 4");

            //var bingoNumbersDrawnRaw = File.ReadLines(@".\BingoNumbersDrawn-test.txt").ToList().First();
            var bingoNumbersDrawnRaw = File.ReadLines(@".\BingoNumbersDrawn-full.txt").ToList().First();
            //var bingoBoardsRaw = File.ReadLines(@".\BingoBoards-test.txt").ToArray();
            var bingoBoardsRaw = File.ReadLines(@".\BingoBoards-full.txt").ToArray();

            var bingoNumbersDrawn = bingoNumbersDrawnRaw.Split(',');
            Console.WriteLine($"* Numbers in draw list: {bingoNumbersDrawn.Length:N0}");

            var bingoBoardsInPlay = bingoBoardsRaw.Length / (boardSize + 1);
            Console.WriteLine($"* Board lines read: {bingoBoardsRaw.Length:N0}");
            Console.WriteLine($"* Boards in play: {bingoBoardsInPlay:N0}");

            PartA(bingoBoardsRaw, bingoNumbersDrawn, bingoBoardsInPlay);
            PartB(bingoBoardsRaw, bingoNumbersDrawn, bingoBoardsInPlay);
        }

        static void PartA(string[] bingoBoardsRaw, string[] bingoNumbersDrawn, int bingoBoardsInPlay)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var bingoBoards = ParseBoardsIntoListOfGridArrays(bingoBoardsRaw, bingoBoardsInPlay);

            var numberDrawn = 0;
            var numberIndex = -1;
            var winningBoard = 0;

            // loop through the numbers, exiting early if a board wins
            for (int bn = 0; bn < bingoNumbersDrawn.Length; bn++)
            {
                numberIndex = bn;
                numberDrawn = int.Parse(bingoNumbersDrawn[bn]);

                // loop through the boards, setting any place the drawn number is encountered to -1
                for (int currentBoard = 0; currentBoard < bingoBoardsInPlay; currentBoard++)
                {
                    for (int row = 0; row < boardSize; row++)
                    {
                        for (int col = 0; col < boardSize; col++)
                        {
                            if (bingoBoards[currentBoard][row, col] == numberDrawn)
                            {
                                bingoBoards[currentBoard][row, col] = -1;

                                // the drawn number should only appear once on any given board, so exit early
                                goto NumberFound;
                            }
                        }
                    }

                NumberFound:
                    if (BoardIsAWinner(bingoBoards, currentBoard))
                    {
                        winningBoard = currentBoard;
                        goto GameEnd;
                    }
                }
            }

        GameEnd:
            Console.WriteLine($"*** Board {winningBoard + 1} won, with the draw of {numberDrawn}, at position {numberIndex + 1}!");

            var boardSum = CalculateBoardSum(bingoBoards, winningBoard);

            Console.WriteLine($"*** Winning board value: {boardSum:N0}");
            Console.WriteLine($"*** Winning board score: {boardSum * numberDrawn:N0}");
        }

        static void PartB(string[] bingoBoardsRaw, string[] bingoNumbersDrawn, int bingoBoardsInPlay)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var bingoBoards = ParseBoardsIntoListOfGridArrays(bingoBoardsRaw, bingoBoardsInPlay);

            var numberDrawn = 0;
            var numberIndex = -1;
            var losingBoard = 0;
            var winningBoards = new List<int>();

            // loop through the numbers, exiting early if a board wins
            for (int bn = 0; bn < bingoNumbersDrawn.Length; bn++)
            {
                numberIndex = bn;
                numberDrawn = int.Parse(bingoNumbersDrawn[bn]);

                // loop through the boards, setting any place the drawn number is encountered to -1
                for (int currentBoard = 0; currentBoard < bingoBoardsInPlay; currentBoard++)
                {
                    if (winningBoards.Contains(currentBoard))
                        continue;

                    for (int row = 0; row < boardSize; row++)
                    {
                        for (int col = 0; col < boardSize; col++)
                        {
                            if (bingoBoards[currentBoard][row, col] == numberDrawn)
                            {
                                bingoBoards[currentBoard][row, col] = -1;

                                // the drawn number should only appear once on any given board, so exit early
                                goto NumberFound;
                            }
                        }
                    }

                NumberFound:
                    if (BoardIsAWinner(bingoBoards, currentBoard))
                    {
                        winningBoards.Add(currentBoard);

                        if (winningBoards.Count == bingoBoardsInPlay)
                        {
                            losingBoard = currentBoard;
                            goto GameEnd;
                        }
                    }
                }
            }

        GameEnd:
            Console.WriteLine($"*** Board {losingBoard + 1} won last, with the draw of {numberDrawn}, at position {numberIndex + 1}!");

            var boardSum = CalculateBoardSum(bingoBoards, losingBoard);

            Console.WriteLine($"*** Losing board value: {boardSum:N0}");
            Console.WriteLine($"*** Losing board score: {boardSum * numberDrawn:N0}");
        }

        private static List<int[,]> ParseBoardsIntoListOfGridArrays(string[] bingoBoardsRaw, int bingoBoardsInPlay)
        {
            var bingoBoards = new List<int[,]>();

            for (int i = 0; i < bingoBoardsInPlay; i++)
            {
                var board = new int[boardSize, boardSize];
                for (int j = 0; j < boardSize; j++)
                {
                    var boardRow = bingoBoardsRaw[i * (boardSize + 1) + j].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int k = 0; k < boardSize; k++)
                    {
                        board[j, k] = int.Parse(boardRow[k]);
                    }
                }

                bingoBoards.Add(board);
            }

            return bingoBoards;
        }

        private static bool BoardIsAWinner(List<int[,]> bingoBoards, int currentBoard)
        {
            // check the board rows for a win
            for (int row = 0; row < boardSize; row++)
            {
                var rowSum = 0;

                for (int col = 0; col < boardSize; col++)
                {
                    rowSum += bingoBoards[currentBoard][row, col];
                }

                var rowIsAWinner = rowSum == (-1 * boardSize);
                
                if (rowIsAWinner)
                    return true;
            }

            // check the board columns for a win
            for (int col = 0; col < boardSize; col++)
            {
                var colSum = 0;

                for (int row = 0; row < boardSize; row++)
                {
                    colSum += bingoBoards[currentBoard][row, col];
                }

                var colIsAWinner = colSum == (-1 * boardSize);
                if (colIsAWinner)
                    return true;
            }

            return false;
        }

        static int CalculateBoardSum(List<int[,]> bingoBoards, int boardToSum)
        {
            var boardSum = 0;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (bingoBoards[boardToSum][row, col] != -1)
                        boardSum += bingoBoards[boardToSum][row, col];
                }
            }

            return boardSum;
        }
    }
}
