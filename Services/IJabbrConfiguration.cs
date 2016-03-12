using Orchard;
using System.Configuration;

namespace JabbR.Services
{
    public interface IJabbrConfiguration :IDependency
    {
        // Marioz
        bool EnableAutomaticAnswering { get; set; }
        int RequiredAnswerAccuracy { get; set; }
        string SendAnswerAs { get; set; }
        bool AllowRoomCreation { get; set; }

        string GoogleAnalyticsId { get; set; }
        string AppInsights { get; set; }


        string GoogleAnalytics { get; set; }
        bool DebugMode { get; set; }

        int MaxFileUploadBytes { get; set; }
        int MaxMessageLength { get; set; }

        // end Marioz

        bool RequireHttps { get; }
        bool MigrateDatabase { get; }

        string DeploymentSha { get; }
        string DeploymentBranch { get; }
        string DeploymentTime { get; }

        string ServiceBusConnectionString { get; }
        string ServiceBusTopicPrefix { get; }

        ConnectionStringSettings SqlConnectionString { get; }
        bool ScaleOutSqlServer { get; }
    }
}