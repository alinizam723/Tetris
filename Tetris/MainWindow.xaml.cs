using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris.Blocks;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] _tiles = new ImageSource[]
        {
            new BitmapImage(new Uri("Images/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/TileRed.png", UriKind.Relative)),
        };

        private readonly ImageSource[] _blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Images/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Images/Block-Z.png", UriKind.Relative)),
        };

        private Image[,] _imageControls;

        private GameState _gameState = new GameState();
        private readonly int  _minDelay = 75;
        private readonly int  _maxDelay = 1000;
        private readonly int  _delayDecrease = 25;

        public MainWindow()
        {
            InitializeComponent();
            _imageControls = SetUpGameCanvas(_gameState.GameGrid);
        }

        private Image[,] SetUpGameCanvas(GameGrid gameGrid)
        {
            Image[,] imageControls = new Image[gameGrid.Rows, gameGrid.Columns];
            int cellSize = 25;

            for (int r = 0; r < gameGrid.Rows; r++)
            {
                for (int c = 0; c < gameGrid.Columns; c++)
                {
                    Image imageControl = new Image()
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);

                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        
        private async Task GameLoop()
        {
            Draw(_gameState);
            while (!_gameState.GameOver)
            {
                int delay = Math.Max(_minDelay, _maxDelay - (_gameState.Score * _delayDecrease));
                await Task.Delay(delay);
                _gameState.MoveDown();
                Draw(_gameState);
            }
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Final Score: {_gameState.Score}";
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHoldBlock(gameState.HeldBlock);
            ScoreText.Text = $"Score: {gameState.Score}";
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            var nextBlock = blockQueue.NextBlock;
            NextImage.Source = _blockImages[nextBlock.Id];
        }

        private void DrawHoldBlock(BaseBlock heldBlock)
        {
            if(heldBlock == null)
            {
                HoldImage.Source = _blockImages[0];
            }
            else
            {
                HoldImage.Source = _blockImages[heldBlock.Id];
            }
        }

        private void DrawGhostBlock(BaseBlock block)
        {
            int dropDistance = _gameState.BlockDropDistance();

            foreach(var position in block.TilePositions())
            {
                _imageControls[position.Row+dropDistance, position.Column].Opacity = 0.25;
                _imageControls[position.Row+dropDistance, position.Column].Source = _tiles[block.Id];
            }
        }

        private void DrawGrid(GameGrid gameGrid)
        {
            for (int r = 0; r < gameGrid.Rows; r++)
            {
                for (int c = 0; c < gameGrid.Columns; c++)
                {
                    _imageControls[r, c].Opacity = 1;
                    _imageControls[r, c].Source = _tiles[gameGrid[r, c]];
                }
            }
        }

        private void DrawBlock(BaseBlock block)
        {
            foreach (var position in block.TilePositions())
            {
                _imageControls[position.Row, position.Column].Opacity = 1;
                _imageControls[position.Row, position.Column].Source = _tiles[block.Id];
            }
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            _gameState =  new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    _gameState.MoveLeft();
                    break;
                case Key.Right:
                    _gameState.MoveRight();
                    break;
                case Key.Down:
                    _gameState.MoveDown();
                    break;
                case Key.Up:
                    _gameState.RotateClockWise();
                    break;
                case Key.Z:
                    _gameState.RotateCounterClockWise();
                    break;
                case Key.C:
                    _gameState.HoldBlock();
                    break;
                case Key.Space:
                    _gameState.DropBlock();
                    break;
                default:
                    return;
            }
            Draw(_gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
    }
}
