using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MensaBotParsing
{
    class Program
    {

        private static string regexRemoveTDWithInner = "(<td.*?>)(.*?)(</td>)";
        private static string regexRemoveTRWithInner = "(<tr.*?>)(.*?)(</tr>)";
        

        private static string regexReplaceHeader = "(<thead.*?>)(.*?)(</thead>)";
        private static string regexFindHeader ="<thead.*?></thead>";

        private static string splitOperator = "|||";

        static void Main(string[] args)
        {
            string html = new WebClient().DownloadString("https://www.studentenwerk-magdeburg.de/mensen-cafeterien/mensa-unicampus/speiseplan-unten/");
            string regexFindTable = "<table>.*?</table>";


            if (System.Text.RegularExpressions.Regex.IsMatch(html, regexFindTable))
            {

                MatchCollection tableCollection = System.Text.RegularExpressions.Regex.Matches(html, regexFindTable);

                for (int i = 0; i < tableCollection.Count; i++)
                {
                    var table = tableCollection[i].ToString();
                    var header = System.Text.RegularExpressions.Regex.Match(table, regexFindHeader).ToString();
                    System.Console.WriteLine(findHeader(header) + " - " + i);

                    var body = table.Replace(header, "");
                    System.Console.WriteLine(findBodyElements(body) + " - " + i);

                }

            }


            Console.ReadKey(true);
        }

        private static String findHeader(String headerTag)
        {
            String findHeader = System.Text.RegularExpressions.Regex.Match(headerTag, regexReplaceHeader).Groups[2].ToString();
            String headerWithoutTR = System.Text.RegularExpressions.Regex.Match(findHeader, regexRemoveTRWithInner).Groups[2].ToString();
            String headerWithoutTD = System.Text.RegularExpressions.Regex.Match(headerWithoutTR, regexRemoveTDWithInner).Groups[2].ToString();

            return headerWithoutTD;
        }

        private static String findBodyElements(String bodyTag){

            //String findBody = System.Text.RegularExpressions.Regex.Match(bodyTag, regexReplaceBody).Groups[2].ToString();

            MatchCollection foodElements = System.Text.RegularExpressions.Regex.Matches(bodyTag, regexRemoveTDWithInner);

           // Console.WriteLine(foodElements[0].ToString()+ "   ");

            var elementsNum = foodElements.Count;

            List<FoodElement> elements = new List<FoodElement>();

            for (int i = 0; i < elementsNum/2; i++)
            {
                String basicElementInformation = ExtractFoodBasicInformation(foodElements[i * 2].Groups[2].ToString());
                Console.WriteLine(basicElementInformation);

                List<FoodTags> foodTags = ExtractTagInformation(foodElements[(i * 2) + 1].Groups[2].ToString());
                for (int f = 0; f < foodTags.Count; f++)
                {
                    Console.Write(foodTags[f].ToString()+",");
                }
                Console.WriteLine();

                String additivesAndAllergenics = ExtractAdditivesAndAllergenicInformation(foodElements[(i * 2) + 1].Groups[2].ToString());
                Console.WriteLine(additivesAndAllergenics);

                String [] basicElementInformationSplit = basicElementInformation.Split(splitOperator.ToCharArray());

                if(basicElementInformationSplit.Length == 2)
                    elements.Add(new FoodElement(basicElementInformationSplit[0],basicElementInformationSplit[1],"none", foodTags, additivesAndAllergenics));
                
            }


            

            return "";
        }

        private static List<FoodTags> ExtractTagInformation(String element)
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
            if (element.Contains("Lamm") || element.Contains("lamm") ||element.Contains("hogget"))
                tags.Add(FoodTags.HOGGET);

            return tags;
        }

        private static String ExtractAdditivesAndAllergenicInformation(String element)
        {
            String result = element;

            String additivesAllergenicDiv = "<div style='clear:both;'>";
            result = result.Remove(0, result.IndexOf(additivesAllergenicDiv));
            result = result.Replace(additivesAllergenicDiv, "").Replace("</div>","").Replace(" </ div>","");
            

            return result;
        }


        private static String ExtractFoodBasicInformation(String element)
        {
            String result = element;
            result = result.Replace("<strong>", "").Replace("</strong>", "");
            result = result.Replace("<br>", "").Replace("</br>", "").Replace("<br />", "");
            result = result.Replace(" class='gruen'", "").Replace(" class='grau'", "");

            if (result.StartsWith("<span><span>"))
                result = result.Remove(0, 12);
            
            if (result.StartsWith("<span>"))
                result = result.Remove(0, 6);

            result = result.Replace("</span></span><span><span></span><span></span>", splitOperator);
            result = result.Replace("</span><span><span></span>", splitOperator);
            result = result.Replace("</span>", splitOperator);
            result = result.Replace("<span>", splitOperator);

            return result;
        }
    }
}
