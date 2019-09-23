using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CriandoBlockchain
{
    class Chain
    {
        public List<Block> ChainList { get; set; }

        private static Chain instance = null;

        public static Chain GetInstance()
        {
            if (instance == null)
                instance = new Chain();

            return instance;
        }

        private Chain()
        {
            var genesis = GetGenesisBlock();
            this.ChainList = new List<Block>();
            this.ChainList.Add(genesis);
        }

        private Block GetGenesisBlock() => new Block
        {
            Index = 0,
            Hash = "213123123123",
            PreviousHash = "",
            Timestamp = 1465154705,
            Data = "bloco 0"
        };

        public Block GetLatestBlock() => this.ChainList.Last();

        public void AddBlock(Block block)
        {
            this.ChainList.Add(block);
        }

        private string Sha256(string value)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var result = hash.ComputeHash(enc.GetBytes(value));

                foreach (var b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
