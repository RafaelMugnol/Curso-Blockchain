
using System;
using System.Collections.Generic;
using System.Text;

namespace CriandoBlockchain
{
    class Block
    {
        public int Index { get; set; }
        public long Timestamp { get; set; }
        public string Data { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
    }
}
