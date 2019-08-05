//-----------------------------------------------------------------------------------
// <copyright file="ConnectionStringHelper.cs" company="Tesco">
//     Copyright © Tesco.com 2012.
// </copyright>
// <summary>ConnectionStringHelper</summary>
//-----------------------------------------------------------------------------------

namespace Json2DB
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.Caching;
    using System.Xml;
    using Microsoft.Win32;    
    using Microsoft.Practices.EnterpriseLibrary.Data;
    using Tesco.Com.Exceptions;    
    


    /// <summary>
    /// Used to retrieve connection string.
    /// </summary>
    internal static class ConnectionStringHelper
    {
        /// <summary>
        /// Registry path to get configuration DB connection
        /// </summary>
        private const string TescoRetailServicesKeyRoot = "Software\\Tesco.com\\RetailServices";

        /// <summary>
        /// Local variable to hold DB server details.
        /// </summary>
        private const string CONFIGURATIONCONNECTIONSTRINGGET = "[configuration].[ConnectionStringGet]";

        /// <summary>
        /// Local variable to hold database name.
        /// </summary>
        private const string CONFIGSDATABASEKEY = "ConfigSettingsDB";

        /// <summary>
        /// Cache object to hold connection string
        /// </summary>
        private static ObjectCache cache = MemoryCache.Default;

        /// <summary>
        /// gets the ApplicationConnectionString based on the connectionStringID
        /// </summary>
        /// <param name="connectionStringID">value of connectionStringID</param>
        /// <returns>application connection   string</returns>
        internal static string GetApplicationConnectionString(string connectionStringID)
        {
            string cacheItemKey = "AppConnStrCache_" + "_" + connectionStringID;
            string connectionString = null;
            object cachedConnectionString = cache[cacheItemKey];
            string connectionStringSettiings;
            if (cachedConnectionString != null)
            {
                connectionString = (string)cachedConnectionString;
            }
            else
            {
                connectionStringSettiings = GetApplicationConnectionStringFromDB(connectionStringID);
                ConnectionStringSettings appConnectionStringSettings = DeserializeConnectionString(connectionStringSettiings);
                connectionString = appConnectionStringSettings.ConnectionString;
                cache[cacheItemKey] = connectionString;
            }

            return connectionString;
        }

        /// <summary>
        /// Reads the application connection   string from Config database
        /// </summary>
        /// <param name="connectionStringID">value of connectionStringID</param>
        /// <returns>application connection   string</returns>
        private static string GetApplicationConnectionStringFromDB(string connectionStringID)
        {
            string configDatabaseConnectionString = null;
            string applicationConnectionString = null;
            configDatabaseConnectionString = GetDatabaseConnectionStringFromRegistry(CONFIGSDATABASEKEY);

            Database database = new Microsoft.Practices.EnterpriseLibrary.Data.Sql.SqlDatabase(configDatabaseConnectionString);

            using (DbCommand cmd = database.GetStoredProcCommand(CONFIGURATIONCONNECTIONSTRINGGET))
            {
                database.AddInParameter(cmd, "@connectionStringKey", DbType.AnsiString, connectionStringID);
                database.AddOutParameter(cmd, "@connectionStringValue", DbType.AnsiString, 8000);
                database.ExecuteNonQuery(cmd);
                applicationConnectionString = (string)database.GetParameterValue(cmd, "@connectionStringValue");
            }

            return applicationConnectionString;
        }

        /// <summary>
        /// Returns connection string from Registry
        /// </summary>
        /// <param name="key">Registry key value</param>
        /// <returns>returns connection string from registry</returns>
        private static string GetDatabaseConnectionStringFromRegistry(string key)
        {
            ConnectionStringSettings configDBConnectionStringSetting = null;

            try
            {
                string result;
                result = GetRegistryValue(key);
                configDBConnectionStringSetting = DeserializeConnectionString(result);
            }
            catch (Exception ex)
            {
                throw new TechnicalException(1001, "Cound not retrieve the Configuration database configuration string from the registry", ex);
            }

            return configDBConnectionStringSetting.ConnectionString.Trim();
        }

        /// <summary>
        /// Reads a registry value
        /// </summary>
        /// <param name="key">registry key value </param>
        /// <returns>registry value</returns>
        private static string GetRegistryValue(string key)
        {
            RegistryKey targetRegKey;
            object settingValue;
            targetRegKey = Registry.LocalMachine.OpenSubKey(TescoRetailServicesKeyRoot);

            if (targetRegKey == null)
            {
                throw new InvalidOperationException(string.Format("Could not open the registry root key'{0}'.", key));
            }

            settingValue = targetRegKey.GetValue(key);

            if (settingValue == null)
            {
                throw new ArgumentException("Specified setting was not found: " + key);
            }
            else
            {
                return settingValue.ToString();
            }
        }

        /// <summary>
        /// Creates a ConnectionStringSettings object from an XML representation.
        /// </summary>
        /// <param name="connStringXml">XML string containing the properties of the <see cref="System.Configuration.ConnectionStringSettings"/>.  
        /// Normally this will be a string that was generated by <see cref="SerializeConnectionString"/>.</param>
        /// <returns>A new ConnectionStringSettings object with the properties specified in <paramref name="connStringXml"/>.</returns>
        private static ConnectionStringSettings DeserializeConnectionString(string connStringXml)
        {
            try
            {
                XmlDocument deserializedConnStr = new XmlDocument();
                deserializedConnStr.LoadXml(connStringXml);
                string name, connStr, providerName;
                name = deserializedConnStr.ChildNodes[0].ChildNodes[0].InnerText;
                connStr = deserializedConnStr.ChildNodes[0].ChildNodes[1].InnerText;
                providerName = deserializedConnStr.ChildNodes[0].ChildNodes[2].InnerText;
                ConnectionStringSettings result;
                if (providerName.Length == 0)
                {
                    result = new ConnectionStringSettings(name, connStr);
                }
                else
                {
                    result = new ConnectionStringSettings(name, connStr, providerName);
                }

                return result;
            }
            catch (XmlException ex)
            {
                throw new TechnicalException(1002, "An error occurred while trying to read the connection string's XML representation.", ex);
            }
        }
    }
}
