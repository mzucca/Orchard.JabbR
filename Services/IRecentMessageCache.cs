using System.Collections.Generic;
using JabbR.Models;
using JabbR.ViewModels;
using Orchard;

namespace JabbR.Services
{
    public interface IRecentMessageCache : IDependency
    {
        void Add(ChatMessage message);

        void Add(string room, ICollection<MessageViewModel> messages);

        ICollection<MessageViewModel> GetRecentMessages(string roomName);
    }
}
