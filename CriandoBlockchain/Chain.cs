using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CriandoBlockchain
{
    class Chain
    {
        // Lista de blocos gerados
        public List<Block> ChainList { get; set; }

        // Intervalo de tempo para gerar o bloco (segundos)
        const int BlockGenerationIntervalInSeconds = 5;

        // Intervalo de blocos para a atulização da dificuldade
        const int DifficultyAdjustmentIntervalInBlocks = 5;

        #region Constructor
        // Padrão Singleton
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
        #endregion

        // Genesis é o bloco 0
        private Block GetGenesisBlock() => new Block
        {
            Index = 0,
            Hash = "213123123123",
            PreviousHash = "",
            Timestamp = 1465154705,
            Data = "bloco 0",
            Difficulty = 0,
            Nonce = 0
        };

        public Block GetLatestBlock() => this.ChainList.Last();

        public Block NewBlock(string data)
        {
            var prevBlock = GetLatestBlock();
            var newIndex = prevBlock.Index + 1;
            var prevHash = prevBlock.Hash;
            var newTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            var difficulty = GetDifficulty();
            var newBlock = FindBlock(newIndex, prevHash, newTimestamp, data, difficulty);

            if (AddBlock(newBlock))
            {
                return newBlock;
            }
            else
            {
                return null;
            }
        }

        // Verifica para atulizar a dificuldade conforme a constante difinida acima
        public int GetDifficulty()
        {
            var latestBlock = GetLatestBlock();

            if (latestBlock.Index % DifficultyAdjustmentIntervalInBlocks == 0 && latestBlock.Index != 0)
                return GetAdjustedDifficulty(latestBlock);

            else
                return latestBlock.Difficulty;
        }

        public int GetAdjustedDifficulty(Block latestBlock)
        {
            var prevAdjustmentBlock = this.ChainList[this.ChainList.Count() - DifficultyAdjustmentIntervalInBlocks];
            var timeExpected = BlockGenerationIntervalInSeconds * DifficultyAdjustmentIntervalInBlocks; // 25
            var timeTaken = (latestBlock.Timestamp - prevAdjustmentBlock.Timestamp) / 1000; // tempo que demorou para gerar os 5 últimos

            if (timeTaken < (timeExpected / 2))
                return prevAdjustmentBlock.Difficulty + 1;

            else if (timeTaken > (timeExpected * 2))
                return prevAdjustmentBlock.Difficulty - 1;

            else
                return prevAdjustmentBlock.Difficulty;
        }

        // Monta um bloco válido
        public Block FindBlock(int index, string previousHash, long timestamp, string data, int difficulty)
        {
            var nonce = 0;
            while (true)
            {
                // Concatena todos o dados para gerar o hash
                var concatenatedObj = string.Concat(
                    index.ToString(),
                    previousHash,
                    timestamp.ToString(),
                    data,
                    nonce.ToString(),
                    difficulty.ToString()
                );

                var hash = Sha256(concatenatedObj);
                if (HashMatchesDifficulty(hash, difficulty))
                {
                    return new Block
                    {
                        Index = index,
                        Timestamp = timestamp,
                        PreviousHash = previousHash,
                        Data = data,
                        Difficulty = difficulty,
                        Nonce = nonce,
                        Hash = hash
                    };
                };

                nonce++;
            }
        }

        // Valida se o hash gerado inicia com a quantidade de "0" conforme a dificulade definida
        public bool HashMatchesDifficulty(string hash, int difficulty)
        {
            var prefix = string.Concat(Enumerable.Repeat("0", difficulty));
            return hash.StartsWith(prefix);
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

        public bool AddBlock(Block block)
        {
            if (IsValidBlock(GetLatestBlock(), block))
            {
                this.ChainList.Add(block);
                return true;
            }

            return false;
        }

        // Somente validações para garantir a veracidade (nesse caso, como o bloco está sendo gerado aqui, será sempre válido)
        public bool IsValidBlock(Block latestBlock, Block block)
        {
            try
            {
                if (latestBlock.Index + 1 != block.Index ||
                    latestBlock.Hash != block.PreviousHash ||
                    !IsValidTimestamp(latestBlock, block))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }

        // Somente para validar se o bloco não é muito novo ou muito velho
        private bool IsValidTimestamp(Block prevBlock, Block newBlock)
        {
            if (prevBlock.Index == 0)
                return true;

            return prevBlock.Timestamp - 60000 < newBlock.Timestamp &&
                newBlock.Timestamp - 60000 < new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }
    }
}
