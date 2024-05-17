// -----------------------------------------------------------------------
// <file>Cliente.cs</file>
// <author>edutechy</author>
// <date>2024-05-15</date>
// <description>
// Esta classe representa um cliente na aplicação de loja virtual.
// Cada cliente é identificado por um email único e pode ter um nome associado.
// A classe contém métodos para registrar um novo cliente e fazer a gestão do 
// carrinho de compras do cliente.
//
// Principais Funcionalidades:
// - Registo de um novo cliente com email e nome.
// - Adicionar produtos ao carrinho de compras do cliente.
// - Listar produtos no carrinho de compras.
// - Processar pagamentos e fechar compras no carrinho.
//
// Esta classe encapsula os dados e comportamentos relacionados a um cliente
// na loja virtual, seguindo os princípios de encapsulamento e modularidade
// da programação orientada a objetos.
// </description>
// -----------------------------------------------------------------------



using System;
using System.Text;
using System.Security.Cryptography;

using Microsoft.Data.Sqlite;
using System.Data.SQLite;

namespace Loja_Virtual
{
    public class Cliente
    {
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Email { get; set; }
        public string MeioDePagamento { get; set; } // Novo campo
        public string NumeroCartaoCredito { get; set; } // Número do Cartão de crédito
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

        public void ListarCarrinhoDeCompras()
        {
            Console.WriteLine("Produtos no Carrinho de Compras:");
            if (CarrinhoDeCompras.Count == 0)
            {
                Console.WriteLine("O carrinho de compras está vazio.");
            }
            else
            {
                foreach (var produto in CarrinhoDeCompras)
                {
                    Console.WriteLine($"Código: {produto.Codigo}, Nome: {produto.Nome}, Preço: {produto.Preco:C}");
                }
            }
        }

    }
 }
