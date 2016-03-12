using JabbR.Models;
using JabbR.Services;
using JabbR.ViewModels;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using System;
using System.Web.Mvc;
using Orchard.Mvc.Extensions;
using JabbR.Infrastructure;

namespace JabbR.Controllers
{
    public class AccountController : Controller
    {
        private IClock _clock;
        private IJabbrRepository _repository;
        private readonly Orchard.Security.IAuthenticationService _authenticationService;
        private readonly Orchard.Security.IMembershipService _membershipService;
        private readonly IApplicationSettings _settings;

        public AccountController(
            Orchard.Security.IAuthenticationService authenticationService,
            IApplicationSettings settings,
            IJabbrRepository repository,
            IOrchardServices services,
            IClock clock,
            Orchard.Security.IMembershipService membershipService)
        {
            Services = services;
            _settings = settings;
            _authenticationService = authenticationService;
            T = NullLocalizer.Instance;
            _repository = repository;
            Logger = NullLogger.Instance;
            _clock = clock;
            _membershipService = membershipService;
        }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public Orchard.Logging.ILogger Logger { get; set; }

        int MinPasswordLength
        {
            get
            {
                return _membershipService.GetSettings().MinRequiredPasswordLength;
            }
        }
        public ActionResult Index()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser == null)
                Redirect("~/");

            ChatUser user = _repository.GetUserById(currentUser.UserName);
            if (user == null)
                Redirect("~/");

            UserViewModel model = new UserViewModel(user);
            return View("Account", model);
        }
        public ActionResult Logout()
        {
            //TODO
            return View("login");
        }
        public ActionResult Login()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser != null)
                Redirect("~/visitor");

            ChatUser user = _repository.GetUserById(currentUser.UserName);
            if (user != null)
                Redirect("~/visitor");

            return View("login", GetLoginViewModel());
        }

        [HttpPost, ActionName("Login")]
        public JsonResult LoginPOST()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser != null)
            {
                ErrorViewModel err = new ErrorViewModel() { ErrorCode = LoginErrorCode.Ok};
                return Json(err);
            }
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string errorMessage = LanguageResources.Authentication_GenericFailure;

            if (String.IsNullOrEmpty(username))
            {
                ErrorViewModel err = new ErrorViewModel() { ErrorCode = LoginErrorCode.InvalideUserName, ErrorMessage = LanguageResources.Authentication_NameRequired };
                return Json(err);
            }

            if (String.IsNullOrEmpty(password))
            {
                ErrorViewModel err = new ErrorViewModel() { ErrorCode = LoginErrorCode.InvalideUserName, ErrorMessage = LanguageResources.Authentication_PassRequired };
                return Json(err);
            }

            try
            {
                IUser user = _membershipService.ValidateUser(username, password);
                if (user != null)
                {
                    ErrorViewModel err = new ErrorViewModel() { ErrorCode = LoginErrorCode.Ok };
                    return Json(err);
                }
                else
                {
                    ErrorViewModel err = new ErrorViewModel() { ErrorCode = LoginErrorCode.InvalideUserName,ErrorMessage = LanguageResources.Authentication_NotLoggedIn };
                    return Json(err);
                }
            }
            catch(Exception exc)
            {
                Logger.Error("AccountController.LoginPOST error " + exc.Message);
                ErrorViewModel err = new ErrorViewModel() { ErrorCode = LoginErrorCode.UnexepectedError, ErrorMessage = exc.Message };
                return Json(err);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser == null)
                Redirect("~/");
            string currentPassword = Request.Form["password"];
            string newPassword = Request.Form["newPassword"];
            string confirmPassword = Request.Form["confirmPassword"];

            try
            {
                var validated = _membershipService.ValidateUser(User.Identity.Name, currentPassword);

                if (validated != null)
                {
                    _membershipService.SetPassword(validated, newPassword);
                    //foreach (var userEventHandler in _userEventHandlers)
                    //{
                    //    userEventHandler.ChangedPassword(validated);
                    //}
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("_FORM",
                                         T("The current password is incorrect or the new password is invalid."));
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("_FORM", T("The current password is incorrect or the new password is invalid."));
                return RedirectToAction("Index");
            }

        }

        private LoginViewModel GetLoginViewModel()
        {
            ChatUser user = null;

            var currentUser = _authenticationService.GetAuthenticatedUser();
            //if (currentUser == null)
            //    Redirect("~/");

            if(currentUser !=null)
                user = _repository.GetUserById(currentUser.UserName);
            //if (user == null)
            //    Redirect("~/");

            // TODO gestire i social network
            //var viewModel = new LoginViewModel(applicationSettings, authService.GetProviders(), user != null ? user.Identities : null);
            var viewModel = new LoginViewModel(_settings, null, user != null ? user.Identities : null);
            return viewModel;
        }
    }
}