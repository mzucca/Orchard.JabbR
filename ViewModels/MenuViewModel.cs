using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.ViewModels
{
    public class MenuViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public IList<MenuViewModel> InnerMenu { get; set; }

        public MenuViewModel()
        {
            InnerMenu = new List<MenuViewModel>();
        }
    }
}