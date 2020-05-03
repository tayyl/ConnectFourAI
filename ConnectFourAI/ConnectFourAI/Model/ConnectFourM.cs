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
            gameBoard = new byte[7, 6];
            CurrentPlayer = BoardCellState.Player1;
        }

        public bool PlaceCoin(byte column, BoardCellState player)
        {
            for(byte i=0; i<gameBoard.GetLength(0); i++)
            {
                if (gameBoard[i, column] == (byte)BoardCellState.Empty)
                {
                    gameBoard[i, column] = (byte)player;
                    return true;
                }
            }
            return false;
        }
        public bool CheckIfWin(BoardCellState player)
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

            int HEIGHT = gameBoard.GetLength(0);
            int WIDTH = gameBoard.GetLength(1);

            for (int i = 0; i < HEIGHT; i++)
            { // iterate rows, bottom to top
                for (int j = 0; j < WIDTH; j++)
                { // iterate columns, left to right
                    if (gameBoard[i, j] == (byte)player)
                    {
                        if (j + 3 < WIDTH &&
                            (byte)player == gameBoard[i,j + 1] && // look right
                            (byte)player == gameBoard[i,j + 2] &&
                            (byte)player == gameBoard[i,j + 3])
                            return true;
                        if (i + 3 < HEIGHT)
                        {
                            if ((byte)player == gameBoard[i + 1,j] && // look up
                                (byte)player == gameBoard[i + 2,j] &&
                                (byte)player == gameBoard[i + 3,j])
                                return true;
                            if (j + 3 < WIDTH &&
                                (byte)player == gameBoard[i + 1,j + 1] && // look up & right
                                (byte)player == gameBoard[i + 2,j + 2] &&
                                (byte)player == gameBoard[i + 3,j + 3])
                                return true;
                            if (j - 3 >= 0 &&
                                (byte)player == gameBoard[i + 1,j - 1] && // look up & left
                                (byte)player == gameBoard[i + 2,j - 2] &&
                                (byte)player == gameBoard[i + 3,j - 3])
                                return true;
                        }
                    }
                }
            }
            return false; 
        }

    }
    
}
