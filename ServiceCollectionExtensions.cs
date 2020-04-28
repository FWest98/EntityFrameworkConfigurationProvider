using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EntityFrameworkConfigurationProvider {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection ConfigureWithDatabase<TOptions, TContext>(
            this IServiceCollection services,
            IConfiguration config,
            Func<IConfiguration, IConfiguration> configMapper
        ) where TContext : IConfigurationContext where TOptions : class {
            return services.ConfigureWithDatabase<TOptions, TContext>(Options.DefaultName, config, configMapper);
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions, TContext>(
            this IServiceCollection services,
            string name,
            IConfiguration config,
            Func<IConfiguration, IConfiguration> configMapper
        ) where TContext : IConfigurationContext where TOptions : class {
            return services.ConfigureWithDatabase<TOptions, TContext>(name, config, configMapper, _ => { });
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions, TContext>(
            this IServiceCollection services,
            IConfiguration config,
            Func<IConfiguration, IConfiguration> configMapper,
            Action<BinderOptions> configureBinder
        ) where TContext : IConfigurationContext where TOptions : class {
            return services.ConfigureWithDatabase<TOptions, TContext>(Options.DefaultName, config, configMapper, configureBinder);
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions, TContext>(
            this IServiceCollection services,
            string name,
            IConfiguration config,
            Func<IConfiguration, IConfiguration> configMapper,
            Action<BinderOptions> configureBinder
        ) where TContext : IConfigurationContext where TOptions : class {
            services.AddOptions();
            return services.AddScoped<IConfigureOptions<TOptions>>(sp => {
                var contextLazy = new Lazy<IConfigurationContext>(() => sp.GetRequiredService<TContext>());

                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddConfiguration(config, false);
                configBuilder.Add(new EntityFrameworkConfigurationSource(contextLazy));

                var newConfig = configBuilder.Build();
                return new NamedConfigureFromConfigurationOptions<TOptions>(name, newConfig, configureBinder);
            });
        }
    }
}
