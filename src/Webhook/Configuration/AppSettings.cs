namespace Webhook.Configuration
{
    public class AppSettings
    {
        public DiscordConfiguration DiscordConfiguration { get;set; }
        public SendGridConfiguration SendGridConfiguration { get;set; }
    }

    public class DiscordConfiguration
    {
        public string Url { get; set; }
    }
    public class SendGridConfiguration
    {
        public string ApiKey { get; set; }
        public string Sender { get; internal set; }
    }
}
