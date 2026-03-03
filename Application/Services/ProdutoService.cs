using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.DTO;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IS3Repository _awsS3Service;

        public ProdutoService(
            IProdutoRepository produtoRepository,
            IS3Repository s3Service
        )
        {
            _produtoRepository = produtoRepository;
            _awsS3Service = s3Service;
        }

        public async Task<Produto> AdicionaProduto(ProdutoDto produto)
        {
            var novoProduto = await _produtoRepository.AdicionaProduto(produto);

            return novoProduto;
        }

        public async Task<Produto> AtualizaProduto(
            Guid id,
            Produto novosDados,
            List<IFormFile>? novasImagens,
            List<string>? imagensExistentes)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(id);

            if (produto == null)
                throw new Exception("Produto não encontrado.");

            produto.AtualizarDados(
                novosDados.NomeProduto,
                novosDados.Descricao,
                novosDados.TipoProduto,
                novosDados.Color,
                novosDados.ColorName);

            produto.AtualizarPreco(
                novosDados.ValorOriginal,
                novosDados.Desconto);

            produto.AtualizarEstoque(novosDados.Quantidade);

            if (!novosDados.Disponivel)
                produto.Desativar();
            
            var imagensAtuais = produto.Imagens.ToList();
            var caminhosMantidos = imagensExistentes ?? imagensAtuais.Select(i => i.Caminho).ToList();

            var imagensParaRemover = imagensAtuais
                .Where(i => !caminhosMantidos.Contains(i.Caminho))
                .ToList();

            foreach (var img in imagensParaRemover)
            {
                await _awsS3Service.DeleteAsync(img.Caminho);

                produto.RemoverImagem(img.Id);
                await _produtoRepository.RemoverImagem(img);
            }

            if (novasImagens != null && novasImagens.Any())
            {
                foreach (var arquivo in novasImagens)
                {
                    var caminhoS3 = await _awsS3Service.UploadAsync(arquivo);

                    var imagem = new Imagem(
                        caminhoS3,
                        $"Imagem do produto {produto.NomeProduto}",
                        produto.Id
                    );

                    produto.AdicionarImagem(imagem);
                    await _produtoRepository.AdicionarImagem(imagem);
                }
            }

            await _produtoRepository.AtualizaProduto(produto);

            return produto;
        }

        public async Task<Produto?> ObterProdutoPorId(Guid id)
        {
            return await _produtoRepository.ObterProdutoPorId(id);
        }


        public async Task<bool> RemoveProduto(Guid id)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(id);

            if (produto == null)
                return false;

            foreach (var item in produto.Imagens)
            {
                var uri = new Uri(item.Caminho);
                var chave = uri.AbsolutePath.TrimStart('/');

                await _awsS3Service.DeleteAsync(chave);
            }

            return await _produtoRepository.RemoveProduto(id);
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutos(int pagina, int tamanhoPagina, decimal? precoMinimo, decimal? precoMaximo, string? nomeProduto)
        {
            return await _produtoRepository.ListaProdutosPaginado(pagina, tamanhoPagina, precoMinimo, precoMaximo, nomeProduto);
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosPorTipo(int tipoProduto, int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosPorTipo(tipoProduto, pagina, tamanhoPagina);
        }

        public Task<Produto> AtualizarPreco(decimal valorOriginal, decimal desconto, int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosPromocao(int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosEmPromocao(pagina, tamanhoPagina);

        }

        public async Task<PaginacaoResultado<Produto>> ListaProdutosNovaGeracao(int pagina, int tamanhoPagina)
        {
            return await _produtoRepository.ListaProdutosPaginado(pagina, tamanhoPagina, 0, 0, "");
        }

        public async Task<Produto> DesativarProduto(Guid id)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(id);

            if (produto is null)
                throw new DirectoryNotFoundException();

            produto.Desativar();

            await _produtoRepository.AtualizaProduto(produto);

            return produto;
        }

        public async Task<Produto> AtivarProduto(Guid id)
        {
            var produto = await _produtoRepository.ObterProdutoPorId(id);

            if (produto is null)
                throw new DirectoryNotFoundException();

            produto.Ativar();

            await _produtoRepository.AtualizaProduto(produto);

            return produto;
        }
        
        public async Task<List<Produto>> BuscaProdutosVariados(int contidade)
        {
            var produtos = await _produtoRepository.ListaProdutos();

            var produtosAleatorios = produtos
                .OrderBy(p => Guid.NewGuid())
                .Take(contidade)
                .ToList();

            return produtosAleatorios;
        }
        
        public async Task<List<string>> SalvarImagens(List<IFormFile> imagens, Guid idProduto)
        {
            List<string> response = new List<string>();
            foreach (var imagen in imagens)
            {
                string link = await _awsS3Service.UploadAsync(imagen);
                
                var imagem = new Imagem(
                    link,
                    "Imagem do produto {produto.NomeProduto}",
                    idProduto);
                
                await _produtoRepository.AdicionarImagem(imagem);
                response.Add(link);
            }
            return response;
        }
    }
}
