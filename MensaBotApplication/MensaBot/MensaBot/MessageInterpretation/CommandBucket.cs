using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Collections.Generic;

    using MensaBotParsing.Mensa;

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
                    text += FoodElement.FoodTagsToEmoji(FoodTags.ALCOHOL) + " Alkohol" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.BIO) + " BIO" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.FISH) + " Fisch" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.CHICKEN) + " Hühnchen" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.GARLIC) + " Knoblauch" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.HOGGET) + " Lamm" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VITAL) + " Mensa Vital" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.BEEF) + " Rindfleisch" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.PORK) + " Schwein" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.SOUP) + " Suppe" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VEGAN) + " Vegan" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VEGETARIAN) + " Vegetarisch" + MessageInterpreter.LineBreak;
                    text += FoodElement.FoodTagsToEmoji(FoodTags.VENSION) + " Wild" + MessageInterpreter.LineBreak;
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

        public string [] CreateMensaReply(LanguageKey key, string[] messageParts)
        {
            if (messageParts.Length < 2)
            {
                return new string[] { "Please add " + MessageInterpreter.MarkBold("mensa name") + "!"};
            }

            CanteenName canteenName = MessageInterpreter.Get.FindCanteen(messageParts[1], key);

            if (canteenName == CanteenName.none)
            {
                return new string[] { "Could not find canteen with name " + MessageInterpreter.MarkBold(messageParts[1])};
            }

            try
            {
                if (_canteens == null)
                    CreateCanteenInfo();

                _canteens[(int)canteenName].LoadElements(3);
            }
            catch (Exception e)
            {
                return new string[] { "Fail to load information about " + MessageInterpreter.MarkBold(messageParts[1])};
            }

            DateIndex dateIndex = DateIndex.TODAY;

            if (messageParts.Length > 2)
            {
                dateIndex = MessageInterpreter.Get.FindDate(messageParts[2], key);
                if (dateIndex == DateIndex.none)
                {
                    return new string[] { "Could not find date with name " + MessageInterpreter.MarkBold(messageParts[2]) };
                }
            }

            //Find correct date element.
            DateTime now = DateTime.Now.ToUniversalTime().AddDays((int)dateIndex).AddHours(1);
            DayElement[] dayElements = null;

            try
            {
                dayElements = _canteens[(int)canteenName].DayElements.FindAll(t => t.Date.Day == now.Day).ToArray();
            }
            catch (Exception e)
            {  }

            if (dayElements == null || dayElements.Length == 0)
            {
                return new string[] { key == LanguageKey.DE ? MessageInterpreter.MarkBold("Es konnten keine Informationen für den " + now.ToString("dd.MM.yyyy") + " gefunden werden. 💔") : MessageInterpreter.MarkBold("Could not find information for " + now.ToString("dd.MM.yyyy") + ". 💔")};
            }

            string [] menuItems = new string[dayElements.Length];

            //List all elements for dayElement
            for (int i = 0; i < dayElements.Length; i++)
            {
                menuItems[i] = (key == LanguageKey.DE ? "Speiseplan für" : "Menu for") + ":" + MessageInterpreter.LineBreak + MessageInterpreter.MarkBold((key == LanguageKey.DE ? _canteens[(int)canteenName].GermanDescriptions[i] : _canteens[(int)canteenName].EnglishDescriptions[i])) + " "
                               + (key == LanguageKey.DE ? "am  " : "at ") + MessageInterpreter.MarkItalic(dayElements[i].Date.ToString("dd.MM.yyyy"))
                               + MessageInterpreter.DrawLine;

                foreach (var foodElement in dayElements[i].FoodElements)
                {
                    var tagResult = "";
                    if (foodElement.Tags != null)
                    {
                        foreach (var tag in foodElement.Tags)
                        {
                            if (tag != null)
                                tagResult += FoodElement.FoodTagsToEmoji(tag) + ",";
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

                    if (key == LanguageKey.DE || warning != null)
                        menuItems[i] += MessageInterpreter.MarkBold(foodElement.GermanName) + warning + " " + MessageInterpreter.LineBreak + tagResult + MessageInterpreter.DrawLine;
                    else
                        menuItems[i] += MessageInterpreter.MarkBold(MessageInterpreter.FirstCharToUpper(foodElement.EnglishName)) + " " + MessageInterpreter.LineBreak + tagResult
                                     + MessageInterpreter.DrawLine;
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