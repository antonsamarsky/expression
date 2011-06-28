using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace ExpressionMapper.Tests
{
	[TestClass]
	public class SimpleTypesConvertionTests
	{
		[TestMethod]
		public void LosslessConvertion()
		{
			var m1 = Mapper.Create<short, int>();
			Assert.AreEqual(1, m1(1));

			var m2 = Mapper.Create<int, double>();
			Assert.AreEqual(2.0, m2(2));
		}

		[TestMethod]
		[ExpectedException(typeof(OverflowException))]
		public void NarrowingConvertionThrowException()
		{
			var m1 = Mapper.Create<long, int>();
			m1(long.MaxValue);
		}

		[TestMethod]
		[Ignore]
		public void SpeedTest()
		{
			var m1 = Mapper.Create<short, int>();
			m1(100);

			var sw = Stopwatch.StartNew();
			for (int i = 0; i < 1000000; i++)
			{
				var c = m1(100);
			}
			var mapperTimes = sw.ElapsedTicks;
			for (int i = 0; i < 1000000; i++)
			{
				short s = 100;
				var c = Convert.ToInt16(s);
			}
			var clearTimes = sw.ElapsedTicks - mapperTimes;
			sw.Stop();

			Assert.Inconclusive("{0} {1}", mapperTimes, clearTimes);
		}
	}
}
