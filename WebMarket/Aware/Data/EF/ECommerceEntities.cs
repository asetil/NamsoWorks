using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Aware.Authenticate.Model;
using Aware.Authority.Model;
using Aware.ECommerce.Model;
using Aware.File.Model;
using Aware.Mail;
using Aware.Payment.Model;
using Aware.Regional.Model;

namespace Aware.Data.EF
{
    public class ECommerceEntities : DbContext
    {
        public ECommerceEntities()
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false; //Lazy loading olsun mu?
        }

        public DbSet<User> User { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<StoreItem> StoreItem { get; set; }
        public DbSet<PropertyValue> PropertyValue { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<BasketItem> BasketItem { get; set; }
        public DbSet<Favorite> Favorite { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<ShippingMethod> ShippingMethod { get; set; }
        public DbSet<FileRelation> FileRelation { get; set; }
        public DbSet<PropertyRelation> PropertyRelation { get; set; }
        public DbSet<Campaign> Campaign { get; set; }
        public DbSet<CampaignItem> CampaignItem { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<MailTemplate> MailTemplate { get; set; }
        public DbSet<PosDefinition> PosDefinition { get; set; }
        public DbSet<OnlineSales> OnlineSales { get; set; }
        public DbSet<SimpleItem> SimpleItem { get; set; }
        public DbSet<AuthorityDefinition> AuthorityDefinition { get; set; }
        public DbSet<AuthorityUsage> AuthorityUsage { get; set; }
        public DbSet<Comment> Comment { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ECommerceEntities>());
            //modelBuilder.Entity<User>().ToTable("User");

            //        modelBuilder.Entity<Order>()
            //.HasRequired(c => c.BillingAddress)
            //.WithMany()
            //.WillCascadeOnDelete(false);

            //            modelBuilder.Entity<Order>()
            //.HasRequired(c => c.ShippingAddress)
            //.WithMany()
            //.WillCascadeOnDelete(false);

            //modelBuilder.Entity<Course>()
            //    .HasMany(c => c.Instructors).WithMany(i => i.Courses)
            //    .Map(t => t.MapLeftKey("CourseID")
            //        .MapRightKey("PersonID")
            //        .ToTable("CourseInstructor"));
        }
    }
}
