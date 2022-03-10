using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day16
{
    // Puzzle Description: https://adventofcode.com/2021/day/16

    class Program
    {
        const int ConvertFromBase2 = 2;

        static readonly Dictionary<char, string> hexToBin = new ()
            {
                { '0', "0000" },
                { '1', "0001" },
                { '2', "0010" },
                { '3', "0011" },
                { '4', "0100" },
                { '5', "0101" },
                { '6', "0110" },
                { '7', "0111" },
                { '8', "1000" },
                { '9', "1001" },
                { 'A', "1010" },
                { 'B', "1011" },
                { 'C', "1100" },
                { 'D', "1101" },
                { 'E', "1110" },
                { 'F', "1111" }
            };

        static void Main(string[] args)
        {
            Console.WriteLine("Advent of Code 2021: Day 16");

            //var bitsTransmission = File.ReadLines(@".\BITStransmission-test.txt").ToList();
            //var bitsTransmission = File.ReadLines(@".\BITStransmission-test2.txt").ToList();
            var bitsTransmission = File.ReadLines(@".\BITStransmission-full.txt").ToList();

            PartA(bitsTransmission);
            PartB(bitsTransmission);
        }

        static void PartA(List<string> bitsTransmission)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part A");

            var entryCount = 0;

            foreach (var entry in bitsTransmission)
            {
                var entryBin = string.Concat(entry.ToCharArray().Select(c => hexToBin[c]));
                Console.WriteLine($"\r\n** Entry {entry} = {entryBin}");

                var packetIndex = 0;
                var packetVersions = new List<int>();

                DecodePacket(ref entryBin, ref packetIndex, packetVersions);

                Console.WriteLine($"** Total of packet versions: {packetVersions.Sum():N0}");

                entryCount++;
            }
        }

        static void PartB(List<string> bitsTransmission)
        {
            Console.WriteLine("\r\n**********");
            Console.WriteLine("* Part B");

            var entryCount = 0;

            foreach (var entry in bitsTransmission)
            {
                var entryBin = string.Concat(entry.ToCharArray().Select(c => hexToBin[c]));
                Console.WriteLine($"\r\n** Entry {entry} = {entryBin}");

                var packetIndex = 0;

                var result = GetPacketResult(ref entryBin, ref packetIndex);

                Console.WriteLine($"** Result of packet processing: {result:N0}");

                entryCount++;
            }
        }

        // Used for PartA
        static void DecodePacket(ref string packet, ref int packetIndex, List<int> packetVersions)
        {
            const int versionTypeBits = 3;

            var packetVersion = Convert.ToInt32(packet.Substring(packetIndex, versionTypeBits), ConvertFromBase2);
            packetIndex += versionTypeBits;

            var packetType = Convert.ToInt32(packet.Substring(packetIndex, versionTypeBits), ConvertFromBase2);
            packetIndex += versionTypeBits;

            packetVersions.Add(packetVersion);

            switch (packetType)
            {
                case 4:     // literal value
                    Console.WriteLine($"** Literal value packet encountered; version {packetVersion:N0}");
                    MoveIndexToEndOfLiteral(ref packet, ref packetIndex);
                    Console.WriteLine($"*** End index of literal: {packetIndex:N0}");
                    break;
                default:    // operator; flesh this out as more are defined
                    Console.WriteLine($"** Operator packet encountered; version {packetVersion:N0}");
                    ParseOperator(ref packet, ref packetIndex, packetVersions);
                    break;
            }
        }

        static void MoveIndexToEndOfLiteral(ref string packet, ref int packetIndex)
        {
            const int literalPieceSize = 5;
            var currentIndex = 0;
            var literalValue = "";

            while (packet[packetIndex + currentIndex] == '1')
            {
                literalValue += packet.Substring(packetIndex + currentIndex + 1, literalPieceSize - 1);
                currentIndex += literalPieceSize;
            }

            literalValue += packet.Substring(packetIndex + currentIndex + 1, literalPieceSize - 1);
            packetIndex += currentIndex + literalPieceSize;
            Console.WriteLine($"*** Literal is: {literalValue} => {Convert.ToInt64(literalValue, ConvertFromBase2)}");
        }

        static void ParseOperator(ref string packet, ref int packetIndex, List<int> packetVersions)
        {
            const int fixedSubpacketAreaSize = 15;
            const int variableSubpacketAreaSize = 11;

            var lengthType = packet[packetIndex];
            packetIndex++;

            if (lengthType == '0')      // next 15 bits converted to decimal indicates number of bits for the sub-packet area
            {
                long subPacketBitSize = Convert.ToInt64(packet.Substring(packetIndex, fixedSubpacketAreaSize), ConvertFromBase2);
                Console.WriteLine($"*** Operator contains {subPacketBitSize:N0} bits of sub-packets");

                packetIndex += fixedSubpacketAreaSize;
                var startingPacketIndex = packetIndex;

                while (packetIndex < startingPacketIndex + subPacketBitSize)
                {
                    DecodePacket(ref packet, ref packetIndex, packetVersions);
                }
            }
            else    // must be a 1; next 11 bits converted to decimal indicates number of sub-packets
            {
                long subPacketCount = Convert.ToInt64(packet.Substring(packetIndex, variableSubpacketAreaSize), ConvertFromBase2);
                Console.WriteLine($"*** Operator contains {subPacketCount:N0} sub-packets");

                packetIndex += variableSubpacketAreaSize;

                for (var p = 0; p < subPacketCount; p++)
                {
                    DecodePacket(ref packet, ref packetIndex, packetVersions);
                }
            }
        }

        // Used for PartB
        static long GetPacketResult(ref string packet, ref int packetIndex)
        {
            const int versionOrTypeBits = 3;

            // don't care about the version, so skip over it
            packetIndex += versionOrTypeBits;
            var packetType = Convert.ToInt32(packet.Substring(packetIndex, versionOrTypeBits), ConvertFromBase2);
            packetIndex += versionOrTypeBits;

            long packetResult;

            switch (packetType)
            {
                case 4:     // literal value
                    Console.WriteLine($"** Literal value packet encountered");
                    Console.WriteLine($"*** End index of literal: {packetIndex:N0}");
                    packetResult = ParseLiteralValue(ref packet, ref packetIndex);
                    break;
                // operators
                case 0:     // sum
                    packetResult = ParseSumOperator(ref packet, ref packetIndex);
                    break;
                case 1:     // product
                    packetResult = ParseProductOperator(ref packet, ref packetIndex);
                    break;
                case 2:     // minimum
                    packetResult = ParseMinimumOperator(ref packet, ref packetIndex);
                    break;
                case 3:     // maximum
                    packetResult = ParseMaximumOperator(ref packet, ref packetIndex);
                    break;
                case 5:     // greater than
                    packetResult = ParseGreaterThanOperator(ref packet, ref packetIndex);
                    break;
                case 6:     // less than
                    packetResult = ParseLessThanOperator(ref packet, ref packetIndex);
                    break;
                case 7:     // equal to
                    packetResult = ParseEqualToOperator(ref packet, ref packetIndex);
                    break;
                default:    // unknown operator
                    Console.WriteLine($"** Unknown operator packet type of {packetType} encountered; returning -1!");
                    packetResult = -1;
                    break;
            }

            return packetResult;
        }

        static long ParseLiteralValue(ref string packet, ref int packetIndex)
        {
            const int literalPieceSize = 5;
            var currentIndex = 0;
            var literalValueBits = "";

            while (packet[packetIndex + currentIndex] == '1')
            {
                literalValueBits += packet.Substring(packetIndex + currentIndex + 1, literalPieceSize - 1);
                currentIndex += literalPieceSize;
            }

            literalValueBits += packet.Substring(packetIndex + currentIndex + 1, literalPieceSize - 1);
            var literalValue = Convert.ToInt64(literalValueBits, ConvertFromBase2);
            packetIndex += currentIndex + literalPieceSize;
            Console.WriteLine($"*** Literal is: {literalValueBits} => {literalValue:N0}");
            return literalValue;
        }

        static long ParseSumOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);

            return operatorValues.Sum();
        }

        static long ParseProductOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);
            var product = operatorValues.First();

            if (operatorValues.Count > 1)
            {
                for (int v = 1; v < operatorValues.Count; v++)
                {
                    product *= operatorValues[v];
                }
            }

            return product;
        }

        static long ParseMinimumOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);

            return operatorValues.Min();
        }

        static long ParseMaximumOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);

            return operatorValues.Max();
        }

        static long ParseGreaterThanOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);

            if (operatorValues.Count != 2)
            {
                Console.WriteLine($"*** Greater than operator has {operatorValues.Count:N0} operands; should only have 2!");
                return 0;
            }

            var leftOperand = operatorValues[0];
            var rightOperand = operatorValues[1];

            return leftOperand > rightOperand ? 1 : 0;
        }

        static long ParseLessThanOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);

            if (operatorValues.Count != 2)
            {
                Console.WriteLine($"*** Less than operator has {operatorValues.Count:N0} operands; should only have 2!");
                return 0;
            }

            var leftOperand = operatorValues[0];
            var rightOperand = operatorValues[1];

            return leftOperand < rightOperand ? 1 : 0;
        }

        static long ParseEqualToOperator(ref string packet, ref int packetIndex)
        {
            var operatorValues = GetOperatorValues(ref packet, ref packetIndex);

            if (operatorValues.Count != 2)
            {
                Console.WriteLine($"*** Equal to operator has {operatorValues.Count:N0} operands; should only have 2!");
                return 0;
            }

            var leftOperand = operatorValues[0];
            var rightOperand = operatorValues[1];

            return leftOperand == rightOperand ? 1 : 0;
        }

        static List<long> GetOperatorValues(ref string packet, ref int packetIndex)
        {

            const int fixedSubpacketAreaSize = 15;
            const int variableSubpacketAreaSize = 11;

            var subPacketValues = new List<long>();
            var packetLengthType = packet[packetIndex];

            packetIndex++;

            if (packetLengthType == '0')      // next 15 bits converted to decimal indicates number of bits for the sub-packet area
            {
                var subPacketBitSize = Convert.ToInt64(packet.Substring(packetIndex, fixedSubpacketAreaSize), ConvertFromBase2);
                Console.WriteLine($"*** Operator contains {subPacketBitSize:N0} bits of sub-packets");

                packetIndex += fixedSubpacketAreaSize;
                var startingPacketIndex = packetIndex;

                while (packetIndex < startingPacketIndex + subPacketBitSize)
                {
                    subPacketValues.Add(GetPacketResult(ref packet, ref packetIndex));
                }
            }
            else    // must be a 1; next 11 bits converted to decimal indicates number of sub-packets
            {
                var subPacketCount = Convert.ToInt64(packet.Substring(packetIndex, variableSubpacketAreaSize), ConvertFromBase2);
                Console.WriteLine($"*** Operator contains {subPacketCount:N0} sub-packets");

                packetIndex += variableSubpacketAreaSize;

                for (var p = 0; p < subPacketCount; p++)
                {
                    subPacketValues.Add(GetPacketResult(ref packet, ref packetIndex));
                }
            }

            return subPacketValues;
        }
    }
}
