using System;
using System.Linq;

namespace MensaBot
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using MensaBot.MessageInterpretation;

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

            if (activity.Type == ActivityTypes.Message)
            {

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                if (string.IsNullOrEmpty(activity.Text) || !activity.Text.StartsWith("/"))
                {
                    var noContent = Request.CreateResponse(HttpStatusCode.NoContent);
                    return noContent;
                }
                if (activity.Text.StartsWith("/start"))
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(CommandBucket.Get.CreateStartMessage(LanguageKey.none)));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/help"))
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(CommandBucket.Get.CreateHelpMessage(LanguageKey.none)));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/ping"))
                {
                    if (_random.Next(10) > 8)
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("Pong!"));
                    else
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("I do not play ping pong."));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/legende"))
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(CommandBucket.Get.CreateHelpMessage(LanguageKey.DE)));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/key"))
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(CommandBucket.Get.CreateKeyMessage(LanguageKey.EN)));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                //remove initial /
                string messageText = activity.Text.Remove(0, 1).ToLower();
                string[] messageParts = messageText.Split(' ');

                messageParts[0] = messageParts[0].Replace(_botHandle, "");

                LanguageKey key = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, messageParts[0]);
                if (key == LanguageKey.none)
                {
                    Activity reply = activity.CreateReply(MessageInterpreter.MarkBold("Unknown command!)" + " - Please do usefull things, otherwise you still will be hungry." + MessageInterpreter.LineBreak + "Use \"/help\" for help."));
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                string[] results = CommandBucket.Get.CreateMensaReply(key, messageParts);

                foreach (var result in results)
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(result));
                }

                return Request.CreateResponse(HttpStatusCode.OK);
                
            }


            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            { }
            else if (message.Type == ActivityTypes.ConversationUpdate)
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