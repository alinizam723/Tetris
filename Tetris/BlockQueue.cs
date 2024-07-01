using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Blocks;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Random _random = new Random();

        private readonly BaseBlock[] _blocks = new BaseBlock[]
        {
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        public BaseBlock NextBlock { get; private set; }
        public BlockQueue()
        {
            NextBlock = GetRandomBlock();
        }

        public BaseBlock GetAndUpdate()
        {
            var block = NextBlock;
            do
            {
                NextBlock = GetRandomBlock();
            } 
            while (block.Id == NextBlock.Id);

            return block;
        }

        private BaseBlock GetRandomBlock()
        {
            return _blocks[_random.Next(_blocks.Length)];
        }
    }
}
