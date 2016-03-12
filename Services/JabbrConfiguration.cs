using System;
using System.Configuration;

namespace JabbR.Services
{
    public class JabbrConfiguration : IJabbrConfiguration
    {
        private readonly ApplicationSettings _settings;
        public JabbrConfiguration(ISettingsManager settingsManager)
        {
            _settings = settingsManager.Load();
        }
        public bool RequireHttps
        {
            get
            {
                string requireHttpsValue = ConfigurationManager.AppSettings["jabbr:requireHttps"];
                bool requireHttps;
                if (Boolean.TryParse(requireHttpsValue, out requireHttps))
                {
                    return requireHttps;
                }
                return false;
            }
        }

        public bool MigrateDatabase
        {
            get
            {
                string migrateDatabaseValue = ConfigurationManager.AppSettings["jabbr:migrateDatabase"];
                bool migrateDatabase;
                if (Boolean.TryParse(migrateDatabaseValue, out migrateDatabase))
                {
                    return migrateDatabase;
                }
                return false;
            }
        }

        public string DeploymentSha { get { return _settings.DeploymentSha; } }

        public string DeploymentBranch { get { return _settings.DeploymentBranch; } }

        public string DeploymentTime { get { return _settings.DeploymentTime; } }

        public string ServiceBusConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["jabbr:serviceBusConnectionString"];
            }
        }

        public string ServiceBusTopicPrefix
        {
            get
            {
                return ConfigurationManager.AppSettings["jabbr:serviceBusTopicPrefix"];
            }
        }

        public bool ScaleOutSqlServer
        {
            get
            {
                string scaleOutSqlServerValue = ConfigurationManager.AppSettings["jabbr:scaleOutSqlServer"];
                bool scaleOutSqlServer;
                if (Boolean.TryParse(scaleOutSqlServerValue, out scaleOutSqlServer))
                {
                    return scaleOutSqlServer;
                }
                return false;
            }
        }

        public ConnectionStringSettings SqlConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Jabbr"];
            }
        }

        public bool EnableAutomaticAnswering
        {
            get { return _settings.EnableAutomaticAnswering; }
            set { _settings.EnableAutomaticAnswering = value; }
        }

        public int RequiredAnswerAccuracy
        {
            get { return _settings.RequiredAnswerAccuracy; }
            set { _settings.RequiredAnswerAccuracy = value; }
        }

        public string SendAnswerAs
        {
            get { return _settings.SendAnswerAs; }
            set { _settings.SendAnswerAs = value; }
        }

        public bool AllowRoomCreation
        {
            get { return _settings.AllowRoomCreation; }
            set { _settings.AllowRoomCreation = value; }
        }

        public string GoogleAnalyticsId
        {
            get { return _settings.GoogleAnalyticsId; }
            set { _settings.GoogleAnalyticsId = value; }
        }

        public string AppInsights
        {
            get { return _settings.AppInsights; }
            set { _settings.AppInsights = value; }
        }

        //TODO ??? duplicated?
        public string GoogleAnalytics
        {
            get { return _settings.GoogleAnalyticsId; }
            set { _settings.GoogleAnalyticsId = value; }
        }

        public bool DebugMode
        {
            get { return _settings.DebugMode; }
            set { _settings.DebugMode = value; }
        }

        public int MaxFileUploadBytes
        {
            get { return _settings.MaxFileUploadBytes; }
            set { _settings.MaxFileUploadBytes = value; }
        }

        public int MaxMessageLength
        {
            get { return _settings.MaxMessageLength; }
            set { _settings.MaxMessageLength = value; }
        }


    }
}