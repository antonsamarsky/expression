using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionMapper.Tests
{
	public class CalendarEvent
	{
		public DateTime EventDate { get; set; }
		public string Title { get; set; }
	}

	public class CalendarEventForm
	{
		public DateTime EventDate { get; set; }
		public int EventHour { get; set; }
		public int EventMinute { get; set; }
		public string Title { get; set; }
	}

	[TestClass]
	public class CustomMappingTests
	{
		[TestMethod]
		public void CustomMappingPrimitive()
		{
			var m = Mapper.Create<int, int?>(i => ((i + 1) % 2 == 0) ? (int?)(i + 1) : null);
			Assert.AreEqual(2, m(1));
			Assert.AreEqual(null, m(2));
		}

		[TestMethod]
		public void CustomMappingSimpleType()
		{
			var m = Mapper.Create<CalendarEvent, CalendarEventForm>(ce =>
					new CalendarEventForm
					{
						EventDate = ce.EventDate.Date,
						EventHour = ce.EventDate.Hour,
						EventMinute = ce.EventDate.Minute
					});

			var calendarEvent = new CalendarEvent
			{
				EventDate = new DateTime(2008, 12, 15, 20, 30, 0),
				Title = "Company Holiday Party"
			};

			var form = m(calendarEvent);
			Assert.AreSame(calendarEvent.Title, form.Title);
			Assert.AreEqual(new DateTime(2008, 12, 15), form.EventDate);
			Assert.AreEqual(20, form.EventHour);
			Assert.AreEqual(30, form.EventMinute);
		}

		[TestMethod]
		public void MapComplexAtoCompexBWithCustomMapping()
		{
			var now = DateTime.Now;

			var ca = new ComplexA
			{
				P1 = new A { P1 = 1, P2 = "2", P3 = 3.45f },
				P2 = 6,
				P3 = now,
				P4 = new A { P1 = 7, P2 = "8", P3 = 9.0f }
			};
			var m = Mapper.Create<ComplexA, ComplexB>(c =>
					new ComplexB
					{
						P1 = new B { P2 = c.P4.P2 },
						P2 = c.P2,
						P3 = c.P3.ToString()
					});
			var cb = m(ca);

			Assert.AreEqual("1", cb.P1.P1);
			Assert.AreEqual("8", cb.P1.P2);
			Assert.IsTrue((3.45 - cb.P1.P3) < 0.0000001);

			Assert.IsTrue((6 - cb.P2) < 0.0000001);
			Assert.AreEqual(now.ToString(), cb.P3);

			Assert.AreEqual("7", cb.P4.P1);
			Assert.AreEqual("8", cb.P4.P2);
			Assert.IsTrue((9.0 - cb.P4.P3) < 0.0000001);

		}
	}
}
