using System.Collections.Generic;
using JabbR.Models;
using JabbR.Services;
using SimpleAuthentication.Core;

namespace JabbR.ViewModels
{
    public class LoginViewModel
    {
        public LoginViewModel(IApplicationSettings settings, IEnumerable<IAuthenticationProvider> configuredProviders, IEnumerable<ChatUserIdentity> userIdentities)
        {
            SocialDetails = new SocialLoginViewModel(configuredProviders, userIdentities);
            if (settings != null)
            {
                AllowUserRegistration = settings.AllowUserRegistration;
                AllowUserResetPassword = settings.AllowUserResetPassword;
            }
            else // TODO non dovrebbero mai esserlo
            {
                AllowUserRegistration = true;
                AllowUserResetPassword = true;
            }
            HasEmailSender = !string.IsNullOrWhiteSpace(settings.EmailSender);
        }

        public bool AllowUserRegistration { get; set; }
        public bool AllowUserResetPassword { get; set; }
        public bool HasEmailSender { get; set; }
        public SocialLoginViewModel SocialDetails { get; private set; }
    }
}