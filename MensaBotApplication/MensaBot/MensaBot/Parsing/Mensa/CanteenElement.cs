using System;
using System.Linq;

namespace MensaBotParsing.Mensa
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;

    using MensaBot.MessageInterpretation;
    using MensaBot.Resources;
    using MensaBot.Resources.Language;

    class CanteenElement
    {
        #region constants

        private static readonly string regexFindHeader = "<thead.*?></thead>";

        private static readonly string regexFindTable = "<table>.*?</table>";

        private static readonly string regexRemoveTDWithInner = "(<td.*?>)(.*?)(</td>)";

        private static readonly string regexRemoveTRWithInner = "(<tr.*?>)(.*?)(</tr>)";

        private static readonly string regexReplaceHeader = "(<thead.*?>)(.*?)(</thead>)";

        private static readonly char splitOperator = '#';

        #endregion

        #region member vars

        private readonly string [] _pages;

        #endregion

        #region properties

        public List<DayElement> DayElements { get; private set; }

        public string[] DescriptionId { get; private set; }
        public CanteenName CanteenName { get; private set; }

        #endregion

        #region constructors and destructors

        public CanteenElement(CanteenName canteenName, string[] pages, string[] descriptions)
        {
            this._pages = pages;
            this.CanteenName = canteenName;
            DescriptionId = descriptions;

        }

        public string GetDescription(int index)
        {
            return Lang.ResourceManager.GetString(DescriptionId[index]);
        }
        /*public CanteenElement(CanteenName canteenName ,string[] pages, string [] germanDescriptions, string[] englishDescriptions)
        {
            this._pages = pages;
            this.CanteenName = canteenName;
            this.GermanDescriptions = germanDescriptions;
            this.EnglishDescriptions = englishDescriptions;
        }*/

        #endregion

        #region methods

        public void LoadElements(int maxElements)
        {
            DayElements = new List<DayElement>();
            foreach (var single_page in _pages)
            {
                Console.WriteLine(single_page);
                DayElements.AddRange(ListDayElementOnPage(single_page, maxElements));
            }
            
        }

        private static string ExtractAdditivesAndAllergenicInformation(string element)
        {
            string result = element;

            string additivesAllergenicDiv = "<div style='clear:both;'>";
            result = result.Remove(0, result.IndexOf(additivesAllergenicDiv));
            result = result.Replace(additivesAllergenicDiv, "").Replace("</div>", "").Replace(" </ div>", "");

            return result;
        }

        private static string ExtractFoodBasicInformation(string element)
        {
            string result = element;
            result = result.Replace("<strong>", "").Replace("</strong>", "");
            result = result.Replace("<br>", "").Replace("</br>", "").Replace("<br />", "");
            result = result.Replace(" class='gruen'", "").Replace(" class='grau'", "");

            if (result.StartsWith("<span><span>"))
                result = result.Remove(0, 12);

            if (result.StartsWith("<span>"))
                result = result.Remove(0, 6);

            result = result.Replace("</span></span><span><span></span><span></span>", splitOperator.ToString());
            result = result.Replace("</span><span><span></span>", splitOperator.ToString());
            result = result.Replace("</span>", splitOperator.ToString());
            result = result.Replace("<span>", splitOperator.ToString());

            return result;
        }

        private static List<FoodTags> ExtractTagInformation(string element)
        {
            List<FoodTags> tags = new List<FoodTags>();

            if (element.Contains("vital") || element.Contains("MensaVital"))
                tags.Add(FoodTags.VITAL);
            if (element.Contains("vegan"))
                tags.Add(FoodTags.VEGAN);
            if (element.Contains("knoblauch") || element.Contains("knoblauch") || element.Contains("garlic"))
                tags.Add(FoodTags.GARLIC);
            if (element.Contains("Alkohol") || element.Contains("alkohol") || element.Contains("alcohol"))
                tags.Add(FoodTags.ALCOHOL);
            if (element.Contains("Schwein") || element.Contains("schwein") || element.Contains("pork"))
                tags.Add(FoodTags.PORK);
            if (element.Contains("Vegetarisch") || element.Contains("vegetarisch") || element.Contains("vegetarian") || element.Contains("veggie") || element.Contains("veggy"))
                tags.Add(FoodTags.VEGETARIAN);
            if (element.Contains("Suppe") || element.Contains("suppe") || element.Contains("soup"))
                tags.Add(FoodTags.SOUP);
            if (element.Contains("Fisch") || element.Contains("fisch") || element.Contains("fish"))
                tags.Add(FoodTags.FISH);
            if (element.Contains("Geflügel") || element.Contains("geflügel") || element.Contains("Gefluegel") || element.Contains("gefluegel") || element.Contains("chicken"))
                tags.Add(FoodTags.CHICKEN);
            if (element.Contains("Rind") || element.Contains("rind") || element.Contains("beef"))
                tags.Add(FoodTags.BEEF);
            if (element.Contains("Wild") || element.Contains("wild") || element.Contains("venison"))
                tags.Add(FoodTags.VENSION);
            if (element.Contains("Bio") || element.Contains("bio"))
                tags.Add(FoodTags.BIO);
            if (element.Contains("Lamm") || element.Contains("lamm") || element.Contains("hogget"))
                tags.Add(FoodTags.HOGGET);

            return tags;
        }

        private static List<FoodElement> FindBodyElements(string bodyTag)
        {
            MatchCollection foodElements = Regex.Matches(bodyTag, regexRemoveTDWithInner);

            List<FoodElement> elements = new List<FoodElement>();

            for (int i = 0; i < (foodElements.Count + 1) / 2; i++)
            {
                string basicElementInformation = ExtractFoodBasicInformation(foodElements[i * 2].Groups[2].ToString());

                List<FoodTags> foodTags = null;
                string additivesAndAllergenics = null;
                if ((i * 2) + 1 < foodElements.Count)
                {
                    foodTags = ExtractTagInformation(foodElements[(i * 2) + 1].Groups[2].ToString());
                    additivesAndAllergenics = ExtractAdditivesAndAllergenicInformation(foodElements[(i * 2) + 1].Groups[2].ToString());
                }

                string[] basicElementInformationSplit = basicElementInformation.Split(splitOperator);

                if (basicElementInformationSplit.Length == 2)
                    elements.Add(new FoodElement(basicElementInformationSplit[0], basicElementInformationSplit[1], null, foodTags, additivesAndAllergenics));
                else if (basicElementInformationSplit.Length == 3)
                    elements.Add(new FoodElement(basicElementInformationSplit[0], basicElementInformationSplit[1], basicElementInformationSplit[2], foodTags, additivesAndAllergenics));
            }

            return elements;
        }

        private string FindDateInHeader(string headerTag)
        {
            string findHeader = Regex.Match(headerTag, regexReplaceHeader).Groups[2].ToString();
            string headerWithoutTr = Regex.Match(findHeader, regexRemoveTRWithInner).Groups[2].ToString();
            string headerWithoutTd = Regex.Match(headerWithoutTr, regexRemoveTDWithInner).Groups[2].ToString();

            string dateString = Regex.Match(headerWithoutTd, "\\d\\d.\\d\\d.\\d\\d\\d\\d").ToString();

            return dateString;
        }

        private string DownloadString(string page)
        {

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(page);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            string data = null;
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = null;

                if (httpWebResponse.CharacterSet == null)
                    streamReader = new StreamReader(responseStream);
                else
                    streamReader = new StreamReader(responseStream, Encoding.GetEncoding(httpWebResponse.CharacterSet));

                data = streamReader.ReadToEnd();

                httpWebResponse.Close();
                streamReader.Close();
            }

            return data ?? "";
        }

        private List<DayElement> ListDayElementOnPage(String page, int maxElement)
        {
            string htmlAsString = DownloadString(page);

            List <DayElement> dayElements = new List<DayElement>();

            if (Regex.IsMatch(htmlAsString, regexFindTable))
            {
                MatchCollection tableCollection = Regex.Matches(htmlAsString, regexFindTable);

                for (int i = 0; i < Math.Min(tableCollection.Count, maxElement); i++)
                {
                    string table = tableCollection[i].ToString();
                    string header = Regex.Match(table, regexFindHeader).ToString();
                    string date = FindDateInHeader(header);
                    
                    var body = table.Replace(header, "");
                    List<FoodElement> foodElements = FindBodyElements(body);

                    dayElements.Add(new DayElement(date, foodElements));
                }
            }

            return dayElements;
        }

        #endregion
    }
}