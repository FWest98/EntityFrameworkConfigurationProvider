using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EntityFrameworkConfigurationProvider {
    public class EntityFrameworkConfigurationSource : IConfigurationSource {
        private readonly Lazy<IConfigurationContext> _database;
        public EntityFrameworkConfigurationSource(Lazy<IConfigurationContext> database) => _database = database;

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new EntityFrameworkConfigurationProvider(_database);
        }
    }
}
