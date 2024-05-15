
namespace Biblioteca_Loja_Virtual
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
