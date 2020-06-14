using System;
using Discord.Webhook;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SendGrid;

[assembly: FunctionsStartup(typeof(WebHook.Kibana.Startup))]

namespace WebHook.Kibana
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            #region Discord
            builder.Services.AddTransient(services => new DiscordWebhookClient(System.Environment.GetEnvironmentVariable("Discord_Url", EnvironmentVariableTarget.Process)));
            #endregion

            #region SendGrid
            builder.Services.AddTransient<ISendGridClient>(_ => new SendGridClient(new SendGridClientOptions { ApiKey = "test" }));
            #endregion
        }
    }
}