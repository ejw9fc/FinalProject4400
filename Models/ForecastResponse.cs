using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace FinalProject4400.Models
{
    internal class ForecastResponse
    {
        // This matches the "list" property in the JSON
        public ForecastDay[] List { get; set; }
    }
    public class ForecastDay : WeatherEntity
    {
        public TempInfo Temp { get; set; }

        /// <summary>
        /// The loaded icon image for binding
        /// </summary>
        public BitmapImage IconImage { get; set; }

        public class TempInfo
        {
            public double Day { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
        }
    }
}
