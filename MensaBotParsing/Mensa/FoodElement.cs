using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaBotParsing
{
    class FoodElement
    {
        public String GermanName { get; private set; }
        public String EnglishName { get; private set; }
        public String Price { get; private set; }
        public List<FoodTags> Tags { get; private set; }
        public String AdditivesAndAllergenics { get; private set; }

        public FoodElement(String germanName, String englishName, String price, List<FoodTags> tags, String additivesAndAllergenics)
        {
            this.GermanName = germanName;
            this.EnglishName = englishName;
            this.Price = price;
            this.Tags = tags;
            this.AdditivesAndAllergenics = additivesAndAllergenics;
        }

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
