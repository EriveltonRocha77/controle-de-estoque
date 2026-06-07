namespace ControleEstoque.Models
{
    public class Produto
    {
        public string IdProduto { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Quantidade { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Localizacao { get; set; } = string.Empty;
    }
}
