using System.Globalization;
using TiVerse.Application.DTO;
using TiVerse.Core.Entity;

namespace TiVerse.WebUI.CityLocalizer
{
    public interface ICityLocalization
    {
        List<string> GetLocalizedList(List<string> originalList);
        List<RouteDTO> GetLocalizedList(List<RouteDTO> routes);
        List<TopRoutesDTO> GetLocalizedList(List<TopRoutesDTO> origianList);
        string GetUkrainianCityName(string englishCityName);
    }
}
