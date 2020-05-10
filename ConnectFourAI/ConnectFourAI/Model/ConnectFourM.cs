using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFourAI.Model
{
    public enum BoardCellState   { Empty=0,Player1=1,Player2=2}
    public class ConnectFourM
    {
        #region Attributes
        public BoardCellState CurrentPlayer { get; set; }
        public byte[,] GameBoard { get => gameBoard; }
        #endregion

        #region Variables
        byte[,] gameBoard;
        #endregion
        public ConnectFourM()
        {
            gameBoard = new byte[6, 7];
            CurrentPlayer = BoardCellState.Player1;
        }

        public bool PlaceCoin(int column, BoardCellState player, ref int[] placedCoin)
        {
            for (int i = gameBoard.GetLength(0) - 1; i > -1; i--)
            {
                if (gameBoard[i, column] == (byte)BoardCellState.Empty)
                {
                    gameBoard[i, column] = (byte)player;
                    placedCoin[0] = i;
                    placedCoin[1] = column;
                    return true;
                }
            }
            return false;
        }
        public bool CheckIfWin(byte[,] board,BoardCellState player)
        {
            #region Rules description
            /*
             There are 4 possibilities of winning:
              - XXXX => horizontal line
              
              - X
                X
                X
                X => vertical line

              - X
                 X
                  X
                   X => right-slant line

              -    X
                  X
                 X
                X => lef-slant line

             All of them have to be checked manually
             */
            #endregion

            int HEIGHT = board.GetLength(0);
            int WIDTH = board.GetLength(1);

            for (int i = 0; i < HEIGHT; i++)
            { // iterate rows, bottom to top
                for (int j = 0; j < WIDTH; j++)
                { // iterate columns, left to right
                    if (board[i, j] == (byte)player)
                    {
                        if (j + 3 < WIDTH &&
                            (byte)player == board[i, j + 1] && // look right
                            (byte)player == board[i, j + 2] &&
                            (byte)player == board[i, j + 3])
                            return true;
                        if (i + 3 < HEIGHT)
                        {
                            if ((byte)player == board[i + 1, j] && // look up
                                (byte)player == board[i + 2, j] &&
                                (byte)player == board[i + 3, j])
                                return true;
                            if (j + 3 < WIDTH &&
                                (byte)player == board[i + 1, j + 1] && // look up & right
                                (byte)player == board[i + 2, j + 2] &&
                                (byte)player == board[i + 3, j + 3])
                                return true;
                            if (j - 3 >= 0 &&
                                (byte)player == board[i + 1, j - 1] && // look up & left
                                (byte)player == board[i + 2, j - 2] &&
                                (byte)player == board[i + 3, j - 3])
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        public void ChangePlayer()
        {
            CurrentPlayer = CurrentPlayer == BoardCellState.Player1 ? BoardCellState.Player2 : BoardCellState.Player1;
        }

        #region AI
        int evaluate(byte[] toCheck, BoardCellState player)
        {
            int score = 0;
            BoardCellState opponent = player == BoardCellState.Player1 ? BoardCellState.Player2 : BoardCellState.Player1;

            int playerCoinCounter = 0, emptyCellCounter = 0, opponentCoinCounter=0;

            for(int i=0; i<toCheck.Length; i++)
            {
                if (toCheck[i] == (int)player)
                    playerCoinCounter++;
                if (toCheck[i] == (int)BoardCellState.Empty)
                    emptyCellCounter++;
                if (toCheck[i] == (int)opponent)
                    opponentCoinCounter++;
            }

            if (playerCoinCounter == 4)
                score += 100;
            else if (playerCoinCounter == 3 && emptyCellCounter == 1)
                score += 5;
            else if (playerCoinCounter == 2 && emptyCellCounter == 2)
                score += 2;
            if (opponentCoinCounter == 3 && emptyCellCounter == 1)
                score -= 4;

            return score;
        }
        int scorePosition(byte[,] board,BoardCellState player)
        {
            int score = 0;
            List<byte> window = new List<byte>();
            //Score center column
            int centerArrayCounter=0;
            for (int i = 0; i < board.GetLength(0); i++)
                if (board[i, board.GetLength(1) / 2 + 1] == (byte)player)
                    centerArrayCounter++;
            score += centerArrayCounter * 3;

            //Score Horizontal
            for(int i=0; i < board.GetLength(0); i++)
            {
                for(int j=0; j<board.GetLength(1)-3; j++)
                {
                    for (int c = j; c <= j + window.Count(); c++)
                        window.Add(board[i, c]);
                    score += evaluate(window.ToArray(), player);
                    window.Clear();
                }
            }
            //Score Vertical
            for (int i = 0; i < board.GetLength(1); i++)
            {
                for (int j = 0; j < board.GetLength(0) - 3; j++)
                {
                    for (int c = j; c <= j + window.Count(); c++)
                        window.Add(board[c, i]);
                    score += evaluate(window.ToArray(), player);
                    window.Clear();
                }
            }
            //Score positive sloped diagonal
            for (int i = 0; i < board.GetLength(0)-3; i++)
            {
                for (int j = 0; j < board.GetLength(1) - 3; j++)
                {
                    for (int c = 0; c <= window.Count(); c++)
                        window.Add(board[i+c, j+c]);
                    score += evaluate(window.ToArray(), player);
                    window.Clear();
                }
            }
            //Score negative sloped diagonal
            for (int i = 0; i < board.GetLength(0) - 3; i++)
            {
                for (int j = 0; j < board.GetLength(1) - 3; j++)
                {
                    for (int c = 0; c <= window.Count(); c++)
                        window.Add(board[i +3 - c, j + c]);
                    score += evaluate(window.ToArray(), player);
                    window.Clear();
                }
            }

            return score;
        }
        bool isTerminalNode(byte[,] board)
        {
            return CheckIfWin(board, BoardCellState.Player1) || CheckIfWin(board, BoardCellState.Player2) || getValidLocations(board).Length == 0;
        }

        int[] getValidLocations(byte[,] board)
        {
            List<int> validLocations=new List<int>();
            for (int i = 0; i < gameBoard.GetLength(1); i++)
                if (isValidLocation(i))
                    validLocations.Add(i);
            return validLocations.ToArray();
        }
        bool isValidLocation(int col)
        {
            return gameBoard[gameBoard.GetLength(0) - 1, col] == 0;
        }
        int pickBestMove(byte[,] board, BoardCellState player)
        {
            Random rand = new Random();
            int[] validLocations = getValidLocations(board);
            int bestScore = -10000;
            int bestCol = validLocations[rand.Next(-1, validLocations.Length)];
            foreach(int col in validLocations)
            {
                int row = getNextOpenRow(board,col);
                byte[,] tmpBoard = (byte[,])board.Clone();
                CheckIfWin(tmpBoard, player);
                int score = scorePosition(tmpBoard, player);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCol = col;
                }
            }
            return bestCol;
        }
        int getNextOpenRow(byte[,] board,int col)
        {
            for (int i = 0; i < board.GetLength(0); i++)
                if (board[i, col] == (byte)BoardCellState.Empty)
                    return i;
            return 0;
        }
        int minmax(byte[,] board, int depth, int alpha, int beta, bool maximizingPlayer, ref int value)
        {
            Random rand = new Random();
            int column = -1,row=-1;
            int[] placedCoin=new int[2];
            int[] validLocations = getValidLocations(board);
            bool isTerminal = isTerminalNode(board);
            int newScore = 0;
            if(depth==0 || isTerminal)
            {
                if (isTerminal)
                {
                    if (CheckIfWin(board, BoardCellState.Player2))
                    {
                        value = int.MaxValue;
                        return -1;
                    }
                    else if (CheckIfWin(board, BoardCellState.Player1))
                    {
                        value = int.MinValue;
                        return -1;
                    }
                    else
                    {
                        value = 0;
                        return -1;
                    }                        
                }
                else
                {
                    value = scorePosition(board, BoardCellState.Player2);
                    return -1;
                }
            }
            if (maximizingPlayer)
            {
                value = int.MinValue;
                column = validLocations[rand.Next(-1, validLocations.Length)];
                foreach(int i in validLocations)
                {
                    row = getNextOpenRow(board, i);
                    byte[,] tmpBoard = (byte[,])board.Clone();
                    PlaceCoin(column, BoardCellState.Player2, ref placedCoin);
                    newScore = minmax(tmpBoard, depth - 1, alpha, beta, false, ref value);
                    if (newScore > value)
                    {
                        value = newScore;
                        column = i;
                    }
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
            }
            else
            {
                value = int.MinValue;
                column = validLocations[rand.Next(-1, validLocations.Length)];
                foreach (int i in validLocations)
                {
                    row = getNextOpenRow(board, i);
                    byte[,] tmpBoard = (byte[,])board.Clone();
                    PlaceCoin(column, BoardCellState.Player1, ref placedCoin);
                    newScore = minmax(tmpBoard, depth - 1, alpha, beta, true, ref value);
                    if (newScore < value)
                    {
                        value = newScore;
                        column = i;
                    }
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
            }
        }
        #endregion
    }

}
