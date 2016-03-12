using JabbR.Models;
using JabbR.ViewModels;
using Orchard.Logging;
using JabbR.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using WordsMatching;

namespace JabbR.Services
{
    public class QAManager : IQAManager
    {
        private readonly ICache _cache;
        private readonly IRecentMessageCache _recentMessageCache;
        private readonly IChatService _chatService;
        private readonly ApplicationSettings _chatSettings;
        private readonly IChatQuestionsService _chatQuestionService;
        private readonly IJabbrRepository _repository;
        private double _requiredAnswerAccuracy;

        public QAManager(ICache cache,
            IRecentMessageCache recentMessageCache,
            IChatService chatService,
            IJabbrRepository repository,
            ISettingsManager chatSettingsService,
            IChatQuestionsService chatQuestionService)
        {
            _cache = cache;
            _repository = repository;
            _recentMessageCache = recentMessageCache;
            _chatQuestionService = chatQuestionService;
            _chatService = chatService;
            _chatSettings = chatSettingsService.Load();

            _requiredAnswerAccuracy = (double)(_chatSettings.RequiredAnswerAccuracy) / 100;

            Logger = NullLogger.Instance;
        }
        public ILogger Logger { get; set; }
        public MessageViewModel GetAnswer(string room, string question, out int delay)
        {
            delay = 3000; // default delay
            try
            {
                QuestionRecord[] knowledgeBase = _cache.Get<QuestionRecord[]>("knowledgeBase");
                if(knowledgeBase==null)
                {
                    knowledgeBase = _chatQuestionService.Get().ToArray();
                    if (knowledgeBase == null)
                        return null;
    
                    _cache.Set("knowledgeBase", knowledgeBase, new TimeSpan(0, 10, 0));
                }
                string answer = ProcessCommand(question,false,knowledgeBase,out delay);
                if (string.IsNullOrEmpty(answer))
                    return null;

                ChatUser sender = _repository.GetUserByName(_chatSettings.SendAnswerAs); //TODO
                //IChatService service = new ChatService(_cache, _recentMessageCache, repository, _chatSettingsService);
                //TODO look in the repository to find answers

                string answerId = Guid.NewGuid().ToString("d");
                ChatRoom destinationRoom = _repository.GetRoomByName(room);
                ChatMessage response = _chatService.AddMessage(sender, destinationRoom, answerId, answer);
                _repository.CommitChanges();
                return new MessageViewModel(response);
            }
            catch (Exception exc)
            {
                Logger.Error("Cannot find answer:" + exc.Message);
                return null;
            }

        }

        // this method is the core of our message processing
        private string ProcessCommand(string msg, bool showSimilarity, QuestionRecord[] knowledgeBase, out int delay)
        {
            delay = 3000; // Default delay
            // remouve all special characters, ponctuation etc ...
            msg = preTreatMessage(msg);

            // we build our answer search query
            string answer = "";
            //string searchQuery = "";

            // we tokenize the given message
            string[] tokens = msg.Split(' ');
            //foreach (string token in tokens)
            //{
            //    if (token != "")
            //    {
            //        searchQuery += oper + " receive LIKE '%" + token + "%' ";
            //        oper = "OR";
            //    }
            //}
            // we store the results


            // we search for all rules with the 'question' that looks like the given message
            var results = (from a in knowledgeBase
                           from w in tokens
                           where a.Question.Contains(w)
                           && a.Active
                           select a).Distinct();

            //    this.database.Rules.Select("active = True AND " + searchQuery);

            // set the minimum wordsMatching score (precision) 
            // messages that are matched below this threshold will be ignored
            List<string> answerlist = new List<string>();
            // we calculate the matching score foreach rule and store the ones
            // with the highest score in the answerlist array

            double maxScore = _requiredAnswerAccuracy; // Minimum required accuracy
            foreach (var result in results)
            {
                MatchsMaker m = new MatchsMaker(msg, result.Question);
                if (m.Score > maxScore)
                {
                    answerlist.Clear();
                    //answerlist.Add(result["send"] + ((showSimilarity) ? " (" + m.Score + ")" : ""));
                    answerlist.Add(result.Answer);
                    delay = result.Delay;
                    maxScore = m.Score;
                }
                else if (m.Score == maxScore)
                {
                    answerlist.Add(result.Answer);
                    delay = result.Delay;
                }
            }
            if (answerlist.Count == 0) // if no rule was found (scored high)
            {
                // we give an empty message
                // OR , you can modify this to be a default message
                // like : "I dont understand !"
                answer = "";
            }
            else
            {
                // if there are rules with the same score
                // we just choose a random one
                Random rand = new Random();
                answer = answerlist[rand.Next(0, answerlist.Count)];
            }
            return answer;
        }

        // this will remove all the unwanted characters
        private string preTreatMessage(string msg)
        {
            msg = msg.Replace('\'', ' ');
            msg = msg.Replace('\"', ' ');
            msg = msg.Replace(',', ' ');
            msg = msg.Replace(';', ' ');
            msg = msg.Replace('.', ' ');
            msg = msg.Replace("?", " ?");
            msg = msg.Replace("!", " !");
            msg = msg.Replace('-', ' ');
            msg = msg.Replace('_', ' ');
            msg = msg.Replace('(', ' ');
            msg = msg.Replace(')', ' ');

            return msg;
        }

    }
}