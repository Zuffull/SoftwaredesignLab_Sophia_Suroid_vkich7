using System;
using System.IO;
using System.Diagnostics;

class TicTacToe
{
    private char[] board;
    private char currentPlayer;
    private char lastPlayer;
    private string saveFileName = "saved-game.txt";
    private bool singlePlayerMode;
    private int playerXWins;
    private int playerOWins;
    private int draws;
    private bool lastMoveCancelled;
    private int lastMoveIndex;
    private Stopwatch timer;

    public TicTacToe(bool singlePlayerMode)
    {
        this.singlePlayerMode = singlePlayerMode;
        board = new char[9];
        currentPlayer = 'X';
        InitializeBoard();
        playerXWins = 0;
        playerOWins = 0;
        draws = 0;
        lastMoveCancelled = false;
        lastMoveIndex = -1;
        timer = new Stopwatch();
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = ' ';
        }
    }

    public void Play()
    {
        
        bool playAgain = true;

        while (playAgain)
        {
            bool gameover = false;
            currentPlayer = 'X';
            InitializeBoard();

            timer.Restart(); // таймер

            while (!gameover)
            {
                Console.Clear();
                DrawBoard();
                Console.WriteLine($"Progress of player {currentPlayer}.");

                if (singlePlayerMode && currentPlayer == 'O')
                {
                    MakeComputerMove();
                }
                else
                {
                    MakePlayerMove();
                }

                gameover = CheckWin() || CheckDraw();
                if (!gameover)
                    SwitchPlayer();
            }

            timer.Stop(); // Зупинка таймера

            Console.Clear();
            DrawBoard();
            if (CheckWin())
            {
                Console.WriteLine($"Player {currentPlayer} won!");
                if (currentPlayer == 'X')
                    playerXWins++;
                else
                    playerOWins++;
            }
            else
            {
                Console.WriteLine("The game ended in a draw!");
                draws++;
            }

            Console.WriteLine($"Account: Player X - {playerXWins}, Player O - {playerOWins}, Draws - {draws}");
            Console.WriteLine($"Total game time: {timer.Elapsed}");

            playAgain = PromptToPlayAgain();
        }
    }

    private void DrawBoard()
    {
        Console.WriteLine("Let`s play Tic Tac Toe");
        Console.WriteLine("Player 1: X");
        Console.WriteLine("Player 2: O");
        Console.WriteLine(" ");

        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
                Console.Write($" {i + 1} ");
            else
                Console.Write($" {board[i]} ");

            if (i % 3 != 2)
                Console.Write("|");
            else if (i != 8)
                Console.WriteLine("\n-----------");
        }
        Console.WriteLine();
    }

    private void MakePlayerMove()
    {
        int move = 0;
        bool validInput = false;
        do
        {
            Console.Write("Enter a cell number (1-9), press 'S' to save the game, 'U' to cancel the previous move, or 'Q' to exit the game: ");
            string input = Console.ReadLine().ToUpper();
            if (input == "S")
            {
                SaveGame();
                continue;
            }
            else if (input == "U")
            {
                UndoMove();
                continue;
            }
            else if (input == "Q")
            {
                Environment.Exit(0);
            }

            validInput = int.TryParse(input, out move) && move >= 1 && move <= 9;

            if (!validInput)
            {
                Console.WriteLine("Error: Please enter a valid number from 1 to 9.");
            }
            else if (!IsValidMove(move))
            {
                Console.WriteLine($"Error: Cell {move} is already occupied.");
                validInput = false;
            }
        } while (!validInput);

        board[move - 1] = currentPlayer;
        lastPlayer = currentPlayer;
        lastMoveIndex = move - 1;
    }

    private void MakeComputerMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < 9; i++)
        {
            if (board[i] == ' ')
            {
                board[i] = currentPlayer;
                int score = MiniMax(board, 0, false);
                board[i] = ' '; // Відмінити хід

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        board[bestMove] = currentPlayer;
        lastPlayer = currentPlayer;
    }

    private int MiniMax(char[] board, int depth, bool isMaximizing)
    {
        // Перевірка чи гра завершилася і повернути оцінку
        if (CheckWin())
        {
            return isMaximizing ? -10 + depth : 10 - depth;
        }
        else if (CheckDraw())
        {
            return 0;
        }

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == ' ')
                {
                    board[i] = 'O';
                    int score = MiniMax(board, depth + 1, false);
                    board[i] = ' ';
                    bestScore = Math.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] == ' ')
                {
                    board[i] = 'X';
                    int score = MiniMax(board, depth + 1, true);
                    board[i] = ' ';
                    bestScore = Math.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    private bool IsValidMove(int move)
    {
        return board[move - 1] == ' ';
    }

    private bool CheckWin()
    {
        // Перевірка рядків стовпців та діагоналей
        for (int i = 0; i < 3; i++)
        {
            if (board[i * 3] != ' ' && board[i * 3] == board[i * 3 + 1] && board[i * 3 + 1] == board[i * 3 + 2])
                return true;
            if (board[i] != ' ' && board[i] == board[i + 3] && board[i + 3] == board[i + 6])
                return true;
        }

        if (board[0] != ' ' && board[0] == board[4] && board[4] == board[8])
            return true;
        if (board[2] != ' ' && board[2] == board[4] && board[4] == board[6])
            return true;

        return false;
    }

    private bool CheckDraw()
    {
        // Перевірка чи залишилися порожні клітинки
        foreach (char cell in board)
        {
            if (cell == ' ')
                return false;
        }
        return true;
    }

    private void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
    }

    public void SaveGame()
    {
        using (StreamWriter writer = new StreamWriter(saveFileName))
        {
            writer.WriteLine(board);
            writer.WriteLine(currentPlayer);
        }
        Console.WriteLine("Game saved successfully.");
    }

    public void LoadGame()
    {
        if (File.Exists(saveFileName))
        {
            using (StreamReader reader = new StreamReader(saveFileName))
            {
                string? boardState = reader.ReadLine();
                char[] loadedBoard = boardState.ToCharArray();
                for (int i = 0; i < 9; i++)
                {
                    board[i] = loadedBoard[i];
                }
                currentPlayer = char.Parse(reader.ReadLine());
            }
            Console.WriteLine("Game saved successfully.");
        }
        else
        {
            Console.WriteLine("No saved game was found!");
            InitializeBoard();
        }
    }

    private void UndoMove()
    {
        if (lastMoveIndex != -1)
        {
            board[lastMoveIndex] = ' ';
            currentPlayer = lastPlayer;
            lastPlayer = ' ';
            lastMoveCancelled = true;
        }
    }

    private bool PromptToPlayAgain()
    {
        Console.Write("Do you want to play again? (y/n): ");
        string input = Console.ReadLine().ToLower();
        return input == "y";
    }
}

class Program
{
    static void Main(string[] args)
    {
        bool singlePlayerMode = Array.Exists(args, arg => arg == "--single-player");
        TicTacToe game = new TicTacToe(singlePlayerMode);

        if (args.Length > 0 && args[0] == "--load-saved")
        {
            game.LoadGame();
        }

        game.Play();
    }
}
