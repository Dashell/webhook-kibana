using Discord.Webhook;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;

namespace Webhook.Configuration
{
    public static class DependencyConfig
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, AppSettings appSettings)
        {

            #region Discord
            services.AddTransient(services => new DiscordWebhookClient(appSettings.DiscordConfiguration.Url));
            #endregion

            #region SendGrid
            services.AddTransient<ISendGridClient>(_ => new SendGridClient(new SendGridClientOptions { ApiKey = appSettings.SendGridConfiguration.ApiKey }));
            #endregion

            #region Repositories

            #endregion

            return services;
        }
    }
}
