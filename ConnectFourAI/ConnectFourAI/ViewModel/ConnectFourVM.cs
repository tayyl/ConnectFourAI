using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ConnectFourAI.Model;
namespace ConnectFourAI.ViewModel
{
    public class CircleItem
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public SolidColorBrush Color { get; set; }
    }
    public class ConnectFourVM : INotifyPropertyChanged
    {
        #region Attributes
        public double PanelX
        {
            get { return _panelX; }
            set
            {
                if (value.Equals(_panelX)) return;
                _panelX = value;
                OnPropertyChanged("PanelX");
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
        #endregion
        #region Variables
        ConnectFourM model;
        int rowIndex = 0, columnIndex = 0, circleMarginBottomIndex = 1, circleMarginLeftIndex = 1;
        int circleMargin = 9;
        private double _panelX;
        private double _panelY;
        #endregion
        #region Commands
        ICommand placeCoin;
        public ICommand PlaceCoin
        {
            get
            {
                return placeCoin;
            }
        }
        #endregion
        public ConnectFourVM()
        {
            model = new ConnectFourM();
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
            placeCoin = new RelayCommand()
            {
                CanExecuteDelegate = x => true,
                ExecuteDelegate = x =>
                {
                    int[] mouseXY=ConvertMousePositionToArrayIndex(_panelX,_panelY);
                    if (model.PlaceCoin(mouseXY[1], model.CurrentPlayer))
                    {
                        model.CheckIfWin(model.CurrentPlayer);
                        model.ChangePlayer();
                    }
                }
            };
        }

        int[] ConvertMousePositionToArrayIndex(double mouseX, double mouseY)
        {
            int[] mouseXY = new int[2];

            mouseXY[1] = Convert.ToInt32(Math.Floor(mouseX / ((GameBoardWidth / model.GameBoard.GetLength(1)))));
            mouseXY[0] = Convert.ToInt32(Math.Floor(mouseY / ((GameBoardWidth / model.GameBoard.GetLength(1)))));
            return mouseXY;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
