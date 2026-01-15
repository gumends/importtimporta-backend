using Domain.Entities;
using Domain.Models.carrinho;
using Domain.Models.Endereco;
using Domain.Models.Imagem;
using Domain.Models.Produto;
using Domain.Models.UserMenu;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts)
            : base(opts)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Informacoes> Informacoes_Produtos => Set<Informacoes>();
        public DbSet<Imagem> Imagens => Set<Imagem>();
        public DbSet<Endereco> Enderecos => Set<Endereco>();
        public DbSet<UserMenu> Menus => Set<UserMenu>();
        public DbSet<Carrinho> Carrinho  => Set<Carrinho>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Senha);

                entity.Property(x => x.Nascimento)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .IsRequired();

                entity.Property(x => x.Acesso)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .IsRequired();
            });


            modelBuilder.Entity<Endereco>(entity =>
            {
                entity.ToTable("enderecos");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Cep);

                entity.Property(x => x.Numero);

                entity.Property(x => x.Complemento)
                    .HasMaxLength(200);

                entity.Property(x => x.IdUsuario);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Produto>(entity =>
            {
                entity.ToTable("produtos");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.NomeProduto)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Valor);
                entity.Property(x => x.ValorOriginal).IsRequired();
                entity.Property(x => x.ValorParcelado).IsRequired();
                entity.Property(x => x.Desconto).IsRequired();
                entity.Property(x => x.Quantidade).IsRequired();
                
                entity.Property(x => x.Descricao)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(x => x.TipoProduto).IsRequired();
                entity.Property(x => x.NovoLancamento).IsRequired();
                entity.Property(x => x.Disponivel).IsRequired();
                entity.Property(x => x.MesesGarantia).IsRequired();

                entity.Property(x => x.Color)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.ColorName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasOne(p => p.InformacoesAdicionais)
                    .WithOne()
                    .HasForeignKey<Produto>(p => p.InformacoesAdicionaisId)
                    .OnDelete(DeleteBehavior.Cascade);
            });



            modelBuilder.Entity<Informacoes>(entity =>
            {
                entity.ToTable("informacoes_produtos");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Marca)
                    .HasMaxLength(100);
                entity.Property(x => x.ArmazenamentoInterno)
                    .HasMaxLength(100);
                entity.Property(x => x.TipoTela)
                    .HasMaxLength(100);
                entity.Property(x => x.TamanhoTela)
                    .HasMaxLength(100);
                entity.Property(x => x.ResolucaoTela)
                    .HasMaxLength(100);
                entity.Property(x => x.Tecnologia)
                    .HasMaxLength(100);
                entity.Property(x => x.Processador)
                    .HasMaxLength(100);
                entity.Property(x => x.SistemaOperacional)
                    .HasMaxLength(100);
                entity.Property(x => x.CameraTraseira)
                    .HasMaxLength(100);
                entity.Property(x => x.CameraFrontal)
                    .HasMaxLength(100);
                entity.Property(x => x.Bateria)
                    .HasMaxLength(100);
                entity.Property(x => x.QuantidadeChips)
                    .HasMaxLength(100);
                entity.Property(x => x.Material)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Imagem>(entity =>
            {
                entity.ToTable("imagens");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Caminho)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(i => i.Descricao)
                    .HasMaxLength(500);

                entity.HasOne(i => i.Produto)
                    .WithMany(p => p.Imagens)
                    .HasForeignKey(i => i.ProdutoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserMenu>(entity =>
            {
                entity.ToTable("menus");
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(i => i.Link)
                    .HasMaxLength(500);

                entity.Property(i => i.Icon)
                    .HasMaxLength(50);

                entity.Property(i => i.Role)
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Carrinho>(entity =>
            {
                entity.ToTable("carrinho");
                
                entity.HasKey(i => i.Id);

                entity.Property(i => i.IdProduto)
                    .IsRequired();
                entity.Property(i => i.IdUsuario)
                    .IsRequired();
                entity.Property(i => i.Quantidade)
                    .IsRequired();
                entity.Property(i => i.CriadoEm)
                    .IsRequired();
            });
        }
    }
}