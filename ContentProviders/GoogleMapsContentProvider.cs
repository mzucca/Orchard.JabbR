﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JabbR.ContentProviders.Core;
using JabbR.Infrastructure;
using JabbR;

namespace JabbR.ContentProviders
{
    public class GoogleMapsContentProvider : EmbedContentProvider
    {
        public override IEnumerable<string> Domains
        {
            get
            {
                yield return "http://maps.google.com";
                yield return "https://maps.google.com";
                yield return "http://maps.google.it";
                yield return "https://maps.google.it";
            }
        }

        public override string MediaFormatString
        {
            get
            {
                return String.Format(@"<iframe width=""500"" height=""350"" frameborder=""0"" scrolling=""no"" marginheight=""0"" marginwidth=""0"" src=""http://maps.google.com/maps?f=q&amp;source=s_q&amp;hl={1}&amp;geocode=&amp;q={0}&amp;aq=0&amp;sll={4}&amp;sspn={5}&amp;vpsrc={6}&amp;ie=UTF8&amp;hq=&amp;hnear={7}&amp;ll={2}&amp;spn={3}&amp;t={8}&amp;z={9}&amp;output=embed""></iframe><br /><small><a href=""http://maps.google.com/maps?f=q&amp;source=embed&amp;hl={{1}}&amp;geocode=&amp;q={{0}}&amp;aq=0&amp;sll={{4}}&amp;sspn={{5}}&amp;vpsrc={{6}}&amp;ie=UTF8&amp;hq=&amp;hnear={{7}}&amp;ll={{2}}&amp;spn={{3}}&amp;t={{8}}&amp;z={{9}}"" style=""color:#0000FF;text-align:left"" target=""_blank"">{0}</a></small>", LanguageResources.ViewLargerMap);
            }
        }

        protected override IList<string> ExtractParameters(Uri responseUri)
        {
            var queryString = new QueryStringCollection(responseUri.Query);
            string query = queryString["q"];
            string hl = queryString["hl"];
            string ll = queryString["ll"];
            string spn = queryString["spn"];
            string sll = queryString["sll"];
            string sspn = queryString["sspn"];
            string vpsrc = queryString["vpsrc"];
            string hnear = queryString["hnear"];
            string t = queryString["t"];
            string z = queryString["z"];

            if (String.IsNullOrEmpty(sll)&& String.IsNullOrEmpty(ll))
            {
                return null;
            }

            return new List<string>() { query, hl, ll, spn, sll, sspn, vpsrc, hnear, t, z };
        }


        protected override Task<ContentProviderResult> GetCollapsibleContent(ContentProviderHttpRequest request)
        {
            return base.GetCollapsibleContent(request).Then(content =>
            {
                var queryString = new QueryStringCollection(request.RequestUri.Query);
                content.Title = queryString["q"] ?? LanguageResources.GoogleMapsContent_DefaultTitle;
                return content;
            });
        }
    }
}