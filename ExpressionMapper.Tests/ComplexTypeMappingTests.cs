using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionMapper.Tests
{
	[TestClass]
	public class ComplexTypeMappingTests
	{
		[TestMethod]
		public void MapAtoB()
		{
			var a = new A { P1 = 1, P2 = "test", P3 = 3.45f };
			var m = Mapper.Create<A, B>();
			var b = m(a);

			Assert.AreEqual("1", b.P1);
			Assert.AreEqual("test", b.P2);
			Assert.IsTrue((3.45 - b.P3) < 0.0000001);
		}

		[TestMethod]
		public void MapComplexAtoCompexB()
		{
			var now = DateTime.Now;

			var ca = new ComplexA
			{
				P1 = new A { P1 = 1, P2 = "2", P3 = 3.45f },
				P2 = 6,
				P3 = now,
				P4 = new A { P1 = 7, P2 = "8", P3 = 9.0f }
			};
			var mapper = Mapper.Create<ComplexA, ComplexB>();
			var cb = mapper(ca);

			Assert.AreEqual("1", cb.P1.P1);
			Assert.AreEqual("2", cb.P1.P2);
			Assert.IsTrue((3.45 - cb.P1.P3) < 0.0000001);

			Assert.IsTrue((6 - cb.P2) < 0.0000001);
			Assert.AreEqual(now.ToString(), cb.P3);

			Assert.AreEqual("7", cb.P4.P1);
			Assert.AreEqual("8", cb.P4.P2);
			Assert.IsTrue((9.0 - cb.P4.P3) < 0.0000001);

		}

		[TestMethod]
		public void FlatteringTest()
		{
			var customer = new Customer
			{
				Name = "George Costanza"
			};
			var order = new Order
			{
				Customer = customer
			};
			var bosco = new Product
			{
				Name = "Bosco",
				Price = 4.99m
			};
			order.AddOrderLineItem(bosco, 15);

			var m = Mapper.Create<Order, OrderDto>();
			var dto = m(order);
			Assert.AreEqual(74.85m, dto.Total);
			Assert.AreSame(customer.Name, dto.CustomerName);
		}
	}

	public class A
	{
		public int P1 { get; set; }
		public string P2 { get; set; }
		public float P3 { get; set; }
	}

	public class B
	{
		public string P1 { get; set; }
		public string P2 { get; set; }
		public double P3 { get; set; }
	}

	public class ComplexA
	{
		public A P1 { get; set; }
		public int P2 { get; set; }
		public DateTime P3 { get; set; }
		public A P4 { get; set; }
	}

	public class ComplexB
	{
		public B P1 { get; set; }
		public float P2 { get; set; }
		public string P3 { get; set; }
		public B P4 { get; set; }
	}

	public class Order
	{
		private readonly IList<OrderLineItem> _orderLineItems = new List<OrderLineItem>();
		public Customer Customer { get; set; }
		public OrderLineItem[] GetOrderLineItems()
		{
			return _orderLineItems.ToArray();
		}
		public void AddOrderLineItem(Product product, int quantity)
		{
			_orderLineItems.Add(new OrderLineItem(product, quantity));
		}
		public decimal Total
		{
			get
			{
				return _orderLineItems.Sum(li => li.GetTotal());
			}
		}
	}
	public class Product
	{
		public decimal Price { get; set; }
		public string Name { get; set; }
	}
	public class OrderLineItem
	{
		public OrderLineItem(Product product, int quantity)
		{
			Product = product;
			Quantity = quantity;
		}
		public Product Product { get; private set; }
		public int Quantity { get; private set; }
		public decimal GetTotal()
		{
			return Quantity * Product.Price;
		}
	}
	public class Customer
	{
		public string Name { get; set; }
	}

	public class OrderDto
	{
		public string CustomerName { get; set; }
		public decimal Total { get; set; }
	}
}
