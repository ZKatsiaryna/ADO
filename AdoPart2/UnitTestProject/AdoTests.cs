using System;
using System.Linq;
using AdoPart2.Common;
using AdoPart2.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class AdoTests
    {
        DBManager db;

        [TestInitialize]
        public void InitializedMethod()
        {
            db = new DBManager("DBCS");
        }

        [TestMethod]
        public void GetOrdersTest()
        {
            var resultAllOrders = db.GetOrders();
            Assert.IsTrue(resultAllOrders.Count > 10);
        }

        [TestMethod]
        public void GetOrderDetailsTest()
        {
            var resultAllOrders = db.GetOrderDetails(10248);

            Assert.AreEqual(3, resultAllOrders.Count);
            Assert.AreEqual("Queso Cabrales", resultAllOrders[0].ProductName);
            Assert.IsNotNull(resultAllOrders);
        }

        [TestMethod]
        public void MoveStatusToDeliveredTest()
        {
            var date = DateTime.Today;
            var myorderId = db.GetOrders();
            var updateOrder = db.GetOrders().FindLast(s => s.StatusOrder == AdoPart2.Enum.StatusOrder.InProgress);
            var getOrder = db.GetOrderDetails(updateOrder.OrderID).FirstOrDefault();

            db.MoveStatusToDelivered(getOrder.OrderID, date);
            var getOrderAfterUpdate = db.GetOrderDetails(updateOrder.OrderID).FirstOrDefault();
            Assert.AreEqual(date, getOrderAfterUpdate.ShippedDate);
        }

        [TestMethod]
        public void UpdatedOrderTest()
        {
            var getOrders = db.GetOrderDetails(10248).FirstOrDefault();
            getOrders.ShipAddress = "Updated address" + DateTime.Now;
            db.UpdatedOrder(getOrders, 10248);

            var updateOrders = db.GetOrderDetails(10248).FirstOrDefault();
            Assert.AreEqual(getOrders.ShipAddress, getOrders.ShipAddress);
        }

        [TestMethod]
        public void MoveStatusToInProgressTest()
        {
            var date = DateTime.Today;
            var myorderId = db.GetOrders();
            var updateOrder = db.GetOrders().FindLast(s => s.StatusOrder == AdoPart2.Enum.StatusOrder.New);
            var getOrder = db.GetOrderDetails(updateOrder.OrderID).FirstOrDefault();

            db.MoveStatusToInProgress(getOrder.OrderID, date);
            var getOrderAfterUpdate = db.GetOrderDetails(updateOrder.OrderID).FirstOrDefault();
            Assert.AreEqual(date, getOrderAfterUpdate.OrderDate);
        }

        [TestMethod]
        public void CustOrderHistTest()
        {
            var result = db.GetCustOrderHist("VINET");
            Assert.IsNotNull(result);           
        }

        [TestMethod]
        public void GetCustOrdersDetailTest()
        {
            var result = db.GetCustOrdersDetail(10248);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void DeleteOrderTest()
        {
            var addOrderRequestModel = CreateData();
            var count = db.GetOrders().Count;

            db.AddOrder(addOrderRequestModel);
            var resultAllOrders = db.GetOrders();
            var addedOrder = resultAllOrders.FirstOrDefault(i => i.ShipName == addOrderRequestModel.ShipName);

            db.DeleteOrder(addedOrder.OrderID);
            var count2 = db.GetOrders().Count;
            Assert.AreEqual(count, count2);
        }

        [TestMethod]
        public void AddOrderTest()
        {
            var addOrderRequestModel = CreateData();
            var count = db.GetOrders().Count;
            db.AddOrder(addOrderRequestModel);

            var resultAllOrders = db.GetOrders();
            var addedOrder = resultAllOrders.Last(i => i.ShipName == addOrderRequestModel.ShipName);
            var MyOrderDetails = db.GetOrderDetails(addedOrder.OrderID).FirstOrDefault();

            Assert.AreEqual(1, resultAllOrders.Count - count);
            Assert.IsNotNull(MyOrderDetails);
            db.DeleteOrder(addedOrder.OrderID);
        }

        private AddOrderRequestModel CreateData()
        {
            var addOrderRequestModel = new AddOrderRequestModel();
            var id = DateTime.Now;
            var ms = id.Ticks;
            addOrderRequestModel.CustomerID = "VICTE";
            addOrderRequestModel.EmployeeID = 3;
            addOrderRequestModel.ShipAddress = "Street 1";
            addOrderRequestModel.ShipCity = "Minsk ";
            addOrderRequestModel.ShipCountry = "BLR";
            addOrderRequestModel.ShipName = "S1 " + ms;
            addOrderRequestModel.ShippedDate = id;
            addOrderRequestModel.ShipPostalCode = "220089";
            addOrderRequestModel.ShipRegion = "MSQ";
            addOrderRequestModel.ShipVia = 1;
            addOrderRequestModel.Freight = 2;
            addOrderRequestModel.OrderDate = id;
            addOrderRequestModel.RequiredDate = id;
            addOrderRequestModel.ProductID = 51;
            addOrderRequestModel.Quantity = 999;
            addOrderRequestModel.UnitPrice = 100;
            addOrderRequestModel.Discount = 1;
            return addOrderRequestModel;
        }
    }
}
