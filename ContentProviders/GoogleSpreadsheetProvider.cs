using System;
using System.Collections.Generic;
using System.Web;
using JabbR.ContentProviders.Core;

namespace JabbR.ContentProviders
{
    public class GoogleDocumentProvider : EmbedContentProvider
    {
        public override string MediaFormatString
        {
            get
            {
                return @"<iframe src=""https://docs.google.com/spreadsheet/ccc?key={0}&type=view&embedded=true"" style=""width:100%;height:500px;"" frameborder=""0"" marginheight=""0"" marginwidth=""0"">Loading...</iframe>";
            }
        }
        public override IEnumerable<string> Domains
        {
            get
            {
                yield return "https://docs.google.com/spreadsheet";
                yield return "http://docs.google.com/spreadsheet";
            }
        }
        protected override IList<string> ExtractParameters(Uri responseUri)
        {
            var queryString = HttpUtility.ParseQueryString(responseUri.Query);
            string formKey = queryString["key"];
            
            if (!String.IsNullOrEmpty(formKey))
            {
                return new [] { formKey };
            }
            return null;
        }
    }
}