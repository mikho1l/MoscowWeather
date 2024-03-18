using Microsoft.AspNetCore.Mvc.Rendering;
using MoscowWeather.Models;

namespace MoscowWeather.ViewModel
{
    public class Filter
    {
        public FilterCriteria Criteria { get; set; }
        public List<int> Years { get; set; }
        public List<SelectListItem> Months { get; set; }
        public IEnumerable<Weather> Weather {  get; set; } 
    }
}
