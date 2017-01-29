using System;
using System.Linq;

namespace MensaBot.MessageInterpretation
{
    public class MessageInterpreter
    {
        #region constants

        public static string DrawLine = "\n\n---\n\n";

        public static string LineBreak = "\n\n";

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
        private readonly CommandAlternatives _canteenNameLowerOrUpper;

        private readonly CommandAlternatives _dateNames;
        private readonly CommandAlternatives _mainCommands;

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

            _canteenNameLowerHall = new CommandAlternatives();
            _canteenNameLowerHall.ReplaceCommands(LanguageKey.DE, new[] { "unten", "campus_unten", "mensa_unten", "erdgeschoss" });
            _canteenNameLowerHall.ReplaceCommands(LanguageKey.EN, new[] { "lower", "campus_lower", "canteen_lower", "main_floor", "lower_hall" });

            _canteenNameUpperHall = new CommandAlternatives();
            _canteenNameUpperHall.ReplaceCommands(LanguageKey.DE, new[] { "oben", "campus_oben", "mensa_oben", "obergeschoss" });
            _canteenNameUpperHall.ReplaceCommands(LanguageKey.EN, new[] { "upper", "campus_upper", "canteen_upper", "upper_floor", "upper_hall" });

            _canteenNameLowerOrUpper = new CommandAlternatives();
            _canteenNameLowerOrUpper.ReplaceCommands(LanguageKey.DE, new[] { "campus", "unten&oben", "oben&unten", "uni_campus" });
            _canteenNameLowerOrUpper.ReplaceCommands(LanguageKey.EN, new[] { "campus", "lower&upper", "upper&lower", "uni_campus" });

            _canteenNameKellerCafe = new CommandAlternatives();
            _canteenNameKellerCafe.ReplaceCommands(LanguageKey.DE, new[] { "keller", "kellercafe", "kellercafé" });
            _canteenNameKellerCafe.ReplaceCommands(LanguageKey.EN, new[] { "cellar", "kellercafe", "cellar_cafe", "cellar_café", "basement", "basementcafe", "basementcafé" });

            _canteenNameHerrenkrug = new CommandAlternatives();
            _canteenNameHerrenkrug.ReplaceCommands(LanguageKey.DE, new[] { "herrenkrug", "krug", "herren" });
            _canteenNameHerrenkrug.ReplaceCommands(LanguageKey.EN, new[] { "herrenkrug", "krug" });

            _canteenNameWernigerode = new CommandAlternatives();
            _canteenNameWernigerode.ReplaceCommands(LanguageKey.DE, new[] { "wernigerode", "rode", "wernig" });
            _canteenNameWernigerode.ReplaceCommands(LanguageKey.EN, new[] { "wernigerode", "rode", "wernig" });

            _canteenNameHalberstadt = new CommandAlternatives();
            _canteenNameHalberstadt.ReplaceCommands(LanguageKey.DE, new[] { "halberstadt", "halber" });
            _canteenNameHalberstadt.ReplaceCommands(LanguageKey.EN, new[] { "halberstadt", "halber" });
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
            if (_canteenNameLowerHall.Contains(messagePart, languageKey))
                return CanteenName.LOWER_HALL;
            if (_canteenNameUpperHall.Contains(messagePart, languageKey))
                return CanteenName.UPPER_HALL;
            if (_canteenNameHalberstadt.Contains(messagePart, languageKey))
                return CanteenName.HALBERSTADT;
            if (_canteenNameWernigerode.Contains(messagePart, languageKey))
                return CanteenName.WERNIGERODE;
            if (_canteenNameHerrenkrug.Contains(messagePart, languageKey))
                return CanteenName.HERRENKRUG;
            if (_canteenNameKellerCafe.Contains(messagePart, languageKey))
                return CanteenName.KELLERCAFE;
            if (_canteenNameLowerOrUpper.Contains(messagePart, languageKey))
                return CanteenName.UPPER_HALL_LOWER_HALL;

            return CanteenName.none;
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