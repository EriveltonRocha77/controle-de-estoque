using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ControleEstoque.Models.Repository
{
    public class ProdutosRepository
    {
        private readonly string _dbPath;

        public ProdutosRepository()
        {
            // Estratégia híbrida: tenta encontrar a pasta do projeto subindo 3 níveis a partir do bin do Debug/Release,
            // garantindo que os arquivos texto fiquem visíveis na pasta do projeto no VS Code / Explorer.
            // Caso contrário, usa a pasta base de execução.
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            
            if (Directory.Exists(Path.Combine(projectRoot, "BancoDados")))
            {
                _dbPath = Path.Combine(projectRoot, "BancoDados", "produtos.txt");
            }
            else
            {
                _dbPath = Path.Combine(baseDir, "BancoDados", "produtos.txt");
            }

            GarantirArquivoExiste();
        }

        private void GarantirArquivoExiste()
        {
            var pasta = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrEmpty(pasta) && !Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }
            if (!File.Exists(_dbPath))
            {
                File.WriteAllText(_dbPath, string.Empty);
            }
        }

        public void Salvar(Produto produto)
        {
            // Se o ID for vazio, gera um novo ID único (GUID)
            if (string.IsNullOrEmpty(produto.IdProduto))
            {
                produto.IdProduto = Guid.NewGuid().ToString();
            }

            // Listamos todos os produtos cadastrados
            var listaProdutos = Listar();

            // Verificamos se já existe um item com o mesmo ID ou Código de Barras
            var item = listaProdutos.Where(t => t.IdProduto == produto.IdProduto || 
                                                (!string.IsNullOrEmpty(t.CodigoBarras) && t.CodigoBarras == produto.CodigoBarras)).FirstOrDefault();

            // Se existir, nós removemos para poder sobrescrever com as novas informações (Upsert)
            if (item != null)
            {
                Deletar(item.IdProduto);
            }

            var produtoTexto = JsonConvert.SerializeObject(produto) + "," + Environment.NewLine;
            File.AppendAllText(_dbPath, produtoTexto);
        }

        public List<Produto> Listar()
        {
            GarantirArquivoExiste();
            var produtos = File.ReadAllText(_dbPath).Trim();

            if (string.IsNullOrEmpty(produtos))
            {
                return new List<Produto>();
            }

            // Remove a vírgula final se houver
            if (produtos.EndsWith(","))
            {
                produtos = produtos.Substring(0, produtos.Length - 1);
            }

            try
            {
                List<Produto> produtosLista = JsonConvert.DeserializeObject<List<Produto>>("[" + produtos + "]");
                return produtosLista.OrderBy(t => t.Nome).ToList();
            }
            catch
            {
                return new List<Produto>();
            }
        }

        public bool Deletar(string idProduto)
        {
            var listaProdutos = Listar();
            var item = listaProdutos.Where(t => t.IdProduto == idProduto).FirstOrDefault();

            if (item != null)
            {
                listaProdutos.Remove(item);

                // LIMPAMOS O BANCO DE DADOS
                File.WriteAllText(_dbPath, string.Empty);

                // ESCREVER A LISTA SEM O ITEM DELETADO
                foreach (var prod in listaProdutos)
                {
                    Salvar(prod);
                }

                return true;
            }

            return false;
        }

        public Produto GetProduto(string idProduto)
        {
            var listaProdutos = Listar();
            var item = listaProdutos.Where(t => t.IdProduto == idProduto).FirstOrDefault();

            return item;
        }

        public bool AjustarEstoque(string idProduto, int diferenca)
        {
            var listaProdutos = Listar();
            var item = listaProdutos.Where(t => t.IdProduto == idProduto).FirstOrDefault();

            if (item != null)
            {
                // Evita que a quantidade no estoque fique negativa
                item.Quantidade = Math.Max(0, item.Quantidade + diferenca);
                Salvar(item);
                return true;
            }

            return false;
        }
    }
}
