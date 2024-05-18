// -----------------------------------------------------------------------
// <file>LojaVirtual.cs</file>
// <date>2024-05-15</date>
// <description>
// Esta classe representa a aplicação de loja virtual, que faz a gestão de clientes,
// produtos e transações de compra.
// A classe contém métodos para interagir com a base de dados SQLite, 
// registo de novos clientes, adicionar produtos ao catálogo de produtos, gerir o
// carrinho de compras e processar transações de compra.
//
// Principais Funcionalidades:
// - Registo de novos clientes.
// - Adicionar novos produtos ao catálogo.
// - Listar produtos disponíveis.
// - Adicionar produtos ao carrinho de compras do cliente.
// - Processar pagamentos e fechar compras.
//
// Esta classe serve como ponto de entrada para a aplicação de loja virtual,
// coordenando as interações entre clientes, produtos e transações.
// </description>
// -----------------------------------------------------------------------

using System;
using System.Security.Cryptography;
using System.Text;

using System.Data.SQLite;
using System.Data.SqlTypes;

namespace Loja_Virtual
{
    public class LojaVirtual
    {
        private string connectionString = "Data Source=loja_virtual.db";

        // Construtor da classe Loja_Virtual
        public LojaVirtual()
        {
            CriarBaseDeDados();
        }

        // Método para criar a base de dados e as tabelas produtos, clientes e carrinhodecompras.
        private void CriarBaseDeDados()
        {
            try
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
                    using (var cmd = new SQLiteCommand(criarProdutos, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string criarClientes = @"CREATE TABLE IF NOT EXISTS clientes (
                                    Email TEXT PRIMARY KEY,
                                    Nome TEXT,
                                    Endereco TEXT,
                                    MeioDePagamento TEXT,
                                    NumeroCartaoCredito TEXT)";
                    using (var cmd = new SQLiteCommand(criarClientes, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string criarCarrinho = @"CREATE TABLE IF NOT EXISTS carrinhodecompras (
                                    Email TEXT,
                                    CodigoProduto INTEGER,
                                    Quantidade INTEGER,
                                    CompraFechada BOOLEAN DEFAULT 0, -- Novo campo para determinar se a compra foi fechada
                                    FOREIGN KEY(Email) REFERENCES clientes(Email),
                                    FOREIGN KEY(CodigoProduto) REFERENCES produtos(Codigo))";
                    using (var cmd = new SQLiteCommand(criarCarrinho, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar a base de dados: {ex.Message}");
            }
        }

        // Método para registo de um produto na tabela produtos
        public void RegistarProduto(Produto produto)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    try 
                    { 

                        string numeroMaximoProdutos = @"SELECT MAX(Codigo) FROM produtos"; // Lê o numero máximo de produtos em stock
                        var cmd = new SQLiteCommand(numeroMaximoProdutos, connection);
                        var reader = cmd.ExecuteReader();

                        long codigoMax = 0;
                        // A condição do if é verdadeira quando o comando executado retornar dados
                        if (reader.Read())
                        {
                            // Na primeira vez a tabela não tem dados então temos de despitar
                            // que o retorno não é null.
                            // Se for então significa que estamos a inserir o primeiro produto na tabela.
                            // Nesse caso o codogoMax=0+1 (ver mais abaixo)
                            if (!reader.IsDBNull(reader.GetOrdinal("MAX(Codigo)")))
                            {
                                codigoMax = (long)reader["MAX(Codigo)"];
                            }
                        }

                        // DEBUG: O campo código é chave primária e é necessário garantir que este não seja duplicado.
                        Console.WriteLine("O código do produto max:" + codigoMax);

                        string inserirProduto = @"INSERT INTO produtos (Codigo, Nome, Descricao, Preco, Disponiveis) 
                                                   VALUES (@Codigo, @Nome, @Descricao, @Preco, @Disponiveis)";
                        cmd = new SQLiteCommand(inserirProduto, connection);
                        cmd.Parameters.AddWithValue("@Codigo", codigoMax + 1);
                        cmd.Parameters.AddWithValue("@Nome", produto.Nome);
                        cmd.Parameters.AddWithValue("@Descricao", produto.Descricao);
                        cmd.Parameters.AddWithValue("@Preco", produto.Preco);
                        cmd.Parameters.AddWithValue("@Disponiveis", produto.Disponiveis);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao inserir na tabela de produtos: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a ligação à base de dados: {ex.Message}");
            }
        }

        // Método para registo de um cliente na tabela clientes
        public void RegistarCliente(Cliente cliente, string numeroCartaoCredito)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao inserir na tabela de clientes: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a conexão com a base de dados: {ex.Message}");
            }
        }

        // Método para registo de uma compra na tabela carrinhodecompras
        public void RegistoDeCompra(Cliente cliente, Produto produto)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
                        string inserirCarrinhoDeCompras = @"INSERT INTO carrinhodecompras (Email, CodigoProduto, Quantidade, CompraFechada) 
                                                    VALUES (@Email, @CodigoProduto, @Quantidade, @CompraFechada)";
                        using (var cmd = new SQLiteCommand(inserirCarrinhoDeCompras, connection))
                        {
                            cmd.Parameters.AddWithValue("@Email", cliente.Email);
                            cmd.Parameters.AddWithValue("@CodigoProduto", produto.Codigo);
                            cmd.Parameters.AddWithValue("@Quantidade", produto.Disponiveis);
                            cmd.Parameters.AddWithValue("@CompraFechada", 0); // 0 - Compra aberta

                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao inserir no carrinho de compras: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a conexão com a base de dados: {ex.Message}");
            }
        }

