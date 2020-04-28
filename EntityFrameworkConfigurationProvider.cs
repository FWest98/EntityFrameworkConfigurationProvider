using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
