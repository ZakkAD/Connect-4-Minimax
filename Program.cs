using System.Diagnostics;
using System.Net.Sockets;

namespace Connect_4_Minimax
{
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

    static int EvaluateBoard(byte[][] board, byte player)
    {
        byte winner = WinCheck(board);       
        if (winner == player)
        {
            return 10000000;
        }
        else if (winner != player)
        {
            if (winner != 0)
            {
                return -10000000;
            }
        }

        int score = 0;
        for (int y = 0; y < board.Length; y++){
            if (board[y][4] == player)
            {
                    score += 15;
            }
        }
        for (int y = board[0].Length - 1; y > -1; y--)
        {
            for (int x = 0; x < board.Length; x++)
            {
                    if (board[x][y] == 0)
                    {
                        continue;
                    }
                    else if (board[x][y] == player)
                    {

                        if (x + 3 < board.Length && board[x][y] == board[x + 1][y] && board[x][y] == board[x + 2][y] && board[x][y] == 0)
                        {
                            score += 100;
                        }
                        if (x + 3 < board.Length && x > 0 && board[x][y] == board[x + 1][y] && board[x][y] == board[x + 2][y] && board[x - 1][y] == 0)
                        {
                            score += 100;
                        }
                        if (y + 3 < board[0].Length)
                        {
                            if (board[x][y] == board[x][y + 1] && board[x][y] == board[x][y + 2] && board[x][y + 3] == 0)
                            {
                                score += 100;
                            }
                            if (y > 0 && board[x][y] == board[x][y + 1] && board[x][y] == board[x][y + 2] && board[x][y - 1] == 0)
                            {
                                score += 100;
                            }

                            if (x + 3 < board.Length && board[x][y] == board[x + 1][y + 1] && board[x][y] == board[x + 2][y + 2] && board[x + 3][y + 3] == 0)
                            {
                                score += 100;
                            }
                            if (x - 3 > -1 && board[x][y] == board[x - 1][y + 1] && board[x][y] == board[x - 2][y + 2] && board[x - 3][y + 3] == 0)
                            {
                                score += 100;
                            }
                        }
                        if (x + 2 < board.Length && y + 2 < board[0].Length && y > 0 && x > 0 && board[x][y] == board[x + 1][y + 1] && board[x][y] == board[x + 2][y + 2] && board[x - 1][y - 1] == 0)
                        {
                            score += 100;
                        }
                        if (x - 2 < board.Length && y + 2 < board[0].Length && y > 0 && x > board.Length - 1 && board[x][y] == board[x - 1][y + 1] && board[x][y] == board[x - 2][y + 2] && board[x + 1][y - 1] == 0)
                        {
                            score += 100;
                        }
                        if (x + 2 < board.Length && board[x][y] == board[x + 1][y] && board[x + 2][y] == 0)
                        {
                            score += 50;
                        }
                        if (x + 2 < board.Length && x > 0 && board[x][y] == board[x + 1][y] && board[x - 1][y] == 0)
                        {
                            score += 50;
                        }
                        if (y + 2 < board[0].Length)
                        {
                            if (board[x][y] == board[x][y + 1] && board[x][y + 2] == 0)
                            {
                                score += 50;
                            }
                            if (y > 0 && board[x][y] == board[x][y + 1] && board[x][y - 1] == 0)
                            {
                                score += 50;
                            }
                            if (x + 3 < board.Length && board[x][y] == board[x + 1][y + 1] && board[x + 2][y+2] == 0)
                            {
                                score += 50;
                            }
                            if (x - 3 > -1 && board[x][y] == board[x - 1][y + 1] && board[x - 2][y + 2] == 0)
                            {
                                score += 50;
                            }
                        }
                        if (x + 1 < board.Length && y + 1 < board[0].Length && x > 0 && y > 0 && board[x][y] == board[x + 1][y + 1] && board[x - 1][y - 1] == 0)
                        {
                            score += 50;
                        }
                        if (x + 1 < board.Length && y + 1 < board[0].Length && x > 0 && y > 0 && board[x][y] == board[x - 1][y + 1] && board[x + 1][y - 1] == 0)
                        {
                            score += 50;
                        }
                    }
                    else if (board[x][y] != player)
                    {
                        if (x + 3 < board.Length && board[x][y] == board[x + 1][y] && board[x][y] == board[x + 2][y] && board[x][y] == 0)
                        {
                            score -= 100;
                        }
                        if (x + 3 < board.Length && x > 0 && board[x][y] == board[x + 1][y] && board[x][y] == board[x + 2][y] && board[x - 1][y] == 0)
                        {
                            score -= 100;
                        }
                        if (y + 3 < board[0].Length)
                        {
                            if (board[x][y] == board[x][y + 1] && board[x][y] == board[x][y + 2] && board[x][y + 3] == 0)
                            {
                                score -= 100;
                            }
                            if (y > 0 && board[x][y] == board[x][y + 1] && board[x][y] == board[x][y + 2] && board[x][y - 1] == 0)
                            {
                                score -= 100;
                            }

                            if (x + 3 < board.Length && board[x][y] == board[x + 1][y + 1] && board[x][y] == board[x + 2][y + 2] && board[x + 3][y + 3] == 0)
                            {
                                score -= 100;
                            }
                            if (x - 3 > -1 && board[x][y] == board[x - 1][y + 1] && board[x][y] == board[x - 2][y + 2] && board[x - 3][y + 3] == 0)
                            {
                                score -= 100;
                            }
                        }
                        if (x + 2 < board.Length && y + 2 < board[0].Length && y > 0 && x > 0 && board[x][y] == board[x + 1][y + 1] && board[x][y] == board[x + 2][y + 2] && board[x - 1][y - 1] == 0)
                        {
                            score -= 100;
                        }
                        if (x - 2 < board.Length && y + 2 < board[0].Length && y > 0 && x > board.Length - 1 && board[x][y] == board[x - 1][y + 1] && board[x][y] == board[x - 2][y + 2] && board[x + 1][y - 1] == 0)
                        {
                            score -= 100;
                        }
                        if (x + 2 < board.Length && board[x][y] == board[x + 1][y] && board[x + 2][y] == 0)
                        {
                            score -= 50;
                        }
                        if (x + 2 < board.Length && x > 0 && board[x][y] == board[x + 1][y] && board[x - 1][y] == 0)
                        {
                            score -= 50;
                        }
                        if (y + 2 < board[0].Length)
                        {
                            if (board[x][y] == board[x][y + 1] && board[x][y + 2] == 0)
                            {
                                score -= 50;
                            }
                            if (y > 0 && board[x][y] == board[x][y + 1] && board[x][y - 1] == 0)
                            {
                                score -= 50;
                            }
                            if (x + 3 < board.Length && board[x][y] == board[x + 1][y + 1] && board[x + 2][y + 2] == 0)
                            {
                                score -= 50;
                            }
                            if (x - 3 > -1 && board[x][y] == board[x - 1][y + 1] && board[x - 2][y + 2] == 0)
                            {
                                score -= 50;
                            }
                        }
                        if (x + 1 < board.Length && y + 1 < board[0].Length && x > 0 && y > 0 && board[x][y] == board[x + 1][y + 1] && board[x - 1][y - 1] == 0)
                        {
                            score -= 50;
                        }
                        if (x + 1 < board.Length && y + 1 < board[0].Length && x > 0 && y > 0 && board[x][y] == board[x - 1][y + 1] && board[x + 1][y - 1] == 0)
                        {
                            score -= 50;
                        }
                    }
            }
        }

        return score;
    }

