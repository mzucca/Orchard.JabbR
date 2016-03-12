using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JabbR.Services;
using Microsoft.AspNet.SignalR.Hubs;
//using Ninject;

namespace JabbR.ContentProviders.Core
{
    public class ContentProviderProcessor
    {
        private readonly IResourceProcessor _resourceProcessor;
        private readonly IJabbrRepository _repository;
        //private readonly IKernel _kernel;

        //public ContentProviderProcessor(IKernel kernel)
        //{
        //    _kernel = kernel;
        //}
        public ContentProviderProcessor(IResourceProcessor resourceProcessor, IJabbrRepository repository)
        {
            _resourceProcessor = resourceProcessor;
            _repository = repository;
        }

        public void ProcessUrls(IEnumerable<string> links,
                                IHubConnectionContext<dynamic> clients,
                                string roomName,
                                string messageId)
        {

            //var resourceProcessor = _kernel.Get<IResourceProcessor>();
            
            var contentTasks = links.Select(_resourceProcessor.ExtractResource).ToArray();

            Task.Factory.ContinueWhenAll(contentTasks, tasks =>
            {
                foreach (var task in tasks)
                {
                    if (task.IsFaulted)
                    {
                        Trace.TraceError(task.Exception.GetBaseException().Message);
                        continue;
                    }

                    if (task.Result == null || String.IsNullOrEmpty(task.Result.Content))
                    {
                        continue;
                    }

                    // Update the message with the content

                    // REVIEW: Does it even make sense to get multiple results?
                        var message = _repository.GetMessageById(messageId);

                        // Should this be an append?
                        message.HtmlContent = task.Result.Content;

                        _repository.CommitChanges();

                    // Notify the room
                    clients.Group(roomName).addMessageContent(messageId, task.Result.Content, roomName);
                }
            });
        }
    }
}