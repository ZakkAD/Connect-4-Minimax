using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Connect_4_Minimax
{
    public static class TranspositionTableHandler
    {
        public static void SaveTranspositionTable(Dictionary<string, int[]> table, string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(table, new JsonSerializerOptions
                {
                    WriteIndented = true // Makes the JSON file more human-readable
                });
                File.WriteAllText(filePath, json);
                Console.WriteLine("Transposition table saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transposition table: {ex.Message}");
            }
        }

        public static Dictionary<string, int[]> LoadTranspositionTable(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found. Returning a new empty table.");
                    return new Dictionary<string, int[]>();
                }

                string json = File.ReadAllText(filePath);
                var table = JsonSerializer.Deserialize<Dictionary<string, int[]>>(json);
                Console.WriteLine("Transposition table loaded successfully.");
                return table ?? new Dictionary<string, int[]>(); // Ensure table is not null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading transposition table: {ex.Message}");
                return new Dictionary<string, int[]>();
            }
        }
    }
    internal class Program
    {
        static byte WinCheck(byte[][] board)
        {
            for (int y = board[0].Length - 1; y > -1; y--)
            {
                for (int x = 0; x < board.Length; x++)
                {
                    if (board[x][y] == 0)
                    {
                        continue;
                    }
                    if (x + 3 < board.Length && board[x][y] == board[x + 1][y] && board[x][y] == board[x + 2][y] && board[x][y] == board[x + 3][y])
                    {
                        return board[x][y];
                    }
                    if (y + 3 < board[0].Length)
                    {
                        if (board[x][y] == board[x][y + 1] && board[x][y] == board[x][y + 2] && board[x][y] == board[x][y + 3])
                        {
                            return board[x][y];
                        }
                        if (x + 3 < board.Length && board[x][y] == board[x + 1][y + 1] && board[x][y] == board[x + 2][y + 2] && board[x][y] == board[x + 3][y + 3])
                        {
                            return board[x][y];
                        }
                        if (x - 3 > -1 && board[x][y] == board[x - 1][y + 1] && board[x][y] == board[x - 2][y + 2] && board[x][y] == board[x - 3][y + 3])
                        {
                            return board[x][y];
                        }
                    }

                }
            }
            return 0;
        }
        static void Draw(byte[][] board)
        {
            for (int y = board[0].Length - 1; y > -1; y--)
            {
                for (int x = 0; x < board.Length; x++)
                {
                    if (board[x][y] == 0)
                    {
                        Console.Write("0 ");
                        continue;
                    }
                    if (board[x][y] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("0 ");
                        Console.ResetColor();
                        continue;
                    }
                    if (board[x][y] == 2)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("0 ");
                        Console.ResetColor();
                        continue;
                    }
                }
            
                Console.WriteLine();
            }
        }
        static void DropCounter(byte[][] board, int column, byte player)
        {
            for (int y = 0; y < board[0].Length; y++)
            {
                if (board[column][y] == 0)
                {
                    board[column][y] = player;
                    break;
                }
            }
        }

        private static int EvaluateBoard(byte[][] board, byte player)
        {
            const int TWO_IN_A_ROW = 10;
            const int THREE_IN_A_ROW = 50;
            const int FOUR_IN_A_ROW = 1000000;
            const int CENTER_BONUS = 5;

            int opponent = 3 - player; // Determine the opponent
            int score = 0;

            // Central column bonus
            int centerColumn = board.Length / 2;
            for (int y = 0; y < board[centerColumn].Length; y++)
            {
                if (board[centerColumn][y] == player) score += CENTER_BONUS;
                else if (board[centerColumn][y] == opponent) score -= CENTER_BONUS;
            }

            // Helper function to count streaks
            static int EvaluateStreak(byte[] streak, byte player)
            {
                int playerCount = 0;
                int emptyCount = 0;

                foreach (var cell in streak)
                {
                    if (cell == player) playerCount++;
                    else if (cell == 0) emptyCount++;
                }

                if (playerCount == 4) return FOUR_IN_A_ROW;
                if (playerCount == 3 && emptyCount == 1) return THREE_IN_A_ROW;
                if (playerCount == 2 && emptyCount == 2) return TWO_IN_A_ROW;

                return 0;
            }

            // Horizontal check
            for (int y = 0; y < board[0].Length; y++)
            {
                for (int x = 0; x <= board.Length - 4; x++) // Only start where a streak is possible
                {
                    byte[] streak = new byte[4];
                    for (int i = 0; i < 4; i++) streak[i] = board[x + i][y];
                    score += EvaluateStreak(streak, player);
                    score -= EvaluateStreak(streak, (byte)opponent);
                }
            }

            // Vertical check
            for (int x = 0; x < board.Length; x++)
            {
                for (int y = 0; y <= board[x].Length - 4; y++) // Only start where a streak is possible
                {
                    byte[] streak = new byte[4];
                    for (int i = 0; i < 4; i++) streak[i] = board[x][y + i];
                    score += EvaluateStreak(streak, player);
                    score -= EvaluateStreak(streak, (byte)opponent);
                }
            }

            // Diagonal check (positive slope)
            for (int x = 0; x <= board.Length - 4; x++)
            {
                for (int y = 0; y <= board[0].Length - 4; y++)
                {
                    byte[] streak = new byte[4];
                    for (int i = 0; i < 4; i++) streak[i] = board[x + i][y + i];
                    score += EvaluateStreak(streak, player);
                    score -= EvaluateStreak(streak, (byte)opponent);
                }
            }

            // Diagonal check (negative slope)
            for (int x = 0; x <= board.Length - 4; x++)
            {
                for (int y = 3; y < board[0].Length; y++)
                {
                    byte[] streak = new byte[4];
                    for (int i = 0; i < 4; i++) streak[i] = board[x + i][y - i];
                    score += EvaluateStreak(streak, player);
                    score -= EvaluateStreak(streak, (byte)opponent);
                }
            }

            return score;
        }

        private static byte[][] CopyBoard(byte[][] board)
        {
            byte[][] newBoard = new byte[board.Length][];
            for (int i = 0; i < board.Length; i++)
            {
                newBoard[i] = (byte[])board[i].Clone();
            }
            return newBoard;
        }
        private static string GenerateBoardKey(byte[][] board, byte currentPlayer)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    sb.Append(board[i][j]);
                }
            }
            sb.Append("-P" + currentPlayer); // Append current player's ID to the key
            return sb.ToString();
        }


        public static int[] Minimax(byte[][] board, int depth, bool isMaximizing, int alpha, int beta, byte player,Dictionary<string, int[]> transpositionTable)
        {
            // Generate a unique key for the current board state
            string boardKey = GenerateBoardKey(board, player);

            // Check if the current state is already evaluated
            if (transpositionTable.ContainsKey(boardKey) && transpositionTable[boardKey][0] >= depth)
            {
                return transpositionTable[boardKey];
            }

            // Terminal condition: win/loss or maximum depth reached
            int isWin = WinCheck(board);
            if (isWin != 0 || depth == 0)
            {
                int stateScore = EvaluateBoard(board, player);
                int[] result = new int[] { 0, stateScore};
                transpositionTable[boardKey] = result; // Cache the result
                return result;
            }

            int bestCol = -1;
            int bestScore = isMaximizing ? int.MinValue : int.MaxValue;

            // Iterate over all possible moves
            for (int x = 0; x < board.Length; x++)
            {
                if (board[x][board[0].Length - 1] != 0) continue; // Skip full columns

                // Create a copy of the board and apply the move
                byte[][] tempBoard = CopyBoard(board);
                DropCounter(tempBoard, x, isMaximizing ? player : (byte)(3 - player));

                int isNextWin = WinCheck(tempBoard);
                if (isNextWin == player)
                {
                    int []output = new int[] { x, isMaximizing ? int.MaxValue : int.MinValue };
                    transpositionTable[boardKey] = output; // Cache the result
                    return output;
                }

                // Recursively evaluate the move
                int[] result = Minimax(tempBoard, depth - 1, !isMaximizing, alpha, beta, player, transpositionTable);
                if (isMaximizing)
                {
                    if (result[1] > bestScore)
                    {
                        bestScore = result[1];
                        bestCol = x;
                    }
                    alpha = Math.Max(alpha, result[1]);
                }
                else
                {
                    if (result[1] < bestScore)
                    {
                        bestScore = result[1];
                        bestCol = x;
                    }
                    beta = Math.Min(beta, result[1]);
                }

                // Alpha-beta pruning
                if (beta <= alpha) break;
            }

            // Cache the result and return
            int[] resultToCache = new int[] { bestCol, bestScore };
            transpositionTable[boardKey] = resultToCache;
            return resultToCache;
        }


        static void Main(string[] args)
        {
            bool playerFirst;
            playerFirst = Console.ReadLine() == "1";
            byte[][] board = new byte[7][];
            int AIDepth = 11;
            Dictionary<string, int[]> transpositionTable = TranspositionTableHandler.LoadTranspositionTable("tpt.json");
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new byte[6];
            }
            Draw(board);
            while (true)
            {
                int column = 0;
                if (playerFirst)
                {
                    Console.WriteLine("Enter column number");
                    column = byte.Parse(Console.ReadLine());
                    DropCounter(board, column - 1, 1);
                }
                else
                {
                    Stopwatch stopwatch = Stopwatch.StartNew(); // Start timing
                    int[] bestMove = Minimax(board, AIDepth, true, int.MinValue, int.MaxValue, 1, transpositionTable);
                    stopwatch.Stop(); // Stop timing
                    DropCounter(board, bestMove[0], 1);
                    Draw(board);
                    Console.WriteLine($"AI Played : {bestMove[0] + 1}");
                    Console.WriteLine("AI current best score: " + bestMove[1]);
                    Console.WriteLine($"Minimax took: {stopwatch.ElapsedMilliseconds} ms");
                    TranspositionTableHandler.SaveTranspositionTable(transpositionTable, "tpt.json");
                }
                if (WinCheck(board) != 0)
                {
                    Console.WriteLine("Game over!");
                    break;
                }
                if (playerFirst)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew(); // Start timing
                    int[] bestMove = Minimax(board, AIDepth, true, int.MinValue, int.MaxValue, 2, transpositionTable);
                    stopwatch.Stop(); // Stop timing
                    DropCounter(board, bestMove[0], 2);
                    Draw(board);
                    Console.WriteLine($"AI Played : {bestMove[0] + 1}");
                    Console.WriteLine("AI current best score: " + bestMove[1]);
                    Console.WriteLine($"Minimax took: {stopwatch.ElapsedMilliseconds} ms");
                    TranspositionTableHandler.SaveTranspositionTable(transpositionTable, "tpt.json");
                }
                else
                {
                    Console.WriteLine("Enter column number");
                    column = byte.Parse(Console.ReadLine());
                    DropCounter(board, column - 1, 2);
                }
                if (WinCheck(board) != 0)
                {
                    Draw(board);
                    Console.WriteLine("Game over!");
                    break;
                }
            }
        }
    }
}

