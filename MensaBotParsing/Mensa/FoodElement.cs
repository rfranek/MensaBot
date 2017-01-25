using System;
using System.Linq;

namespace MensaBotParsing.Mensa
{
    using System.Collections.Generic;

    class FoodElement
    {
        #region properties

        public string AdditivesAndAllergenics { get; private set; }

        public string EnglishName { get; private set; }

        public string GermanName { get; private set; }

        public string Price { get; private set; }

        public List<FoodTags> Tags { get; private set; }

        #endregion

        #region constructors and destructors

        public FoodElement(string germanName, string englishName, string price, List<FoodTags> tags, string additivesAndAllergenics)
        {
            GermanName = germanName;
            EnglishName = englishName;
            Price = price;
            Tags = tags;
            AdditivesAndAllergenics = additivesAndAllergenics;
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
        HOGGET
    }
}