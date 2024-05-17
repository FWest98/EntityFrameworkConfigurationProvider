using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EntityFrameworkConfigurationProvider {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection AddEntityFrameworkConfigurationProvider<TContext>(
            this IServiceCollection services,
            IConfiguration config
        ) where TContext : class, IConfigurationContext {
            // Add DbContext
            services.AddSingleton<IConfigurationContext, TContext>();

            // Add new configuration source
            return services.AddSingleton<IConfiguration>(sp => {
                var dbContext = new Lazy<IConfigurationContext>(sp.GetRequiredService<IConfigurationContext>);

                var builder = new ConfigurationBuilder();
                builder.AddConfiguration(config);
                builder.Add(new EntityFrameworkConfigurationSource(dbContext));

                return builder.Build();
            });
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions>(
            this IServiceCollection services,
            Func<IConfiguration, IConfiguration> configMapper
        ) where TOptions : class {
            return services.ConfigureWithDatabase<TOptions>(Options.DefaultName, configMapper);
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions>(
            this IServiceCollection services,
            string name,
            Func<IConfiguration, IConfiguration> configMapper
        ) where TOptions : class {
            return services.ConfigureWithDatabase<TOptions>(name, configMapper, _ => { });
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions>(
            this IServiceCollection services,
            Func<IConfiguration, IConfiguration> configMapper,
            Action<BinderOptions> configureBinder
        ) where TOptions : class {
            return services.ConfigureWithDatabase<TOptions>(Options.DefaultName, configMapper, configureBinder);
        }

        public static IServiceCollection ConfigureWithDatabase<TOptions>(
            this IServiceCollection services,
            string name,
            Func<IConfiguration, IConfiguration> configMapper,
            Action<BinderOptions> configureBinder
        ) where TOptions : class {
            // Add options support in general
            services.AddOptions();

            // Add option reloading changetokensource
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(sp =>
                new ConfigurationChangeTokenSource<TOptions>(sp.GetRequiredService<IConfiguration>()));

            // Add option writing
            services.AddSingleton<IOptionsWriter<TOptions>>(sp => {
                var config = sp.GetRequiredService<IConfiguration>();
                var mappedConfig = configMapper(config);
                return new DefaultOptionsWriter<TOptions>(mappedConfig);
            });

            // Add option configuring itself
            return services.AddSingleton<IConfigureOptions<TOptions>>(sp => {
                var config = sp.GetRequiredService<IConfiguration>();
                var mappedConfig = configMapper(config);
                return new NamedConfigureFromConfigurationOptions<TOptions>(name, mappedConfig, configureBinder);
            });
        }
    }
}