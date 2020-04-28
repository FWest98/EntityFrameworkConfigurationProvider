using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EntityFrameworkConfigurationProvider {
    public class DefaultOptionsWriter<TOptions> : IOptionsWriter<TOptions> where TOptions : class {
        private readonly IConfiguration _config;
        public DefaultOptionsWriter(IConfiguration config) {
            _config = config;
        }

        public void SetValue<TProperty>(Expression<Func<TOptions, TProperty>> expression, string value) {
            var key = GetPropertyName(expression);
            _config[key] = value;
        }

        private string GetPropertyName<TObj, TProperty>(Expression<Func<TObj, TProperty>> exp) {
            if (!TryFindMemberExpression(exp.Body, out var memberExp)) return "";

            var names = new Stack<string>();
            do {
                names.Push(memberExp.Member.Name);
            } while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(":", names);
        }

        private bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp) {
            memberExp = exp as MemberExpression;
            if (memberExp != null) return true;
            if (!IsConversion(exp) || !(exp is UnaryExpression unExp)) {
                return false;
            }

            memberExp = unExp.Operand as MemberExpression;
            return memberExp != null;
        }
        private bool IsConversion(Expression exp) => exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked;
    }
}
