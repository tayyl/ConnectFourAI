using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectFourAI.Model
{
    public enum BoardCellState   { Empty=0,Player1=1,Player2=2}
    public class ColumnScore
    {
        public int Column { get; set; }
        public int Score { get; set; }
        public ColumnScore(int column, int score)
        {
            Column = column;
            Score = score;
        }
    }
    public class ConnectFourM
    {
        #region Attributes
        public bool IsGameEnded { get; private set; }
        public BoardCellState CurrentPlayer { get; set; }
        public byte[,] GameBoard { get => gameBoard; }
        public List<int[]> WinningSequence { get; set; }
        #endregion

        #region Variables
        byte[,] gameBoard;
        #endregion
        public ConnectFourM()
        {
            gameBoard = new byte[6, 7];
            CurrentPlayer = BoardCellState.Player1;
        }
        public void RestartGame()
        {
            gameBoard = new byte[6, 7];
            CurrentPlayer = BoardCellState.Player1;
            WinningSequence = new List<int[]>();
            IsGameEnded = false;

        }
        public bool PlaceCoin(byte[,] board,int column, BoardCellState player, ref int[] placedCoin)
        {
            if (column > -1)
            {
                for (int i = board.GetLength(0) - 1; i > -1; i--)
                {
                    if (board[i, column] == (byte)BoardCellState.Empty)
                    {
                        board[i, column] = (byte)player;
                        placedCoin[0] = i;
                        placedCoin[1] = column;
                        return true;
                    }
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
                        {
                            WinningSequence = new List<int[]>()
                            {
                                new int[2]{i,j},
                                new int[2]{i,j+1},
                                new int[2]{i,j+2},
                                new int[2]{i,j+3}
                            };
                            return IsGameEnded=true;
                        }
                        if (i + 3 < HEIGHT)
                        {
                            if ((byte)player == board[i + 1, j] && // look up
                                (byte)player == board[i + 2, j] &&
                                (byte)player == board[i + 3, j])
                            {

                                WinningSequence = new List<int[]>()
                            {
                                new int[2]{i,j},
                                new int[2]{i+1,j},
                                new int[2]{i+2,j},
                                new int[2]{i+3,j}
                            };
                                return IsGameEnded = true;
                            }
                            if (j + 3 < WIDTH &&
                                (byte)player == board[i + 1, j + 1] && // look up & right
                                (byte)player == board[i + 2, j + 2] &&
                                (byte)player == board[i + 3, j + 3])
                            {

                                WinningSequence = new List<int[]>()
                            {
                                new int[2]{i,j},
                                new int[2]{i+1,j+1},
                                new int[2]{i+2,j+2},
                                new int[2]{i+3,j+3}
                            };
                                return IsGameEnded = true;
                            }
                            if (j - 3 >= 0 &&
                                (byte)player == board[i + 1, j - 1] && // look up & left
                                (byte)player == board[i + 2, j - 2] &&
                                (byte)player == board[i + 3, j - 3])
                            {

                                WinningSequence = new List<int[]>()
                            {
                                new int[2]{i,j},
                                new int[2]{i+1,j-1},
                                new int[2]{i+2,j-2},
                                new int[2]{i+3,j-3}
                            };
                                return IsGameEnded = true;
                            }
                        }
                    }
                }
            }
            return IsGameEnded = false;
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
                else if (toCheck[i] == (int)BoardCellState.Empty)
                    emptyCellCounter++;
                else if (toCheck[i] == (int)opponent)
                    opponentCoinCounter++;
            }

            if (playerCoinCounter == 4)
                score = 100;
            else if (playerCoinCounter == 3 && emptyCellCounter == 1)
                score = 5;
            else if (playerCoinCounter == 2 && emptyCellCounter == 2)
                score = 2;
            if (opponentCoinCounter == 3 && emptyCellCounter == 1)
                score = -4;

            return score;
        }
        int scorePosition(byte[,] board,BoardCellState player)
        {
            int score = 0,windowLength=4;
            List<byte> window = new List<byte>();
            //Score center column
            int centerArrayCounter=0;
            for (int i = 0; i < board.GetLength(0); i++)
                if (board[i, board.GetLength(1) / 2] == (byte)player)
                    centerArrayCounter++;
            score = centerArrayCounter * 3;

            //Score Horizontal
            for(int i=0; i < board.GetLength(0); i++)
            {
                for(int j=0; j<board.GetLength(1)-3; j++)
                {
                    for (int c = j; c < j +windowLength; c++)
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
                    for (int c = j; c < j + windowLength; c++)
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
                    for (int c = 0; c < windowLength; c++)
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
                    for (int c = 0; c < windowLength; c++)
                        window.Add(board[i +3- c, j + c]);
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
                if (gameBoard[0, i] == (int)BoardCellState.Empty)
                    validLocations.Add(i);
            return validLocations.ToArray();
        }
  
        public ColumnScore Minmax(byte[,] board, int depth, int alpha, int beta, bool maximizingPlayer)
        {
            int column,value;
            int[] placedCoin=new int[2];
            int[] validLocations = getValidLocations(board);
            bool isTerminal = isTerminalNode(board);
            int newScore;
            if(depth==0 || isTerminal)
            {
                if (isTerminal)
                {
                    if (CheckIfWin(board, BoardCellState.Player2))
                    {
                        return new ColumnScore(-1,1000000);
                    }
                    else if (CheckIfWin(board, BoardCellState.Player1))
                    {
                        return new ColumnScore(-1, -1000000);

                    }
                    else
                    {
                        return new ColumnScore(-1, 0);
                    }
                }
                else
                
                {
                    return new ColumnScore(-1, scorePosition(board, BoardCellState.Player2));

                }
            }
            if (maximizingPlayer)
            {
                value = int.MinValue;
                column = validLocations[0];
                foreach(int col in validLocations)
                {
                    byte[,] tmpBoard = (byte[,])board.Clone();
                    PlaceCoin(tmpBoard,col, BoardCellState.Player2, ref placedCoin);
                    newScore= Minmax(tmpBoard, depth - 1, alpha, beta, false).Score;
                    if (newScore > value)
                    {
                        value = newScore;
                        column = col;
                    }
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                return new ColumnScore(column, value);
            }
            else
            {
                value = int.MaxValue;
                column = validLocations[0];
                foreach (int col in validLocations)
                {
                    byte[,] tmpBoard = (byte[,])board.Clone();
                    PlaceCoin(tmpBoard,col, BoardCellState.Player1, ref placedCoin);
                    newScore= Minmax(tmpBoard, depth - 1, alpha, beta, true).Score;
                    if (newScore < value)
                    {
                        value = newScore;
                        column = col;
                    }
                    beta = Math.Min(beta, value);
                   if (alpha >= beta)
                        break;
                }
                return new ColumnScore(column,value);
            }
        }
        #endregion
    }

}
