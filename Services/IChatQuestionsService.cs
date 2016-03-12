using System.Linq;
using Orchard;
using JabbR.Models;

namespace JabbR.Infrastructure.Services
{
    public interface IChatQuestionsService : IDependency {
        IQueryable<QuestionRecord> Get();
        bool Add(QuestionRecord model);
        bool Update(QuestionRecord model);
        bool Remove(QuestionRecord questionRecord);
    }
}