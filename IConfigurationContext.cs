using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EntityFrameworkConfigurationProvider {
    public interface IConfigurationContext {
        public DbSet<ConfigurationOption> ConfigurationOptions { get; set; }

        // Some DbContext methods
        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        public int SaveChanges();
    }
}
