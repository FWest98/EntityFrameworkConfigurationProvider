using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkConfigurationProvider {
    public interface IConfigurationContext {
        public DbSet<ConfigurationOption> ConfigurationOptions { get; set; }
    }
}
