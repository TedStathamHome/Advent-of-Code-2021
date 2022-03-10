using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day21
{
    // Puzzle Description: https://adventofcode.com/2021/day/21

    class Program
    {
        // Shared values
        private const int spacesOnBoard = 10;

        // Part A values
        private const int maximumDieValue = 100;         // the die has 100 faces, so we need to wrap back around to 0 once we hit it
        private const int partAMaximumScore = 1000;
        private const int rollsPerTurn = 3;
        private static int dieValue = 1;

        // Part B values
        private const int partBMaximumScore = 21;
        private static long player1DimensionWins = 0;
        private static long player2DimensionWins = 0;

        // Each player turn generates 3 layers of die rolls, resulting in 39 new
        // dimensions, and 27 score paths, but they're always the same. This is
        // based on the following tree:
        //
        //      1           2           3        <-  3 dimensions
        //   /  |  \     /  |  \     /  |  \
        //  1   2   3   1   2   3   1   2   3    <-  9 dimensions
        // /|\ /|\ /|\ /|\ /|\ /|\ /|\ /|\ /|\
        // 123 123 123 123 123 123 123 123 123   <- 27 dimensions
        // 345 456 567 456 567 678 567 678 789   <- turn totals
        //
        // Number of times a turn (throwing the dice 3 times) produces a given
        // score, based on the above turn totals.
        private static Dictionary<int, int> throwOdds = new()
        {
            { 3, 1 },   // 3 occurs 1 time
            { 4, 3 },   // 4 occurs 3 times
            { 5, 6 },   // 5 occurs 6 times
            { 6, 7 },   // 6 occurs 7 times
            { 7, 6 },   // 7 occurs 6 times
            { 8, 3 },   // 8 occurs 3 times
            { 9, 1 }    // 9 occurs 1 time
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 21");

            //var playerStartingPositions = File.ReadLines(@".\PlayerStartingPositions-test.txt").ToList();
            var playerStartingPositions = File.ReadLines(@".\PlayerStartingPositions-full.txt").ToList();

            PartA(playerStartingPositions);
            PartB(playerStartingPositions);
        }

        static void PartA(List<string> playerStartingPositions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var player1CurrentPosition = int.Parse(playerStartingPositions[0][^1].ToString());
            var player1DiceRolls = 0;
            var player1Score = 0;

            var player2CurrentPosition = int.Parse(playerStartingPositions[1][^1].ToString());
            var player2DiceRolls = 0;
            var player2Score = 0;

            Console.WriteLine($"* Player 1 starting position: {player1CurrentPosition}");
            Console.WriteLine($"* Player 2 starting position: {player2CurrentPosition}");

            var currentPlayer = 1;

            while (true)
            {
                var rollResult = GetRollResult();

                switch (currentPlayer)
                {
                    case 1:
                        player1DiceRolls += rollsPerTurn;
                        player1CurrentPosition = DeterminePlayerPosition(player1CurrentPosition, rollResult);
                        player1Score += player1CurrentPosition;
                        currentPlayer = 2;
                        break;

                    case 2:
                        player2DiceRolls += rollsPerTurn;
                        player2CurrentPosition = DeterminePlayerPosition(player2CurrentPosition, rollResult);
                        player2Score += player2CurrentPosition;
                        currentPlayer = 1;
                        break;

                    default:
                        break;
                }

                if (player1Score >= partAMaximumScore || player2Score >= partAMaximumScore)
                    break;
            }

            Console.WriteLine($"*** Player 1: # of rolls: {player1DiceRolls:N0} / Score: {player1Score:N0}");
            Console.WriteLine($"*** Player 2: # of rolls: {player2DiceRolls:N0} / Score: {player2Score:N0}");
            var losingPlayer = player1Score >= partAMaximumScore ? 2 : 1;
            var losingScore = (player1DiceRolls + player2DiceRolls) * (losingPlayer == 1 ? player1Score : player2Score);
            Console.WriteLine($"*** Player {losingPlayer} lost, giving a final score of {losingScore:N0}");
        }
        private static int GetRollResult()
        {
            var rollResult = 0;

            for (int i = 0; i < rollsPerTurn; i++)
            {
                rollResult += dieValue;

                // if the die is at 100, wrap back to 1
                dieValue += dieValue == maximumDieValue ? -(maximumDieValue - 1) : 1;
            }

            return rollResult;
        }

        private static int DeterminePlayerPosition(int currentPlayerPosition, int rollResult)
        {
            // It doesn't matter how many times they go around the board (advancing 10 positions),
            // we just need the remainder after all of those loops.
            var newPlayerPosition = currentPlayerPosition + rollResult % spacesOnBoard;

            // If they're at a position greater than 10, then their new position is that minus 10.
            return newPlayerPosition > spacesOnBoard ? newPlayerPosition - spacesOnBoard : newPlayerPosition;
        }

        static void PartB(List<string> playerStartingPositions)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var player1StartingPosition = int.Parse(playerStartingPositions[0][^1].ToString());
            var player2StartingPosition = int.Parse(playerStartingPositions[1][^1].ToString());

            PlayTurn(player1StartingPosition, 0, player2StartingPosition, 0, 1, 1);

            Console.WriteLine($"*** Player 1 wins: {player1DimensionWins:N0}");
            Console.WriteLine($"*** Player 2 wins: {player2DimensionWins:N0}");
            Console.WriteLine($"*** Winner: Player {(player1DimensionWins > player2DimensionWins ? 1 : 2)}");
        }

        static void PlayTurn(int player1CurrentPosition, int player1CurrentScore, int player2CurrentPosition, int player2CurrentScore, long rollOccurrences, int currentPlayer)
        {
            foreach (var turnScore in throwOdds.Keys)
            {
                if (currentPlayer == 1)
                {
                    var player1NewPosition = player1CurrentPosition + turnScore;

                    if (player1NewPosition > spacesOnBoard)
                        player1NewPosition -= spacesOnBoard;

                    var player1NewScore = player1CurrentScore + player1NewPosition;
                    var newRollOccurrences = rollOccurrences * throwOdds[turnScore];

                    if (player1NewScore >= partBMaximumScore)
                    {
                        player1DimensionWins += newRollOccurrences;
                    }
                    else
                    {
                        PlayTurn(player1NewPosition, player1NewScore, player2CurrentPosition, player2CurrentScore, newRollOccurrences, 2);
                    }
                }
                else
                {
                    var player2NewPosition = player2CurrentPosition + turnScore;

                    if (player2NewPosition > spacesOnBoard)
                        player2NewPosition -= spacesOnBoard;

                    var player2NewScore = player2CurrentScore + player2NewPosition;
                    var newRollOccurrences = rollOccurrences * throwOdds[turnScore];

                    if (player2NewScore >= partBMaximumScore)
                    {
                        player2DimensionWins += newRollOccurrences;
                    }
                    else
                    {
                        PlayTurn(player1CurrentPosition, player1CurrentScore, player2NewPosition, player2NewScore, newRollOccurrences, 1);
                    }
                }
            }
        }
    }
}
