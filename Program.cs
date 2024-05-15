using Biblioteca_Loja_Virtual;

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
            Console.WriteLine("7. Sair");
            Console.Write("Escolha uma opção: ");

            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    Console.Write("Código do Produto: ");
                    int codigo = int.Parse(Console.ReadLine());
                    Console.Write("Nome do Produto: ");
                    string nome = Console.ReadLine();
                    Console.Write("Descrição do Produto: ");
                    string descricao = Console.ReadLine();
                    Console.Write("Preço do Produto: ");
                    double preco = double.Parse(Console.ReadLine());
                    Console.Write("Quantidade Disponível: ");
                    int disponiveis = int.Parse(Console.ReadLine());

                    Produto produto = new Produto(codigo, nome, descricao, preco, disponiveis);
                    loja.RegistarProduto(produto);
                    break;

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
                    }

                    Cliente cliente = new Cliente(nomeCliente, endereco, email, meioDePagamento, numeroCartaoCredito);
                    loja.RegistarCliente(cliente, numeroCartaoCredito);
                    break;

                // Adicionar compra ao carrinho de compras
                case "3":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
                    if (cliente != null)
                    {
                        Console.Write("Código do Produto: ");
                        codigo = int.Parse(Console.ReadLine());
                        produto = loja.ObterProdutoPorCodigo(codigo);
                        Console.WriteLine("Produto disponiveis: " + produto.Disponiveis + " Código: " + codigo);
                        if (produto != null ) // && produto.Disponiveis > 0)
                        {
                            cliente.AdicionarAoCarrinho(produto);
                            Console.WriteLine("Produto adicionado ao carrinho.");
                        }
                        else
                        {
                            Console.WriteLine("Produto não encontrado ou fora de estoque.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cliente não encontrado.");
                    }
                    break;

                case "4":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
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

                case "5":
                    Console.Write("Email do Cliente: ");
                    email = Console.ReadLine();
                    cliente = loja.ObterClientePorEmail(email);
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

                case "6":
                    loja.ListarProdutos();
                    break;

                case "7":
                    sair = true;
                    break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }
}


