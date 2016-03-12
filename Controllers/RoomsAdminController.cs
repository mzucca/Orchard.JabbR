using Orchard;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;
using Orchard.Settings;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.UI.Notify;
using JabbR.Services;
using JabbR.Models;
using JabbR.ViewModels;
using Orchard.Logging;

namespace JabbR.Controllers
{
    [ValidateInput(false), Admin]
    [OrchardFeature("JabbR")]
    public class RoomsAdminController : BaseChatAdminController
    {
        public RoomsAdminController(
            IOrchardServices services,
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IJabbrRepository repository)
            : base(services, shapeFactory, siteService, repository)
        {
        }

        public ActionResult Index(IndexOptions options, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list room")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            // default options
            if (options == null)
                options = new IndexOptions();


            var rooms = _repository.Rooms; // Services.ContentManager.Query<ChatRoomPart, ChatRoomRecord>();

            if (!String.IsNullOrWhiteSpace(options.Search))
            {
                rooms = rooms.Where(u => u.Name.Contains(options.Search) || u.Topic.Contains(options.Search));
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(rooms.Count());

            switch (options.Order)
            {
                case Order.Name:
                    rooms = rooms.OrderBy(u => u.Name);
                    break;
                case Order.Description:
                    rooms = rooms.OrderBy(u => u.Topic);
                    break;
            }

            var results = rooms.ToList();

                //.Skip(pager.GetStartIndex())
                //.Take(pager.PageSize)
                //.ToList();

            var model = new IndexViewModel<ChatRoom>
            {
                Items = results
                    .Select(x => new Entry<ChatRoom> { Item = x })
                    .ToList(),
                Options = options,
                Pager = pagerShape
            };

            // maintain previous route data when generating page links
            var routeData = new RouteData();
            routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.Search", options.Search);
            routeData.Values.Add("Options.Order", options.Order);

            pagerShape.RouteData(routeData);

            return View(model);
        }
        [HttpPost]
        [Orchard.Mvc.FormValueRequired("submit.BulkEdit")]
        public ActionResult Index(FormCollection input)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage rooms")))
                return new HttpUnauthorizedResult();

            var viewModel = new IndexViewModel<ChatRoom> { Items = new List<Entry<ChatRoom>>(), Options = new IndexOptions() };
            UpdateModel(viewModel);

            var checkedEntries = viewModel.Items.Where(c => c.IsChecked);
            switch (viewModel.Options.BulkAction)
            {
                case BulkAction.None:
                    break;
                case BulkAction.Disable:
                    foreach (var entry in checkedEntries)
                    {
                        //    Moderate(entry.User.Id);
                    }
                    break;
                case BulkAction.Delete:
                    foreach (var entry in checkedEntries)
                    {
                        //   Delete(entry.User.Id);
                    }
                    break;
            }

            return RedirectToAction("Index", ControllerContext.RouteData.Values);
        }

        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();
            var room = new ChatRoom();

            return View(room);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(ChatRoom chatRoom)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage rooms")))
                return new HttpUnauthorizedResult();
            if (chatRoom == null)
            {
                return HttpNotFound();
            }

            try
            {
                _repository.Add(chatRoom);
                Services.Notifier.Information(T("Room created"));
            }
            catch (Exception exc)
            {
                // TODO
                Logger.Error(exc, "Cannot create room");
                Services.Notifier.Error(T("Room NOT created:"));
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id=0)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();

            //var room = Services.ContentManager.Get<ChatRoomPart>(id, VersionOptions.Latest);

            var room = (from r in _repository.Rooms
                        where r.Key == id
                        select r).FirstOrDefault();

            if (room == null)
            {
                return HttpNotFound(string.Format("Room with id={0} cannot be found",id));
            }

            // dynamic model = Services.ContentManager.BuildEditor(room);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            //return View((object)model);
            return View(room);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(ChatRoom chatRoom)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();
            if (chatRoom == null)
            {
                return HttpNotFound();
            }

            //TODO Update ???
//            _repository.Update(chatRoom);

            //var room = Services.ContentManager.Get<ChatRoomPart>(id, VersionOptions.Latest);
            //if (room == null)
            //{
            //    return HttpNotFound();
            //}

            //dynamic model = Services.ContentManager.UpdateEditor(room, this);

            //if (!ModelState.IsValid)
            //{
            //    Services.TransactionManager.Cancel();

            //    // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            //    return View((object)model);
            //}

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id=0)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();

            var room = (from r in _repository.Rooms
                        where r.Key == id
                        select r).FirstOrDefault();

            if (room == null)
            {
                return HttpNotFound(string.Format("Room with id={0} cannot be found", id));
            }

            _repository.Remove(room);
            return RedirectToAction("Index");
        }
    }


}