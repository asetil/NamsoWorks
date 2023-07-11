using Aware.Data;
using Cinescope.Web.Data;
using Cinescope.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinescope.Data
{
    public class CinescopeDbContext : AwareLiteDbContext<CinescopeDbContext>
    {
        public CinescopeDbContext(DbContextOptions<CinescopeDbContext> options) : base(options)
        {
        }

        public DbSet<Film> Films { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerFilm> PlayerFilms { get; set; }

        //public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SeedData.Seed(modelBuilder);

            //Apply outer configuration
            //modelBuilder.ApplyConfiguration(new CampaignConfigurations());

            //modelBuilder.Entity<Order>().HasKey("Order_Id_Os"); //Primary key
            //modelBuilder.Entity<Order>().HasKey(o => new { o.Order_ID, o.OwnerId }); //Composite primary key

            //modelBuilder.Entity<OrderItem>().ToTable("tbl_OrderItems");
            //modelBuilder.Entity<OrderItem>().HasKey(k => k.Order_Id);
            //modelBuilder.Entity<OrderItem>().Property(p => p.Name).IsRequired().HasMaxLength(200);
            //modelBuilder.Entity<OrderItem>().Property(p => p.UnitPrice).HasColumnName("Unit_Price");
            //modelBuilder.Entity<OrderItem>().Ignore(p => p.Price); //NotMapped
            //modelBuilder.Entity<OrderItem>().HasOne(p => p.Order).WithMany().HasForeignKey(p => p.Order_Id);

        }
    }
}
