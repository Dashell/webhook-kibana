using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Discord.Webhook;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq;
using System.Collections.Generic;
using Discord;

namespace WebHook.Kibana
{
    public class Webhook
    {
        private readonly DiscordWebhookClient discordWebhookClient;
        private readonly ISendGridClient iSendGridClient;


        public Webhook(DiscordWebhookClient discordWebhookClient, ISendGridClient iSendGridClient)
        {
            this.discordWebhookClient = discordWebhookClient;
            this.iSendGridClient = iSendGridClient;
        }

        [FunctionName("Kibana")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string channelType = req.Headers["Channel"];
            string emails = req.Headers["Recievers"];
            string severity = req.Headers["Severity"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            switch (channelType)
            {
                case "Discord":
                    await discordWebhookClient.SendMessageAsync(requestBody, false, BuildDiscordEmbed(requestBody, severity));
                    break;
                case "SendGrid":
                    await iSendGridClient.SendEmailAsync(BuildSendGridMessage(requestBody, emails));
                    break;
            }
            
            
            return new NoContentResult();
        }

        private SendGridMessage BuildSendGridMessage(string content, string? emails)
        {
            if (emails == null)
            {
                throw new ArgumentNullException("Recievers");
            }
            return MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(""), emails.Split(";").Select(email => new EmailAddress(email)).ToList(), "Kibana", content, content);
        }

        private List<Embed> BuildDiscordEmbed(string content,string severity)
        {
            Color color = severity == "1" ? Color.Red : Color.Orange;
            return new List<Embed>{
                 new EmbedBuilder() { Title = "Kibana Log" }
                .WithDescription(content)
                .WithUrl(Environment.GetEnvironmentVariable("Kibana_Url", EnvironmentVariableTarget.Process))
                .WithThumbnailUrl("http://www.bujarra.com/wp-content/uploads/2018/11/kibana0-978x500.jpg")
                .WithColor(color)
                .Build()
             };

        }
    }
}
