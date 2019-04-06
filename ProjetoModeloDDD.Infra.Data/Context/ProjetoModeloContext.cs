using ProjetoModeloDDD.Domain.Entities;
using ProjetoModeloDDD.Infra.Data.EntityConfig;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace ProjetoModeloDDD.Infra.Data.Context
{
    public class ProjetoModeloContext : DbContext
    {
        public ProjetoModeloContext()
            : base("ProjetoModeloDDD")
        {

        }   

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); //Remove convenção que automaticamente coloca o nome das tabelas no plural
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>(); //Remove convenção de deletar por cascata quando tiver relações um para muitos
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Properties()
                .Where(p => p.Name == p.ReflectedType.Name + "Id")
                .Configure(p => p.IsKey()); //Quando houver alguma propriedade que o nome dela é a classe + Id, automaticamente essa propriedade é a chave primária

            modelBuilder.Properties<string>()
                .Configure(p => p.HasColumnType("varchar")); //Sempre que o campo for string, cria no banco de dados como varchar como default

            modelBuilder.Properties<string>()
                .Configure(p => p.HasMaxLength(200)); //Seta os campos varchar como MaxLength = 200 como default - pode ser alterado de acordo com a necessidade de cada campo 

            modelBuilder.Configurations.Add(new ClienteConfiguration());
            modelBuilder.Configurations.Add(new ProdutoConfiguration());
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            return base.SaveChanges();
        }
    }
}
