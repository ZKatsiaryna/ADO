using AdoPart2.Enum;
using AdoPart2.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;

namespace AdoPart2.Common
{
    public class DBManager
    {
        private DataBaseHandlerFactory dbFactory;
        private IDataBaseHandler dataBase;
        private string providerName;

        public DBManager(string connectionStringName)
        {
            dbFactory = new DataBaseHandlerFactory(connectionStringName);
            dataBase = dbFactory.CreateGDataBase();
            providerName = dbFactory.GetProviderName();
        }

        public IDbConnection GetDataBaseConnection()
        {
            return dataBase.CreateConnection();
        }

        public void CloseConnection(IDbConnection connection)
        {
            dataBase.CloseConnection(connection);
        }

        public List<OrderStatusModel> GetOrders()
        {
            List<OrdersModel> listOrders = new List<OrdersModel>();

            using (var connection = dataBase.CreateConnection())
            {
                var com = dataBase.CreateCommand("select * from dbo.Orders", CommandType.Text, connection);
                connection.Open();
                var reader = com.ExecuteReader();
                listOrders = PopulationUtils<OrdersModel>.CreateList(reader);
            }

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<OrdersModel, OrderStatusModel>(); });
            var mapper = config.CreateMapper();
            List<OrderStatusModel> result = mapper.Map<List<OrderStatusModel>>(listOrders);

