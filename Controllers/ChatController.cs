using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using JabbR.Models;
using JabbR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JabbR.ViewModels;
using Orchard.Mvc;
using Orchard.MediaLibrary.Services;
using Orchard.Mvc.Extensions;
using JabbR.Infrastructure;
using Microsoft.Security.Application;
using System.Globalization;
using Microsoft.AspNet.SignalR.Infrastructure;
using Orchard.FileSystems.VirtualPath;
using Orchard.Environment.Configuration;

namespace JabbR.Controllers
{
    public class ChatController : Controller
    {
        private IClock _clock;
        private IJabbrRepository _repository;
        private readonly Orchard.Security.IAuthenticationService _authenticationService;
        private readonly IMediaLibraryService _mediaService;
        private const string JabbRUploadContainer = "JabbR-uploads";
        private readonly ApplicationSettings _settings;
		private IConnectionManager _connectionManager;
        private readonly ShellSettings _orchardSettings;


		public ChatController(Orchard.Security.IAuthenticationService authenticationService,
            ISettingsManager applicationSettings,
            IJabbrRepository repository,
            IOrchardServices services,
            IMediaLibraryService mediaService,
            IClock clock,
            ICache cache,
            IShellSettingsManager settingsManager)//, IJabbrRepository repository)
        {
            _settings = applicationSettings.Load();
            Services = services;
            _authenticationService = authenticationService;
            T = NullLocalizer.Instance;
           _repository = repository;
            Logger = NullLogger.Instance;
            _clock = clock;
            _mediaService = mediaService;
            _orchardSettings = settingsManager.LoadSettings().FirstOrDefault();

        }
		public ChatController(Orchard.Security.IAuthenticationService authenticationService,
			ISettingsManager applicationSettings,
			IJabbrRepository repository,
			IOrchardServices services,
			IMediaLibraryService mediaService,
			IClock clock,
			ICache cache,
			IConnectionManager connectionManager,
			IVirtualPathProvider pathProvider,
            IShellSettingsManager settingsManager)//, IJabbrRepository repository)
		{
            _orchardSettings = settingsManager.LoadSettings().FirstOrDefault();
			_settings = applicationSettings.Load();
			Services = services;
			_authenticationService = authenticationService;
			T = NullLocalizer.Instance;
			_repository = repository;
			Logger = NullLogger.Instance;
			_clock = clock;
			_mediaService = mediaService;
			_connectionManager = connectionManager;
            //LanguageResources.ResourceManager =
            //    ResourceManager.CreateFileBasedResourceManager("LanguageResources",
            //    pathProvider.MapPath("~/Modules/JabbR/bin"),
            //    null);

        }
		public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public Orchard.Logging.ILogger Logger { get; set; }
        public ActionResult Index()
        {

			Logger.Debug("JabbRController.Index -- Enter");
			var currentUser = _authenticationService.GetAuthenticatedUser();

            //System.Reflection.Assembly assembly = typeof(LanguageResources).Assembly;
            //string[] names = assembly.GetManifestResourceNames();
            if (LanguageResources.ResourceManager==null)
            {
                Logger.Error("VisitorController.Index invalid resource manifest");
            }

			if (currentUser == null)
            {
                Logger.Information("Access denied to anonymous request ");
                var shape = Services.New.LogOn().Title(T("Access Denied").Text);
                return new ShapeResult(this, shape);
            }
			// Check Repository
			Logger.Debug("JabbRController.Index -- Check Repository");
			var user = _repository.GetUserByName(currentUser.UserName);
            if ((user == null) || (string.IsNullOrEmpty(user.Id)))
            {
                // TODO verificare se l'utente si è già collegato prima
				Logger.Debug("JabbRController.Index -- User not found creating a new one");
                user = new ChatUser();
                user.Id = currentUser.UserName;
                user.Name = currentUser.UserName;
                user.Email = currentUser.Email;
                user.LastActivity = DateTime.UtcNow;
                user.Status = (int)UserStatus.Active;
                _repository.Add(user);
            }

            JabbR.ViewModels.SettingsViewModel viewModel = new SettingsViewModel
            {
                GoogleAnalytics = "", //_settings.GoogleAnalytics,
                Sha = _settings.DeploymentSha,
                Branch = _settings.DeploymentBranch,
                Time = _settings.DeploymentTime,
                DebugMode = true, // TODO mzu (bool)Context.Items["_debugMode"],
                Version = Constants.JabbRVersion,
                IsAdmin = user.IsAdmin,
                ClientLanguageResources = BuildClientResources()
            };

			//// TODO autenticazione
			//var owinContext = new OwinContext();

			//var identity = new ClaimsIdentity(claims, Constants.JabbRAuthType);
			//owinContext.Authentication.SignIn(identity);
			//// END TDO
			Logger.Debug("JabbRController.Index -- Completed");

			return View(viewModel);
            //}

            //return RedirectToAction(String.Format("~/account/login?returnUrl={0}", HttpUtility.UrlEncode(Request.Path)));
        }
		public ActionResult Status()
		{
			var model = new StatusViewModel();
            model.Systems.Add(
                new SystemStatus {
                    SystemName = "OperatingSystem",
                    StatusMessage = Environment.OSVersion.VersionString,
                    IsOK=true });

            model.Systems.Add(
                new SystemStatus {
                    SystemName = "ASP.NET Version",
                    StatusMessage = System.Runtime.InteropServices.RuntimeEnvironment.GetSystemVersion(),
                    IsOK=true });

            model.Systems.Add(
                new SystemStatus {
                    SystemName = "Is Full Trust",
                    StatusMessage = AppDomain.CurrentDomain.IsFullyTrusted.ToString(),
                    IsOK=true });

            model.Systems.Add(
                new SystemStatus {
                    SystemName = "Dataprovider",
                    StatusMessage = _orchardSettings.DataProvider,
                    IsOK=true });

            model.Systems.Add(
                new SystemStatus {
                    SystemName = "EncryptionAlgorithm",
                    StatusMessage = _orchardSettings.EncryptionAlgorithm,
                    IsOK=true });

            model.Systems.Add(
                new SystemStatus {
                    SystemName = "Tenant State",
                    StatusMessage = _orchardSettings.State.ToString(),
                    IsOK=true });

			// Try to send a message via SignalR
			// NOTE: Ideally we'd like to actually receive a message that we send, but right now
			// that would require a full client instance. SignalR 2.1.0 plans to add a feature to
			// easily support this on the server.

            var signalrStatus = new SystemStatus { SystemName = "SignalR messaging" };
			model.Systems.Add(signalrStatus);

			try
			{
				var hubContext = _connectionManager.GetHubContext<JabbR.Hubs.Chat>();
				hubContext.Clients.Client("doesn't exist").noMethodCalledThis();

				signalrStatus.SetOK();
			}
			catch (Exception ex)
			{
				signalrStatus.SetException(ex.GetBaseException());
			}

			// Try to talk to database
			var dbStatus = new SystemStatus { SystemName = "Database" };
			model.Systems.Add(dbStatus);

			try
			{
				var roomCount = _repository.Rooms.Count();
				dbStatus.SetOK();
			}
			catch (Exception ex)
			{
				dbStatus.SetException(ex.GetBaseException());
			}

			// Try to talk to azure storage
			var azureStorageStatus = new SystemStatus { SystemName = "Azure Upload storage" };
			model.Systems.Add(azureStorageStatus);

			//try
			//{
			//	if (!String.IsNullOrEmpty(settings.AzureblobStorageConnectionString))
			//	{
			//		var azure = new AzureBlobStorageHandler(settings);
			//		UploadResult result;
			//		using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test")))
			//		{
			//			result = await azure.UploadFile("statusCheck.txt", "text/plain", stream);
			//		}

			//		azureStorageStatus.SetOK();
			//	}
			//	else
			//	{
			//		azureStorageStatus.StatusMessage = "Not configured";
			//	}
			//}
			//catch (Exception ex)
			//{
			//	azureStorageStatus.SetException(ex.GetBaseException());
			//}

			//try to talk to local storage
			var localStorageStatus = new SystemStatus { SystemName = "Local Upload storage" };
			model.Systems.Add(localStorageStatus);

			//try
			//{
			//	if (!String.IsNullOrEmpty(settings.LocalFileSystemStoragePath) && !String.IsNullOrEmpty(settings.LocalFileSystemStorageUriPrefix))
			//	{
			//		var local = new LocalFileSystemStorageHandler(settings);
			//		UploadResult localResult;
			//		using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test")))
			//		{
			//			localResult = await local.UploadFile("statusCheck.txt", "text/plain", stream);
			//		}

			//		localStorageStatus.SetOK();
			//	}
			//	else
			//	{
			//		localStorageStatus.StatusMessage = "Not configured";
			//	}
			//}
			//catch (Exception ex)
			//{
			//	localStorageStatus.SetException(ex.GetBaseException());
			//}

			// Force failure
			//if (Context.Request.Query.fail)
			//{
			//	var failedSystem = new SystemStatus { SystemName = "Forced failure" };
			//	failedSystem.SetException(new ApplicationException("Forced failure for test purposes"));
			//	model.Systems.Add(failedSystem);
			//}

			var view = View(model);

			if (!model.AllOK)
			{
				//throw new ArgumentException();
				//return view.WithStatusCode(HttpStatusCode.InternalServerError);
			}

			return view;
		}

