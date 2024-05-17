// -----------------------------------------------------------------------
// <file>Produto.cs</file>
// <author>edutechy</author>
// <date>2024-05-15</date>
// <description>
// Esta classe representa um produto na aplicação de loja virtual.
// Cada produto tem um código único, nome, descrição, preço e quantidade 
// disponível em inventário.
// A classe fornece métodos para adicionar produtos ao inventário, atualizar 
// a quantidade disponível e listar os detalhes de um produto.
//
// Principais Funcionalidades:
// - Adicionar novo produto ao inventário.
// - Atualizar quantidade disponível de um produto.
// - Listar detalhes de um produto.
//
// Esta classe encapsula os dados e comportamentos relacionados a um produto
// na loja virtual, seguindo os princípios de encapsulamento e modularidade
// da programação orientada a objetos.
// </description>
// -----------------------------------------------------------------------

namespace Sistema_Loja_Virtual
{
    public class Produto
    {
        public long Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public double Preco { get; set; }
        public long Disponiveis { get; set; }

        public Produto(long codigo, string nome, string descricao, double preco, long disponiveis)
        {
            Codigo = codigo;
            Nome = nome;
            Descricao = descricao;
            Preco = preco;
            Disponiveis = disponiveis;
        }

        public void AtualizarDisponiveis(int quantidade)
        {
            Disponiveis += quantidade;
        }
    }
}
