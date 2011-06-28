using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionMapper.Tests
{
	[TestClass]
	public class FacadeTests
	{
		[TestMethod]
		public void CreateShallowReturnsNotNull()
		{
			var mapper = Mapper.Create<string, int>();
			Assert.IsNotNull(mapper);
		}

		[TestMethod]
		public void CreateShallowReturnsIdentityFunctionOnSameTypes()
		{
			var mapper = Mapper.Create<int, int>();
			Assert.AreEqual(1, mapper(1));

			var test = "test";
			var mapper1 = Mapper.Create<string, string>();
			Assert.AreSame(test, test);

		}

		class A
		{

		}

		class B : A
		{
		}

		[TestMethod]
		public void CreateShallowReturnsIdentityFunctionOnAssignableTypes()
		{
			var b = new B();
			var mapper = Mapper.Create<B, A>();
			Assert.AreEqual(b, mapper(b));
		}

	}
}
