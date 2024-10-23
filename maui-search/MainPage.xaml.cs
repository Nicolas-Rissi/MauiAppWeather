using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Service;
using System.Diagnostics;

namespace maui_search
{
    public partial class MainPage : ContentPage
    {
        CancellationTokenSource _cancelTokenSource;
        bool _isCheckingLocation;
        string? cidade;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSearchWeatherClicked(object sender, EventArgs e)
        {
            string city = CityEntry.Text;
            DateTime date = DateEntry.Date;

            if (string.IsNullOrEmpty(city))
            {
                await DisplayAlert("Erro", "Por favor, insira o nome da cidade", "OK");
                return;
            }

            // Fetch weather data for the entered city
            WeatherData weatherData = await FetchWeatherData(city, date);
            if (weatherData != null)
            {
                ResultLabel.Text = $"Previsão em {city} para {date.ToShortDateString()}:\n" +
                                   $"Temperatura: {weatherData.Temperature}°C\n" +
                                   $"Condição: {weatherData.Condition}\n" +
                                   $"Humidade: {weatherData.Humidity}%";
            }
            else
            {
                ResultLabel.Text = "Não foi possível obter a previsão do tempo.";
            }
        }

        private async Task<WeatherData> FetchWeatherData(string city, DateTime date)
        {
            try
            {
                Tempo? previsao = await DataService.GetPrevisaoDoTempo(city);
                if (previsao != null)
                {
                    return new WeatherData
                    {
                        City = city,
                        Date = date,
                        Temperature = previsao.Temperature,
                        Condition = previsao.Weather,
                        Humidity = previsao.Humidity,
                        Latitude = previsao.Latitude,
                        Longitude = previsao.Longitude
                    };
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro 1", ex.Message, "OK");
            }

            return new WeatherData
            {
                City = city,
                Date = date,
                Temperature = "25.0",
                Condition = "Ensolarado",
                Humidity = "50"
            };
        }

        private async Task<string> GetGeocodeReverseData(double latitude, double longitude)
        {
            IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);
            Placemark? placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                cidade = placemark.Locality;
                return cidade;
            }
            return "Nada";
        }

        private async void OnFetchLocationClicked(object sender, EventArgs e)
        {
            try
            {
                _cancelTokenSource = new CancellationTokenSource();

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                Location? location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

                if (location != null)
                {
                    double latitude = location.Latitude;
                    double longitude = location.Longitude;

                    string cidadeObtida = await GetGeocodeReverseData(latitude, longitude);

                    ResultLabel.Text = $"Cidade obtida: {cidadeObtida}\nLatitude: {latitude}\nLongitude: {longitude}";
                    CityEntry.Text = cidadeObtida;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }

    public class WeatherData
    {
        public string? City { get; set; }
        public DateTime Date { get; set; }
        public string? Temperature { get; set; }
        public string? Condition { get; set; }
        public string? Humidity { get; set; }

        public string? Latitude { get; set; }
        public string? Longitude { get; set; }

    }


}
