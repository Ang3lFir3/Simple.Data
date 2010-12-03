﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.SqlCeTest
{
    [TestFixture]
    public class OrderDetailTests
    {
        private static readonly string DatabasePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)),
            "TestDatabase.sdf");

        [Test]
        [Ignore]
        public void TestOrderDetail()
        {
            var db = Database.OpenFile(DatabasePath);
            var order = db.Orders.FindByOrderDate(new DateTime(2010, 8, 11));
            IEnumerable<dynamic> orderItems = order.OrderItems;
            var orderItem = orderItems.FirstOrDefault();
            var item = orderItem.Item;
            Assert.AreEqual("Widget", item.Name);
        }
    }
}
