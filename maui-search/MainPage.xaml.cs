namespace maui_search
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnSearchWeatherClicked(object sender, EventArgs e)
        {
            string city = CityEntry.Text;
            DateTime date = DateEntry.Date;

            if (string.IsNullOrEmpty(city))
            {
                DisplayAlert("Erro", "Por favor, insira o nome da cidade", "OK");
                return;
            }

            WeatherData weatherData = SimulateWeatherData(city, date);
            if (weatherData != null)
            {
                ResultLabel.Text = $"Previsão em {city} para {date.ToShortDateString()}:\n" +
                                   $"Temperatura: {weatherData.Temperature}°C\n" +
                                   $"Condição: {weatherData.Condition}";
            }
            else
            {
                ResultLabel.Text = "Não foi possível obter a previsão do tempo.";
            }
        }

        private WeatherData SimulateWeatherData(string city, DateTime date)
        {
            return new WeatherData
            {
                City = city,
                Date = date,
                Temperature = 25.0,
                Condition = "Ensolarado"
            };
        }
    }

    public class WeatherData
    {
        public string City { get; set; }
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; }
    }
}
