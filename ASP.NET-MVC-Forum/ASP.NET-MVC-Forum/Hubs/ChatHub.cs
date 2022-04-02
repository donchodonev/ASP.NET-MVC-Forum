﻿namespace ASP.NET_MVC_Forum.Web.Hubs
{
    using ASP.NET_MVC_Forum.Data.Contracts;
    using ASP.NET_MVC_Forum.Domain.Models.Chat;

    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Threading.Tasks;

    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRepository chatRepo;
        private readonly IMapper mapper;

        public ChatHub(IChatRepository chatService, IMapper mapper)
        {
            this.chatRepo = chatService;
            this.mapper = mapper;
        }


        public async Task SendMessageToGroup(string senderIdentityId, string receiverIdentityId, string message, long chatId, string senderUsername)
        {

            var persistedMessage = await chatRepo.AddMessageAsync(chatId, message, senderUsername);

            var time = persistedMessage
                .CreatedOn
                .AddHours(2) // FOR GMT+2
                .ToString("HH:mm:ss");

            var response = new ChatMessageResponseData(senderUsername, time, persistedMessage.Text);

            await Clients.Group(senderIdentityId + receiverIdentityId).SendAsync("ReceiveMessage", response);

            await Clients.Group(receiverIdentityId + senderIdentityId).SendAsync("ReceiveMessage", response);
        }

        public async Task GetHistory(long chatId, string sender, string receiver)
        {
            try
            {
                var messages = await chatRepo
                .GetLastMessagesAsNoTracking(chatId)
                .ProjectTo<ChatMessageResponseData>(mapper.ConfigurationProvider)
                .ToListAsync();

                await Clients.Group(sender + receiver).SendAsync("ReceiveHistory", messages);
            }
            catch (Exception ex)
            {
                string msg  = ex.Message;
            }
        }

        public void Disconnect()
        {
            Dispose();
        }

        public async Task ConnectUserGroups(string senderIdentityId, string recipientIdentityId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, senderIdentityId + recipientIdentityId);
        }

        public async Task DisconnectUserGroups(string senderIdentityId, string recipientIdentityId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, senderIdentityId + recipientIdentityId);
        }
    }
}