using System;
using System.Linq;

namespace MensaBotParsing.Mensa
{
    using System.Collections.Generic;

    class FoodElement
    {
        #region properties

        public string AdditivesAndAllergenics { get; private set; }

        public string Cost { get; private set; }

        public string EnglishName { get; private set; }

        public string GermanName { get; private set; }

        public List<FoodTags> Tags { get; private set; }

        #endregion

        #region constructors and destructors

        public FoodElement(string germanName, string englishName, string cost, List<FoodTags> tags, string additivesAndAllergenics)
        {
            GermanName = germanName;
            EnglishName = englishName;
            Cost = cost;
            Tags = tags;
            AdditivesAndAllergenics = additivesAndAllergenics;
        }

        #endregion

        #region methods

        public static string FoodTagsToEmoji(FoodTags tag)
        {
            switch (tag)
            {
                case FoodTags.PORK:
                    return "🐷";
                case FoodTags.CHICKEN:
                    return "🐔";
                case FoodTags.FISH:
                    return "🐟";
                case FoodTags.ALCOHOL:
                    return "🍷";
                case FoodTags.HOGGET:
                    return "🐑";
                case FoodTags.BEEF:
                    return "🐄";
                case FoodTags.VEGETARIAN:
                    return "🌽";
                case FoodTags.VEGAN:
                    return "🌻";
                case FoodTags.BIO:
                    return "✅";
                case FoodTags.VITAL:
                    return "🚴";
                case FoodTags.SOUP:
                    return "🍲";
                case FoodTags.GARLIC:
                    return "K";
                case FoodTags.VENSION:
                    return "🐗";

                default:
                    return tag.ToString();
            }
        }

        #endregion
    }

    public enum FoodTags
    {
        VITAL,
        VEGAN,
        GARLIC,
        ALCOHOL,
        PORK,
        VEGETARIAN,
        SOUP,
        FISH,
        CHICKEN,
        BEEF,
        VENSION,
        BIO,
        HOGGET,

        size
    }
}