using JabbR.Models;
using JabbR.Services;
using Newtonsoft.Json;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Services;
using Orchard.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JabbR.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IClock _clock;
        protected IJabbrRepository _repository;

        protected readonly ISiteService _siteService;

        public BaseController(IOrchardServices services, IJabbrRepository repository, IClock clock)
        {
            Services = services;
            T = NullLocalizer.Instance;
            //           _repository = repository;//.Repository;
            Logger = NullLogger.Instance;
            //_repositoryManager = repositoryManager;
            _repository = repository;
            _clock = clock;

        }

        public IOrchardServices Services { get; set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public void AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
        protected ClientState ClientState
        {
            get
            {
                // New client state
                var jabbrState = GetCookieValue("jabbr.state");

                ClientState clientState = null;

                if (String.IsNullOrEmpty(jabbrState))
                {
                    clientState = new ClientState();
                }
                else
                {
                    clientState = JsonConvert.DeserializeObject<ClientState>(jabbrState);
                }
                return clientState;
            }
            set
            {
                DateTime now = DateTime.UtcNow;
                string state = JsonConvert.SerializeObject(value);
                state = Uri.EscapeDataString(state);
                var cookie = new HttpCookie("jabbr.state", state)
                {
                    Expires = now.AddDays(30)
                };
                HttpContext.Response.SetCookie(cookie);
            }
        }
        private string GetCookieValue(string key)
        {
            HttpCookie cookie = HttpContext.Request.Cookies.Get(key);
            string value = cookie != null ? cookie.Value : null;
            return value != null ? Uri.UnescapeDataString(value) : null;
        }

    }
}