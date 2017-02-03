using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    using System.Globalization;

    using MensaBot.Resources;
    using MensaBot.Resources.CanteenCommand;
    using MensaBot.Resources.CanteenNames;
    using MensaBot.Resources.Dates;
    using MensaBot.Resources.Language;

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
            _mainCommands.ReplaceCommands(new[] { "canteen_0", "canteen_1", "canteen_2", "canteen_3", "canteen_4", "canteen_5", "canteen_6", "canteen_7", "canteen_8", "canteen_9" });

            _dateNames = new CommandAlternatives();
            _dateNames.ReplaceCommands(new[] { "today", "tomorrow", "the_day_after_tomorrow" });

            _possibleTagName = new CommandAlternatives();
            _possibleTagName.ReplaceCommands(new[] { "food_tag_alcohol", "food_tag_beef", "food_tag_bio", "food_tag_chicken", "food_tag_fish", "food_tag_garlic", "food_tag_hogget", "food_tag_pork", "food_tag_soup", "food_tag_vital", "food_tag_vegan", "food_tag_vegetarian", "food_tag_vension" });

            _canteenNameLowerHall = new CommandAlternatives();
            _canteenNameLowerHall.ReplaceCommands(new[] { "canteen_Name_LowerUpper_0", "canteen_Name_LowerUpper_1","canteen_Name_LowerUpper_2", "canteen_Name_LowerUpper_3", "canteen_Name_LowerUpper_4", "canteen_Name_LowerUpper_5" });

            _canteenNameUpperHall = new CommandAlternatives();
            _canteenNameUpperHall.ReplaceCommands(new[] { "canteen_Name_Upper_Hall_0", "canteen_Name_Upper_Hall_1", "canteen_Name_Upper_Hall_2", "canteen_Name_Upper_Hall_3", "canteen_Name_Upper_Hall_4", "canteen_Name_Upper_Hall_5" });

            _canteenNameLowerOrUpper = new CommandAlternatives();
            _canteenNameLowerOrUpper.ReplaceCommands(new[] { "canteen_Name_LowerUpper_0", "canteen_Name_LowerUpper_1", "canteen_Name_LowerUpper_2", "canteen_Name_LowerUpper_3", "canteen_Name_LowerUpper_4", "canteen_Name_LowerUpper_5" });

            _canteenNameKellerCafe = new CommandAlternatives();
            _canteenNameKellerCafe.ReplaceCommands(new[] { "canteen_Name_Cellar_0", "canteen_Name_Cellar_1", "canteen_Name_Cellar_2", "canteen_Name_Cellar_3" });

            _canteenNameHerrenkrug = new CommandAlternatives();
            _canteenNameHerrenkrug.ReplaceCommands(new[] { "canteen_Name_Cellar_0", "canteen_Name_Cellar_1", "canteen_Name_Cellar_2", "canteen_Name_Cellar_3" });

            _canteenNameWernigerode = new CommandAlternatives();
            _canteenNameWernigerode.ReplaceCommands(new[] { "canteen_Name_Wernigerode_0", "canteen_Name_Wernigerode_1", "canteen_Name_Wernigerode_2", "canteen_Name_Wernigerode_3" });

            _canteenNameHalberstadt = new CommandAlternatives();
            _canteenNameHalberstadt.ReplaceCommands(new[] { "canteen_Name_Halberstadt_0", "canteen_Name_Halberstadt_1", "canteen_Name_Halberstadt_2" });

            _canteenNameStendal = new CommandAlternatives();
            _canteenNameStendal.ReplaceCommands(new[] { "canteen_Name_Stendal_0", "canteen_Name_Stendal_1" });
        }

        #endregion

        #region methods

        public bool ContainsCommands(MessageInterType type, string messagePart)
        {
            switch (type)
            {
                case MessageInterType.MAIN_COMMAND:
                    if (_mainCommands.ContainsCommand(CanteenCommand.ResourceManager.GetString(messagePart)))
                        return true;
                    break;
                case MessageInterType.DATE:
                    if (_dateNames.ContainsCommand(Dates.ResourceManager.GetString(messagePart)))
                        return true;
                    break;

            }
            return false;
        }

        public CanteenName FindCanteen(string messagePart)
        {
            if (_canteenNameLowerHall.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.LOWER_HALL.ToString())
                return CanteenName.LOWER_HALL;
            if (_canteenNameUpperHall.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.UPPER_HALL.ToString())
                return CanteenName.UPPER_HALL;
            if (_canteenNameHalberstadt.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.HALBERSTADT.ToString())
                return CanteenName.HALBERSTADT;
            if (_canteenNameWernigerode.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.WERNIGERODE.ToString())
                return CanteenName.WERNIGERODE;
            if (_canteenNameHerrenkrug.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.HERRENKRUG.ToString())
                return CanteenName.HERRENKRUG;
            if (_canteenNameKellerCafe.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.KELLERCAFE.ToString())
                return CanteenName.KELLERCAFE;
            if (_canteenNameStendal.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.STENDAL.ToString())
                return CanteenName.STENDAL;
            if (_canteenNameLowerOrUpper.ContainsCommand(CanteenNames.ResourceManager.GetString(messagePart)) || messagePart == CanteenName.UPPER_HALL_LOWER_HALL.ToString())
                return CanteenName.UPPER_HALL_LOWER_HALL;

            return CanteenName.none;
        }

        public FoodTags FindTag(string messagePart)
        {
            var index = _possibleTagName.IndexOf(messagePart);
            if (index<0)
                return FoodTags.NONE_FOOD_TAG;

            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.ALCOHOL)))
                return FoodTags.ALCOHOL;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.BIO)))
                return FoodTags.BIO;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.GARLIC)))
                return FoodTags.GARLIC;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.HOGGET)))
                return FoodTags.HOGGET;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.PORK)))
                return FoodTags.PORK;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.SOUP)))
                return FoodTags.SOUP;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VEGAN)))
                return FoodTags.VEGAN;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VEGETARIAN)))
                return FoodTags.VEGETARIAN;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VITAL)))
                return FoodTags.VITAL;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.VENSION)))
                return FoodTags.VENSION;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.BEEF)))
                return FoodTags.BEEF;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.CHICKEN)))
                return FoodTags.CHICKEN;
            if (index == _possibleTagName.IndexOf(FoodElement.FoodTagsToString(FoodTags.FISH)))
                return FoodTags.FISH;


            return FoodTags.NONE_FOOD_TAG;
        }

        public string [] FindCanteenNames(CanteenName name)
        {
            switch (name)
            {
                case CanteenName.LOWER_HALL:
                    return this._canteenNameLowerHall.listCommands();
                case CanteenName.UPPER_HALL:
                    return this._canteenNameUpperHall.listCommands();
                case CanteenName.HERRENKRUG:
                    return this._canteenNameHerrenkrug.listCommands();
                case CanteenName.HALBERSTADT:
                    return this._canteenNameHalberstadt.listCommands();
                case CanteenName.KELLERCAFE:
                    return this._canteenNameKellerCafe.listCommands();
                case CanteenName.STENDAL:
                    return this._canteenNameStendal.listCommands();
                case CanteenName.WERNIGERODE:
                    return this._canteenNameWernigerode.listCommands();
                case CanteenName.UPPER_HALL_LOWER_HALL:
                    return this._canteenNameLowerOrUpper.listCommands();
            }
            return new[] { "" };
        }

        public DateIndex FindDate(string messagePart)
        {
            var index = _dateNames.IndexOf(messagePart);

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
            if (string.IsNullOrEmpty(message))
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