using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Blocks
{
    public abstract class BaseBlock
    {
        public abstract int Id { get; }
        public abstract Position[][] Tiles { get; }
        public abstract Position StartOffset { get; }

        private int _rotationState;
        private Position _offset;

        public BaseBlock()
        {
            _offset = new Position(StartOffset.Row, StartOffset.Column);
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach (var tile in Tiles[_rotationState])
            {
                yield return new Position(tile.Row + _offset.Row, tile.Column + _offset.Column);
            }
        }

        public void RotateClockWise()
        {
            _rotationState = (_rotationState + 1) % Tiles.Length;
        }

        public void RotateCounterClockwise()
        {
            _rotationState = (_rotationState - 1 + Tiles.Length) % Tiles.Length;
        }

        public void Move(int rows, int columns)
        {
            _offset.Row += rows;
            _offset.Column += columns;
        }

        public void Reset()
        {
            _offset.Row = StartOffset.Row;
            _offset.Column = StartOffset.Column;
            _rotationState = 0;
        }
    }
}
