using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoscowWeather.Models;
using MoscowWeather.ViewModel;
using System.Globalization;

namespace MoscowWeather.Controllers
{
    public class ViewController : Controller
    {
        private WeatherContext db;

        public ViewController(WeatherContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var weather = db.Weather.Include(t => t.WeatherCondition).Include(t => t.WindDirection).OrderBy(t => t.DateTime).ToList();
            var years = db.Weather.Select(t => t.DateTime.Year).Distinct().ToList();
            var months = db.Weather.Select(t => t.DateTime.Month).Distinct().ToList();
            months.Sort();
            List<SelectListItem> monthsList = new();
            foreach(var month in months)
            {
                monthsList.Add(new SelectListItem
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Value = month.ToString()
                });
            }
            var criteria = new FilterCriteria
            {
                Month = null,
                Year = null
            };
            var filterModel = new Filter
            {
                Criteria = criteria,
                Months = monthsList,
                Years = years,
                Weather = weather
            };
            return View("DoFilter", filterModel);
        }
        [HttpPost]
        public IActionResult DoFilter(string Year, string Month)
        {
            var yearInt = TryParseNullableInt(Year);
            var monthInt = TryParseNullableInt(Month);
            var criteria = new FilterCriteria { Year = yearInt, Month = monthInt};
            var weather = db.Weather.Include(t => t.WeatherCondition).Include(t => t.WindDirection).ToList();
            var years = db.Weather.Select(t => t.DateTime.Year).Distinct().ToList();
            var months = db.Weather.Select(t => t.DateTime.Month).Distinct().ToList();
            months.Sort();
            List<SelectListItem> monthsList = new();
            foreach (var month in months)
            {
                monthsList.Add(new SelectListItem
                {
                    Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    Value = month.ToString()
                });
            }
            if (criteria.Year.HasValue)
            {
                weather = weather.Where(t => t.DateTime.Year == criteria.Year.Value).ToList();
            }
            if (criteria.Month.HasValue)
            {
                weather = weather.Where(t => t.DateTime.Month == criteria.Month.Value).ToList();
            }
            var filterModel = new Filter
            {
                Criteria = criteria,
                Months = monthsList,
                Years = years,
                Weather = weather
            };
            return View(filterModel);
        }
        int? TryParseNullableInt(string value)
        {
            int outValue;
            return int.TryParse(value, out outValue) ? (int?)outValue : null;
        }
    }
}
