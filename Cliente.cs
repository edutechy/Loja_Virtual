using System.Text;
using System;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace Biblioteca_Loja_Virtual
{
    public class Cliente
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Email { get; set; }
        public string MeioDePagamento { get; set; } // Novo campo
        public string NumeroCartaoCredito { get; set; } // Novo campo
        public List<Produto> CarrinhoDeCompras { get; set; }


        public Cliente(string nome, string endereco, string email, string meioDePagamento, string numeroCartaoCredito)
        {
            Nome = nome;
            Endereco = endereco;
            Email = email;
            MeioDePagamento = meioDePagamento;
            NumeroCartaoCredito = Encrypt(numeroCartaoCredito); // Encripta o número do cartão de crédito ao criar o cliente
      
            CarrinhoDeCompras = new List<Produto>(); // Inicializa a lista no construtor
    
        }
        public string GetNumeroCartaoCredito()
        {
            return NumeroCartaoCredito;
        }

        public void SetNumeroCartaoCredito(string numeroCartaoCredito)
        {
            this.NumeroCartaoCredito = numeroCartaoCredito;
        }
        // Método para encriptar o número do cartão de crédito
        private string Encrypt(string input)
        {
            // Implementar a lógica de encriptação aqui (pode-se usar bibliotecas de criptografia disponíveis em C#)
            return input; // Por enquanto, apenas retorna o número original (implementação simulada)
        }


            public void AdicionarAoCarrinho(Produto produto)
            {
                CarrinhoDeCompras.Add(produto);
            }

            public void RemoverDoCarrinho(Produto produto)
            {
                CarrinhoDeCompras.Remove(produto);
            }

            public double CalcularTotal()
            {
                double total = 0;

                foreach (var produto in CarrinhoDeCompras)
                {
                    total += produto.Preco;
                }
                return total;
            }
        }

    

}