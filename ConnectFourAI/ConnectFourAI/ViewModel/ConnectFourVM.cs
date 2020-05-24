using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ConnectFourAI.Model;
namespace ConnectFourAI.ViewModel
{
    #region Enums
    enum Mode { PvP=0,PvC=1,CvC=2}
    enum Difficulty { Easy=3,Medium=6,Hard=9}
    #endregion
    public class CircleItem : INotifyPropertyChanged
    {
        int x;
        int y;
        int r;
        SolidColorBrush color;
        public int X { 
            get 
            {
                return x;
            }
            set 
            {
                x = value;
                OnPropertyChanged("X");
            }
            
        }
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }
        public int R
        {
            get
            {
                return r;
            }
            set
            {
                r = value;
                OnPropertyChanged("R");
            }

        }
        public SolidColorBrush Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                OnPropertyChanged("Color");
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ConnectFourVM : INotifyPropertyChanged
    {
        #region Attributes
        SolidColorBrush currentPlayerColor;
        public SolidColorBrush CurrentPlayerColor {
            get
            {
                return currentPlayerColor;
            }
            set
            {
                currentPlayerColor = value;
                OnPropertyChanged("CurrentPlayerColor");
            }
        }
        public double PanelXUpper
        {
            get { return PanelX-50; }
        }
        public double PanelX
        {
            get { return _panelX; }
            set
            {
                if (value.Equals(_panelX)) return;
                _panelX = value;
                OnPropertyChanged("PanelX");
                OnPropertyChanged("PanelXUpper");
            }
        }
        public double PanelY
        {
            get { return _panelY; }
            set
            {
                if (value.Equals(_panelY)) return;
                _panelY = value;
                OnPropertyChanged("PanelY");
            }
        }
        public ObservableCollection<CircleItem> CircleItems { get; set; }
        public int GameBoardWidth { get; set; } = 890;
        public int GameBoardHeight { get; set; }  = 770;
        public int FirstPanelHeight { get; set; } = 100;
        public int ModeSelection { get; set; }
        #endregion
        #region Variables
        bool aiMove = false;
        Mode mode = Mode.PvP;
        Difficulty difficulty = Difficulty.Medium;
        Difficulty difficulty2nd = Difficulty.Medium;
        ConnectFourM model;
        int rowIndex = 0, columnIndex = 0, circleMarginBottomIndex = 1, circleMarginLeftIndex = 1;
        int circleMargin = 9;
        private double _panelX;
        private double _panelY;
        #endregion
        #region Commands
        ICommand restart;
        public ICommand Restart
        {
            get
            {
                return restart;
            }
        }
        ICommand placeCoin;
        public ICommand PlaceCoin
        {
            get
            {
                return placeCoin;
            }
        }
        ICommand pvp;
        public ICommand Pvp
        {
            get
            {
                return pvp;
            }
        }
        ICommand pvc;
        public ICommand Pvc
        {
            get
            {
                return pvc;
            }
        }
        ICommand cvc;
        public ICommand Cvc
        {
            get
            {
                return cvc;
            }
        }
                ICommand easy;
        public ICommand Easy
        {
            get
            {
                return easy;
            }
        }

        ICommand medium;
        public ICommand Medium
        {
            get
            {
                return medium;
            }
        }
        ICommand hard;
        public ICommand Hard
        {
            get
            {
                return hard;
            }
        }
        ICommand easy2nd;
        public ICommand Easy2nd
        {
            get
            {
                return easy2nd;
            }
        }

        ICommand medium2nd;
        public ICommand Medium2nd
        {
            get
            {
                return medium2nd;
            }
        }
        ICommand hard2nd;
        public ICommand Hard2nd
        {
            get
            {
                return hard2nd;
            }
        }
        #endregion
        public ConnectFourVM()
        {
            model = new ConnectFourM();
            currentPlayerColor = model.CurrentPlayer == BoardCellState.Player1 ?
                    new SolidColorBrush(Color.FromRgb(150, 0, 0)) :
                    new SolidColorBrush(Color.FromRgb(0, 150, 0));
            CircleItems = new ObservableCollection<CircleItem>();
            for(int i=1; i<model.GameBoard.GetLength(0)* model.GameBoard.GetLength(1)+1; i++)
            {              
                CircleItem circleItem = new CircleItem();
                circleItem.R = GameBoardWidth/model.GameBoard.GetLength(1)-10;
                circleItem.X = columnIndex * circleItem.R + circleMarginLeftIndex*circleMargin;
                circleItem.Y = rowIndex+circleMarginBottomIndex*circleMargin;
                circleItem.Color = new SolidColorBrush(Color.FromRgb(255, 255,255));
                columnIndex++;
                circleMarginLeftIndex++;
                if (i % model.GameBoard.GetLength(1) == 0 && i!=0)
                {
                    rowIndex += GameBoardWidth / model.GameBoard.GetLength(1) - 10;
                    columnIndex = 0;
                    circleMarginBottomIndex++;
                    circleMarginLeftIndex = 1;
                }
                CircleItems.Add(circleItem);
            }
            #region Commands
            restart = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    model.RestartGame();
                    for (int i = 0; i < CircleItems.Count; i++)
                    {
                        CircleItems[i].Color = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    }
                    CurrentPlayerColor =
                 model.CurrentPlayer == BoardCellState.Player1 ?
                    new SolidColorBrush(Color.FromRgb(150, 0, 0)) :
                    new SolidColorBrush(Color.FromRgb(0, 150, 0));
                }
            };
            pvp = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    mode = Mode.PvP;
                }
            };
            pvc = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    mode = Mode.PvC;
                }
            };
            cvc = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    mode = Mode.CvC;
                }
            };
            easy = new RelayCommand()
            {
                CanExecuteDelegate = x => mode != Mode.PvP,
                ExecuteDelegate = x =>
                {
                    difficulty= Difficulty.Easy;
                }
            };
            medium = new RelayCommand()
            {
                CanExecuteDelegate = x => mode != Mode.PvP,
                ExecuteDelegate = x =>
                {
                    difficulty = Difficulty.Medium;
                }
            };
            hard = new RelayCommand()
            {
                CanExecuteDelegate = x => mode!=Mode.PvP,
                ExecuteDelegate = x =>
                {
                    difficulty = Difficulty.Hard;
                }
            };
            easy2nd = new RelayCommand()
            {
                CanExecuteDelegate = x => mode != Mode.PvP,
                ExecuteDelegate = x =>
                {
                    difficulty2nd= Difficulty.Easy;
                }
            };
            medium2nd = new RelayCommand()
            {
                CanExecuteDelegate = x => mode != Mode.PvP,
                ExecuteDelegate = x =>
                {
                    difficulty2nd = Difficulty.Medium;
                }
            };
            hard2nd = new RelayCommand()
            {
                CanExecuteDelegate = x => mode != Mode.PvP,
                ExecuteDelegate = x =>
                {
                    difficulty2nd = Difficulty.Hard;
                }
            };
            placeCoin = new RelayCommand()
            {
                CanExecuteDelegate = x => !aiMove && !model.IsGameEnded,
                ExecuteDelegate = x =>
                {
                    int[] placedCoin = new int[2];
                    int[] mouseXY=ConvertMousePositionToArrayIndex(_panelX,_panelY);
                    switch (mode)
                    {
                        case Mode.CvC:
                            Task task1 = Task.Factory.StartNew(()=>{
                                do
                                {
                                    aiMove = true;
                                    ColumnScore columnScore;
                                    do
                                    {
                                        columnScore = model.Minmax(model.GameBoard, (int)difficulty, int.MinValue, int.MaxValue, true);
                                        if (model.PlaceCoin(model.GameBoard, columnScore.Column, model.CurrentPlayer, ref placedCoin))
                                        {
                                            Turn(placedCoin);
                                            break;
                                        }
                                    } while (!model.IsGameEnded);

                                    System.Threading.Thread.Sleep(100);
                                    do
                                    {
                                        columnScore = model.Minmax(model.GameBoard, (int)difficulty2nd, int.MinValue, int.MaxValue, true);
                                        if (model.PlaceCoin(model.GameBoard, columnScore.Column, model.CurrentPlayer, ref placedCoin))
                                        {
                                            Turn(placedCoin);
                                            break;
                                        }
                                    } while (!model.IsGameEnded);
                                    System.Threading.Thread.Sleep(100);
                                } while (!model.IsGameEnded);
                                aiMove = false;
                            });
                            break;
                        case Mode.PvP:
                            if (model.PlaceCoin(model.GameBoard, mouseXY[1], model.CurrentPlayer, ref placedCoin))
                            {
                                Turn(placedCoin);
                            }
                            break;
                        case Mode.PvC:
                            if (model.PlaceCoin(model.GameBoard, mouseXY[1], model.CurrentPlayer, ref placedCoin))
                            {
                                Task task2 = Task.Factory.StartNew(() => {

                                    Turn(placedCoin);
                                    aiMove = true;
                                    ColumnScore columnScore;
                                    columnScore = model.Minmax(model.GameBoard, (int)difficulty, int.MinValue, int.MaxValue, true);
                                    if (model.PlaceCoin(model.GameBoard, columnScore.Column, model.CurrentPlayer, ref placedCoin))
                                    {
                                        Turn(placedCoin);
                                    }
                                });
                                task2.ContinueWith(xc => { aiMove= false; });
                            }
                            break;
                    }
                    
                }
            };
            #endregion
        }
        void Turn(int[] placedCoin)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                int listIndex = placedCoin[1] + placedCoin[0] * model.GameBoard.GetLength(1);
                CurrentPlayerColor =
                model.CurrentPlayer != BoardCellState.Player1 ?
                    new SolidColorBrush(Color.FromRgb(150, 0, 0)) :
                    new SolidColorBrush(Color.FromRgb(0, 150, 0));
                CircleItems[listIndex].Color =
                model.CurrentPlayer == BoardCellState.Player1 ?
                    new SolidColorBrush(Color.FromRgb(150, 0, 0)) :
                    new SolidColorBrush(Color.FromRgb(0, 150, 0));
                OnPropertyChanged("CircleItems");
            if (model.CheckIfWin(model.GameBoard, model.CurrentPlayer))
            {
                foreach(int[] winningElement in model.WinningSequence)
                {
                        CircleItems[winningElement[1] + winningElement[0] * model.GameBoard.GetLength(1)].Color =
                        model.CurrentPlayer == BoardCellState.Player1 ?
                            new SolidColorBrush(Color.FromRgb(200, 0, 0)) :
                            new SolidColorBrush(Color.FromRgb(0, 200, 0));
                }
                if(mode!=Mode.CvC)
                    MessageBox.Show("Zwyciężył gracz o kolorze "
                        + (BoardCellState.Player1 == model.CurrentPlayer ?
                         "czerwonym!" :
                         "zielonym!")
                        );
                    else
                        MessageBox.Show("Zwyciężył komputer o kolorze "
                            + (BoardCellState.Player1 == model.CurrentPlayer ?
                             "czerwonym! (depth="+difficulty+")" :
                             "zielonym! (depth="+difficulty2nd+")")
                            );

                }
            model.ChangePlayer();
            });
        }
        int[] ConvertMousePositionToArrayIndex(double mouseX, double mouseY)
        {
            int[] mouseXY = new int[2];

            mouseXY[1] = Convert.ToInt32(Math.Floor(mouseX / ((GameBoardWidth / model.GameBoard.GetLength(1)))));
            mouseXY[0] = Convert.ToInt32(Math.Floor(mouseY / ((GameBoardWidth / model.GameBoard.GetLength(1)))));
            return mouseXY;
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
