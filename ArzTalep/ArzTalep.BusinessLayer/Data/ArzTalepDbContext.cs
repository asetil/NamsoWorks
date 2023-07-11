using ArzTalep.BL.Model;
using Aware.Data;
using Microsoft.EntityFrameworkCore;

namespace ArzTalep.BL.Data
{
    public class ArzTalepDbContext : AwareDbContext<ArzTalepDbContext>
    {
        public ArzTalepDbContext(DbContextOptions<ArzTalepDbContext> options) : base(options)
        {
        }

        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
