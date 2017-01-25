using System;

namespace MensaBotParsing
{
    using System.Collections.Generic;

    using MensaBotParsing.Mensa;

    class Program
    {
        #region methods

        static void Main(string[] args)
        {
            List<CanteenElement> canteen = new List<CanteenElement>();

            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-unten/", "UniCampus unten"));
            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-oben/", "UniCampus oben"));
            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-kellercafe/speiseplan/", "Kellercafé"));
            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-herrenkrug/speiseplan/", "Herrenkrug"));
            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-stendal/speiseplan/", "Stendal"));
            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-wernigerode/speiseplan/", "Wernigerode"));
            canteen.Add(new CanteenElement("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-halberstadt/speiseplan/", "Halberstadt"));

            for (int i = 0; i < canteen.Count; i++)
            {
                canteen[i].LoadElements();
                var item = canteen[i].DayElements.Count;
                Console.WriteLine("Loaded " + item + " items for " + canteen[i].Name);

            }

            Console.ReadKey(true);
        }

        #endregion
    }
}