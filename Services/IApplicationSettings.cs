using Orchard;
using System.Collections.Generic;

namespace JabbR.Services
{
    public interface IApplicationSettings : IDependency
    {
        string EncryptionKey { get; set; }

        string VerificationKey { get; set; }

        string AzureblobStorageConnectionString { get; set; }

        string LocalFileSystemStoragePath { get; set; }

        string LocalFileSystemStorageUriPrefix { get; set; }

        int MaxFileUploadBytes { get; set; }

        int MaxMessageLength { get; set; }

        string GoogleAnalytics { get; set; }

        string AppInsights { get; set; }

        bool AllowUserRegistration { get; set; }

        bool AllowUserResetPassword { get; set; }

        int RequestResetPasswordValidThroughInHours { get; set; }

        bool AllowRoomCreation { get; set; }

        string FacebookAppId { get; set; }

        string FacebookAppSecret { get; set; }

        string TwitterConsumerKey { get; set; }

        string TwitterConsumerSecret { get; set; }

        string GoogleClientID { get; set; }

        string GoogleClientSecret { get; set; }

        string EmailSender { get; set; }

        List<ContentProviderSetting> ContentProviders { get; set; }

        HashSet<string> DisabledContentProviders { get; set; }

    }
}
