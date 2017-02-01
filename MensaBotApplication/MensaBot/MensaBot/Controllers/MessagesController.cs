using System;
using System.Linq;

namespace MensaBot
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Chronic;

    using MensaBot.MessageInterpretation;

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


            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                if (string.IsNullOrEmpty(activity.Text) || !activity.Text.StartsWith("/"))
                {
                    var noContent = Request.CreateResponse(HttpStatusCode.NoContent);
                    return noContent;
                }

                if (activity.Text.StartsWith("/start"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateStartMessage(LanguageKey.none));

                if (activity.Text.StartsWith("/help"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateHelpMessage(LanguageKey.none));

                if (activity.Text.StartsWith("/ping"))
                {
                    if (_random.Next(10) > 8)
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("Pong! 🏓"));
                    else
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("I do not play ping pong."));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/pong"))
                {
                    if (_random.Next(10) > 8)
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("Ping! 🏓"));
                    else
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("I do not play ping pong."));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/legende"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateKeyMessage(LanguageKey.DE));

                if (activity.Text.StartsWith("/key"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateKeyMessage(LanguageKey.EN));

                if (activity.Text.ToLower().StartsWith("/deletedata"))
                {
                    bool result = DatabaseUtilities.RemoveAllSettingsAndChat(mbe, activity.ChannelId, activity.Conversation.Id);
                    return await SendResponseMessage(connector, activity, (result == true ? "Removed all data from you!" : "Can't remove data from you." + MessageInterpreter.LineBreak + "I'm sorry 💔!"));
                }

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove help"))
                {
                    var message = MessageInterpreter.MarkBold("Help for /remove") + MessageInterpreter.DrawLine
                                + "Use: /remove canteen or /remove mensa to delte information about the default canteen." + MessageInterpreter.DrawLine
                                + "Use: /remove filter to delete all filter settings." + MessageInterpreter.LineBreak;

                    return await SendResponseMessage(connector, activity, message);

                }

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove mensa") || activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove canteen"))
                {
                    string[] msgParts = activity.Text.ToLower().Replace(_botHandle, "").Split(' ');

                    if (msgParts.Length != 2)
                        return await SendResponseMessage(connector, activity, "Did you mean: /remove canteen or /remove mensa");

                    LanguageKey languageKey = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, msgParts[1]);

                    if (languageKey == LanguageKey.none)
                        return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateUnknownCommand());
                    else
                    {
                        DatabaseUtilities.RemoveKey(mbe, DatabaseUtilities.DefaultMensaTag, activity.ChannelId, activity.Conversation.Id);
                        return await SendResponseMessage(connector, activity, (languageKey == LanguageKey.DE ? "Standart-Mensa, sofern vorhanden, gelöscht." : "Removed default canteen, if exists."));
                    }

                }

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove filter"))
                {
                    string[] msgParts = activity.Text.ToLower().Replace(_botHandle, "").Split(' ');

                    if (msgParts.Length != 2)
                        return await SendResponseMessage(connector, activity, "Did you mean: /remove filter");

                    DatabaseUtilities.RemoveKey(mbe, DatabaseUtilities.IgnoreTags, activity.ChannelId, activity.Conversation.Id);
                    return await SendResponseMessage(connector, activity, ( "Alle Filter, sofern vorhanden, wurden gelöscht."+MessageInterpreter.LineBreak+"Removed filter, if exists."));
                }

                if (activity.Text.ToLower().Replace(_botHandle,"").StartsWith("/get mensa") || activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/get canteen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.DefaultMensaTag, activity.ChannelId, activity.Conversation.Id));

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/get filter") || activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/get canteen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.IgnoreTags, activity.ChannelId, activity.Conversation.Id));

                if (activity.Text.ToLower().StartsWith("/set"))
                {
                    if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/set help"))
                    {
                        var message = MessageInterpreter.MarkBold("Help for /set") + MessageInterpreter.DrawLine
                                    + "Use: /set canteen [canteenname] to update the default mensa." + MessageInterpreter.DrawLine 
                                    + "Use: /set filter [filterA],[filterB].... to ignore meals with given tags, e.g. /set filter vegan,pork...";

                        return await SendResponseMessage(connector, activity, message);
                    }

                    string[] setMessageParts = activity.Text.Remove(0, 1).Replace(_botHandle,"").ToLower().Split(' ');

                    if (setMessageParts.Length != 3)
                        return await SendResponseMessage(connector, activity, "This did not work. Use: /set help");

                    LanguageKey languageKey = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, setMessageParts[1]);

                    if (languageKey != LanguageKey.none)
                    {
                        CanteenName canteenName = MessageInterpreter.Get.FindCanteen(setMessageParts[2], languageKey);

                        if (canteenName == CanteenName.none)
                            return await SendResponseMessage(connector, activity, ("Could not find canteen with name " + MessageInterpreter.MarkBold(setMessageParts[2])));

                        return await SendResponseMessage(connector, activity, CommandBucket.Get.SetDefaultCanteen(languageKey, canteenName, mbe, activity.ChannelId, activity.Conversation.Id));
                    }

                    if ((setMessageParts[1].ToLower() == "filter"))
                    {
                        DatabaseUtilities.CreateChatEntry(mbe, activity.ChannelId, activity.Conversation.Id);

                        var tags = CommandBucket.Get.SetIgnoreTags(LanguageKey.EN, setMessageParts[2], MessageInterpreter.ParamDivider);
                        var tagsAdditional = CommandBucket.Get.SetIgnoreTags(LanguageKey.DE, setMessageParts[2], MessageInterpreter.ParamDivider);

                        if (tags != null && tagsAdditional !=null) 
                             tags.AddRange(tagsAdditional);
                        if (tags == null && tagsAdditional != null)
                            tags = tagsAdditional;

                        if(tags == null)
                            return await SendResponseMessage(connector, activity, "FAIL" + MessageInterpreter.LineBreak + "Could not add tags.");

                        tags = tags.Distinct().ToList();

                        string tagsAsStringEnglish = "";
                        string tagsAsStringGerman = "";
                        for (int i = 0; i < tags.Count-1; i++)
                        {
                            tagsAsStringEnglish += tags[i].ToString().ToLower() + MessageInterpreter.ParamDivider;
                            tagsAsStringGerman += FoodElement.FoodTagsToGermanString(tags[i]) + MessageInterpreter.ParamDivider;
                        }
                        tagsAsStringEnglish += tags[tags.Count - 1].ToString().ToLower();
                        tagsAsStringGerman += FoodElement.FoodTagsToGermanString(tags[tags.Count - 1]);

                        if (DatabaseUtilities.AddEntry(DatabaseUtilities.IgnoreTags, tagsAsStringEnglish, mbe, activity.ChannelId, activity.Conversation.Id))
                          return await SendResponseMessage(connector, activity,"Tags werden nun herausgefiltert: "+ MessageInterpreter.MarkBold(tagsAsStringGerman) + MessageInterpreter.LineBreak +"Added tags to ignore list: "+ MessageInterpreter.MarkBold(tagsAsStringEnglish));
                        else
                            return await SendResponseMessage(connector, activity, "Tags konnten nicht gesetzt werden." + MessageInterpreter.LineBreak + "Could not add tags.");
                    }
                    
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateUnknownCommand());
                }

                if (activity.Text.StartsWith("/list canteen") || activity.Text.StartsWith("/list" + _botHandle + " canteen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateListCanteensMessage(LanguageKey.EN));

                if (activity.Text.StartsWith("/list mensen") || activity.Text.StartsWith("/list" + _botHandle + " mensen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateListCanteensMessage(LanguageKey.DE));

                //------------------------------------------------------------------------------------------------
                //Find mensa commands
                //------------------------------------------------------------------------------------------------

                //remove initial /
                string messageText = activity.Text.Remove(0, 1).ToLower();
                string[] messageParts = messageText.Split(' ');

                string[] expectedMessageParts = new string[3];
                expectedMessageParts[0] = null;  //command name
                expectedMessageParts[1] = null;  //mensa name
                expectedMessageParts[2] = null;  //date

                for (int i = 0; i < messageParts.Length; i++)
                {
                    expectedMessageParts[i] = messageParts[i];
                }

                expectedMessageParts[0] = expectedMessageParts[0]?.Replace(_botHandle, "");

                LanguageKey key = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, expectedMessageParts[0]);

                if (key == LanguageKey.none)
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateUnknownCommand());


                string[] results = CommandBucket.Get.CreateMensaReply(key, expectedMessageParts[1], expectedMessageParts[2], mbe, activity.ChannelId,activity.Conversation.Id);

                foreach (var result in results)
                {
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(result));
                }

                return Request.CreateResponse(HttpStatusCode.OK);
                
            }


            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        public async Task<HttpResponseMessage> SendResponseMessage(ConnectorClient client, Activity activity, string message)
        {
            await client.Conversations.ReplyToActivityAsync(activity.CreateReply(message));
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        private Activity HandleSystemMessage(Activity message)
        {

            if (message.Type == ActivityTypes.DeleteUserData)
            {}
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