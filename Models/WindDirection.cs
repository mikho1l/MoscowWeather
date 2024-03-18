using Microsoft.AspNetCore.Mvc;

namespace MoscowWeather.Models
{
    public class WindDirection
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Weather> Weathers { get; set; }
    }
}
