using System;
using System.Linq;
using Orchard.Caching;
using Orchard.Data;
using Orchard.Logging;
using JabbR.Models;

namespace JabbR.Infrastructure.Services
{
    public class ChatQuestionService : IChatQuestionsService {

        private readonly IRepository<QuestionRecord> _repository;
        private readonly ISignals _signals;

        public ChatQuestionService(
            IRepository<QuestionRecord> repository,
            ISignals signals) {
            _repository = repository;
            _signals = signals;
            
        }
        public Orchard.Logging.ILogger Logger { get; set; }
        IQueryable<QuestionRecord> IChatQuestionsService.Get()
        {
            return _repository.Table.AsQueryable<QuestionRecord>();
        }

        public bool Add(QuestionRecord model)
        {
            if (model == null)
                return false;
            try
            {
                _repository.Create(model);
                return true;
            }
            catch (Exception exc)
            {
                Logger.Error("Cannot create QuestionRecord:" + exc.Message);
                return false;
            }
        }

        public bool Update(QuestionRecord model)
        {
            if (model == null)
                return false;
            try
            {
                _repository.Update(model);
                return true;
            }
            catch(Exception exc)
            {
                Logger.Error("Cannot update QuestionRecord: " + exc.Message);
                return false;
            }
        }
        public bool Remove(QuestionRecord model)
        {
            if (model == null)
                return false;
            try
            {
                _repository.Delete(model);
                return true;
            }
            catch (Exception exc)
            {
                Logger.Error("Cannot update QuestionRecord: " + exc.Message);
                return false;
            }
        }
    }
}