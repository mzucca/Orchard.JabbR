using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.ViewModels
{
    public enum LoginErrorCode
    {
        Ok,
        AlreadyAuthenticated,
        InvalideUserName,
        InvalidPassword,
        UnexepectedError
    }
}