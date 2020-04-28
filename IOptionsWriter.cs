using System;
using System.Linq.Expressions;

namespace EntityFrameworkConfigurationProvider {
    public interface IOptionsWriter<TOptions> where TOptions : class {
        public void SetValue<TProperty>(Expression<Func<TOptions, TProperty>> expression, string value);
    }
}
