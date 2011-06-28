using System;
using System.Collections.Generic;

namespace ExpressionMapper
{
	/// <summary>
	/// Defines methods that extract members of types for mapping.
	/// </summary>
	public interface IMemberExtractor
	{
		IEnumerable<MappingMember> GetFromMembers(Type t, IEnumerable<MappingMember> toMembers);
		IEnumerable<MappingMember> GetToMembers(Type t);
	}
}
