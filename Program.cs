using System;
using System.Collections.Generic;
using System.Linq;

public class SlotMachine
{
    // Set of possible outcomes for each reel
    static readonly string[][] ReelSet = {
        new string[] { "sym2", "sym7", "sym7", "sym1", "sym1", "sym5", "sym1", "sym4", "sym5", "sym3", "sym2", "sym3", "sym8", "sym4", "sym5", "sym2", "sym8", "sym5", "sym7", "sym2" },
        new string[] { "sym1", "sym6", "sym7", "sym6", "sym5", "sym5", "sym8", "sym5", "sym5", "sym4", "sym7", "sym2", "sym5", "sym7", "sym1", "sym5", "sym6", "sym8", "sym7", "sym6", "sym3", "sym3", "sym6", "sym7", "sym3" },
        new string[] { "sym5", "sym2", "sym7", "sym8", "sym3", "sym2", "sym6", "sym2", "sym2", "sym5", "sym3", "sym5", "sym1", "sym6", "sym3", "sym2", "sym4", "sym1", "sym6", "sym8", "sym6", "sym3", "sym4", "sym4", "sym8", "sym1", "sym7", "sym6", "sym1", "sym6" },
        new string[] { "sym2", "sym6", "sym3", "sym6", "sym8", "sym8", "sym3", "sym6", "sym8", "sym1", "sym5", "sym1", "sym6", "sym3", "sym6", "sym7", "sym2", "sym5", "sym3", "sym6", "sym8", "sym4", "sym1", "sym5", "sym7" },
        new string[] { "sym7", "sym8", "sym2", "sym3", "sym4", "sym1", "sym3", "sym2", "sym2", "sym4", "sym4", "sym2", "sym6", "sym4", "sym1", "sym6", "sym1", "sym6", "sym4", "sym8" }
    };

    // Paytable defining payouts for 3, 4, and 5 of a kind for each symbol (Index 0 = 3 of a kind, Index 1 = 4 of a kind, Index 2 = 5 of a kind)
    static readonly Dictionary<string, int[]> PayTable = new Dictionary<string, int[]> {
        { "sym1", new int[] { 1, 2, 3 } },
        { "sym2", new int[] { 1, 2, 3 } },
        { "sym3", new int[] { 1, 2, 5 } },
        { "sym4", new int[] { 2, 5, 10 } },
        { "sym5", new int[] { 5, 10, 15 } },
        { "sym6", new int[] { 5, 10, 15 } },
        { "sym7", new int[] { 5, 10, 20 } },
        { "sym8", new int[] { 10, 20, 50 } }
    };

    public static void Main(string[] args)
    {
        Random random = new Random();
        
        // Program loop
        while (true)
        {
            Console.WriteLine("Press Enter to spin or ESC to quit.");
            var key = Console.ReadKey(true);

            // Spin action
            if (key.Key == ConsoleKey.Enter)
            {
                int[] stopPositions = new int[5];
                string[,] screen = new string[3, 5];

                // Generate stop positions and populate the screen with symbols
                for (int reel = 0; reel < 5; reel++)
                {
                    stopPositions[reel] = random.Next(ReelSet[reel].Length);
                    for (int row = 0; row < 3; row++)
                    {
                        screen[row, reel] = ReelSet[reel][(stopPositions[reel] + row) % ReelSet[reel].Length];
                    }
                }

                // Display stop positions and screen symbols
                Console.WriteLine($"Stop Positions: {string.Join(", ", stopPositions)}");
                Console.WriteLine("Screen:");
                for (int row = 0; row < 3; row++)
                {
                    for (int reel = 0; reel < 5; reel++)
                    {
                        Console.Write($"{screen[row, reel]} ");
                    }
                    Console.WriteLine();
                }

                // Calculate and display winnings
                var (totalWinnings, winningDetails) = CalculateWinnings(screen);
                Console.WriteLine($"Total wins: {totalWinnings}");
                foreach (var detail in winningDetails)
                {
                    Console.WriteLine(detail);
                }
            }
            else if (key.Key == ConsoleKey.Escape) // Exit action
            {
                break;
            }
        }
    }

    static (int, List<string>) CalculateWinnings(string[,] screen)
    {
        // Dictionary to hold winning combinations
        Dictionary<string, List<List<int>>> winningCombinations = new Dictionary<string, List<List<int>>>();
        // Total value of winnings
        int totalWinnings = 0;
        // Text to screen for winnings
        List<string> winningDetails = new List<string>();

        // Find winning combinations
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                string symbol = screen[row, col];
                if (!winningCombinations.ContainsKey(symbol))
                {
                    winningCombinations[symbol] = new List<List<int>>();
                }

                List<int> positions = new List<int> { row * 5 + col };
                for (int nextCol = col + 1; nextCol < 5; nextCol++)
                {
                    if (screen[row, nextCol] == symbol)
                    {
                        positions.Add(row * 5 + nextCol);
                    }
                    else
                    {
                        break;
                    }
                }

                // Check if there are at least 3 matching symbols in a row
                if (positions.Count > 2)
                {
                    winningCombinations[symbol].Add(positions);
                    col += positions.Count - 1; // skip processed symbols
                }
            }
        }

        // Calculate total winnings and prepare text to screen of the winnings
        foreach (var combination in winningCombinations)
        {
            string symbol = combination.Key;
            foreach (var positions in combination.Value)
            {
                int count = positions.Count;
                int payout = count >= 5 ? PayTable[symbol][2] : (count == 4 ? PayTable[symbol][1] : PayTable[symbol][0]);
                totalWinnings += payout;
                winningDetails.Add($"- Ways win {string.Join("-", positions)}, {symbol} x{count}, {payout}");
            }
        }

        return (totalWinnings, winningDetails);
    }
}