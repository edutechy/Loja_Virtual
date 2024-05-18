// -----------------------------------------------------------------------
// <file>Program.cs</file>
// <author>edutechy</author>
// <date>2024-05-15</date>
// <description>
// Este ficheiro contém o programa principal para a aplicação da loja virtual.
// O programa permite o registo de clientes, adicionar produtos, a gestão do 
// carrinho de compras e processar transações de compra. 
//
// Funcionalidades principais:
// - Registo novos clientes.
// - Adicionar novos produtos ao catálogo da loja.
// - Listar catálogo de produtos disponíveis.
// - Adicionar produtos ao carrinho de compras do cliente.
// - Processar o pagamento dos itens no carrinho de compras.
// - Atualizar o catálogo de produtos após a compra.
//
// O programa utiliza uma base de dados SQLite para armazenar as informações
// de clientes, produtos e transações. Inclui funcionalidades para
// ler e escrever dados na base de dados, bem como para a gestão das
// operações básicas de uma loja virtual.
//
// Este código demonstra conceitos fundamentais da programação orientada a
// objetos, como classes, objetos, métodos e interação com base de dados.
// </description>
// -----------------------------------------------------------------------


using Loja_Virtual;
using System.Globalization;

class Program
{
    static void Main(string[] args)
    {
        LojaVirtual loja = new LojaVirtual();
        bool sair = false;

        while (!sair)
        {
            Console.WriteLine("\nLoja Virtual - Menu");
            Console.WriteLine("1. Registar Produto");
            Console.WriteLine("2. Registar Cliente");
            Console.WriteLine("3. Adicionar Produto ao Carrinho");
            Console.WriteLine("4. Remover Produto do Carrinho");
            Console.WriteLine("5. Finalizar Compra");
            Console.WriteLine("6. Listar Produtos");
            Console.WriteLine("7. Listar Carrinho De Compras");
            Console.WriteLine("8. Sair");
            Console.Write("Escolha uma opção: ");

            string escolha = Console.ReadLine();

            switch (escolha)
            {
                // Registar produto
                case "1":
                    Console.Write("Código do Produto: ");
                    int codigo = int.Parse(Console.ReadLine());
                    Console.Write("Nome do Produto: ");
                    string nome = Console.ReadLine();
                    Console.Write("Descrição do Produto: ");
                    string descricao = Console.ReadLine();
                    bool flag = true;
                    double preco = 0;
                    while (flag)
                    { 
                        try
                        {
                            Console.Write("Preço do Produto: ");
                            preco = double.Parse(Console.ReadLine());
                            flag = false;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("Formato de preço inválido. Por favor, use um separador decimal adequado (',' ou '.').");
                            flag = true;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
                        }
                    }
                    Console.Write("Quantidade Disponível: ");
                    int disponiveis = int.Parse(Console.ReadLine());

                    Produto produto = new Produto(codigo, nome, descricao, preco, disponiveis);
                    loja.RegistarProduto(produto);
                    break;

                // Registar cliente
                case "2":
                    Console.Write("Nome do Cliente: ");
                    string nomeCliente = Console.ReadLine();
                    Console.Write("Endereço do Cliente: ");
                    string endereco = Console.ReadLine();
                    Console.Write("Email do Cliente: ");
                    string email = Console.ReadLine();
                    Console.Write("Meio de Pagamento (cartao de credito, moeda, mbway): ");
                    string meioDePagamento = Console.ReadLine();
                    string numeroCartaoCredito = "";
                    if (meioDePagamento.ToLower() == "cartao de credito")
                    {
                        Console.Write("Número do Cartão de Crédito: ");
                        numeroCartaoCredito = Console.ReadLine();  
                        // TODO: Encriptação do número de crédito para armazenamento na base de dados 
                    }

                    Cliente cliente = new Cliente(nomeCliente, endereco, email, meioDePagamento, numeroCartaoCredito);
                    loja.RegistarCliente(cliente, numeroCartaoCredito);
                    break;

                // Adicionar compra ao carrinho de compras
                case "3":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
                    loja.CarregarCarrinhoDeComprasNaoFechadas(cliente);
                    if (cliente != null)
                    {
                        Console.Write("Código do Produto: ");
                        codigo = int.Parse(Console.ReadLine());
                        produto = loja.ObterProdutoPorCodigo(codigo);
                        //Console.WriteLine("Produto disponiveis: " + produto.Disponiveis + " Código: " + codigo);
                        if (produto != null && produto.Disponiveis > 0)
                        {
                            cliente.AdicionarAoCarrinho(produto);
                            cliente.ListarCarrinhoDeCompras();
                            loja.RegistoDeCompra(cliente, produto);
                            Console.WriteLine("Produto adicionado ao carrinho.");
                        }
                        else
                        {
                            Console.WriteLine("Produto não encontrado ou não há disponibilidade.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cliente não encontrado.");
                    }
                    break;

                // Remover produto do carrinho de compras
                case "4":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
                    loja.CarregarCarrinhoDeComprasNaoFechadas(cliente);
                    if (cliente != null)
                    {
                        Console.Write("Código do Produto: ");
                        codigo = int.Parse(Console.ReadLine());
                        produto = loja.ObterProdutoPorCodigo(codigo);
                        if (produto != null)
                        {
                            cliente.RemoverDoCarrinho(produto);
                            Console.WriteLine("Produto removido do carrinho.");
                        }
                        else
                        {
                            Console.WriteLine("Produto não encontrado.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cliente não encontrado.");
                    }
                    break;

                // Finalização da compra
                case "5":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
                    loja.CarregarCarrinhoDeComprasNaoFechadas(cliente);
                    if (cliente != null)
                    {
                        double total = cliente.CalcularTotal();
                        loja.ProcessarVenda(cliente);
                        Console.WriteLine($"Compra finalizada. Total: {total:C2}");
                    }
                    else
                    {
                        Console.WriteLine("Cliente não encontrado.");
                    }
                    break;

                // Listar todos os produtos da loja
                case "6":
                    loja.ListarProdutos();
                    break;

                // Listar carrinho de compras de um cliente
                case "7":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
                    loja.CarregarCarrinhoDeComprasNaoFechadas(cliente);
                    if (cliente != null)
                    {
                        cliente.ListarCarrinhoDeCompras();
                    }
                    else
                    {
                        Console.WriteLine("Cliente não encontrado.");
                    }
                    break;
   
                // Sair do programa
                case "8":
                    sair = true;
                    break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }
}


