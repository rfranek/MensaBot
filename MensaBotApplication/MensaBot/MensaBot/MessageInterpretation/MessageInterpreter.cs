using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Globalization;

    using MensaBot.Resources;

    using MensaBotParsing.Mensa;

    public class MessageInterpreter
    {
        #region constants

        public static string DrawLine = "\n\n---\n\n";

        public static string LineBreak = "\n\n";

        public static readonly char ParamDivider = ',';

        public static readonly string[] AvailableLanguages = new string[] {"de", "en"};

        private static MessageInterpreter _instance;
        private static readonly string MessageBold = "__";
        private static readonly string MessageItalic = "*";
        
        #endregion

        #region member vars

        private readonly CommandAlternatives _canteenNameHalberstadt;
        private readonly CommandAlternatives _canteenNameHerrenkrug;
        private readonly CommandAlternatives _canteenNameKellerCafe;
        private readonly CommandAlternatives _canteenNameWernigerode;
        private readonly CommandAlternatives _canteenNameLowerHall;
        private readonly CommandAlternatives _canteenNameUpperHall;
        private readonly CommandAlternatives _canteenNameStendal;
        private readonly CommandAlternatives _canteenNameLowerOrUpper;

        private readonly CommandAlternatives _dateNames;
        private readonly CommandAlternatives _mainCommands;
        private readonly CommandAlternatives _possibleTagName;

        #endregion

        #region properties

        public static MessageInterpreter Get => _instance ?? (_instance = new MessageInterpreter());

        #endregion

        #region constructors and destructors

        private MessageInterpreter()
        {
            _mainCommands = new CommandAlternatives();
            _mainCommands.ReplaceCommands(LanguageKey.DE, new[] { "mensa", "kantine", "futtern", "futter", "hunger", "schnabulieren", "spachteln", "dinieren", "speisen", "essen" });
            _mainCommands.ReplaceCommands(LanguageKey.EN, new[] { "canteen", "nosh", "eat", "menu" });

            _dateNames = new CommandAlternatives();
            _dateNames.ReplaceCommands(LanguageKey.DE, new[] { "heute", "morgen", "übermorgen" });
            _dateNames.ReplaceCommands(LanguageKey.EN, new[] { "today", "tomorrow", "the_day_after_tomorrow" });

            _possibleTagName = new CommandAlternatives();
            _possibleTagName.ReplaceCommands(LanguageKey.DE, new[] { Lang.ResourceManager.GetString("food_tag_alcohol", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_beef", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_bio", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_chicken", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_fish", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_garlic", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_hogget", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_pork", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_soup", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_vital", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_vegan", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_vegetarian", CultureInfo.GetCultureInfo("de")), Lang.ResourceManager.GetString("food_tag_vension", CultureInfo.GetCultureInfo("de")) });
            _possibleTagName.ReplaceCommands(LanguageKey.EN, new[] { Lang.ResourceManager.GetString("food_tag_alcohol", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_beef", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_bio", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_chicken", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_fish", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_garlic", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_hogget", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_pork", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_soup", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_vital", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_vegan", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_vegetarian", CultureInfo.GetCultureInfo("en")), Lang.ResourceManager.GetString("food_tag_vension", CultureInfo.GetCultureInfo("en")) });

            _canteenNameLowerHall = new CommandAlternatives();
            _canteenNameLowerHall.ReplaceCommands(LanguageKey.DE, new[] { "u", "unten", "campus_unten", "mensa_unten", "erdgeschoss" });
            _canteenNameLowerHall.ReplaceCommands(LanguageKey.EN, new[] { "l", "lower", "campus_lower", "canteen_lower", "main_floor", "lower_hall" });

            _canteenNameUpperHall = new CommandAlternatives();
            _canteenNameUpperHall.ReplaceCommands(LanguageKey.DE, new[] { "o", "oben", "campus_oben", "mensa_oben", "obergeschoss" });
            _canteenNameUpperHall.ReplaceCommands(LanguageKey.EN, new[] { "u", "upper", "campus_upper", "canteen_upper", "upper_floor", "upper_hall" });

            _canteenNameLowerOrUpper = new CommandAlternatives();
            _canteenNameLowerOrUpper.ReplaceCommands(LanguageKey.DE, new[] { "c", "uni", "campus", "unten&oben", "oben&unten", "uni_campus" });
            _canteenNameLowerOrUpper.ReplaceCommands(LanguageKey.EN, new[] { "c", "uni", "campus", "lower&upper", "upper&lower", "uni_campus" });

            _canteenNameKellerCafe = new CommandAlternatives();
            _canteenNameKellerCafe.ReplaceCommands(LanguageKey.DE, new[] { "k", "keller", "kellercafe", "kellercafé" });
            _canteenNameKellerCafe.ReplaceCommands(LanguageKey.EN, new[] { "c", "cellar", "kellercafe", "cellar_cafe", "cellar_café", "basement", "basementcafe", "basementcafé" });

            _canteenNameHerrenkrug = new CommandAlternatives();
            _canteenNameHerrenkrug.ReplaceCommands(LanguageKey.DE, new[] { "he", "herrenkrug", "krug", "herren" });
            _canteenNameHerrenkrug.ReplaceCommands(LanguageKey.EN, new[] { "he", "herrenkrug", "krug" });

            _canteenNameWernigerode = new CommandAlternatives();
            _canteenNameWernigerode.ReplaceCommands(LanguageKey.DE, new[] { "w", "wernigerode", "rode", "wernig" });
            _canteenNameWernigerode.ReplaceCommands(LanguageKey.EN, new[] { "w", "wernigerode", "rode", "wernig" });

            _canteenNameHalberstadt = new CommandAlternatives();
            _canteenNameHalberstadt.ReplaceCommands(LanguageKey.DE, new[] { "ha", "halberstadt", "halber" });
            _canteenNameHalberstadt.ReplaceCommands(LanguageKey.EN, new[] { "ha", "halberstadt", "halber" });

            _canteenNameStendal = new CommandAlternatives();
            _canteenNameStendal.ReplaceCommands(LanguageKey.DE, new[] { "s", "stendal" });
            _canteenNameStendal.ReplaceCommands(LanguageKey.EN, new[] { "s", "stendal" });
        }

        #endregion

        #region methods

        public LanguageKey ContainsCommands(MessageInterType type, string messagePart)
        {
            switch (type)
            {
                case MessageInterType.MAIN_COMMAND:
                    if (_mainCommands.Contains(messagePart, LanguageKey.DE))
                        return LanguageKey.DE;
                    if (_mainCommands.Contains(messagePart, LanguageKey.EN))
                        return LanguageKey.EN;
                    break;
                case MessageInterType.DATE:
                    if (_dateNames.Contains(messagePart, LanguageKey.DE))
                        return LanguageKey.DE;
                    if (_dateNames.Contains(messagePart, LanguageKey.EN))
                        return LanguageKey.EN;
                    break;

            }
            return LanguageKey.none;
        }

        public CanteenName FindCanteen(string messagePart, LanguageKey languageKey)
        {
            if (_canteenNameLowerHall.Contains(messagePart, languageKey) || messagePart == CanteenName.LOWER_HALL.ToString())
                return CanteenName.LOWER_HALL;
            if (_canteenNameUpperHall.Contains(messagePart, languageKey) || messagePart == CanteenName.UPPER_HALL.ToString())
                return CanteenName.UPPER_HALL;
            if (_canteenNameHalberstadt.Contains(messagePart, languageKey) || messagePart == CanteenName.HALBERSTADT.ToString())
                return CanteenName.HALBERSTADT;
            if (_canteenNameWernigerode.Contains(messagePart, languageKey) || messagePart == CanteenName.WERNIGERODE.ToString())
                return CanteenName.WERNIGERODE;
            if (_canteenNameHerrenkrug.Contains(messagePart, languageKey) || messagePart == CanteenName.HERRENKRUG.ToString())
                return CanteenName.HERRENKRUG;
            if (_canteenNameKellerCafe.Contains(messagePart, languageKey) || messagePart == CanteenName.KELLERCAFE.ToString())
                return CanteenName.KELLERCAFE;
            if (_canteenNameStendal.Contains(messagePart, languageKey) || messagePart == CanteenName.STENDAL.ToString())
                return CanteenName.STENDAL;
            if (_canteenNameLowerOrUpper.Contains(messagePart, languageKey) || messagePart == CanteenName.UPPER_HALL_LOWER_HALL.ToString())
                return CanteenName.UPPER_HALL_LOWER_HALL;

            return CanteenName.none;
        }

        public FoodTags FindTag(string messagePart, LanguageKey languageKey)
        {
            var index = _possibleTagName.IndexOf(messagePart, languageKey);
            if (index<0)
                return FoodTags.NONE_FOOD_TAG;

            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.ALCOHOL), languageKey))
                return FoodTags.ALCOHOL;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.BIO), languageKey))
                return FoodTags.BIO;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.GARLIC), languageKey))
                return FoodTags.GARLIC;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.HOGGET), languageKey))
                return FoodTags.HOGGET;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.PORK), languageKey))
                return FoodTags.PORK;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.SOUP), languageKey))
                return FoodTags.SOUP;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VEGAN), languageKey))
                return FoodTags.VEGAN;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VEGETARIAN), languageKey))
                return FoodTags.VEGETARIAN;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VITAL), languageKey))
                return FoodTags.VITAL;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VENSION), languageKey))
                return FoodTags.VENSION;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.BEEF), languageKey))
                return FoodTags.BEEF;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.CHICKEN), languageKey))
                return FoodTags.CHICKEN;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.FISH), languageKey))
                return FoodTags.FISH;


            return FoodTags.NONE_FOOD_TAG;
        }

        public string [] FindCanteenNames(CanteenName name, LanguageKey languageKey)
        {
            switch (name)
            {
                case CanteenName.LOWER_HALL:
                    return this._canteenNameLowerHall.listCommands(languageKey);
                case CanteenName.UPPER_HALL:
                    return this._canteenNameUpperHall.listCommands(languageKey);
                case CanteenName.HERRENKRUG:
                    return this._canteenNameHerrenkrug.listCommands(languageKey);
                case CanteenName.HALBERSTADT:
                    return this._canteenNameHalberstadt.listCommands(languageKey);
                case CanteenName.KELLERCAFE:
                    return this._canteenNameKellerCafe.listCommands(languageKey);
                case CanteenName.STENDAL:
                    return this._canteenNameStendal.listCommands(languageKey);
                case CanteenName.WERNIGERODE:
                    return this._canteenNameWernigerode.listCommands(languageKey);
                case CanteenName.UPPER_HALL_LOWER_HALL:
                    return this._canteenNameLowerOrUpper.listCommands(languageKey);
            }
            return new[] { "" };
        }

        public DateIndex FindDate(string messagePart, LanguageKey key)
        {
            var index = _dateNames.IndexOf(messagePart, key);

            switch (index)
            {
                case 0:
                    return DateIndex.TODAY;
                case 1:
                    return DateIndex.TOMORROW;
                case 2:
                    return DateIndex.DAY_AFTER_TOMORROW;
                default:
                    return DateIndex.none;
            }
        }

        public static string FirstCharToUpper(string message)
        {
            if (String.IsNullOrEmpty(message))
                return message;
            return message.First().ToString().ToUpper() + message.Substring(1);
        }

        public static string MarkBold(string message)
        {
            return MessageBold + message + MessageBold;
        }

        public static string MarkItalic(string message)
        {
            return MessageItalic + message + MessageItalic;
        }

        #endregion
    }

    public enum MessageInterType
    {
        MAIN_COMMAND,
        DATE,
        LOCATION
    }

    public enum DateIndex
    {
        TODAY = 0,
        TOMORROW = 1,
        DAY_AFTER_TOMORROW = 2,

        none = 9001
    }

    public enum CanteenName
    {
        LOWER_HALL = 0,
        UPPER_HALL = 1,
        KELLERCAFE = 2,
        HERRENKRUG = 3,
        STENDAL = 4,
        WERNIGERODE = 5,
        HALBERSTADT = 6,
        UPPER_HALL_LOWER_HALL = 7,

        none
    }
}