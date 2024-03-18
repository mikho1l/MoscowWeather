using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using MoscowWeather.Models;

namespace MoscowWeather.Controllers
{
    public class LoadController : Controller
    {
        private WeatherContext db;
        public LoadController(WeatherContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        
        public async Task<IActionResult> AddFile(IFormFileCollection uploads)
        {
            try
            {
                int successcount = 0;
                int failcount = 0;
                foreach (var uploadedFile in uploads)
                {
                    IWorkbook workbook;
                    using (var fileStream = uploadedFile.OpenReadStream())
                    {
                        workbook = new XSSFWorkbook(fileStream);
                    }
                    for (int i = 0; i < workbook.NumberOfSheets; i++)
                    {
                        var sheet = workbook.GetSheetAt(i);
                        Console.WriteLine($"Лист {i}");
                        for (int j = 0; j < sheet.LastRowNum; j++)
                        {
                            var row = sheet.GetRow(j);
                            try
                            {
                                var weather = ParseExcelRow(row);
                                var weatherToOverWrite = db.Weather.Where(t => t.DateTime == weather.DateTime).FirstOrDefault();
                                if (weatherToOverWrite != null)
                                {
                                    db.Weather.Remove(weatherToOverWrite);
                                }
                                db.Weather.Add(weather);
                                db.SaveChanges();
                                successcount++;
                            }
                            catch (Exception ex)
                            {
                                failcount++;
                                //var logString = $"Ошибка чтения данных: файл {uploadedFile.Name}, лист{i}, строка {j}. {ex}";
                                //Console.WriteLine(logString);
                            }

                        }
                    }
                }
                if (successcount == 0 && failcount == 0)
                {
                    ViewData["Success"] = "Здесь нечего загружать :(";
                    ViewData["Error"] = "true";
                }
                else if (successcount == 0 && failcount != 0)
                {
                    ViewData["Success"] = "При загрузке данных возникла ошибка";
                    ViewData["Error"] = "true";
                }
                else
                {
                    ViewData["Success"] = "Архивы успешно загружены";
                    ViewData["Error"] = "false";
                }
            }
            catch(Exception ex)
            {
                ViewData["Success"] = "При загрузке данных возникла ошибка";
                ViewData["Error"] = "true";
            }
            
            return View("Index");
        }
        Weather ParseExcelRow(IRow row)
        {
            var cells = row.Cells;
            if (cells.Count < 3)
            {
                throw new ArgumentException("Количество столбцов в строке меньше 12.");
            }
            DateTime date;
            if(!DateTime.TryParseExact($"{cells[0]} {cells[1]}", "dd.MM.yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out date))
            {
                throw new ArgumentException("Не удается распознать параметр \"Дата и время\".");
            }
            float? temperature = TryParseNullableFloat(cells[2].ToString());
            byte? relativeHumidity = null;
            if (cells.Count > 3)
                relativeHumidity = TryParseNullableByte(cells[3].ToString());
            float? dewPoint = null;
            if (cells.Count > 4)
                dewPoint = TryParseNullableFloat(cells[4].ToString());
            short? atmospherePressure = null;
            if(cells.Count > 5)
                atmospherePressure = TryParseNullableShort(cells[5].ToString());
            string[] windDirections = Array.Empty<string>();
            if (cells.Count > 6)
                windDirections = cells[6].ToString().Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            List<WindDirection> windDirectionList = new();
            foreach(string windDirection in windDirections)
            {
                if (!string.IsNullOrWhiteSpace(windDirection))
                {
                    var directions = db.WindDirection.Where(t => t.Name == windDirection);
                    if (directions.Count() == 0)
                    {
                        var direction = new WindDirection { Name = windDirection };
                        db.WindDirection.Add(direction);
                        windDirectionList.Add(direction);
                        db.SaveChanges();
                    }
                    else
                        windDirectionList.Add(directions.First());
                }
            }
            byte? windSpeed = null;
            if (cells.Count > 7)
                windSpeed = TryParseNullableByte(cells[7].ToString());
            byte? cloudiness = null;
            if (cells.Count > 8)
                cloudiness = TryParseNullableByte(cells[8].ToString());
            short? cloudBase = null;
            if (cells.Count > 9)
                cloudBase = TryParseNullableShort(cells[9].ToString());
            byte? horizontalVisibility = null;
            if (cells.Count > 10)
                horizontalVisibility = TryParseNullableByte(cells[10].ToString());
            string weatherConditionString = null;
            if (cells.Count > 11)
                weatherConditionString = cells[11].ToString();
            WeatherCondition weatherCondition = null;
            if (!string.IsNullOrWhiteSpace(weatherConditionString))
            {
                var weatherConditionDb = db.WeatherCondition.Where(t => t.Name == weatherConditionString);
                if (weatherConditionDb.Count() == 0)
                {
                    weatherCondition = new() { Name = weatherConditionString };
                    db.WeatherCondition.Add(weatherCondition);
                    db.SaveChanges();
                }
                else
                {
                    weatherCondition = weatherConditionDb.First();
                }
            }
            var Weather = new Weather { AtmospherePressure = atmospherePressure, CloudBase = cloudBase,
            Cloudiness = cloudiness, DateTime = date, DewPoint = dewPoint, HorizontalVisibility = horizontalVisibility,
            RelativeHumidity = relativeHumidity, Temperature = temperature, WeatherCondition = weatherCondition,
            WindDirection = windDirectionList, WindSpeed = windSpeed};
            return Weather;
        }
        float? TryParseNullableFloat(string value)
        {
            float outValue;
            return float.TryParse(value, out outValue) ? (float?)outValue : null;
        }
        short? TryParseNullableShort(string value)
        {
            short outValue;
            return short.TryParse(value, out outValue) ? (short?)outValue : null;
        }
        byte? TryParseNullableByte(string value)
        {
            byte outValue;
            return byte.TryParse(value, out outValue) ? (byte?)outValue : null;
        }
    }
}
