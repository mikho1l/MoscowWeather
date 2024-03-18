using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoscowWeather.Models
{
    [Table("Weather")]
    public class Weather
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Display(Name = "Дата и время")]
        public DateTime DateTime { get; set; }
        [Display(Name = "Температура воздуха")]
        public float? Temperature { get; set; }
        [Display(Name = "Относительная влажность воздуха, %")]
        public byte? RelativeHumidity { get; set; }
        [Display(Name = "Точка росы")]
        public float? DewPoint { get; set; }
        [Display(Name = "Атмосферное давление, мм. рт. ст.")]
        public short? AtmospherePressure { get; set; }
        [Display(Name = "Направление ветра")]
        public List<WindDirection> WindDirection { get; set; }
        [Display(Name = "Скорость ветра, м/с")]
        public byte? WindSpeed { get; set; }
        [Display(Name = "Облачность, %")]
        public byte? Cloudiness { get; set; }
        [Display(Name = "Нижняя граница облачности, м")]
        public short? CloudBase { get; set; }
        [Display(Name = "Горизонтальная видимость, км")]
        public byte? HorizontalVisibility { get; set; }
        [Display(Name = "Природные явления")]
        public WeatherCondition? WeatherCondition { get; set; }
    }
   
}
