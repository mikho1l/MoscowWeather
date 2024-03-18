using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static NPOI.HSSF.Util.HSSFColor;

namespace MoscowWeather.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<Weather> Weather { get; set; }
        public DbSet<WeatherCondition> WeatherCondition { get; set; }
        public DbSet<WindDirection> WindDirection { get; set; }
        public WeatherContext(DbContextOptions<WeatherContext> options)
             : base(options)
        {
            //Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
