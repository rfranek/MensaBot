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

                if (string.IsNullOrEmpty(activity.Text) || !activity.Text.StartsWith("/"))
                {
                    var noContent = Request.CreateResponse(HttpStatusCode.NoContent);
                    return noContent;
                }

                if (activity.Text.StartsWith("/start"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateStartMessage());

                if (activity.Text.StartsWith("/help"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateHelpMessage());

                if (activity.Text.StartsWith("/language"))
                {
                    DatabaseUtilities.CreateChatEntry(mbe, activity.ChannelId, activity.Conversation.Id);
                    var lang = activity.Text.Replace(_botHandle, "").ToLower().Split(' ');
                    if(lang.Length != 2)
                        return await SendResponseMessage(connector, activity, Lang.language_help);

                    return await SendResponseMessage(connector, activity, CommandBucket.Get.SetLanguage(mbe, lang[1],activity.ChannelId,activity.Conversation.Id));
                }

                if (activity.Text.StartsWith("/getlanguage"))
                {
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.LanguageTag, activity.ChannelId, activity.Conversation.Id));
                }

                if (activity.Text.StartsWith("/test"))
                {
                    return await SendResponseMessage(connector, activity, Lang.ResourceManager.GetString(Lang.remove_help, CultureInfo.GetCultureInfo(definedLanguage)));
                }

                if (activity.Text.StartsWith("/ping"))
                {
                    if (_random.Next(10) > 8)
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(Lang.pong));
                    else
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(Lang.not_play_ping_pong));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/pong"))
                {
                    if (_random.Next(10) > 8)
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(Lang.ping));
                    else
                        await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(Lang.not_play_ping_pong));
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                if (activity.Text.StartsWith("/legende"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateKeyMessage());

                if (activity.Text.StartsWith("/key"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateKeyMessage());

                if (activity.Text.ToLower().StartsWith("/deletedata"))
                {
                    bool result = DatabaseUtilities.RemoveAllSettingsAndChat(mbe, activity.ChannelId, activity.Conversation.Id);
                    return await SendResponseMessage(connector, activity, (result == true ? Lang.removed_all_data : Lang.removed_all_data_failed + MessageInterpreter.LineBreak + Lang.failed_sorry));
                }

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove help"))
                {
                    var message = MessageInterpreter.MarkBold(Lang.remove_help) + MessageInterpreter.DrawLine
                                + Lang.remove_help_canteen + MessageInterpreter.DrawLine
                                + Lang.remove_help_filter + MessageInterpreter.LineBreak;

                    return await SendResponseMessage(connector, activity, message);

                }

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove mensa") || activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove canteen"))
                {
                    string[] msgParts = activity.Text.ToLower().Replace(_botHandle, "").Split(' ');

                    if (msgParts.Length != 2)
                        return await SendResponseMessage(connector, activity, Lang.wrong_param_remove_canteen);

                    bool containsCommand = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, msgParts[1]);

                    if (containsCommand)//TODO ! ?
                        return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateUnknownCommand());
                    else
                    {
                        DatabaseUtilities.RemoveKey(mbe, DatabaseUtilities.DefaultMensaTag, activity.ChannelId, activity.Conversation.Id);
                        return await SendResponseMessage(connector, activity, Lang.deleted_canteen);
                    }

                }

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/remove filter"))
                {
                    string[] msgParts = activity.Text.ToLower().Replace(_botHandle, "").Split(' ');

                    if (msgParts.Length != 2)
                        return await SendResponseMessage(connector, activity, Lang.wrong_param_remove_filter);

                    DatabaseUtilities.RemoveKey(mbe, DatabaseUtilities.IgnoreTags, activity.ChannelId, activity.Conversation.Id);
                    return await SendResponseMessage(connector, activity, Lang.deleted_filter);
                }

                if (activity.Text.ToLower().Replace(_botHandle,"").StartsWith("/get mensa") || activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/get canteen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.DefaultMensaTag, activity.ChannelId, activity.Conversation.Id));

                if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/get filter") || activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/get canteen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.GetValue(mbe, DatabaseUtilities.IgnoreTags, activity.ChannelId, activity.Conversation.Id));

                if (activity.Text.ToLower().StartsWith("/set"))
                {
                    if (activity.Text.ToLower().Replace(_botHandle, "").StartsWith("/set help"))
                    {
                        var message = MessageInterpreter.MarkBold(Lang.set_help) + MessageInterpreter.DrawLine
                                    + Lang.set_help_canteen + MessageInterpreter.DrawLine 
                                    + Lang.set_help_filter;

                        return await SendResponseMessage(connector, activity, message);
                    }

                    string[] setMessageParts = activity.Text.Remove(0, 1).Replace(_botHandle,"").ToLower().Split(' ');

                    if (setMessageParts.Length != 3)
                        return await SendResponseMessage(connector, activity, Lang.set_fail);

                    bool containsCommand = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, setMessageParts[1]);

                    if (containsCommand)
                    {
                        CanteenName canteenName = MessageInterpreter.Get.FindCanteen(setMessageParts[2]);

                        if (canteenName == CanteenName.none)
                            return await SendResponseMessage(connector, activity, (Lang.canteen_not_found +" " + MessageInterpreter.MarkBold(setMessageParts[2])));

                        return await SendResponseMessage(connector, activity, CommandBucket.Get.SetDefaultCanteen(canteenName, mbe, activity.ChannelId, activity.Conversation.Id));
                    }
                    if ((setMessageParts[1].ToLower() == "style"))
                    {


                    }

                    if ((setMessageParts[1].ToLower() == "filter"))
                    {
                        DatabaseUtilities.CreateChatEntry(mbe, activity.ChannelId, activity.Conversation.Id);
                        
                        var tags = CommandBucket.Get.SetIgnoreTags(setMessageParts[2], MessageInterpreter.ParamDivider);
                        var tagsAdditional = CommandBucket.Get.SetIgnoreTags(setMessageParts[2], MessageInterpreter.ParamDivider);

                        if (tags != null && tagsAdditional !=null) 
                             tags.AddRange(tagsAdditional);
                        if (tags == null && tagsAdditional != null)
                            tags = tagsAdditional;

                        if(tags == null)
                            return await SendResponseMessage(connector, activity, Lang.failed_sorry + MessageInterpreter.LineBreak + Lang.add_tags_failed);

                        tags = tags.Distinct().ToList();

                        string enumToString = "";
                        string displayString = "";
                        for (int i = 0; i < tags.Count-1; i++)
                        {
                            enumToString += tags[i].ToString().ToLower() + MessageInterpreter.ParamDivider;
                            displayString += FoodElement.FoodTagsToString(tags[i]) + MessageInterpreter.ParamDivider;
                        }
                        enumToString += tags[tags.Count - 1].ToString().ToLower();
                        displayString += FoodElement.FoodTagsToString(tags[tags.Count - 1]);

                        if (DatabaseUtilities.AddEntry(DatabaseUtilities.IgnoreTags, enumToString, mbe, activity.ChannelId, activity.Conversation.Id))
                          return await SendResponseMessage(connector, activity, Lang.add_tags + " " + MessageInterpreter.MarkBold(displayString));
                        else
                            return await SendResponseMessage(connector, activity, Lang.add_tags_failed + MessageInterpreter.LineBreak + Lang.add_tags_failed);
                    }
                    
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateUnknownCommand());
                }

                if (activity.Text.StartsWith("/list canteen") || activity.Text.StartsWith("/list" + _botHandle + " canteen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateListCanteensMessage());

                if (activity.Text.StartsWith("/list mensen") || activity.Text.StartsWith("/list" + _botHandle + " mensen"))
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateListCanteensMessage());


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

                bool hasCommand = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, expectedMessageParts[0]);

                if (!hasCommand)
                    return await SendResponseMessage(connector, activity, CommandBucket.Get.CreateUnknownCommand());


                string[] results = CommandBucket.Get.CreateMensaReply(expectedMessageParts[1], expectedMessageParts[2], mbe, activity.ChannelId,activity.Conversation.Id);

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