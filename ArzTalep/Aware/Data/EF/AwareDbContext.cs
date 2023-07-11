using Aware.File.Model;
using Aware.Mail;
using Aware.Model;
using Aware.Util.Lookup;
using Microsoft.EntityFrameworkCore;

namespace Aware.Data
{
    public class AwareDbContext<T> : DbContext where T : DbContext
    {
        public AwareDbContext(DbContextOptions<T> options) : base(options)
        {
        }

        public DbSet<ApplicationModel> Application { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Lookup> Lookup { get; set; }

        public DbSet<MailTemplate> MailTemplates { get; set; }

        public DbSet<FileRelation> FileRelations { get; set; }

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
