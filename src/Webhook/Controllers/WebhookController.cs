using AutoMapper;
using Discord.Webhook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webhook.Configuration;
using Webhook.Dtos;
using Webhook.Enum;

namespace Product.API.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly DiscordWebhookClient discordWebhookClient;
        private readonly ISendGridClient iSendGridClient;
        private readonly IMapper iMapper;
        private readonly AppSettings appSettings;


        public WebhookController(DiscordWebhookClient discordWebhookClient, ISendGridClient iSendGridClient, IMapper iMapper,IOptions<AppSettings> appSettings)
        {
            this.discordWebhookClient = discordWebhookClient;
            this.iSendGridClient = iSendGridClient;
            this.appSettings = appSettings.Value;
            this.iMapper = iMapper ?? throw new ArgumentNullException(nameof(iMapper));
        }

        [HttpPost]
        public async Task<NoContentResult> Post([FromHeader(Name = "Channel")] ChannelType channelType, [FromHeader(Name = "Recievers")] string? emails, [FromBody] KibanaDto kibanaDto)
        {
            switch (channelType)
            {
                case ChannelType.Discord:
                    await discordWebhookClient.SendMessageAsync(JsonConvert.SerializeObject(kibanaDto));
                    break;
                case ChannelType.SendGrid:
                    await iSendGridClient.SendEmailAsync(BuildSendGridMessage(kibanaDto, emails));
                    break;
            }
            return NoContent();
        }

        [HttpGet]
        public string Get()
        {
            return "Test";
        }

        private SendGridMessage BuildSendGridMessage(KibanaDto kibanaDto, string? emails)
        {
            if( emails == null)
            {
                throw new ArgumentNullException("Recievers");
            }
            return MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(appSettings.SendGridConfiguration.Sender), emails.Split(";").Select(email => new EmailAddress(email)).ToList(),"Kibana", JsonConvert.SerializeObject(kibanaDto), JsonConvert.SerializeObject(kibanaDto));
        }
    }
}
