using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabbR.ViewModels
{
    public class Entry<T>
    {
        public T Item { get; set; }
        public bool IsChecked { get; set; }
    }
    public class IndexViewModel<T>
    {
        public IList<Entry<T>> Items { get; set; }
        public IndexOptions Options { get; set; }
        public dynamic Pager { get; set; }
    }
}
