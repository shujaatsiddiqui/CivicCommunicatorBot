﻿using CivicCommunicator.DataAccess.DataModel.Models;
using CivicCommunicator.DataAccess.Repository.Abstraction;
using CivicCommunicator.Services.Abstraction;
using Microsoft.Bot.Builder;
using System;
using System.Linq;

namespace CivicCommunicator.Services.Implementation
{
    public class UserService : IUserService
    {
        IRepository<User> userRepository;
        private static readonly string SITE_DOMAIN_KEY = "siteDomain";

        public UserService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        private string getSiteDomain(ITurnContext context)
        {
            var channelObj = context.Activity?.ChannelData?.ToString();
            if(channelObj == null)
            {
                return null;
            }

            var channelData = Newtonsoft.Json.Linq.JObject.Parse(channelObj);

            if (channelData.ContainsKey(UserService.SITE_DOMAIN_KEY))
            {
               return channelData[UserService.SITE_DOMAIN_KEY].ToString();
            }
            return null;
        }

        public User GetUserModel(ITurnContext turnContext) => this.userRepository.AsQueryable().FirstOrDefault(x => x.ChatId == turnContext.Activity.From.Id);

        public User GetUserModel(int id) => this.userRepository.Get(id);

        public void MarkUserAsAgent(ITurnContext turnContext)
        {
            var user = this.GetUserModel(turnContext);
            user.IsAgent = true;
            this.userRepository.Update(user);
        }

        public User RegisterUser(ITurnContext turnContext)
        {
            var user = new User
            {
                ChatId = turnContext.Activity.From.Id,
                IsAgent = false,
                ServiceUrl = turnContext.Activity.ServiceUrl,
                Name = turnContext.Activity.From.Name,
                ConversationId = turnContext.Activity.Conversation.Id,
                ChannelId = turnContext.Activity.ChannelId,
                BotChannelId = turnContext.Activity.Recipient.Id
            };
            if (user.ChannelId != "msteams")
            {
                user.SiteDomain = this.getSiteDomain(turnContext);
            }
            this.userRepository.Add(user);
            return user;
        }

        public User TryUpdate(User user, ITurnContext context)
        {
            if(user.ConversationId != context.Activity.Conversation.Id)
            {
                user.ConversationId = context.Activity.Conversation.Id;
            }
            if(user.BotChannelId != context.Activity.Recipient.Id)
            {
                user.BotChannelId = context.Activity.Recipient.Id;
            }
            if (user.ChannelId != "msteams")
            {
                var domain = this.getSiteDomain(context);
                if(user.SiteDomain != domain)
                {
                    user.SiteDomain = domain;
                }
            }
            this.userRepository.Update(user);
            return user;
        }
    }
}
