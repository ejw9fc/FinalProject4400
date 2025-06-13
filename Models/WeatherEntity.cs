using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject4400.Models
{
    public abstract class WeatherEntity
    {
        /// <summary>
        /// UNIX timestamp of this data point
        /// </summary>
        public long Dt { get; set; }

        /// <summary>
        /// Array of weather conditions (we’ll take the first for icon/description)
        /// </summary>
        public WeatherInfo[] Weather { get; set; }

        /// <summary>
        /// Convenience: get the first icon code
        /// </summary>
        public string IconCode => Weather?.Length > 0 ? Weather[0].Icon : string.Empty;

        /// <summary>
        /// Convenience: get the first description
        /// </summary>
        public string Description => Weather?.Length > 0 ? Weather[0].Main : string.Empty;

        public class WeatherInfo
        {
            public string Main { get; set; }
            public string Icon { get; set; }
        }
    }
}