    static int[] Minimax(byte[][] board, int depth, bool isMaximizing, int alpha, int beta, byte player)
    {
        int stateScore = EvaluateBoard(board, player);
        if (stateScore == 10000000 || stateScore == -10000000 || depth == 0)
        {
            return new int[] { 0, stateScore};
        }

        int bestCol = 0;
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i][board[0].Length - 1] != 0)
            {
                continue;
            }
            bestCol = i;
            break;
        }
        int bestScore;

        if (isMaximizing)
        {
            bestScore = int.MinValue;
            for (int x = 0; x < board.Length; x++)
            {
                if (board[x][board[0].Length - 1] != 0)
                {
                        continue;
                }
                byte[][] tempBoard = new byte[7][];
                for (int i = board.Length - 1; i > -1; i--)
                {
                        tempBoard[i] = new byte[6];
                }
                for (int i = board[0].Length - 1; i > -1; i--)
                {
                    for (int j = 0; j < board.Length; j++)
                    {
                        tempBoard[j][i] = board[j][i];
                    }
                }
                DropCounter(tempBoard, x, player);
                int score = Minimax(tempBoard, depth - 1, false, alpha, beta, player)[1];
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCol = x;
                }
                alpha = Math.Max(alpha, score);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }
        else
        {
            bestScore = int.MaxValue;
            for (int x = 0; x < board.Length; x++)
            {
                byte[][] tempBoard = new byte[7][];
                for (int i = board.Length - 1; i > -1; i--)
                {
                    tempBoard[i] = new byte[6];
                }
                for (int i = board[0].Length - 1; i > -1; i--)
                {
                    for (int j = 0; j < board.Length; j++)
                    {
                        tempBoard[j][i] = board[j][i];
                    }
                }
                if (player == 1) DropCounter(tempBoard, x, 2);
                else DropCounter(tempBoard, x, 1);
                int score = Minimax(tempBoard, depth - 1, true, alpha, beta, player)[1];
                if (score < bestScore)
                {
                    bestScore = score;
                    bestCol = x;
                }
                beta = Math.Min(beta, score);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }
        return new int[] { bestCol, bestScore };
    }

    static void Main(string[] args)
    {
        bool playerFirst;
        playerFirst = Console.ReadLine() == "1";
        byte[][] board = new byte[7][];
        int AIDepth = 11;
                ;
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
                int[] bestMove = Minimax(board, AIDepth, true, int.MinValue, int.MaxValue, 1);
                DropCounter(board, bestMove[0], 1);
                Draw(board);
                Console.WriteLine($"AI Played : {bestMove[0] + 1}");
                Console.WriteLine("AI current best score " + bestMove[1]);
                }
                if (WinCheck(board) != 0)
            {
                Console.WriteLine("Game over!");
                break;
            }
            if (playerFirst)
            {
                int[] bestMove = Minimax(board, AIDepth, true, int.MinValue, int.MaxValue, 2);
                DropCounter(board, bestMove[0], 2);
                Draw(board);
                Console.WriteLine($"AI Played : {bestMove[0] + 1}");
                Console.WriteLine("AI current best score " + bestMove[1]);
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

