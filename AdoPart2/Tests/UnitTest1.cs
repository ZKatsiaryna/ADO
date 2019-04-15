using AdoPart2.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetOrdersTest()
        {
            var db = new DBManager("DBCS");
            var resultAllOrders = db.GetOrders();

            Assert.IsTrue(resultAllOrders.Count > 10);
        }

        [TestMethod]
        public void GetOrderDetailsTest()
        {
            var db = new DBManager("DBCS");
            var resultAllOrders = db.GetCustOrdersDetail(10248);

            Assert.IsNotNull(resultAllOrders);
            Assert.AreEqual("",resultAllOrders[0].ProductName);
            Assert.IsNotNull(resultAllOrders);

        }
    }
}
