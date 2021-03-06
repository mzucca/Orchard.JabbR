﻿using System;
using System.IO;
using System.Threading.Tasks;
using JabbR.ContentProviders.Core;
using JabbR.Models;
using JabbR.UploadHandlers;
using JabbR.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using JabbR.Hubs;

namespace JabbR.Services
{
    public class UploadCallbackHandler
    {
        private readonly UploadProcessor _processor;
        private readonly ContentProviderProcessor _resourceProcessor;
        private readonly IHubContext _hubContext;
        private readonly IChatService _service;

        public UploadCallbackHandler(UploadProcessor processor,
                                     ContentProviderProcessor resourceProcessor,
                                     IConnectionManager connectionManager,
                                     IChatService service)
        {
            _processor = processor;
            _resourceProcessor = resourceProcessor;
            _hubContext = connectionManager.GetHubContext<Chat>();
            _service = service;
        }

        public async Task Upload(string userId,
                                 string connectionId,
                                 string roomName,
                                 string file,
                                 string contentType,
                                 Stream stream)
        {
            UploadResult result;
            ChatMessage message;

            try
            {
                result = await _processor.HandleUpload(file, contentType, stream, stream.Length);

                if (result == null)
                {
                    string messageContent = String.Format(LanguageResources.UploadFailed, Path.GetFileName(file));
                    _hubContext.Clients.Client(connectionId).postMessage(messageContent, "error", roomName);
                    return;
                }
                else if (result.UploadTooLarge)
                {
                    string messageContent = String.Format(LanguageResources.UploadTooLarge, Path.GetFileName(file), (result.MaxUploadSize / 1048576f).ToString("0.00"));
                    _hubContext.Clients.Client(connectionId).postMessage(messageContent, "error", roomName);
                    return;
                }

                // Add the message to the persistent chat
                message = _service.AddMessage(userId, roomName, result.Url);

                // Keep track of this attachment
                _service.AddAttachment(message, file, contentType, stream.Length, result);
            }
            catch (Exception ex)
            {
                string messageContent = String.Format(LanguageResources.UploadFailedException, Path.GetFileName(file), ex.Message);
                _hubContext.Clients.Client(connectionId).postMessage(messageContent, "error", roomName);
                return;
            }

            var messageViewModel = new MessageViewModel(message);

            // Notify all clients for the uploaded url
            _hubContext.Clients.Group(roomName).addMessage(messageViewModel, roomName);

            _resourceProcessor.ProcessUrls(new[] { result.Url }, _hubContext.Clients, roomName, message.Id);
        }

        private static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { LanguageResources.SizeOrderGB, LanguageResources.SizeOrderMB, LanguageResources.SizeOrderKB, LanguageResources.SizeOrderBytes };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return String.Format("{0:##.##} {1}", Decimal.Divide(bytes, max), order);
                }

                max /= scale;
            }

            return String.Format("0 {0}", LanguageResources.SizeOrderBytes);
        }
    }
}