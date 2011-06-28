using System;
using System.Reflection;
using System.Linq.Expressions;

namespace ExpressionMapper
{
	public class MappingMember
	{
		public MappingMember(string name, Type type, MemberInfo member) : this(name, type, member, e => Expression.MakeMemberAccess(e, member))
		{
		}

		public MappingMember(string name, Type type, MemberInfo member, Func<Expression, Expression> getMemberAccessExpression)
		{
			this.Name = name;
			this.Type = type;
			this.Member = member;
			GetMemberAccessExpression = getMemberAccessExpression;
		}

		public string Name { get; private set; }
		public Type Type { get; private set; }
		public MemberInfo Member { get; private set; }

		public Func<Expression, Expression> GetMemberAccessExpression { get; private set; }
	}
}
