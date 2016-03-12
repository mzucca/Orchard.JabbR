using System.Collections.Generic;

using SimpleAuthentication.Core;

namespace JabbR.Infrastructure
{
    public interface IJabbRAuthenticationService
    {
        IEnumerable<IAuthenticationProvider> GetProviders();
    }
}