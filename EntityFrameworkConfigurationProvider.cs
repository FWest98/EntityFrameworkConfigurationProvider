using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EntityFrameworkConfigurationProvider {
    public class EntityFrameworkConfigurationProvider : ConfigurationProvider {
        private readonly Lazy<IConfigurationContext> _database;
        public EntityFrameworkConfigurationProvider(Lazy<IConfigurationContext> database) {
            _database = database;
        }

        public override void Load() {
            var context = _database.Value;
            Data = context.ConfigurationOptions.ToDictionary(s => s.Key, s => s.Value);
        }

        public override void Set(string key, string value) {
            base.Set(key, value);

            // Store to EF
            var context = _database.Value;
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
    }
}
