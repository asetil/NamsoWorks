using Aware.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aware.Data.EF
{
    public class DbContextFactory<TContext> : IDbContextFactory where TContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DbContext GetDbContext()
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            return dbContext;

            //var dbContext = _serviceProvider.GetService<ArzTalepDbContext>();
            //return dbContext;

            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<ArzTalepDbContext>();
            //    return dbContext;
            //}
        }

        public DbSet<T> GetDbSet<T>() where T : class, IEntity
        {
            return GetDbContext().Set<T>();
        }
    }
}
