using AdoPart2.Providers;
using System.Configuration;

namespace AdoPart2.Common
{
    public class DataBaseHandlerFactory
    {
        private ConnectionStringSettings connectionStringSettings;

        public DataBaseHandlerFactory(string connectionStringName)
        {
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        public IDataBaseHandler CreateGDataBase()
        {
            IDataBaseHandler dataBase = null;

            switch (connectionStringSettings.ProviderName.ToLower())
            {
                case "system.data.sqlclient":
                    dataBase = new SqlDataAccess(connectionStringSettings.ConnectionString);
                    break;
            }
            return dataBase;
        }

        public string GetProviderName()
        {
            return connectionStringSettings.ProviderName;
        }
    }


}
