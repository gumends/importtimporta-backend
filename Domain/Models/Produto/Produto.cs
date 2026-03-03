#nullable enable
using Domain.Entities;

namespace Domain.Models.Produto
{
    public class Produto
    {
        public Guid Id { get; private set; }
        public string NomeProduto { get; private set; }
        public decimal ValorOriginal { get; private set; }
        public decimal Desconto { get; private set; }
        public decimal? Valor { get; private set; }
        public decimal ValorParcelado { get; private set; }
        public string Descricao { get; private set; }
        public int TipoProduto { get; private set; }
        public bool Disponivel { get; private set; }
        public int MesesGarantia { get; private set; }
        public int Quantidade { get; private set; }
        public string Color { get; private set; }
        public string ColorName { get; private set; }
        public ICollection<Imagem.Imagem> Imagens { get; set; } = new List<Imagem.Imagem>();
        public Guid InformacoesProdutoId { get; set; }
        public InformacoesProduto InformacoesProduto { get; private set; }
        protected Produto() { }

        public Produto(
            string nomeProduto,
            decimal valorOriginal,
            decimal desconto,
            decimal valorParcelado,
            string descricao,
            int tipoProduto,
            int mesesGarantia,
            int quantidade,
            string color,
            string colorName,
            InformacoesProduto informacoesProduto
            )
        {
            ValidarNome(nomeProduto);
            ValidarValor(valorOriginal);
            ValidarDesconto(desconto, valorOriginal);
            ValidarQuantidade(quantidade);
            ValidarGarantia(mesesGarantia);

            Id = Guid.NewGuid();
            NomeProduto = nomeProduto;
            ValorOriginal = valorOriginal;
            Desconto = desconto;
            ValorParcelado = valorParcelado;
            Descricao = descricao;
            TipoProduto = tipoProduto;
            MesesGarantia = mesesGarantia;
            Quantidade = quantidade;
            Color = color;
            ColorName = colorName;
            Disponivel = quantidade > 0;

            InformacoesProduto = informacoesProduto;

            CalcularValores();
        }

        public void AtualizarDados(
            string nomeProduto,
            string descricao,
            int tipoProduto,
            string color,
            string colorName)
        {
            ValidarNome(nomeProduto);

            NomeProduto = nomeProduto;
            Descricao = descricao;
            TipoProduto = tipoProduto;
            Color = color;
            ColorName = colorName;
        }

        public void AtualizarPreco(decimal valorOriginal, decimal desconto)
        {
            ValidarValor(valorOriginal);
            ValidarDesconto(desconto, valorOriginal);

            ValorOriginal = valorOriginal;
            Desconto = desconto;

            CalcularValores();
        }

        public void AtualizarEstoque(int quantidade)
        {
            ValidarQuantidade(quantidade);

            Quantidade = quantidade;
            Disponivel = quantidade > 0;
        }

        public void ReduzirEstoque(int quantidade)
        {
            if (quantidade <= 0)
                throw new BadRequestException("Quantidade inválida.");

            if (quantidade > Quantidade)
                throw new BadRequestException("Estoque insuficiente.");

            Quantidade -= quantidade;

            if (Quantidade == 0)
                Disponivel = false;
        }

        public void Ativar()
        {
            Disponivel = true;
        }

        public void Desativar()
        {
            Disponivel = false;
        }

        public void AdicionarImagem(Imagem.Imagem imagem)
        {
            if (imagem == null)
                throw new BadRequestException("Imagem inválida.");

            Imagens.Add(imagem);
        }

        public void RemoverImagem(Guid imagemId)
        {
            var imagem = Imagens.FirstOrDefault(i => i.Id == imagemId);

            if (imagem == null)
                throw new BadRequestException("Imagem não encontrada.");

            Imagens.Remove(imagem);
        }

        private void CalcularValores()
        {
            Valor = ValorOriginal - Desconto;
        }

        private void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new BadRequestException("Nome do produto é obrigatório.");
        }

        private void ValidarValor(decimal valor)
        {
            if (valor <= 0)
                throw new BadRequestException("Valor original deve ser maior que zero.");
        }

        private void ValidarDesconto(decimal desconto, decimal valorOriginal)
        {
            if (desconto < 0)
                throw new BadRequestException("Desconto não pode ser negativo.");

            if (desconto > valorOriginal)
                throw new BadRequestException("Desconto não pode ser maior que o valor original.");
        }

        private void ValidarQuantidade(int quantidade)
        {
            if (quantidade < 0)
                throw new BadRequestException("Quantidade não pode ser negativa.");
        }

        private void ValidarGarantia(int meses)
        {
            if (meses < 0)
                throw new BadRequestException("Garantia inválida.");
        }
    }
}