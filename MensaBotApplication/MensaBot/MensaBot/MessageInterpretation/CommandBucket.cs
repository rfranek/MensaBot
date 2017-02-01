using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

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

        public string CreateHelpMessage(LanguageKey key)
        {
            string text = MessageInterpreter.MarkBold("Use this syntax") + ":" + MessageInterpreter.LineBreak + "/" + MessageInterpreter.MarkBold("[canteencommand] [canteenname] [date] ")
                        + MessageInterpreter.MarkItalic("optional");
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold("[canteencommand]") + "=" + MessageInterpreter.MarkBold("german") + ": /mensa, /kantine, /futtern, /schnabulieren ..." + MessageInterpreter.LineBreak;
            text += MessageInterpreter.MarkBold("[canteencommand]") + "=" + MessageInterpreter.MarkBold("english") + ": /canteen, /menu, /nosh, /eat ...";
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold("[canteenname]") + "=" + MessageInterpreter.MarkBold("german") + ": /list mensen - Zeigt alle verfügbaren Mensen und ihre Befehlsbezeichnungen." + MessageInterpreter.LineBreak;
            text += MessageInterpreter.MarkBold("[canteenname]") + "=" + MessageInterpreter.MarkBold("english") + ": /list canteens - Shows all canteens and their shortcuts.";
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold("[date]") + "=" + MessageInterpreter.MarkBold("german") + ": z.B.: heute, morgen, übermorgen" + MessageInterpreter.LineBreak;
            text += MessageInterpreter.MarkBold("[date]") + "=" + MessageInterpreter.MarkBold("english") + ": e.g: today, tomorrow, the_day_after_tomorrow";
            text += MessageInterpreter.DrawLine;
            text += "Use /" + MessageInterpreter.MarkBold("key") + " or /" + MessageInterpreter.MarkBold("legende") + " to get a description for emojis";
            text += MessageInterpreter.DrawLine;
            text += MessageInterpreter.MarkBold("Please note") + ": This is " + MessageInterpreter.MarkBold("not") + " an official bot from Studentenwerk Magdeburg." + MessageInterpreter.LineBreak;
            text += "This bot uses public informations from their website.";

            return text;
        }

        public string CreateKeyMessage(LanguageKey key)
        {
            string text = "";
            switch (key)
            {
                case LanguageKey.DE:
                    text += FoodElement.FoodTagsToEmoji(FoodTags.ALCOHOL) + " " + FoodElement.FoodTagsToGermanString(FoodTags.ALCOHOL) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.BIO) + " " + FoodElement.FoodTagsToGermanString(FoodTags.BIO) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.FISH) + " " + FoodElement.FoodTagsToGermanString(FoodTags.FISH) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.CHICKEN) + " " + FoodElement.FoodTagsToGermanString(FoodTags.CHICKEN) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.GARLIC) + " " + FoodElement.FoodTagsToGermanString(FoodTags.GARLIC) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.HOGGET) + " " + FoodElement.FoodTagsToGermanString(FoodTags.HOGGET) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VITAL) + " " + FoodElement.FoodTagsToGermanString(FoodTags.VITAL) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.BEEF) + " " + FoodElement.FoodTagsToGermanString(FoodTags.BEEF) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.PORK) + " " + FoodElement.FoodTagsToGermanString(FoodTags.PORK) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.SOUP) + " " + FoodElement.FoodTagsToGermanString(FoodTags.SOUP) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VEGAN) + " " + FoodElement.FoodTagsToGermanString(FoodTags.VEGAN) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VEGETARIAN) + " " + FoodElement.FoodTagsToGermanString(FoodTags.VEGETARIAN) + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VENSION) + " " + FoodElement.FoodTagsToGermanString(FoodTags.VENSION) + MessageInterpreter.LineBreak;
                    break;
                case LanguageKey.none:
                case LanguageKey.EN:
                    text += FoodElement.FoodTagsToEmoji(FoodTags.ALCOHOL) + " " + FoodTags.ALCOHOL.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.BEEF) + " " + FoodTags.BEEF.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.BIO) + " " + FoodTags.BIO.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.CHICKEN) + " " + FoodTags.CHICKEN.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.FISH) + " " + FoodTags.FISH.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.GARLIC) + " " + FoodTags.GARLIC.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.HOGGET) + " " + FoodTags.HOGGET.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.PORK) + " " + FoodTags.PORK.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.SOUP) + " " + FoodTags.SOUP.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VITAL) + " Mensa " + FoodTags.VITAL.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VEGAN) + " " + FoodTags.VEGAN.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VEGETARIAN) + " " + FoodTags.VEGETARIAN.ToString().ToLower() + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VENSION) + " " + FoodTags.VENSION.ToString().ToLower() + MessageInterpreter.LineBreak;

                    break;
            }

            return text;
        }

        public string GetValue(MensaBotEntities mensaBotEntities, string key ,string channelId, string conversationId)
        {
            return DatabaseUtilities.GetValueBytKey(mensaBotEntities, key, channelId, conversationId);
        }

        public string SetDefaultCanteen(LanguageKey key, CanteenName defaultCanteen, MensaBotEntities mensaBotEntities, string channelId, string conversationId)
        {
            DatabaseUtilities.CreateChatEntry(mensaBotEntities, channelId, conversationId);

            if (DatabaseUtilities.AddEntry(DatabaseUtilities.DefaultMensaTag, defaultCanteen.ToString(), mensaBotEntities, channelId, conversationId))
            {
                return key == LanguageKey.DE? "Standard-Mensa hinzugefügt." : "Added default canteen.";
            }
            else
            {
                return key == LanguageKey.DE ? "Standard-Mensa konnte nicht aktualisiert werden." : "Can't add default canteen.";
            }
        }

        public List<FoodTags> SetIgnoreTags(LanguageKey key, string tags, char divider)
        {
            if (string.IsNullOrEmpty(tags))
                return null;

            List<string> tagsList = tags.Split(divider).ToList();
            for (int i = tagsList.Count - 1; i >= 0; i--)
            {
                FoodTags element = MessageInterpreter.Get.FindTag(tagsList[i], key);
                if (element == FoodTags.NONE_FOOD_TAG)
                {
                    tagsList.RemoveAt(i);
                }
            }
            if (tagsList.Count == 0)
                return null;

            List<FoodTags> cleanedTags = new List<FoodTags>();

            for (int i = 0; i < tagsList.Count; i++)
                cleanedTags.Add(MessageInterpreter.Get.FindTag(tagsList[i], key));
            
            return (cleanedTags == null || cleanedTags.Count == 0)? null: cleanedTags;

        }


        public string CreateUnknownCommand()
        {
            return MessageInterpreter.MarkBold("Unknown command!") + " - Please do usefull things, otherwise you still will be hungry." + MessageInterpreter.LineBreak 
                   + "Use "+ MessageInterpreter.MarkBold("\"/help\"") + " for help.";
        }

        public string [] CreateMensaReply(LanguageKey key, string paramFirst, string paramSecond, MensaBotEntities mensaBotEntities, string channelId, string conversationId)
        {
            bool isShortRequest = false;
            //check if paramFirst is date, if true -> change order
            if (!string.IsNullOrEmpty(paramFirst) && string.IsNullOrEmpty(paramSecond))
            {
                var isDate = (MessageInterpreter.Get.FindDate(paramFirst, key) != DateIndex.none);
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
                    return new string[] { "Please add " + MessageInterpreter.MarkBold("mensa name") + "!" };
                else
                    isShortRequest = true;

            }

            CanteenName canteenName = MessageInterpreter.Get.FindCanteen(paramFirst, key);

            if (canteenName == CanteenName.none)
            {
                return new string[] { "Could not find canteen with name " + MessageInterpreter.MarkBold(paramFirst) };
            }

            try
            {
                if (_canteens == null)
                    CreateCanteenInfo();

                _canteens[(int)canteenName].LoadElements(3);
            }
            catch (Exception e)
            {
                return new string[] { "Fail to load information about " + MessageInterpreter.MarkBold(paramFirst) };
            }

            DateIndex dateIndex = DateIndex.TODAY;

            if (!string.IsNullOrEmpty(paramSecond))
            {
                dateIndex = MessageInterpreter.Get.FindDate(paramSecond, key);
                if (dateIndex == DateIndex.none)
                {
                    return new string[] { "Could not find date with name " + MessageInterpreter.MarkBold(paramSecond) };
                }
            }


            //Find correct date element.
            DateTime now = DateTime.Now.ToUniversalTime().AddDays((int)dateIndex).AddHours(1);
            DayElement[] dayElements = null;

            try
            {
                if (isShortRequest)
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
                return new string[] { key == LanguageKey.DE ? MessageInterpreter.MarkBold("Es konnten keine Informationen für den " + now.ToString("dd.MM.yyyy") + " gefunden werden. 💔") : MessageInterpreter.MarkBold("Could not find information for " + now.ToString("dd.MM.yyyy") + ". 💔")};
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
                var sssx = i;
                menuItems[i] = (key == LanguageKey.DE ? "Speiseplan für" : "Menu for") + ":" + MessageInterpreter.LineBreak + MessageInterpreter.MarkBold((key == LanguageKey.DE ? _canteens[(int)canteenName].GermanDescriptions[i] : _canteens[(int)canteenName].EnglishDescriptions[i])) + " "
                               + (key == LanguageKey.DE ? "am  " : "at ") + MessageInterpreter.MarkItalic(dayElements[i].Date.ToString("dd.MM.yyyy"))
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
                    if (!string.IsNullOrEmpty(foodElement.Cost))
                    {
                        tagResult += "  " + foodElement.Cost + "€";
                    }

                    string warning = null;

                    if (string.IsNullOrEmpty(foodElement.EnglishName.Trim()) && key == LanguageKey.EN)
                        warning = MessageInterpreter.MarkItalic("no english translation available");

                    if (!hideElement)
                    {
                        if (key == LanguageKey.DE || warning != null)
                            menuItems[i] += MessageInterpreter.MarkBold(foodElement.GermanName) + warning + " " + MessageInterpreter.LineBreak + tagResult + MessageInterpreter.DrawLine;
                        else
                            menuItems[i] += MessageInterpreter.MarkBold(MessageInterpreter.FirstCharToUpper(foodElement.EnglishName)) + " " + MessageInterpreter.LineBreak + tagResult
                                            + MessageInterpreter.DrawLine;
                    }
                }
            }

            return menuItems;
        }

        public string CreateStartMessage(LanguageKey key)
        {
            string text = MessageInterpreter.MarkBold("Welcome!") + " Hungry? 😋" + MessageInterpreter.LineBreak + "I can help you to solve this problem.";

            return text;
        }

        public string CreateListCanteensMessage(LanguageKey key)
        {
            if(_canteens == null) CreateCanteenInfo();

            string text = MessageInterpreter.MarkBold((key == LanguageKey.DE) ? "Auflistung aller Mensen und ihrer Befehlsbezeichnung:": "List of all canteens and their shortcuts:");
            text += MessageInterpreter.DrawLine;

            for (int i = 0; i < _canteens.Count; i++)
            {
                foreach (var description in (key == LanguageKey.DE) ? _canteens[i].GermanDescriptions : _canteens[i].EnglishDescriptions)
                {
                    text += MessageInterpreter.MarkBold(description) + ", ";
                }
                text = text.Substring(0, text.Length - 2);
                text += ": ";
                foreach (var coName in MessageInterpreter.Get.FindCanteenNames(_canteens[i].CanteenName, key))
                {
                    text += coName + ", ";
                }
                text = text.Substring(0, text.Length - 2) + MessageInterpreter.LineBreak;
            }

            return text;
        }

        private void CreateCanteenInfo()
        {
            _canteens = new List<CanteenElement>();

            _canteens.Add(new CanteenElement(CanteenName.LOWER_HALL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-unten/"}, new string[] { "Uni-Campus unterer Speisesaal"}, new string[] { "Uni-Campus lower hall" }));
            _canteens.Add(new CanteenElement(CanteenName.UPPER_HALL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-oben/"}, new string[] { "Uni-Campus oberer Speisesaal" }, new string[] { "Uni-Campus upper hall" }));
            _canteens.Add(new CanteenElement(CanteenName.KELLERCAFE, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-kellercafe/speiseplan/"}, new string[] { "Kellercafé"}));
            _canteens.Add(new CanteenElement(CanteenName.HERRENKRUG, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-herrenkrug/speiseplan/"}, new string[] { "Herrenkrug"}));
            _canteens.Add(new CanteenElement(CanteenName.STENDAL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-stendal/speiseplan/"}, new string[] { "Stendal"}));
            _canteens.Add(new CanteenElement(CanteenName.WERNIGERODE, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-wernigerode/speiseplan/"}, new string[] { "Wernigerode"}));
            _canteens.Add(new CanteenElement(CanteenName.HALBERSTADT, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-halberstadt/speiseplan/"}, new string[] { "Halberstadt"}));
            _canteens.Add(new CanteenElement(CanteenName.UPPER_HALL_LOWER_HALL, new string[] { "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-unten/",
                                                                                               "https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-oben/" }, new string[] { "Uni-Campus unterer Speisesaal", "Uni-Campus oberer Speisesaal" }, new string[] { "Uni-Campus lower hall", "Uni-Campus upper hall" }));
        }

        #endregion
    }
}