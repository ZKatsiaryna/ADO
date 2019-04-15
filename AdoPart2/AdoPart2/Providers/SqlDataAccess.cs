using AdoPart2.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2.Providers
{
    public class SqlDataAccess : IDataBaseHandler
    {
        private string ConnectionString { get; set; }

        public SqlDataAccess(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void CloseConnection(IDbConnection connection)
        {
            var sqlConnection = (SqlConnection)connection;
            sqlConnection.Close();
            sqlConnection.Dispose();
        }

        public IDataAdapter CreateAddapter(IDbCommand command)
        {
            return new SqlDataAdapter((SqlCommand)command);
        }

        public IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection)
        {
            return new SqlCommand
            {
                CommandText = commandText,
                Connection = (SqlConnection)connection,
                CommandType = commandType
            };
        }

        public IDbCommand CreateCommand(string commandText, IDbTransaction transaction, IDbConnection connection)
        {
            return new SqlCommand
            {
                CommandText = commandText,
                Connection = (SqlConnection)connection,
                Transaction = (SqlTransaction)transaction
            };
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public IDbDataParameter CreateParameter(string name, object value, DbType dbType)
        {
            return new SqlParameter
            {
                DbType = dbType,
                ParameterName = name,
                Value = value
            };
        }
        
        public IDbTransaction CreateTransaction()
        {
            SqlTransaction transaction = null;

            return transaction;
        }
    }
}
