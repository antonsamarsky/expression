using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionMapper.Tests
{
	[TestClass]
	public class StringConvertionTests
	{
		[TestMethod]
		public void ConvertPrimitiveToString()
		{
			var m = Mapper.Create<int, string>();
			Assert.AreEqual("1", m(1));
		}

		[TestMethod]
		public void ConvertStringToPrimitive()
		{
			var m = Mapper.Create<string, int>();
			Assert.AreEqual(1, m("1"));
		}

	}
}
