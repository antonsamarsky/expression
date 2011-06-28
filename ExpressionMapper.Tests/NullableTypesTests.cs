using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionMapper.Tests
{
	[TestClass]
	public class NullableTypesTests
	{
		[TestMethod]
		public void ConvertToNullable()
		{
			var m = Mapper.Create<int, int?>();
			var r = m(1);
			Assert.IsTrue(r.HasValue);
			Assert.AreEqual(1, r.Value);
		}

		[TestMethod]
		public void ConvertFromNullable()
		{
			var m = Mapper.Create<int?, int>();
			var r = m(1);
			Assert.AreEqual(1, r);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ConvertFromNullableNullThrows()
		{
			var m = Mapper.Create<int?, int>();
			var r = m(null);
		}

	}
}
