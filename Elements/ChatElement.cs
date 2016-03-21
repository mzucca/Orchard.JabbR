using Orchard.Layouts.Framework.Elements;
using Orchard.Localization;
using System;

namespace JabbR.Elements
{
    public class ChatElement : Element
    {
        public override string Category
        {
            get { return "Chat"; }
        }
        public override string ToolboxIcon
        {
            get { return "\uf007"; }
        }
        public override LocalizedString Description
        {
            get { return T("A Chat Element"); }
        }

        public string LanguageResources { get; set; }
        public bool IsAdmin { get; set; }
        public bool AllowRoomCreation { get; set; }
        public int MaxMessageLength { get; set; }
        public Version Version { get; set; }

    }
}