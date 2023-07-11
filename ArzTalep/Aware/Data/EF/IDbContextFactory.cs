using Aware.Model;
using Microsoft.EntityFrameworkCore;

namespace Aware.Data.EF
{
    public interface IDbContextFactory
    {
        DbContext GetDbContext();
        DbSet<T> GetDbSet<T>() where T : class, IEntity;
    }
}