		public ActionResult Monitor()
        {
            //ClaimsPrincipal principal = System.Security.Principal;

            //if (principal == null ||
            //    !principal.HasClaim(JabbRClaimTypes.Admin))
            //{
            //    return null; // 403;
            //}
            return View();
        }
        [HttpPost]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            if (currentUser == null)
            {
                return Json(T("Forbidden"));
            }
            try
            {
                _mediaService.GetMediaFiles(JabbRUploadContainer);
            }
            catch //media api needs a little work, like everything else of course ;) <- ;) == my stuff included. to clarify I need a way to know if the path exists or have UploadMediaFile create paths as necessary but there isn't the time to hook that up in the near future
            {
                _mediaService.CreateFolder("", JabbRUploadContainer);
            }

            if(files.Count()>1)
                return Json(T("No more than one file"));

            var file = Request.Files[0];
            try
            {
                string rurl = _mediaService.UploadMediaFile(JabbRUploadContainer, Request.Files[0]);
                // TODO controllare se url !=null
                return Json(rurl + "|" + Url.MakeAbsolute(rurl));
            }
            catch (ArgumentException e)
            {
                return Json("Cannot upload file: "+file.FileName+" "+e.Message);
            }
        }

        private static string BuildClientResources()
        {
            var resourcesToEmbed = new string[]
            {
                "Content_HeaderAndToggle",
                "Chat_YouEnteredRoom",
                "Chat_UserLockedRoom",
                "Chat_RoomNowLocked",
                "Chat_RoomNowClosed",
                "Chat_RoomNowOpen",
                "Chat_UserEnteredRoom",
                "Chat_UserNameChanged",
                "Chat_UserGravatarChanged",
                "Chat_YouGrantedRoomAccess",
                "Chat_UserGrantedRoomAccess",
                "Chat_YourRoomAccessRevoked",
                "Chat_YouRevokedUserRoomAccess",
                "Chat_UserGrantedRoomOwnership",
                "Chat_UserRoomOwnershipRevoked",
                "Chat_YouGrantedRoomOwnership",
                "Chat_YourRoomOwnershipRevoked",
                "Chat_YourGravatarChanged",
                "Chat_YouAreAfk",
                "Chat_YouAreAfkNote",
                "Chat_YourNoteSet",
                "Chat_YourNoteCleared",
                "Chat_UserIsAfk",
                "Chat_UserIsAfkNote",
                "Chat_UserNoteSet",
                "Chat_UserNoteCleared",
                "Chat_YouSetRoomTopic",
                "Chat_YouClearedRoomTopic",
                "Chat_UserSetRoomTopic",
                "Chat_UserClearedRoomTopic",
                "Chat_YouSetRoomWelcome",
                "Chat_YouClearedRoomWelcome",
                "Chat_YouSetFlag",
                "Chat_YouClearedFlag",
                "Chat_UserSetFlag",
                "Chat_UserClearedFlag",
                "Chat_YourNameChanged",
                "Chat_UserPerformsAction",
                "Chat_PrivateMessage",
                "Chat_UserInvitedYouToRoom",
                "Chat_YouInvitedUserToRoom",
                "Chat_UserNudgedYou",
                "Chat_UserNudgedRoom",
                "Chat_UserNudgedUser",
                "Chat_UserLeftRoom",
                "Chat_YouKickedFromRoom",
                "Chat_NoRoomsAvailable",
                "Chat_RoomUsersHeader",
                "Chat_RoomUsersEmpty",
                "Chat_RoomSearchEmpty",
                "Chat_RoomSearchResults",
                "Chat_RoomNotPrivateAllowed",
                "Chat_RoomPrivateNoUsersAllowed",
                "Chat_RoomPrivateUsersAllowedResults",
                "Chat_UserNotInRooms",
                "Chat_UserInRooms",
                "Chat_UserOwnsNoRooms",
                "Chat_UserOwnsRooms",
                "Chat_UserAdminAllowed",
                "Chat_UserAdminRevoked",
                "Chat_YouAdminAllowed",
                "Chat_YouAdminRevoked",
                "Chat_AdminBroadcast",
                "Chat_CannotSendLobby",
                "Chat_InitialMessages",
                "Chat_UserOwnerHeader",
                "Chat_UserHeader",
                "Content_DisabledMessage",
                "Chat_DefaultTopic",
                "Client_ConnectedStatus",
                "Client_Transport",
                "Client_Uploading",
                "Client_Rooms",
                "Client_OtherRooms",
                "Chat_ExpandHiddenMessages",
                "Chat_CollapseHiddenMessages",
                "Client_Connected",
                "Client_Reconnecting",
                "Client_Disconnected",
                "Client_AdminTag",
                "Client_OccupantsZero",
                "Client_OccupantsOne",
                "Client_OccupantsMany",
                "LoadingMessage",
                "Client_LoadMore",
                "Client_UploadingFromClipboard"
            };
            var resourceManager = LanguageResources.ResourceManager;
			if (resourceManager == null)
			{
				//Logger.Error("ChatController::BuildResources - ResourceManager cannot be null");
			}
            return String.Join(",", resourcesToEmbed.Select(e => string.Format("'{0}': {1}", e, Encoder.JavaScriptEncode(resourceManager.GetString(e,CultureInfo.InvariantCulture)))));
        }

    }
}