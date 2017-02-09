using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MensaBot.Controllers
{
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Http;

    using MensaBot.MessageInterpretation;

    using Microsoft.Bot.Connector;

    [RoutePrefix("api/trigger")]
    public class TriggerController : ApiController
    {
        [HttpGet]
        public async Task<string> Get(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key) || key !=  ConfigurationManager.AppSettings.Get("BotTriggerPassword"))
                return "denied";

            var now = CommandBucket.Get.SmoothTime(DateTime.UtcNow.AddHours(1).ToString("HH:mm"));
            
            return "Send Trigger :" + CommandBucket.Get.SendTrigger(now);
        }
    }
}