using FinalProject4400.Services;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalProject4400
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Service for fetching weather data
        private readonly WeatherService _weatherSvc = new();
        // In-memory list storing up to 3 favorite location strings
        private readonly List<string> _favorites = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Triggered when the user clicks 'Search'. Fetches weather data, updates current conditions, and loads forecast icons asynchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous UI state
            CurrentTempText.Text = "";
            CurrentCondText.Text = "";
            CurrentIcon.Source = null;
            ForecastItemsControl.ItemsSource = null;

            var location = LocationTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(location))
            {
                // Prompt if input is missing
                CurrentCondText.Text = "Please enter City,ST,Country";
                return;
            }

            try
            {
                // Fetch 7-day forecast data
                var resp = await _weatherSvc.GetDailyAsync(LocationTextBox.Text.Trim(), cnt: 7);
                var today = resp.List[0]; // Today's forecast

                // Update "Now" display: temperature and description
                CurrentTempText.Text = $"{today.Temp.Day:F0}°F";
                CurrentCondText.Text = today.Description;

                // Load “Now” icon off the UI thread
                var nowCode = today.IconCode;
                await Task.Run(() =>
                {
                    var nowBmp = LoadLocalBitmap(nowCode);
                    // Go back to UI thread
                    Dispatcher.Invoke(() => CurrentIcon.Source = nowBmp);
                });

                // Load all forecast-day icons in parallel
                var iconTasks = resp.List.Select(day =>
                    Task.Run(() =>
                    {
                        var bmp = LoadLocalBitmap(day.IconCode);
                        // Assign back on UI thread
                        Dispatcher.Invoke(() => day.IconImage = bmp);
                    })
                ).ToArray();

                // Wait for all icon-loading tasks to complete
                await Task.WhenAll(iconTasks);

                // Bind the populated list to the ItemsControl for display
                ForecastItemsControl.ItemsSource = resp.List;
            }
            catch (Exception ex)
            {
                // Display any errors inline in the UI
                CurrentCondText.Text = $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Helper to create BitmapImage from local Assets/Icons/<code>@2x.png
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private BitmapImage LoadLocalBitmap(string code)
        {
            // Hardcode String path
            var path = $"Assets/Icons/{code}_w@2x.png";
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(path, UriKind.Relative);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            bmp.Freeze(); // safe to share across threads
            return bmp;
        }
        /// <summary>
        /// Adds the current location to the favorites list (up to 3 entries). Shows errors if validation fails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFavorite_Click(object sender, RoutedEventArgs e)
        {
            FavErrorText.Text = "";

            var loc = LocationTextBox.Text.Trim();
            if (string.IsNullOrEmpty(loc))
            {
                FavErrorText.Text = "Enter location first.";
                return;
            }
            if (_favorites.Contains(loc.ToLower()))
            {
                FavErrorText.Text = "Already in favorites.";
                return;
            }
            if (_favorites.Count >= 3)
            {
                FavErrorText.Text = "Max 3 favorites. Remove one first.";
                return;
            }

            _favorites.Add(loc.ToLower());
            RefreshFavoritesList(); // Refresh display
        }
        /// <summary>
        /// Removes the selected favorite entry from the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (FavoritesList.SelectedItem is string loc)
            {
                _favorites.Remove(loc);
                RefreshFavoritesList();
            }
        }
        /// <summary>
        /// Rebinds the favorites ListBox with an alphabetically sorted list with LINQ Technique.
        /// </summary>
        private void RefreshFavoritesList()
        {
            // Sort alphabetically before binding
            var sorted = _favorites
                           .OrderBy(city => city, StringComparer.OrdinalIgnoreCase)
                           .ToList();

            FavoritesList.ItemsSource = sorted;
            FavErrorText.Text = "";
        }
        /// <summary>
        /// Handles selection of a favorite city: updates the input box and triggers a new search.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FavoritesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FavoritesList.SelectedItem is string loc)
            {
                // populate textbox and trigger a search again
                LocationTextBox.Text = loc;
                SearchButton_Click(this, null);
            }
        }
    }
}