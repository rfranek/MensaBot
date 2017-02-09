using System;
using System.Linq;

namespace MensaBot
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;

    using System.Threading.Tasks;
    using System.Web.Http;

    using MensaBot.MessageInterpretation;
    using MensaBot.Resources.Language;

    using MensaBotParsing.Mensa;

    using Microsoft.Bot.Connector;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        #region constants

        private static Random _random = new Random();
        private static readonly string _botHandle = "@mensa_md_bot";

        #endregion

        #region methods

        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            MensaBotEntities mbe = new MensaBotEntities();

            string definedLanguage = CommandBucket.Get.GetValue(mbe, DatabaseUtilities.LanguageTag, activity.ChannelId, activity.Conversation.Id);

            if (definedLanguage == null)
                definedLanguage = "en";
            
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(definedLanguage);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(definedLanguage);


            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                if (string.IsNullOrEmpty(activity.Text) || (!activity.Text.StartsWith("/") && activity.ChannelId.ToLower() == "telegram"))
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }
                bool commandMessage = false;
                string chatMessage = activity.Text.ToLower().Replace(_botHandle, "");

                if (activity.ChannelId.ToLower() == "telegram" || activity.ChannelId.ToLower() == "emulator")
                {
                    chatMessage = chatMessage.Remove(0, 1);
                    commandMessage = true;
                }

                if (chatMessage.StartsWith("start"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.CreateStartMessage());

                if (chatMessage.StartsWith("help"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.CreateHelpMessage());

                if (chatMessage.StartsWith("ping"))
                    return await SendResponseMessage(connector, activity, commandMessage, (_random.Next(10) > 8 ? Lang.pong  : Lang.not_play_ping_pong));

                if (chatMessage.StartsWith("pong"))
                    return await SendResponseMessage(connector, activity, commandMessage, (_random.Next(10) > 8 ? Lang.ping : Lang.not_play_ping_pong));

                if (chatMessage.StartsWith("key") || chatMessage.StartsWith("legende"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.CreateKeyMessage());

                if (chatMessage.StartsWith("deletedata") || chatMessage.StartsWith("stop"))
                {
                    bool result = DatabaseUtilities.RemoveAllSettingsAndChat(mbe, activity.ChannelId, activity.Conversation.Id);
                    return await SendResponseMessage(connector, activity, commandMessage, (result == true ? Lang.removed_all_data : Lang.removed_all_data_failed + MessageInterpreter.LineBreak + Lang.failed_sorry));
                }

                if (chatMessage.StartsWith("remove"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.RemoveDefaults(chatMessage, mbe, activity.ChannelId, activity.Conversation.Id));

                if (chatMessage.StartsWith("get mensa") || chatMessage.StartsWith("get canteen"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.DefaultMensaTag, activity.ChannelId, activity.Conversation.Id));

                if (chatMessage.StartsWith("get filter") || chatMessage.StartsWith("get canteen"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.IgnoreTags, activity.ChannelId, activity.Conversation.Id));

                if (chatMessage.StartsWith("get language"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.LanguageTag, activity.ChannelId, activity.Conversation.Id));

                if (chatMessage.StartsWith("set"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.SetDefaults(chatMessage, mbe, activity.ChannelId, activity.Conversation.Id, activity.ServiceUrl));

                if (chatMessage.StartsWith("list canteen") || chatMessage.StartsWith("list mensen"))
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.CreateListCanteensMessage());

                //------------------------------------------------------------------------------------------------
                //Find mensa commands
                //------------------------------------------------------------------------------------------------

                //Remove(0, 1)
                string[] messageParts = chatMessage.Split(' ');
                 string[] expectedMessageParts = new string[3];

                for (int i = 0; i < messageParts.Length; i++)
                {
                    expectedMessageParts[i] = messageParts[i];
                } 

                bool hasCommand = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, expectedMessageParts[0]);

                if (!hasCommand)
                    return await SendResponseMessage(connector, activity, commandMessage, CommandBucket.Get.CreateUnknownCommand());


                string[] results = CommandBucket.Get.CreateMensaReply(expectedMessageParts[1], expectedMessageParts[2], mbe, activity.ChannelId,activity.Conversation.Id);

                foreach (var result in results)
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(result));
                }

                return Request.CreateResponse(HttpStatusCode.OK);
                
            }
            else if ((activity.Type == ActivityTypes.DeleteUserData))
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                bool result = DatabaseUtilities.RemoveAllSettingsAndChat(mbe, activity.ChannelId, activity.Conversation.Id);
                return await SendResponseMessage(connector, activity, false, (result == true ? Lang.removed_all_data : Lang.removed_all_data_failed + MessageInterpreter.LineBreak + Lang.failed_sorry));
            }


            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        public async Task<HttpResponseMessage> SendResponseMessage(ConnectorClient client, Activity activity, bool commandMessage, string message)
        {

            message = (commandMessage ? message : message.Replace("/",""));
            await client.Conversations.ReplyToActivityAsync(activity.CreateReply(message));
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        private Activity HandleSystemMessage(Activity message)
        {

            if (message.Type == ActivityTypes.ConversationUpdate)
            { }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            { }
            else if (message.Type == ActivityTypes.Typing)
            { }
            else if (message.Type == ActivityTypes.Ping)
            { }

            return null;
        }

        #endregion
    }
}