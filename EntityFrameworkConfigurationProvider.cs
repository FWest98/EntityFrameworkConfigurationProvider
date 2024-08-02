using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Immutable;

namespace EntityFrameworkConfigurationProvider;

public class EntityFrameworkConfigurationProvider(Lazy<IConfigurationContext> database) : ConfigurationProvider {
    public override void Load() {
        var context = database.Value;
        if (!IsDatabaseAvailable(context)) {
            Data = ImmutableDictionary<string, string>.Empty;
        } else {
            Data = context.ConfigurationOptions.ToDictionary(s => s.Key, s => s.Value);
        }
    }

    public override void Set(string key, string value) {
        // Store to EF
        var context = database.Value;
        if (!IsDatabaseAvailable(context))
            throw new InvalidOperationException("Database is not available for storing configuration options");
        
        base.Set(key, value);
            
        var config = context.ConfigurationOptions.FirstOrDefault(s => s.Key == key);

        if (config == null) {
            config = new ConfigurationOption { Key = key };
            context.ConfigurationOptions.Add(config);
        } else {
            context.Entry(config).State = EntityState.Modified;
        }

        config.Value = value; 
        context.SaveChanges();

        OnReload();
    }

    private static bool IsDatabaseAvailable(IConfigurationContext context) {
        if (context.Database.GetPendingMigrations().Any())
            return false;

        try {
            context.ConfigurationOptions.Any();
        } catch (Exception) {
            return false;
        }

        return true;
    }
}