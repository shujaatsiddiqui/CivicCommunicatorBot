using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Threading;

namespace CivicCommunicator.Commands
{
    public class BotCommandLogic
    {
        public Func<ITurnContext, CancellationToken, bool> IsCommand { get; set; }
        public Func<ITurnContext, IActivity> Validate { get; set; }
        public Func<ITurnContext, object> ParseParameters { get; set; }
        public Func<object, IActivity> ValidateParameters { get; set; }
        public Func<ITurnContext, object, IActivity> HandleCommand{ get; set; }
        public Action<ITurnContext> PostLogic { get; set; }
    }
}
