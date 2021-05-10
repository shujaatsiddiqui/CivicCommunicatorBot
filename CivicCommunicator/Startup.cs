// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.5.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;

using CivicCommunicator.Bots;
using CivicCommunicator.DataAccess.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CivicCommunicator.DataAccess.Repository.Abstraction;
using CivicCommunicator.DataAccess.Repository.Implementation;
using CivicCommunicator.Services.Abstraction;
using CivicCommunicator.Services.Implementation;
using Microsoft.Bot.Builder.AI.QnA;
using System.Net.Http;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Teams.Middlewares;
using Microsoft.Bot.Builder.BotFramework;

namespace CivicCommunicator
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<CivicBotDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServer")));

            services.AddHttpClient();

            services.AddTransient<QnAMaker>(x => new QnAMaker(
                new QnAMakerEndpoint
                {
                    KnowledgeBaseId = x.GetService<IConfiguration>()["QnAKnowledgebaseId"],
                    EndpointKey = x.GetService<IConfiguration>()["QnAEndpointKey"],
                    Host = x.GetService<IConfiguration>()["QnAEndpointHostName"]
                },
                null,
                x.GetService<IHttpClientFactory>().CreateClient())
            );

            services.AddTransient<LuisRecognizer>(x => new LuisRecognizer(
                new LuisApplication(
                    this.Configuration["LuisAppId"],
                    this.Configuration["LuisAPIKey"],
                    this.Configuration["LuisAPIHostName"]
                ))
            );

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ICommandHandlingService, CommandHandlingService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ICommunicationService, CommunicationService>();
            services.AddTransient<ILuisService, LuisService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<ICardService, CardService>();
            services.AddTransient<IOrchestrizationService, OrchestrizationService>();
            services.AddTransient<IActionsService, ActionsService>();
            

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();

            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddBot<CivicBot>(options =>
            {
                options.Middleware.Add(new TeamsMiddleware(new ConfigurationCredentialProvider(this.Configuration)));
            });
        } 

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CivicBotDbContext context)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    );
            });
        }
    }
}
