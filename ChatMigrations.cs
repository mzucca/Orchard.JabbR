using System;
using System.Linq;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using JabbR.Models;
using Orchard.Environment.Configuration;
using System.Transactions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using Orchard.Logging;
using JabbR.Models.Migrations;
namespace JabbR
{
    [OrchardFeature("JabbR")]
    public class ChatMigrations : DataMigrationImpl
    {
        private readonly ShellSettings _settings;
        private const string SqlClient = "System.Data.SqlClient";

        public ChatMigrations(IShellSettingsManager settingsManager)
        {
            _settings = settingsManager.LoadSettings().FirstOrDefault();
        }
        public ILogger Logger { get; set; }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(QuestionRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull().WithLength(20))
                    .Column<string>("Question", column => column.NotNull().Unlimited().WithDefault(""))
                    .Column<string>("Answer", column => column.NotNull().Unlimited().WithDefault(""))
                    .Column<int>("Delay", column => column.NotNull().WithDefault(3))
                    .Column<bool>("Active", column => column.NotNull().WithDefault(true))
                );


            using (new TransactionScope(TransactionScopeOption.Suppress))
            {
                try
                {
                    var dataProvider = _settings.DataProvider;
                    var connectionString = _settings.DataConnectionString;

                    //if (String.IsNullOrEmpty(connectionString.ProviderName) ||
                    //    !connectionString.ProviderName.Equals(SqlClient, StringComparison.OrdinalIgnoreCase))
                    //{
                    //    return;
                    //}

                    Database.SetInitializer<JabbrContext>(null);

                    // Only run migrations for SQL server (Sql ce not supported as yet)
                    var settings = new MigrationsConfiguration();
                    settings.TargetDatabase = new DbConnectionInfo(connectionString, SqlClient);
                    var migrator = new DbMigrator(settings);
                    migrator.Update();
                }
                catch (Exception exc)
                {
                    Logger.Error("ChatMigrations.Create error:{0}", exc.Message);
                    return 0;
                }

            }
            return 1;
        }
    }
}
