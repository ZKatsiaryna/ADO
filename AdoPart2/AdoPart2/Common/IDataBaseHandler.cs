using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoPart2.Common
{
    public interface IDataBaseHandler
    {
        IDbConnection CreateConnection();

        void CloseConnection(IDbConnection connection);

        IDbCommand CreateCommand(string commandText, CommandType commandType, IDbConnection connection);

        IDbCommand CreateCommand(string commandText, IDbTransaction transaction, IDbConnection connection);

        IDataAdapter CreateAddapter(IDbCommand command);

        IDbDataParameter CreateParameter(string name, object value, DbType dbType);

        IDbTransaction CreateTransaction();
    }
}
