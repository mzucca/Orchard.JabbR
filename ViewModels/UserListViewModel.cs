using JabbR.Models;

namespace JabbR.ViewModels
{
    public class UserListViewModel<T> : IndexViewModel<T>
    {
        public ChatUser User { get; set; }
    }
}