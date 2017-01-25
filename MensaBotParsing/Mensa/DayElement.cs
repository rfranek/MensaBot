using System;
using System.Linq;

namespace MensaBotParsing.Mensa
{
    using System.Collections.Generic;
    using System.Globalization;

    class DayElement
    {
        #region properties

        public DateTime Date { get; private set; }

        public List<FoodElement> FoodElements { get; private set; }

        #endregion

        #region constructors and destructors

        public DayElement(String date, List<FoodElement> foodElements)
        {
            Date = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            FoodElements = foodElements;
        }

        #endregion
    }
}