using Aware.Data.EF;
using Aware.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ArzTalep.BL.Data
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DbContext GetDbContext()
        {
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ArzTalepDbContext>();
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