        // Método para listar todos os produtos da loja
        public void ListarProdutos()
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
                        string listarProdutos = "SELECT * FROM produtos";
                        using (var cmd = new SQLiteCommand(listarProdutos, connection))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Console.WriteLine($"Código: {reader["Codigo"]}, Nome: {reader["Nome"]}, Preço: {reader["Preco"]}, Disponíveis: {reader["Disponiveis"]}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao listar produtos: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a ligação com a base de dados: {ex.Message}");
            }
        }

        // Método para ler um produto da base de dados a partir do código do produto
        public Produto ObterProdutoPorCodigo(long codigo)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
                        string obterProduto = "SELECT * FROM produtos WHERE Codigo = @Codigo";
                        using (var cmd = new SQLiteCommand(obterProduto, connection))
                        {
                            cmd.Parameters.AddWithValue("@Codigo", codigo);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Console.WriteLine("Produto encontrado.");
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
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao ler o produto: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a ligação com a base de dados: {ex.Message}");
            }

            return null;
        }

        // Método para ler um cliente da base de dados a partir do email
        public Cliente ObterClientePorEmail(string email)
        {
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    try
                    {
                        // Consulta para ler um cliente apartir do email
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
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao ler o cliente: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a ligação com a base de dados: {ex.Message}");
            }
            return null;
        }

        // Método para carregar a lista CarrinhoDeCompras com as compras não fechadas
        public void CarregarCarrinhoDeComprasNaoFechadas(Cliente cliente)
        {
            cliente.CarrinhoDeCompras.Clear(); // Limpa a lista antes de carregar novos itens

            string consulta = "SELECT p.Codigo, p.Nome, p.Descricao, p.Preco, p.Disponiveis " +
                              "FROM carrinhodecompras c " +
                              "INNER JOIN produtos p ON c.CodigoProduto = p.Codigo " +
                              "WHERE c.Email = @Email AND c.CompraFechada = 0";
            try
            {
                using (var connection = new SQLiteConnection(connectionString))
                {
                    SQLiteCommand command = new SQLiteCommand(consulta, connection);

                    command.Parameters.AddWithValue("@Email", cliente.Email);

                    connection.Open();
                    try
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Produto produto = new Produto
                                (
                                    Convert.ToInt64(reader["Codigo"]),
                                    Convert.ToString(reader["Nome"]),
                                    Convert.ToString(reader["Descricao"]),
                                    Convert.ToDouble(reader["Preco"]),
                                    Convert.ToInt64(reader["Disponiveis"])
                                );
                                cliente.CarrinhoDeCompras.Add(produto);
                            }
                        }
                        // DEBUG: Old school
                        // Console.WriteLine(" TOTAL COMPRAS: " + cliente.CarrinhoDeCompras.Count());
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao ler as transações do carrinho de compras: {ex.Message}");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar o carrinho de compras: {ex.Message}");
            }
        }
      
        // Método para finalização de uma venda (transação).
        public void ProcessarVenda(Cliente cliente)
        {
          using (var connection = new SQLiteConnection(connectionString))
          {
            try
            {
                connection.Open();
                foreach (var produto in cliente.CarrinhoDeCompras)
                {
                    try
                    {
                        // Update product stock
                        string atualizarEstoque = @"UPDATE produtos 
                                                    SET Disponiveis = Disponiveis - 1 
                                                    WHERE Codigo = @Codigo";
                        using (var cmd = new SQLiteCommand(atualizarEstoque, connection))
                        {
                            cmd.Parameters.AddWithValue("@Codigo", produto.Codigo);
                            cmd.ExecuteNonQuery();
                        }

                        // Update shopping cart
                        string atualizaCarrinhoDeCompras = @"UPDATE carrinhodecompras 
                                                            SET CompraFechada = 1 
                                                            WHERE Email = @Email AND 
                                                                    CodigoProduto = @CodigoProduto AND 
                                                                    CompraFechada = 0";
                        using (var cmd = new SQLiteCommand(atualizaCarrinhoDeCompras, connection))
                        {
                            cmd.Parameters.AddWithValue("@Email", cliente.Email);
                            cmd.Parameters.AddWithValue("@CodigoProduto", produto.Codigo);

                            // DEBUG:
                            // foreach (SQLiteParameter param in cmd.Parameters)
                            // {
                            //     Console.WriteLine($"Parameter Name: {param.ParameterName}, Value: {param.Value}");
                            // }
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao atualizar o produto ou carrinho de compras: {ex.Message}");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir a conexão com o banco de dados: {ex.Message}");
            }

            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                        connection.Close();
                }
            }

            cliente.CarrinhoDeCompras.Clear();
           }
        }
    }
}