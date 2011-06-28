using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionMapper.Generators
{
	public static partial class Generators
    {
        /// <summary>
        /// Returns generator for complex type mappings.
        /// </summary>
        public static Func<Expression, Type, Expression, Expression> ComplexToComplex(Func<Expression, Type, Expression, Expression> parent, IMemberExtractor memberExtractor)
        {
            return (fromExpr, to, customMapping) =>
            {
                if (IsComplexType(fromExpr.Type) && IsComplexType(to))
                {
                    return ValidateCustomMapping(customMapping)
                           ?? ComplexToComplexExpressionGenerator(parent, memberExtractor, fromExpr, to, customMapping);
                }
                else
                {
                    return null;
                }
            };
        }

        private static Expression ValidateCustomMapping(Expression customMapping)
        {
            if (customMapping != null &&
                (customMapping.NodeType != ExpressionType.MemberInit
                || (customMapping as MemberInitExpression).Bindings.Count == 0))
            {
                return customMapping;
            }
            else
            {
                return null;
            }
        }
        private static Expression ComplexToComplexExpressionGenerator(Func<Expression, Type, Expression, Expression> parent, IMemberExtractor memberExtractor, Expression fromExpr, Type to, Expression customMapping)
        {
            NewExpression configNewExpr = null;
            IEnumerable<MemberAssignment> configBindings = Enumerable.Empty<MemberAssignment>();
            var customInitExpression = customMapping as MemberInitExpression;
            if (customInitExpression != null)
            {
                configNewExpr = customInitExpression.NewExpression;
                configBindings = customInitExpression.Bindings.OfType<MemberAssignment>();
            }

            var membersTo = memberExtractor
                                .GetToMembers(to)
                                .ToList();

            var membersFrom = memberExtractor
                                .GetFromMembers(fromExpr.Type, membersTo)
                                .ToList();

            var conventionalBindings = (from mfrom in membersFrom
                                        join mto in membersTo on mfrom.Name equals mto.Name
                                        let memberExpr = parent(mfrom.GetMemberAccessExpression(fromExpr), mto.Type, null)
                                        where memberExpr != null
                                        select Expression.Bind(mto.Member, memberExpr)
                                       ).ToList();

            //HACK : In case of no member "from" for custom mapping
            //used binding expression as fromExpr parameter for recursive call
            //because in this case fromExpr used only for type matching
            var customBindings = (from b in configBindings
                                  join mto in membersTo on b.Member equals mto.Member
                                  let mfrom = membersFrom.FirstOrDefault(m => mto.Name == m.Name)
                                  let expr = (mfrom != null)
                                             ? mfrom.GetMemberAccessExpression(fromExpr)
                                             : b.Expression /*hack*/
                                  let memberExpr = parent(expr, mto.Type, b.Expression)
                                  select Expression.Bind(mto.Member, memberExpr)
                                 ).ToList();

            var customBindingMembers = new HashSet<MemberInfo>(customBindings.Select(b => b.Member));
            var allBindings = customBindings
                                .Concat(conventionalBindings
                                            .Where(b => !customBindingMembers.Contains(b.Member)));

            return Expression.MemberInit(configNewExpr ?? Expression.New(to), allBindings.Cast<MemberBinding>());
        }

        private static bool IsComplexType(Type t)
        {
            return (t.IsClass && t != typeof(string))
                    || (t.IsValueType && !t.IsPrimitive && t != typeof(DateTime));
        }
    }
}
