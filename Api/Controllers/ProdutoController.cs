using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Models.Endereco;
using Domain.Models.Produto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [ApiController]
    [Route("produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProdutoById([FromRoute] Guid id)
        {
            var produto = await _produtoService.ObterProdutoPorId(id);
            if (produto == null)
            {
                return NotFound();
            }
            return Ok(produto);
        }

        [HttpPost]
        public async Task<IActionResult> AdicionaProduto(
            [FromBody] ProdutoDto produto)
        {
            var novoProduto = await _produtoService.AdicionaProduto(produto);
            return CreatedAtAction(nameof(GetProdutoById), new { id = novoProduto.Id }, novoProduto);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AtualizaProduto(
            [FromRoute] Guid id,
            [FromForm] Produto produto,
            [FromForm] List<IFormFile>? imagens,
            [FromForm(Name = "imagensExistentes")] List<string>? imagensExistentes
        )
        {
            if (id != produto.Id)
                return BadRequest("Id do produto na rota não confere com o objeto enviado.");

            var produtoAtualizado = await _produtoService.AtualizaProduto(id, produto, imagens, imagensExistentes);

            return Ok(produtoAtualizado);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveProduto([FromRoute] Guid id)
        {
            var sucesso = await _produtoService.RemoveProduto(id);
            if (!sucesso)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> ListaProdutos(
            [FromQuery] int pagina = 1, 
            [FromQuery] int tamanhoPagina = 10,
            [FromQuery] decimal precoMinimo = 0,
            [FromQuery] decimal precoMaximo = 0,
            [FromQuery] string? nomeProduto = null)
        {
            var produtos = await _produtoService.ListaProdutos(pagina, tamanhoPagina, precoMinimo, precoMaximo, nomeProduto);
            return Ok(produtos);
        }

        [HttpGet("tipo/{tipoProduto}")]
        public async Task<IActionResult> ListaProdutosPorTipo([FromRoute] int tipoProduto, [FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
        {
            var produtos = await _produtoService.ListaProdutosPorTipo(tipoProduto, pagina, tamanhoPagina);
            return Ok(produtos);
        }

        [HttpGet("promocao")]
        public async Task<IActionResult> ListaProdutosPromocao([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
        {
            var produtos = await _produtoService.ListaProdutosPromocao(pagina, tamanhoPagina);
            return Ok(produtos);
        }

        [HttpPut("desativar/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DesativarProduto([FromRoute] Guid id)
        {
            var produto = await _produtoService.DesativarProduto(id);
            return Ok(produto);
        }
        
        [HttpPut("ativar/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AtivacaoProduto([FromRoute] Guid id)
        {
            var produto = await _produtoService.AtivarProduto(id);
            return Ok(produto);
        }

        [HttpGet("variados")]
        public async Task<IActionResult> BuscaProdutosVariados([FromQuery] int quantidade)
        {
            var produto = await _produtoService.BuscaProdutosVariados(quantidade);
            return Ok(produto);
        }
        
        [HttpPost("SalvarImagens")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SalvarImagens([FromForm] List<IFormFile> imagens, [FromQuery] Guid idProduto)
        {
            var resultado = await _produtoService.SalvarImagens(imagens, idProduto);
            return Ok(resultado);
        }
    }
}
