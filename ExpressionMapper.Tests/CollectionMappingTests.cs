using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionMapper.Tests
{
	[TestClass]
	public class CollectionMappingTests
	{
		[TestMethod]
		public void ArrayToAarrayTest()
		{
			var m = Mapper.Create<int?[], int[]>();
			var arr1 = new int?[] { 1, 2, 4 };
			var arr2 = m(arr1);

			Assert.AreEqual(arr1.Length, arr2.Length);
			for (int i = 0; i < arr1.Length; i++)
			{
				Assert.AreEqual(arr1[i].Value, arr2[i]);
			}
		}

		[TestMethod]
		public void ListToListTest()
		{
			var m = Mapper.Create<List<int?>, List<int>>();
			var arr1 = new List<int?> { 1, 2, 4 };
			var arr2 = m(arr1);

			Assert.AreEqual(arr1.Count, arr2.Count);
			for (int i = 0; i < arr1.Count; i++)
			{
				Assert.AreEqual(arr1[i].Value, arr2[i]);
			}
		}
	}
}
