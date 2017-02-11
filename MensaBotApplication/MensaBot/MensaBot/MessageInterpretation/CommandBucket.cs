using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using MensaBot.Resources.CanteenNames;
    using MensaBot.Resources.Dates;
    using MensaBot.Resources.Language;

    using MensaBotParsing.Mensa;

    using Microsoft.Bot.Connector;

    public class CommandBucket
    {
        #region constants

        private static CommandBucket _instance;

        #endregion

        #region member vars

        private List<CanteenElement> _canteens;

        #endregion

        #region properties

        public static CommandBucket Get => _instance ?? (_instance = new CommandBucket());

        #endregion

        #region constructors and destructors

        private CommandBucket()
        {
        }

        #endregion

        #region methods

        public string CreateHelpMessage()
        {
            string text = MessageInterpreter.MarkBold(Lang.use_syntax) + ":" + MessageInterpreter.LineBreak + "/" + MessageInterpreter.MarkBold(Lang.canteen_explained)
                        + MessageInterpreter.MarkItalic(Lang.optional);
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold(Lang.canteen_command_tag) + " = " + Lang.canteen_command_examples;
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold(Lang.canteen_name_tag) + " = " + Lang.list_canteen_description;
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold(Lang.canteen_date_tag) + "= "  + Lang.date_examples;
            text += MessageInterpreter.DrawLine;
            text += Lang.command_help_intro + " /" + MessageInterpreter.MarkBold("set help") + " " + Lang.help_default_settings + MessageInterpreter.LineBreak;
            text += Lang.command_help_intro + " /" + MessageInterpreter.MarkBold("remove help") + " " + Lang.help_default_settings_remove;
            text += MessageInterpreter.DrawLine;
            text += Lang.command_help_intro + " /" + MessageInterpreter.MarkBold("deleteData") + " " + Lang.help_delete_all_data;
            text += MessageInterpreter.DrawLine;
            text += Lang.command_help_intro + " /" + MessageInterpreter.MarkBold(Lang.key) + " " + Lang.help_emoji_description;
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold(Lang.please_note) + ": " + Lang.reference_1 + MessageInterpreter.LineBreak;
            text += Lang.reference_2;

            return text;
        }

        public string CreateKeyMessage()
        {
            string text = "";

            text += FoodElement.FoodTagsToEmoji(FoodTags.ALCOHOL) + " " + Lang.food_tag_alcohol + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.BEEF) + " " + Lang.food_tag_beef + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.BIO) + " " + Lang.food_tag_bio + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.CHICKEN) + " " + Lang.food_tag_chicken + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.FISH) + " " + Lang.food_tag_fish + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.GARLIC) + " " + Lang.food_tag_garlic + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.HOGGET) + " " + Lang.food_tag_hogget + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.PORK) + " " + Lang.food_tag_pork + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.SOUP) + " " + Lang.food_tag_soup + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.VITAL) + " " + Lang.canteen + " " + Lang.food_tag_vital + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.VEGAN) + " " + Lang.food_tag_vegan + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.VEGETARIAN) + " " + Lang.food_tag_vegetarian + MessageInterpreter.LineBreak;
            text += FoodElement.FoodTagsToEmoji(FoodTags.VENSION) + " " + Lang.food_tag_vension + MessageInterpreter.LineBreak;

            return text;
        }

        public string SetLanguage(MensaBotEntities mensaBotEntities, string language, string channelId, string conversationId)
        {
            int index = Array.IndexOf(MessageInterpreter.AvailableLanguages, language);

            if (index >= 0)
            {

                if (DatabaseUtilities.AddEntry(DatabaseUtilities.LanguageTag, language, mensaBotEntities, channelId, conversationId))
                    return  Lang.add_language + ": "+language;
                else
                    return Lang.add_language_failed+ " :" + language;

            }
            return Lang.unknown_language;

        }

        public string SetTrigger(MensaBotEntities mensaBotEntities, string trigger, string channelId, string conversationId)
        {
            Regex r = new Regex("2[0-3]:[0-5][0-9]|[0-1][0-9]:[0-5][0-9]", RegexOptions.IgnoreCase);

            Match m = r.Match(trigger);

            if (!m.Success || trigger.Length != 5)
            {
                return Lang.regex_not_matched +" " + trigger+ " "+ trigger.Length;
            }


            var smoothTime = SmoothTime(trigger);

            if (DatabaseUtilities.AddEntry(DatabaseUtilities.Trigger, smoothTime, mensaBotEntities, channelId, conversationId))
                return Lang.set_time_tigger + " " + smoothTime;
            else
                return Lang.failed_set_trigger + " " + smoothTime;

        }

        public string SmoothTime(string timeString)
        {
            var time = Array.ConvertAll(timeString.Split(':'), int.Parse);

            if (time[1] < 15)
                time[1] = 0;
            if (time[1] >= 15 && time[1] < 45)
                time[1] = 30;
            if (time[1] >= 45)
            {
                time[1] = 0;
                time[0] = time[0] + 1;
            }

            var hoursToString = ((time[0].ToString().Length == 1) ? "0" + time[0].ToString() : time[0].ToString());
            var minutesToString = ((time[1] == 0) ? "00" : time[1].ToString());

            return hoursToString + ":" + minutesToString;
        }

        public bool SendTrigger(string trigger)
        {
            MensaBotEntities mensaBotEntities = new MensaBotEntities();

            List<Chat> chats = DatabaseUtilities.GetChatTrigger(mensaBotEntities, trigger);
            if (chats == null)
                return false;

            foreach (var chat in chats)
            {
                string definedLanguage = CommandBucket.Get.GetValue(mensaBotEntities, DatabaseUtilities.LanguageTag, chat.ChannelId, chat.ConversationId);

                if (definedLanguage == null)
                    definedLanguage = "en";

                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(definedLanguage);
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(definedLanguage);

                string[] results = CommandBucket.Get.CreateMensaReply(Dates.today, null, mensaBotEntities, chat.ChannelId, chat.ConversationId, true);

                ConnectorClient connector2 = new ConnectorClient(new Uri(chat.ServiceURL));

                foreach (var result in results)
                {
                    connector2.Conversations.SendToConversationAsync(
                        new Activity
                        {
                            Type = ActivityTypes.Message,
                            ChannelId = chat.ChannelId,
                            Conversation = new ConversationAccount(id: chat.ConversationId),
                            From = new ChannelAccount(id: "mensa_md_bot", name: "mensa_md_bot"),
                            Text = result
                        });

                }
            }

            return chats.Count > 0;
        }


        public string GetValue(MensaBotEntities mensaBotEntities, string key ,string channelId, string conversationId)
        {
            return DatabaseUtilities.GetValueBytKey(mensaBotEntities, key, channelId, conversationId);
        }

        public string SetStyle(string style, MensaBotEntities mensaBotEntities, string channelId, string conversationId, string serviceURL)
        {
            FoodDisplayStyle foodDisplayStyle;

            if (FoodDisplayStyle.TryParse(style.ToUpper(), out foodDisplayStyle))
            {
                DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);
                if (DatabaseUtilities.AddEntry(DatabaseUtilities.StyleTag, foodDisplayStyle.ToString().ToLower(), mensaBotEntities, channelId, conversationId))
                    return Lang.style_update + ": " + foodDisplayStyle.ToString().ToLower();
                else
                    return Lang.style_fail_to_add_database;
            }
            else
            {
                return Lang.style_not_found + ": " + style;
            }
        }

        public string RemoveDefaults(string activityMessage, MensaBotEntities mensaBotEntities, string channelId, string conversationId)
        {
            if (activityMessage.StartsWith("remove mensa") || activityMessage.StartsWith("remove canteen"))
            {
                string[] msgParts = activityMessage.Split(' ');

                if (msgParts.Length != 2)
                    return Lang.wrong_param_remove_canteen;

                bool containsCommand = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, msgParts[1]);

                if (containsCommand)
                    return CommandBucket.Get.CreateUnknownCommand();
                else
                {
                    DatabaseUtilities.RemoveKey(mensaBotEntities, DatabaseUtilities.DefaultMensaTag, channelId, conversationId);
                    return Lang.deleted_canteen;
                }

            }

            if (activityMessage.StartsWith("remove filter"))
            {
                string[] msgParts = activityMessage.Split(' ');

                if (msgParts.Length != 2)
                    return Lang.wrong_param_remove_filter;

                DatabaseUtilities.RemoveKey(mensaBotEntities, DatabaseUtilities.IgnoreTags, channelId, conversationId);
                return Lang.deleted_filter;
            }

            if (activityMessage.StartsWith("remove style"))
            {
                string[] msgParts = activityMessage.Split(' ');

                if (msgParts.Length != 2)
                    return Lang.wrong_param_remove_filter;

                DatabaseUtilities.RemoveKey(mensaBotEntities, DatabaseUtilities.StyleTag, channelId, conversationId);
                return Lang.deleted_filter;
            }

            if (activityMessage.StartsWith("remove trigger"))
            {
                string[] msgParts = activityMessage.Split(' ');

                if (msgParts.Length != 2)
                    return Lang.wrong_param_remove_filter;

                DatabaseUtilities.RemoveKey(mensaBotEntities, DatabaseUtilities.Trigger, channelId, conversationId);
                return Lang.deleted_filter;
            }

            if (activityMessage.StartsWith("remove help"))
            {
                var message = MessageInterpreter.MarkBold(Lang.remove_help) + MessageInterpreter.DrawLine
                            + Lang.remove_help_canteen + MessageInterpreter.DrawLine
                            + Lang.remove_help_filter + MessageInterpreter.DrawLine
                            + Lang.remove_help_trigger + MessageInterpreter.DrawLine
                            + Lang.remove_help_style + MessageInterpreter.LineBreak;

                return message;
            }

            return Lang.command_help_intro +" /remove help " + Lang.help_default_settings_remove;
        }


        public string SetDefaults(string activityMessage, MensaBotEntities mensaBotEntities, string channelId, string conversationId, string serviceURL)
        {
            if (activityMessage.StartsWith("set help"))
            {
                var message = MessageInterpreter.MarkBold(Lang.set_help) + MessageInterpreter.DrawLine
                            + Lang.set_help_canteen + MessageInterpreter.DrawLine
                            + Lang.set_help_filter + MessageInterpreter.DrawLine
                            + Lang.set_help_style + MessageInterpreter.DrawLine
                            + Lang.set_help_trigger + MessageInterpreter.DrawLine
                            + Lang.language_help;

                return message;
            }

            string[] setMessageParts = activityMessage.Remove(0, 1).ToLower().Split(' ');

            if (setMessageParts.Length != 3)
                return Lang.set_fail;

            bool containsCommand = MessageInterpreter.Get.ContainsCommands(MessageInterType.MAIN_COMMAND, setMessageParts[1]);

            if (containsCommand)
            {
                DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);
                CanteenName canteenName = MessageInterpreter.Get.FindCanteen(setMessageParts[2]);

                if (canteenName == CanteenName.none)
                    return Lang.canteen_not_found + " " + MessageInterpreter.MarkBold(setMessageParts[2]);

                return CommandBucket.Get.SetDefaultCanteen(canteenName, mensaBotEntities, channelId, conversationId, serviceURL);
            }

            if (setMessageParts[1] == "language")
            {
                DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);

                var lang = activityMessage.Split(' ');
                if (lang.Length != 3)
                    return Lang.language_help;

                return CommandBucket.Get.SetLanguage(mensaBotEntities, lang[2], channelId, conversationId);
            }

            if (setMessageParts[1] == "trigger")
            {
                DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);

                var time = activityMessage.Split(' ');
                if (time.Length != 3)
                    return Lang.fail_so_define_trigger;

                return CommandBucket.Get.SetTrigger(mensaBotEntities, time[2], channelId, conversationId);
            }


            if ((setMessageParts[1].ToLower() == "style"))
            {
                DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);
                return CommandBucket.Get.SetStyle(setMessageParts[2], mensaBotEntities, channelId, conversationId, serviceURL);
            }

            if ((setMessageParts[1].ToLower() == "filter"))
            {
                DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);

                var tags = CommandBucket.Get.SetIgnoreTags(setMessageParts[2], MessageInterpreter.ParamDivider);
                var tagsAdditional = CommandBucket.Get.SetIgnoreTags(setMessageParts[2], MessageInterpreter.ParamDivider);

                if (tags != null && tagsAdditional != null)
                    tags.AddRange(tagsAdditional);
                if (tags == null && tagsAdditional != null)
                    tags = tagsAdditional;

                if (tags == null)
                    return Lang.failed_sorry + MessageInterpreter.LineBreak + Lang.add_tags_failed;

                tags = tags.Distinct().ToList();

                string enumToString = "";
                string displayString = "";
                for (int i = 0; i < tags.Count - 1; i++)
                {
                    enumToString += tags[i].ToString().ToLower() + MessageInterpreter.ParamDivider;
                    displayString += FoodElement.FoodTagsToString(tags[i]) + MessageInterpreter.ParamDivider;
                }
                enumToString += tags[tags.Count - 1].ToString().ToLower();
                displayString += FoodElement.FoodTagsToString(tags[tags.Count - 1]);

                if (DatabaseUtilities.AddEntry(DatabaseUtilities.IgnoreTags, enumToString, mensaBotEntities, channelId, conversationId))
                    return Lang.add_tags + " " + MessageInterpreter.MarkBold(displayString);
                else
                    return Lang.add_tags_failed + MessageInterpreter.LineBreak + Lang.add_tags_failed;
            }

            return CommandBucket.Get.CreateUnknownCommand();
        }

        public string SetDefaultCanteen(CanteenName defaultCanteen, MensaBotEntities mensaBotEntities, string channelId, string conversationId, string serviceURL)
        {
            DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId, serviceURL);

            if (DatabaseUtilities.AddEntry(DatabaseUtilities.DefaultMensaTag, defaultCanteen.ToString(), mensaBotEntities, channelId, conversationId))
                return Lang.add_canteen;
            else
                return Lang.add_canteen_failed;
        }

        public List<FoodTags> SetIgnoreTags(string tags, char divider)
        {
            if (string.IsNullOrEmpty(tags))
                return null;

            List<string> tagsList = tags.Split(divider).ToList();
            for (int i = tagsList.Count - 1; i >= 0; i--)
            {
                FoodTags element = MessageInterpreter.Get.FindTag(tagsList[i]);
                if (element == FoodTags.NONE_FOOD_TAG)
                {
                    tagsList.RemoveAt(i);
                }
            }
            if (tagsList.Count == 0)
                return null;

            List<FoodTags> cleanedTags = new List<FoodTags>();

            for (int i = 0; i < tagsList.Count; i++)
                cleanedTags.Add(MessageInterpreter.Get.FindTag(tagsList[i]));
            
            return (cleanedTags == null || cleanedTags.Count == 0)? null: cleanedTags;

        }


        public string CreateUnknownCommand()
        {
            return MessageInterpreter.MarkBold(Lang.unknown_command) + " "+ Lang.unknown_command_joke_msg + MessageInterpreter.LineBreak 
                   + Lang.command_help_intro + " " + MessageInterpreter.MarkBold("\""+Lang.command_help+"\"") + " " + Lang.command_help_exit;
        }

        public string [] CreateMensaReply(string paramFirst, string paramSecond, MensaBotEntities mensaBotEntities, string channelId, string conversationId, bool forceDate)
        {
            bool isShortRequest = false;
            //check if paramFirst is date, if true -> change order
            if (!string.IsNullOrEmpty(paramFirst) && string.IsNullOrEmpty(paramSecond))
            {
                var isDate = (MessageInterpreter.Get.FindDate(paramFirst) != DateIndex.none);
                if (isDate)
                {
                    paramSecond = paramFirst;
                    paramFirst = null;
                }
            }

            if (string.IsNullOrEmpty(paramFirst))
            {
                paramFirst = DatabaseUtilities.GetValueBytKey(mensaBotEntities, DatabaseUtilities.DefaultMensaTag, channelId, conversationId);

                if (string.IsNullOrEmpty(paramFirst))
                    return new string[] { Lang.please_add + " " + MessageInterpreter.MarkBold(Lang.canteen_name) + "!" };
                else
                    isShortRequest = true;

            }

            CanteenName canteenName = MessageInterpreter.Get.FindCanteen(paramFirst);

            if (canteenName == CanteenName.none)
            {
                return new string[] { Lang.canteen_not_found+": " + MessageInterpreter.MarkBold(paramFirst) };
            }

            try
            {
                if (_canteens == null)
                    CreateCanteenInfo();

                _canteens[(int)canteenName].LoadElements(3);
            }
            catch (Exception e)
            {
                return new string[] { Lang.fail_to_load_information + ": " + MessageInterpreter.MarkBold(paramFirst) };
            }

            DateIndex dateIndex = DateIndex.TODAY;

            if (!string.IsNullOrEmpty(paramSecond))
            {
                dateIndex = MessageInterpreter.Get.FindDate(paramSecond);
                if (dateIndex == DateIndex.none)
                {
                    return new string[] { Lang.date_not_found + " " + MessageInterpreter.MarkBold(paramSecond) };
                }
            }


            //Find correct date element.
            DateTime now = DateTime.Now.ToUniversalTime().AddDays((int)dateIndex).AddHours(1);
            DayElement[] dayElements = null;

            try
            {
                if (isShortRequest && !forceDate)
                {
                    int maxDays = 3;
                    for (int i = 0; i < maxDays; i++)
                    {
                        if(i==0 && now.Hour>16)//TODO read close time via website
                            continue;

                        dayElements = _canteens[(int)canteenName].DayElements.FindAll(t => t.Date.Day == now.AddDays(i).Day).ToArray();
                        if (dayElements != null && dayElements.Length != 0)
                            break;
                    }

                }
                else
                {
                    dayElements = _canteens[(int)canteenName].DayElements.FindAll(t => t.Date.Day == now.Day).ToArray();
                }
            }
            catch (Exception e)
            {
                dayElements = null;
            }

            if (dayElements == null || dayElements.Length == 0)
            {
                return new string[] { MessageInterpreter.MarkBold(Lang.could_not_find_date+" " + now.ToString("dd.MM.yyyy") + Lang.broken_heart)};
            }

            IEnumerable<FoodTags> filter = null;
            try
            {
                filter =
                    CommandBucket.Get.GetValue(mensaBotEntities, DatabaseUtilities.IgnoreTags, channelId, conversationId)
                        .Split(MessageInterpreter.ParamDivider)
                        .Select(t => Enum.Parse(typeof(FoodTags), t.ToUpper()))
                        .Cast<FoodTags>();
            }
            catch (Exception e)
            {
                filter = null;
            }

            string[] menuItems = new string[dayElements.Length];

            //List all elements for dayElement
            for (int i = 0; i < dayElements.Length; i++)
            {
                menuItems[i] = Lang.menu_for + ":" + MessageInterpreter.LineBreak + MessageInterpreter.MarkBold(_canteens[(int)canteenName].GetDescription(i)) + " "
                               + Lang.menu_for_at + " " + MessageInterpreter.MarkItalic(dayElements[i].Date.ToString("dd.MM.yyyy"))
                               + MessageInterpreter.DrawLine;
                
                foreach (var foodElement in dayElements[i].FoodElements)
                {
                    bool hideElement = false;
                    var tagResult = "";
                    if (foodElement.Tags != null)
                    {
                        foreach (var tag in foodElement.Tags)
                        {
                            if (tag != null)
                            {
                                if (filter!=null && filter.Contains(tag))
                                {
                                    hideElement = true;
                                    break;
                                }
                                tagResult += FoodElement.FoodTagsToEmoji(tag) + ",";
                            }
                            else
                            {
                                tagResult += "❎";
                            }
                        }
                    }
                    if (tagResult.Length > 1)
                    {
                        tagResult = tagResult.Remove(tagResult.Length - 1, 1).ToLower() + ";";
                    }

                    string warning = null;

                    if (string.IsNullOrEmpty(foodElement.EnglishName.Trim()) && CultureInfo.CurrentCulture.ToString() != (new CultureInfo("de")).ToString())
                        warning = MessageInterpreter.MarkItalic(Lang.no_english_available);

                    FoodDisplayStyle foodDisplayStyle = FoodDisplayStyle.none;
                    string styleString = GetValue(mensaBotEntities, DatabaseUtilities.StyleTag, channelId, conversationId);
                    if (!string.IsNullOrEmpty(styleString))
                    {
                        if (!FoodDisplayStyle.TryParse(styleString.ToUpper(), out foodDisplayStyle))
                            foodDisplayStyle = FoodDisplayStyle.none;
                    }

                    var tagsDisplay = "";
                    switch (foodDisplayStyle)
                    {
                        case FoodDisplayStyle.none:
                        case FoodDisplayStyle.MAXIMUM:
                            tagsDisplay = MessageInterpreter.LineBreak + tagResult + (!string.IsNullOrEmpty(foodElement.Cost) ? foodElement.Cost + "€" : "") + MessageInterpreter.DrawLine;
                            break;
                        case FoodDisplayStyle.MINIMUM_NOLINES:
                            tagsDisplay = MessageInterpreter.LineBreak;
                            break;
                        case FoodDisplayStyle.MINIMUM:
                            tagsDisplay = MessageInterpreter.DrawLine;
                            break;
                        case FoodDisplayStyle.INLINE:
                            tagsDisplay = " " + ((tagResult.Length > 1) ? tagResult.Remove(tagResult.Length - 1, 1) : tagResult) + MessageInterpreter.DrawLine;
                            break;
                    }

                    if (!hideElement)
                    {
                        if (CultureInfo.CurrentCulture.ToString() == (new CultureInfo("de")).ToString() || warning != null)
                            menuItems[i] += MessageInterpreter.MarkBold(foodElement.GermanName) + warning + " " + tagsDisplay;
                        else
                            menuItems[i] += MessageInterpreter.MarkBold(MessageInterpreter.FirstCharToUpper(foodElement.EnglishName)) + " " + tagsDisplay;
                    }
                }
            }

            return menuItems;
        }

        public string CreateStartMessage()
        {
            string text = MessageInterpreter.MarkBold(Lang.start_welcome) + " " + Lang.start_intro + MessageInterpreter.LineBreak + Lang.start_exit + MessageInterpreter.LineBreak
                + Lang.command_help_intro + " " + MessageInterpreter.MarkBold("\"" + Lang.command_help + "\"") + " " + Lang.command_help_exit;

            return text;
        }

        public string CreateListCanteensMessage()
        {
            if(_canteens == null) CreateCanteenInfo();

            string text = MessageInterpreter.MarkBold(Lang.list_of_canteens);
            text += MessageInterpreter.DrawLine;

            for (int i = 0; i < _canteens.Count; i++)
            {
                foreach (var description in _canteens[i].DescriptionId)
                {
                    text += MessageInterpreter.MarkBold(Lang.ResourceManager.GetString(description)) + ", ";
                }
                text = text.Substring(0, text.Length - 2);
                text += ": ";
                foreach (var coName in MessageInterpreter.Get.FindCanteenNames(_canteens[i].CanteenName))
                {
                    text += CanteenNames.ResourceManager.GetString(coName)+ ", ";
                }
                text = text.Substring(0, text.Length - 2) + MessageInterpreter.LineBreak;
            }

            return text;
        }

        private void CreateCanteenInfo()
        {
            _canteens = new List<CanteenElement>();

            _canteens.Add(new CanteenElement(CanteenName.LOWER_HALL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-unten/"}, new string[] { "name_canteen_lower" }));
            _canteens.Add(new CanteenElement(CanteenName.UPPER_HALL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-oben/"}, new string[] { "name_canteen_upper" }));
            _canteens.Add(new CanteenElement(CanteenName.KELLERCAFE, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-kellercafe/speiseplan/"}, new string[] { "name_canteen_keller_cafe" }));
            _canteens.Add(new CanteenElement(CanteenName.HERRENKRUG, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-herrenkrug/speiseplan/"}, new string[] { "name_canteen_herrenkrug" }));
            _canteens.Add(new CanteenElement(CanteenName.STENDAL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-stendal/speiseplan/"}, new string[] { "name_canteen_stendal" }));
            _canteens.Add(new CanteenElement(CanteenName.WERNIGERODE, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-wernigerode/speiseplan/"}, new string[] { "name_canteen_wernigerode" }));
            _canteens.Add(new CanteenElement(CanteenName.HALBERSTADT, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-halberstadt/speiseplan/"}, new string[] { "name_canteen_halberstadt" }));
            _canteens.Add(new CanteenElement(CanteenName.UPPER_HALL_LOWER_HALL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-unten/",
                                                                                               "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-oben/" }, new string[] { "name_canteen_lower", "name_canteen_upper" }));
        }

        #endregion
    }
}