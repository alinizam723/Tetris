using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Blocks;

namespace Tetris
{
    public class GameState
    {
        private BaseBlock _currentBlock;

        public BaseBlock CurrentBlock
        {
            get => _currentBlock;
            private set
            {
                _currentBlock = value;
                _currentBlock.Reset();
                for(int i =0;i < 2; ++i)
                {
                    _currentBlock.Move(1, 0);
                    if(!DoesBlockFit())
                    {
                        _currentBlock.Move(-1, 0);
                        break;
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }

        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }

        public int Score { get; private set; }

        public bool Canhold { get; private set; }
        public BaseBlock HeldBlock { get; private set; }


        public GameState()
        {
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            Canhold = true;
        }

        public bool DoesBlockFit()
        {
            foreach(var postion in CurrentBlock.TilePositions())
            {
                if(!GameGrid.IsEmpty(postion.Row, postion.Column))
                {
                    return false;
                }
            }

            return true;
        }

        public void RotateClockWise()
        {
            CurrentBlock.RotateClockWise();
            if(!DoesBlockFit())
            {
                CurrentBlock.RotateCounterClockwise();
            }
        }

        public void RotateCounterClockWise()
        {
            CurrentBlock.RotateCounterClockwise();
            if (!DoesBlockFit())
            {
                CurrentBlock.RotateClockWise();
            }
        }

        public void MoveLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!DoesBlockFit())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveRight()
        {
            CurrentBlock.Move(0, 1);
            if (!DoesBlockFit())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        public void MoveDown()
        {
            CurrentBlock.Move(1, 0);
            if (!DoesBlockFit())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }


        public void HoldBlock()
        {
            if (!Canhold)
            {
                return;
            }

            if(HeldBlock ==  null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else
            {
                var temp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = temp;
            }
            Canhold = false;
        }

        private void PlaceBlock()
        {
            foreach(var position in CurrentBlock.TilePositions())
            {
                GameGrid[position.Row, position.Column] = CurrentBlock.Id;
            }
            
            Score += GameGrid.ClearFullRows();

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
                Canhold = true;
            }
        }

        private int TileDropDistance(Position position)
        {
            int drop = 0;
            while(GameGrid.IsEmpty(position.Row + drop +1, position.Column))
            {
                drop++;
            }
            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach(var position in CurrentBlock.TilePositions())
            {
                drop = Math.Min(drop, TileDropDistance(position));
            }

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
