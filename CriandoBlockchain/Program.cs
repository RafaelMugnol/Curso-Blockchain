using System;

namespace CriandoBlockchain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Início");

            var blockchain = Chain.GetInstance();
            var data = string.Empty;

            while (data != "exit")
            {
                Console.Write("Digite o dado: ");
                data = "asdasd";

                var tempoInicial = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                var block = blockchain.NewBlock(data);
                var tempoGeracao = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() - tempoInicial;

                Console.WriteLine("Bloco gerado: " + block.Hash);
                Console.WriteLine("Indíce: " + block.Index);
                Console.WriteLine("Dificuldade: " + block.Difficulty);
                Console.WriteLine("Nonce: " + block.Nonce);
                Console.WriteLine("Tempo: " + tempoGeracao);
                Console.WriteLine("");
            }

            Console.WriteLine("Fim");
        }
    }
}
