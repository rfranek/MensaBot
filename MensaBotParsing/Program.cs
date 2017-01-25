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

            canteen.Add(new CanteenElement("HiddenLinkn-unten/", "UniCampus unten"));
            canteen.Add(new CanteenElement("HiddenLinkn-oben/", "UniCampus oben"));
            canteen.Add(new CanteenElement("HiddenLinkan/", "Kellercafé"));
            canteen.Add(new CanteenElement("HiddenLinkan/", "Herrenkrug"));
            canteen.Add(new CanteenElement("HiddenLink", "Stendal"));
            canteen.Add(new CanteenElement("HiddenLinklan/", "Wernigerode"));
            canteen.Add(new CanteenElement("HiddenLinklan/", "Halberstadt"));

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