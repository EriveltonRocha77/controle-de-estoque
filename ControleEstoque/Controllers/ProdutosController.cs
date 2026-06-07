using ControleEstoque.Models;
using ControleEstoque.Models.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ControleEstoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        [HttpPost("Salvar")]
        public object Salvar([FromBody] Produto cadastro)
        {
            try
            {
                ProdutosRepository produtosRepo = new ProdutosRepository();
                produtosRepo.Salvar(cadastro);
                return new { Sucesso = true, Mensagem = "Produto gravado com sucesso!" };
            }
            catch (Exception ex)
            {
                return new { Sucesso = false, Mensagem = "Erro ao gravar produto: " + ex.Message };
            }
        }

        [HttpPost("Alterar")]
        public object Alterar([FromBody] Produto cadastro)
        {
            try
            {
                ProdutosRepository produtosRepo = new ProdutosRepository();
                produtosRepo.Salvar(cadastro);
                return new { Sucesso = true, Mensagem = "Produto alterado com sucesso!" };
            }
            catch (Exception ex)
            {
                return new { Sucesso = false, Mensagem = "Erro ao alterar produto: " + ex.Message };
            }
        }

        [HttpGet("Listar")]
        public object Listar()
        {
            List<Produto> listaProd = new List<Produto>();
            try
            {
                ProdutosRepository produtosRepo = new ProdutosRepository();
                listaProd = produtosRepo.Listar();
            }
            catch (Exception ex)
            {
                // Registra o erro se necessário
            }

            return listaProd;
        }

        [HttpDelete("Deletar")]
        public object Deletar(string IdProduto)
        {
            try
            {
                ProdutosRepository produtosRepo = new ProdutosRepository();
                bool retornoDelete = produtosRepo.Deletar(IdProduto);

                return retornoDelete;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpGet("GetProduto")]
        public object GetProduto(string IdProduto)
        {
            try
            {
                ProdutosRepository produtosRepo = new ProdutosRepository();
                var retorno = produtosRepo.GetProduto(IdProduto);
                return retorno;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost("AjustarEstoque")]
        public object AjustarEstoque(string IdProduto, int Diferenca)
        {
            try
            {
                ProdutosRepository produtosRepo = new ProdutosRepository();
                bool retorno = produtosRepo.AjustarEstoque(IdProduto, Diferenca);
                if (retorno)
                {
                    var prod = produtosRepo.GetProduto(IdProduto);
                    return new { Sucesso = true, Quantidade = prod.Quantidade, Mensagem = "Estoque ajustado com sucesso!" };
                }
                return new { Sucesso = false, Mensagem = "Produto não encontrado!" };
            }
            catch (Exception ex)
            {
                return new { Sucesso = false, Mensagem = "Erro ao ajustar estoque: " + ex.Message };
            }
        }
    }
}
