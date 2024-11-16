using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TExample.Models;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Managers;

namespace Tavstal.TExample.Managers
{
    public class DatabaseManager : DatabaseManagerBase
    {
#pragma warning disable IDE1006 //
        private static ExampleConfig _pluginConfig => ExampleMain.Instance.Config;
#pragma warning restore IDE1006 //

        public DatabaseManager(IPlugin plugin, IConfigurationBase config) : base(plugin, config)
        {

        }

        /// <summary>
        /// Checks the schema of the database, creates or modifies the tables if needed
        /// <br/>PS. If you change the Primary Key then you must delete the table.
        /// </summary>
        public override async Task CheckSchemaAsync()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    if (!await connection.OpenSafeAsync())
                        ExampleMain.IsConnectionAuthFailed = true;
                    if (connection.State != System.Data.ConnectionState.Open)
                        throw new Exception("# Failed to connect to the database. Please check the plugin's config file.");

                    // Player Table
                    if (await connection.DoesTableExistAsync<PlayerData>(_pluginConfig.Database.DatabaseTable_Players))
                        await connection.CheckTableAsync<PlayerData>(_pluginConfig.Database.DatabaseTable_Players);
                    else
                        await connection.CreateTableAsync<PlayerData>(_pluginConfig.Database.DatabaseTable_Players);

                    if (connection.State != System.Data.ConnectionState.Closed)
                        await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                ExampleMain.Logger.LogException("Error in checkSchema:");
                ExampleMain.Logger.LogError(ex);
            }
        }

        #region Player Table
        public async Task<bool> AddPlayer(ulong steamId, string steamName, string characterName)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return await MySQLConnection.AddTableRowAsync(tableName: _pluginConfig.Database.DatabaseTable_Players, value: new PlayerData(steamId, steamName, characterName, DateTime.Now));
        }

        public async Task<bool> RemovePlayer(ulong steamId)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return  await MySQLConnection.RemoveTableRowAsync<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, whereClause: $"SteamId='{steamId}'", parameters: null);
        }

        public async Task<bool> UpdatePlayer(ulong steamId, string characterName)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return await MySQLConnection.UpdateTableRowAsync<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, $"SteamId='{steamId}'", new List<SqlParameter>
            {
                SqlParameter.Get<PlayerData>(x => x.LastCharacterName, characterName),
                SqlParameter.Get<PlayerData>(x => x.LastLogin, DateTime.Now)
            });
        }

        public async Task<List<PlayerData>> GetPlayers()
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return await MySQLConnection.GetTableRowsAsync<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, whereClause: string.Empty, null);
        }

        public async Task<PlayerData> FindPlayer(ulong steamId)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return await MySQLConnection.GetTableRowAsync<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, whereClause: $"SteamId='{steamId}'", null);
        }
        #endregion
    }
}