            return result;
        }

        public List<AllOrdersStatusModel> GetOrderDetails(int orderId)
        {
            List<AllOrdersDetailsModel> listOrders = new List<AllOrdersDetailsModel>();

            using (var connection = dataBase.CreateConnection())
            {
                connection.Open();
                var command = $"select o.OrderID, o.OrderDate, o.RequiredDate, o.ShippedDate, o.ShipName, o.ShipAddress, o.ShipRegion, o.ShipPostalCode, o.ShipCountry, o.ShipCity, o.ShipVia, od.Quantity, od.UnitPrice, od.Discount, od.Discount, p.ProductName, p.ProductID from((Orders as o join [Order Details] as od on o.OrderID=od.OrderID) join Products as p on od.ProductID = p.ProductID) where o.OrderID={orderId}";
                var com = dataBase.CreateCommand(command, CommandType.Text, connection);
                var reader = com.ExecuteReader();
                listOrders = PopulationUtils<AllOrdersDetailsModel>.CreateList(reader);
            }

            var config = new MapperConfiguration(cfg => { cfg.CreateMap<AllOrdersDetailsModel, AllOrdersStatusModel>(); });
            var mapper = config.CreateMapper();
            List<AllOrdersStatusModel> result = mapper.Map<List<AllOrdersStatusModel>>(listOrders);

            return result;
        }

        public void AddOrder(AddOrderRequestModel addOrderRequestModel)
        {
            string orderDate1 = addOrderRequestModel.OrderDate.ToString() ?? ((DateTime)addOrderRequestModel.OrderDate).ToString("yyyy-MM-dd");

            string orderDate = addOrderRequestModel.OrderDate == null? null: ((DateTime)addOrderRequestModel.OrderDate).ToString("yyyy-MM-dd");
            string requiredDate = addOrderRequestModel.RequiredDate == null ? null : ((DateTime)addOrderRequestModel.RequiredDate).ToString("yyyy-MM-dd");
            string shippedDate = addOrderRequestModel.ShippedDate == null ? null : ((DateTime)addOrderRequestModel.ShippedDate).ToString("yyyy-MM-dd");

            using (var connection = dataBase.CreateConnection())
            {
                var transaction = dataBase.CreateTransaction();
                string sql1 = $"INSERT INTO Orders(CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight, ShipName, ShipAddress, ShipCity, ShipRegion, ShipPostalCode, ShipCountry) values('{addOrderRequestModel.CustomerID}', {addOrderRequestModel.EmployeeID}, {orderDate},{requiredDate}, {shippedDate}, {addOrderRequestModel.ShipVia}, '{addOrderRequestModel.Freight}', '{addOrderRequestModel.ShipName}', '{addOrderRequestModel.ShipAddress}', '{addOrderRequestModel.ShipCity}', '{addOrderRequestModel.ShipRegion}', '{addOrderRequestModel.ShipPostalCode}', '{addOrderRequestModel.ShipCountry}' ) ";
                string sql2 = $"DECLARE @IDENTITY INT SELECT @IDENTITY = SCOPE_IDENTITY() INSERT INTO[Order Details] (OrderID, ProductID, UnitPrice, Quantity, Discount) values(@IDENTITY, {addOrderRequestModel.ProductID}, {addOrderRequestModel.UnitPrice}, {addOrderRequestModel.Quantity} ,{addOrderRequestModel.Discount} )";

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    using (var cmd = dataBase.CreateCommand(sql1, transaction, connection)) { cmd.ExecuteNonQuery(); }
                    using (var cmd = dataBase.CreateCommand(sql2, transaction, connection)) { cmd.ExecuteNonQuery(); }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Attempt to roll back the transaction.
                    try
                    {
                        Console.WriteLine("Order have not been added");
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("An error occured whiel reversing transaction");
                    }
                }
            }
        }

        public void UpdatedOrder(AllOrdersStatusModel updatedModel, int orderId)
        {
            if (updatedModel.StatusOrder == StatusOrder.Delivered || updatedModel.StatusOrder == StatusOrder.InProgress)
            {
                Console.WriteLine($"The order with status: {updatedModel.StatusOrder } forbidden to be updated");
            }
            else
            {
                using (var connection = dataBase.CreateConnection())
                {
                    var requiredDate = (DateTime)updatedModel.RequiredDate;
                    string sql1 = $"UPDATE Orders SET RequiredDate={requiredDate.ToString("yyyy-MM-dd")}, ShipVia={updatedModel.ShipVia}, ShipName='{updatedModel.ShipName}', ShipAddress='{updatedModel.ShipAddress}', ShipCity='{updatedModel.ShipCity}', ShipRegion='{updatedModel.ShipRegion}', ShipPostalCode='{updatedModel.ShipPostalCode}', ShipCountry='{updatedModel.ShipCountry}' where OrderID={orderId}";
                    string sql2 = $"UPDATE [Order Details] SET UnitPrice={updatedModel.UnitPrice}, Quantity={updatedModel.Quantity}, Discount={updatedModel.Discount} where OrderID={orderId}";
                    var transaction = dataBase.CreateTransaction();

                    try
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();
                        using (var cmd = dataBase.CreateCommand(sql1, transaction, connection)) { cmd.ExecuteNonQuery(); }
                        using (var cmd = dataBase.CreateCommand(sql2, transaction, connection)) { cmd.ExecuteNonQuery(); }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Attempt to roll back the transaction.
                        try
                        {
                            Console.WriteLine("Order have not been updated");
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("An error occured whiel reversing transaction");
                        }
                    }
                }
            }
        }

        public void DeleteOrder(int orderId)
        {
            var order = GetOrderDetails(orderId);

            if (order[0].StatusOrder == StatusOrder.Delivered)
            {
                Console.WriteLine($"The order with status: {order[0].StatusOrder } forbidden to be updated");
            }
            else
            {
                using (var connection = dataBase.CreateConnection())
                {
                    var transaction = dataBase.CreateTransaction();

                    string sql1 = $"DELETE FROM [Order Details] WHERE OrderID={orderId}";
                    string sql2 = $"DELETE FROM Orders WHERE OrderID={orderId}";

                    try
                    {
                        connection.Open();
                        transaction = connection.BeginTransaction();
                        using (var cmd = dataBase.CreateCommand(sql1, transaction, connection)) { cmd.ExecuteNonQuery(); }
                        using (var cmd = dataBase.CreateCommand(sql2, transaction, connection)) { cmd.ExecuteNonQuery(); }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Attempt to roll back the transaction.
                        try
                        {
                            Console.WriteLine("Order have not been updated");
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("An error occured whiel reversing transaction");
                        }
                    }
                }
            }
        }

        public void MoveStatusToInProgress(int orderId, DateTime orderDate)
        {
            using (var connection = dataBase.CreateConnection())
            {
                string sql1 = $"UPDATE Orders SET OrderDate={orderDate.ToString("yyyy-MM-dd")} where OrderID={orderId}";
                connection.Open();
                var cmd = dataBase.CreateCommand(sql1, CommandType.Text, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public void MoveStatusToDelivered(int orderId, DateTime shippedDate)
        {
            using (var connection = dataBase.CreateConnection())
            {
                string sql1 = $"UPDATE Orders SET ShippedDate={shippedDate.ToString("yyyy-MM-dd")} where OrderID={orderId}";
                connection.Open();
                var cmd = dataBase.CreateCommand(sql1, CommandType.Text, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public List<ResponseCustOrderHistModel> GetCustOrderHist(string customerID)
        {
            List<ResponseCustOrderHistModel> listCustOrderHist = new List<ResponseCustOrderHistModel>();
            string expression = "CustOrderHist";

            IDataParameter param;

            using (var connection = dataBase.CreateConnection())
            {
                connection.Open();
                var command = dataBase.CreateCommand(expression, CommandType.StoredProcedure, connection);
                param = dataBase.CreateParameter("@CustomerID", customerID, DbType.String);
                command.Parameters.Add(param);
                var reader = command.ExecuteReader();
                listCustOrderHist = PopulationUtils<ResponseCustOrderHistModel>.CreateList(reader);
            }

            return listCustOrderHist;
        }

        public List<ResponseCustOrdersDetailModel> GetCustOrdersDetail(int orderID)
        {
            List<ResponseCustOrdersDetailModel> listCustOrdersDetail = new List<ResponseCustOrdersDetailModel>();
            string expression = "CustOrdersDetail";
            IDataParameter param;

            using (var connection = dataBase.CreateConnection())
            {
                connection.Open();
                var command = dataBase.CreateCommand(expression, CommandType.StoredProcedure, connection);
                param = dataBase.CreateParameter("@OrderID", orderID, DbType.Int32);
                command.Parameters.Add(param);

                var reader = command.ExecuteReader();
                listCustOrdersDetail = PopulationUtils<ResponseCustOrdersDetailModel>.CreateList(reader);
            }

            return listCustOrdersDetail;
        }

    }
}
