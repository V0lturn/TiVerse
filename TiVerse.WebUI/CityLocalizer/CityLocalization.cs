using Microsoft.Extensions.Localization;
using TiVerse.Application.DTO;

namespace TiVerse.WebUI.CityLocalizer
{
    public class CityLocalization : ICityLocalization
    {
        private readonly IStringLocalizer<CityLocalization> _localizer;

        public CityLocalization(IStringLocalizer<CityLocalization> localizer)
        {
            _localizer = localizer;
        }

        public List<string> GetLocalizedList(List<string> originalList)
        {
            List<string> localizedList = new List<string>();

            foreach (var item in originalList)
            {
                string localizedItem = _localizer[item]?.Value ?? item;
                localizedList.Add(localizedItem);
            }

            return localizedList;
        }

        public List<RouteDTO> GetLocalizedList(List<RouteDTO> originalList)
        {
            foreach(var item in originalList)
            {
                item.DeparturePoint = _localizer[item.DeparturePoint]?.Value ?? item.DeparturePoint;
                item.DestinationPoint = _localizer[item.DestinationPoint]?.Value ?? item.DestinationPoint;
            }

            return originalList;
        }

        public List<TopRoutesDTO> GetLocalizedList(List<TopRoutesDTO> originalList)
        {
            foreach (var item in originalList)
            {
                item.DeparturePoint = _localizer[item.DeparturePoint]?.Value ?? item.DeparturePoint;
                item.DestinationPoint = _localizer[item.DestinationPoint]?.Value ?? item.DestinationPoint;
            }

            return originalList;
        }

        public string GetUkrainianCityName(string cityName)
        {
            if (_localizer.GetAllStrings().Any(entry => entry.Value == cityName))
            {

                foreach (var entry in _localizer.GetAllStrings())
                {
                    if (entry.Value == cityName)
                    {
                        return entry.Name;
                    }
                }
            }

            return cityName;
        }
    }
}
