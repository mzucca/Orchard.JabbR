using Orchard;
using Orchard.Core.Contents.Controllers;
using Orchard.DisplayManagement;
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
using Orchard.Logging;

using JabbR.Infrastructure.Services;
using JabbR.Models;
using Orchard.Localization;
using JabbR.ViewModels;

namespace JabbR.Controllers
{
    [ValidateInput(false), Admin]
    public class QuestionAnswerController : Controller
    {
        private readonly IChatQuestionsService _questionsService;
        private readonly ISiteService _siteService;

        public QuestionAnswerController(IChatQuestionsService questionsService,
            IOrchardServices orchardServices,
            IShapeFactory shapeFactory,
            ISiteService siteService)
        {
            _questionsService = questionsService;
            _siteService = siteService;
            Services = orchardServices;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }
        protected dynamic Shape { get; set; }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        public IOrchardServices Services { get; set; }

        public ActionResult Index(IndexOptions options, PagerParameters pagerParameters)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list department")))
                return new HttpUnauthorizedResult();

            var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
            // default options
            if (options == null)
            {
                options = new IndexOptions();
            }

            var questions = _questionsService.Get();

            if (!String.IsNullOrWhiteSpace(options.Search))
            {
                questions = questions.Where(u => u.Name.Contains(options.Search) || u.Question.Contains(options.Search));
            }

            var pagerShape = Shape.Pager(pager).TotalItemCount(questions.Count());

            switch (options.Order)
            {
                case Order.Name:
                    questions = questions.OrderBy(u => u.Name);
                    break;
                case Order.Description:
                    questions = questions.OrderBy(u => u.Question);
                    break;
            }

            var results = questions
                // TODO
                //.Slice(pager.GetStartIndex(), pager.PageSize)
                .ToList();

            var model = new IndexViewModel<QuestionRecord>
            {
                Items = results
                    .Select(x => new Entry<QuestionRecord> { Item = x })
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

            var viewModel = new IndexViewModel<QuestionRecord> { Items = new List<Entry<QuestionRecord>>(), Options = new IndexOptions() };
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
            var questionRecord = new QuestionRecord();
            questionRecord.Active = true;

            return View(questionRecord);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(QuestionRecord questionRecord)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage rooms")))
                return new HttpUnauthorizedResult();
            if (questionRecord == null)
            {
                return HttpNotFound();
            }

            try
            {
                _questionsService.Add(questionRecord);
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

        public ActionResult Edit(int id = 0)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();

            var room = (from r in _questionsService.Get()
                        where r.Id == id
                        select r).FirstOrDefault();

            if (room == null)
            {
                return HttpNotFound(string.Format("Question with id={0} cannot be found", id));
            }

            return View(room);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(QuestionRecord questionRecord)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();
            if (questionRecord == null)
            {
                return HttpNotFound();
            }

            _questionsService.Update(questionRecord);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage room")))
                return new HttpUnauthorizedResult();

            var questionRecord = (from r in _questionsService.Get()
                        where r.Id == id
                        select r).FirstOrDefault();

            if (questionRecord == null)
            {
                return HttpNotFound(string.Format("Room with id={0} cannot be found", id));
            }

            _questionsService.Remove(questionRecord);
            return RedirectToAction("Index");
        }

    }
}