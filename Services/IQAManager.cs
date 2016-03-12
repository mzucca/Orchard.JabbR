using JabbR.Models;
using JabbR.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Services
{
    public interface IQAManager : IDependency
    {
        MessageViewModel GetAnswer(string destinationRoom, string question, out int delay);
    }
}