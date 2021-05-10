using CivicCommunicator.DataAccess.DataModel.Models;
using CivicCommunicator.DataAccess.Repository.Abstraction;
using CivicCommunicator.Services.Abstraction;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CivicCommunicator.Bots
{
    public class CivicBot : ActivityHandler
    {
        private readonly ICommandHandlingService commandHandlingService;
        private readonly IUserService userService;
        private readonly ICommunicationService communicationService;
        private readonly IRepository<ConversationRequest> requestRepository;
        private readonly QnAMaker qnAMaker;
        private readonly IMessageService messageService;
        private readonly ICardService cardService;
        private readonly IActionsService actionsService;
        private readonly IRepository<BotReply> replyRepository;

        public CivicBot(ICommandHandlingService commandHandlingService, 
            IUserService userService, 
            IRepository<ConversationRequest> requestRepository,
            ICommunicationService communicationService,
            QnAMaker qnAMaker,
            IMessageService messageService,
            ICardService cardService,
            IActionsService actionsService,
            IRepository<BotReply> replyRepository)
        { 
            this.commandHandlingService = commandHandlingService;
            this.userService = userService;
            this.requestRepository = requestRepository;
            this.communicationService = communicationService;
            this.qnAMaker = qnAMaker;
            this.messageService = messageService;
            this.cardService = cardService;
            this.actionsService = actionsService;
            this.replyRepository = replyRepository;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> context, CancellationToken cancellationToken)
        {
            var user = this.userService.GetUserModel(context);
            if (user == null)
            {
                user = this.userService.RegisterUser(context);
            } else
            {
                this.userService.TryUpdate(user, context);
            }


            this.messageService.StoreTheMessage(user, context.Activity.Text);

            var aboutCivic = this.actionsService.HandleAction(context);
            if(aboutCivic != null)
            {
                await context.SendActivityAsync(aboutCivic);
                return;
            }

            var commandResult = this.commandHandlingService.HandleCommand(context, cancellationToken);
            if (commandResult != null)
            {
                this.StoreBotReply(commandResult, user.UserId);
                await context.SendActivityAsync(commandResult);
                return;
            }

            if (this.RunHandOff(user, context))
            {
                return;
            }


            var qnaResult = this.ProcessWithQna(context);
            if(qnaResult != null)
            {
                this.StoreBotReply(qnaResult, user.UserId);
                await context.SendActivityAsync(qnaResult);
                return;
            }

            var message = Activity.CreateMessageActivity();
            message.Text = "Sorry, I did not understand your question. Consider rephrasing your question or if you would like to contact a customer agent, just ask!";
            this.StoreBotReply(message, user.UserId);
            await context.SendActivityAsync(message);
        }

        protected void StoreBotReply(IActivity context, int userId)
        {
            var mes = context.AsMessageActivity();
            if (mes == null)
                return;

            if (string.IsNullOrEmpty(mes.Text))
                return;

            this.replyRepository.Add(new BotReply{
               Text = mes.Text,
               CreationDate = DateTime.Now,
               ToId = userId
            });
        }

        protected override async Task OnConversationUpdateActivityAsync
            (ITurnContext<IConversationUpdateActivity> turnContext, 
            CancellationToken cancellationToken)
        {
            
            if (turnContext.Activity.From.Role != null || turnContext.Activity.From.Name == "Customer")
            {
                var user = this.userService.GetUserModel(turnContext);
                if(user == null)
                {
                    this.userService.RegisterUser(turnContext);
                } else
                {
                    this.userService.TryUpdate(user, turnContext);
                }
                return;
            }

            var replyActivity = Activity.CreateMessageActivity();

            replyActivity.Attachments = new List<Attachment>() { this.cardService.CreateAdaptiveCardAttachment() };
            await turnContext.SendActivityAsync(replyActivity);
        }

        private IActivity ProcessWithQna(ITurnContext turnContext)
        { 
            var response = this.qnAMaker.GetAnswersAsync(turnContext).Result;
            if (response != null && response.Length > 0)
            {
                return MessageFactory.Text(response[0].Answer);
            }

            return null;
        }

        private bool RunHandOff(User user, ITurnContext turnContext)
        {
            var activeRequest = this.requestRepository
                .AsQueryable()
                .Include(x => x.Agent)
                .Include(x => x.Requester)
                .FirstOrDefault(x => x.State == RequestState.InProgress && (x.RequesterId == user.UserId || x.AgentId == user.UserId));

            if(activeRequest == null)
            {
                return false;
            }

            var userToSend = activeRequest.Agent;
            if (userToSend.UserId == user.UserId)
            {
                userToSend = activeRequest.Requester;
            }

            var userPrefix = string.IsNullOrEmpty(user.Name) ? "User: " : $"{user.Name}: ";
            this.communicationService.SendMessageToUserAsync(userToSend, $"{userPrefix}{turnContext.Activity.Text}");

            return true;
        }
    }
}
