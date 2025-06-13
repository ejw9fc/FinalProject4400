using FinalProject4400.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject4400.Services
{
    /// <summary>
    /// Service for fetching weather forecast data from OpenWeatherMap API.
    /// </summary>
    internal class WeatherService
    {
        private const string ApiKey = "2d35a15a09b63d5bda1d764ee47532ef"; //API KEY
        private readonly HttpClient _http = new(); // HTTP client for API calls

        /// <summary>
        /// // Asynchronously fetches 'cnt' days of forecast (including today) for a given location string
        /// </summary>
        public async Task<ForecastResponse> GetDailyAsync(string location, int cnt = 7)
        {
            // Validate input format
            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("You must supply city,state,country");

            // Construct the API URL with escaping to handle spaces/special characters
            var url =
              $"https://api.openweathermap.org/data/2.5/forecast/daily" +
              $"?q={Uri.EscapeDataString(location)}" +
              $"&cnt={cnt}" +
              $"&units=imperial" +
              $"&appid={ApiKey}";

            try
            {
                // Send GET request and get raw JSON string
                var json = await _http.GetStringAsync(url);
                
                // Deserialize JSON into our ForecastResponse model
                return JsonConvert.DeserializeObject<ForecastResponse>(json)
                       ?? throw new Exception("Failed to parse forecast data.");
            }
            catch (HttpRequestException ex)
            {
                // Thrown on network errors or bad HTTP status codes
                throw new Exception("Incorrect location format.", ex);
            }
            catch (JsonException ex)
            {
                // Thrown if JSON is malformed or doesn't match model
                throw new Exception("Received unexpected data format.", ex);
            }
        }
    }
}
