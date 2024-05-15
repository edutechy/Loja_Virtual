using System;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

using System.Data.SQLite;
using System.Data.SqlTypes;

namespace Biblioteca_Loja_Virtual
{
    public class LojaVirtual
    {
        private string connectionString = "Data Source=loja_virtual.db";

        public LojaVirtual()
        {
            CriarBaseDeDados();
        }

        private void CriarBaseDeDados()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string criarProdutos = @"CREATE TABLE IF NOT EXISTS produtos (
                                        Codigo INTEGER PRIMARY KEY,
                                        Nome TEXT,
                                        Descricao TEXT,
                                        Preco REAL,
                                        Disponiveis INTEGER)";
                var cmd = new SQLiteCommand(criarProdutos, connection);
                cmd.ExecuteNonQuery();

                string criarClientes = @"CREATE TABLE IF NOT EXISTS clientes (
                                        Email TEXT PRIMARY KEY,
                                        Nome TEXT,
                                        Endereco TEXT,
                                        MeioDePagamento TEXT,
                                        NumeroCartaoCredito TEXT)";
                cmd = new SQLiteCommand(criarClientes, connection);
                cmd.ExecuteNonQuery();

                string criarCarrinho = @"CREATE TABLE IF NOT EXISTS carrinhodecompra (
                                        Email TEXT,
                                        CodigoProduto INTEGER,
                                        Quantidade INTEGER,
                                        FOREIGN KEY(Email) REFERENCES clientes(Email),
                                        FOREIGN KEY(CodigoProduto) REFERENCES produtos(Codigo))";
                cmd = new SQLiteCommand(criarCarrinho, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public void RegistarProduto(Produto produto)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string inserirProduto = @"INSERT INTO produtos (Codigo, Nome, Descricao, Preco, Disponiveis) 
                                      VALUES (@Codigo, @Nome, @Descricao, @Preco, @Disponiveis)";
                var cmd = new SQLiteCommand(inserirProduto, connection);
                cmd.Parameters.AddWithValue("@Codigo", produto.Codigo);
                cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                cmd.Parameters.AddWithValue("@Descricao", produto.Descricao);
                cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                cmd.Parameters.AddWithValue("@Disponiveis", produto.Disponiveis);
                cmd.ExecuteNonQuery();
            }
        }

        public void RegistarCliente(Cliente cliente, string numeroCartaoCredito)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                cliente.SetNumeroCartaoCredito(numeroCartaoCredito);
                string inserirCliente = @"INSERT INTO clientes (Email, Nome, Endereco, MeioDePagamento, NumeroCartaoCredito) 
                                      VALUES (@Email, @Nome, @Endereco, @MeioDePagamento, @NumeroCartaoCredito)";
                var cmd = new SQLiteCommand(inserirCliente, connection);
                cmd.Parameters.AddWithValue("@Email", cliente.Email);
                cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@Endereco", cliente.Endereco);
                cmd.Parameters.AddWithValue("@MeioDePagamento", cliente.MeioDePagamento);
                cmd.Parameters.AddWithValue("@NumeroCartaoCredito", cliente.GetNumeroCartaoCredito());
                cmd.ExecuteNonQuery();
            }
        }

        public void ListarProdutos()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string listarProdutos = "SELECT * FROM produtos";
                var cmd = new SQLiteCommand(listarProdutos, connection);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Código: {reader["Codigo"]}, Nome: {reader["Nome"]}, Preço: {reader["Preco"]}, Disponíveis: {reader["Disponiveis"]}");
                    }
                }
            }
        }

        public Produto ObterProdutoPorCodigo(int codigo)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string obterProduto = "SELECT * FROM produtos WHERE Codigo = @Codigo";
                var cmd = new SQLiteCommand(obterProduto, connection);
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Produto(
                            (long)reader["Codigo"],
                            (string)reader["Nome"],
                            (string)reader["Descricao"],
                            (double)reader["Preco"],
                            (long)reader["Disponiveis"]
                        );
                    }
                }
            }
            return null;
        }

        public Cliente ObterClientePorEmail(string email)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string obterCliente = "SELECT * FROM clientes WHERE Email = @Email";
                var cmd = new SQLiteCommand(obterCliente, connection);
                cmd.Parameters.AddWithValue("@Email", email);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var cliente = new Cliente(
                            (string)reader["Nome"],
                            (string)reader["Endereco"],
                            (string)reader["Email"],
                            (string)reader["MeioDePagamento"],
                            (string)reader["NumeroCartaoCredito"]
                        );
                        cliente.SetNumeroCartaoCredito((string)reader["NumeroCartaoCredito"]);
                        return cliente;
                    }
                }
            }
            return null;
        }

        public void ProcessarVenda(Cliente cliente)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (var produto in cliente.CarrinhoDeCompras)
                {
                    string atualizarEstoque = @"UPDATE produtos 
                                            SET Disponiveis = Disponiveis - 1 
                                            WHERE Codigo = @Codigo";
                    var cmd = new SQLiteCommand(atualizarEstoque, connection);
                    cmd.Parameters.AddWithValue("@Codigo", produto.Codigo);
                    cmd.ExecuteNonQuery();
                }
                cliente.CarrinhoDeCompras.Clear();
            }
        }
    }

}
