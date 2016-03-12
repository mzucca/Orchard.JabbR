using JabbR.Models;
using JabbR.Services;
using JabbR.ViewModels;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.Environment.Extensions;

namespace JabbR.Controllers
{
    [OrchardFeature("JabbR")]
    public class NotificationsController : Controller
    {
        private IClock _clock;
        private IJabbrRepository _repository;
        private readonly IAuthenticationService _authenticationService;
        private INotificationService _notificationServices;

        public NotificationsController(IAuthenticationService authenticationService,
            IJabbrRepository repository,
            INotificationService notificationServices,
            IOrchardServices services,
            IClock clock,
            ICache cache)
        {
            Services = services;
            _authenticationService = authenticationService;
            _notificationServices = notificationServices;
            T = NullLocalizer.Instance;
           _repository = repository;
            Logger = NullLogger.Instance;
            _clock = clock;
        }
        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Index()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();
            var request = new NotificationRequestModel();
            var all = Request.Params["all"];
            if (!string.IsNullOrEmpty(all))
            {
                bool allV=false;
                if (bool.TryParse(all, out allV))
                    request.All = allV;
             }

            ChatUser user = _repository.GetUserById(currentUser.UserName);
            int unreadCount = _repository.GetUnreadNotificationsCount(user);
            IPagedList<NotificationViewModel> notifications = GetNotifications(_repository, user, all: request.All, page: request.Page, roomName: request.Room);

            var viewModel = new NotificationsViewModel
            {
                ShowAll = request.All,
                UnreadCount = unreadCount,
                Notifications = notifications,
            };
            return View("Notifications",viewModel);

        }
        [HttpPost]
        public JsonResult MarkAllAsRead()
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser==null)
            {
                return Json(new { Success = false, Message = T("Forbidden").Text });
            }

            ChatUser user = _repository.GetUserById(currentUser.UserName);
            IList<Notification> unReadNotifications = _repository.GetNotificationsByUser(user).Unread().ToList();

            if (!unReadNotifications.Any())
            {
                return Json(new { Success = false, Message=T("No unread notifications").Text });
            }

            foreach (var notification in unReadNotifications)
            {
                notification.Read = true;
            }

            _repository.CommitChanges();

            // TODO
            //UpdateUnreadCountInChat(repository, notificationService, user);

            return Json(new { Success = true });

        }
        [HttpPost]
        public JsonResult MarkAsRead(int notificationId)
        {
            var currentUser = _authenticationService.GetAuthenticatedUser();

            if (currentUser == null)
            {
                return Json(new { Success = false, Message = T("Forbidden").Text });
            }
            Notification notification = _repository.GetNotificationById(notificationId);

            if (notification == null)
            {
                return Json(new { Success = false, Message = T("Notification not found").Text });
            }

            ChatUser user = _repository.GetUserById(currentUser.UserName);
            if(user==null)
                return Json(new { Success = false, Message = T("User not found").Text });

            if (notification.UserKey != user.Key)
            {
                return Json(new { Success = false, Message = T("Invalid notificationId for user").Text });
            }
            UpdateUnreadCountInChat(_repository, user);//, notificationService);

            notification.Read = true;
            _repository.CommitChanges();


            // TODO
            //UpdateUnreadCountInChat(repository, notificationService, user);

            return Json(new { Success = true });

        }
        private static void UpdateUnreadCountInChat(IJabbrRepository repository,ChatUser user)//, IChatNotificationService notificationService)
        {
            var unread = repository.GetUnreadNotificationsCount(user);
            //TODO
            //notificationService.UpdateUnreadMentions(user, unread);
        }

        private static IPagedList<NotificationViewModel> GetNotifications(IJabbrRepository repository, ChatUser user, bool all = false,
                                                                  int page = 1, int take = 20, string roomName = null)
        {
            IQueryable<Notification> notificationsQuery = repository.GetNotificationsByUser(user);

            if (!all)
            {
                notificationsQuery = notificationsQuery.Unread();
            }

            if (!String.IsNullOrWhiteSpace(roomName))
            {
                var room = repository.VerifyRoom(roomName);

                if (room != null)
                {
                    notificationsQuery = notificationsQuery.ByRoom(roomName);
                }
            }

            return notificationsQuery.OrderByDescending(n => n.Message.When)
                .Select(n => new NotificationViewModel()
                {
                    NotificationKey = n.Key,
                    FromUserName = n.Message.User.Name,
                    FromUserImage = n.Message.User.Hash,
                    Message = n.Message.Content,
                    HtmlEncoded = n.Message.HtmlEncoded,
                    RoomName = n.Room.Name,
                    Read = n.Read,
                    When = n.Message.When
                })
                .ToPagedList(page, take);
        }

        private class NotificationRequestModel
        {
            public NotificationRequestModel()
            {
                All = false;
                Page = 1;
                Room = null;
            }

            public bool All { get; set; }
            public int Page { get; set; }
            public string Room { get; set; }
        }
    }
}