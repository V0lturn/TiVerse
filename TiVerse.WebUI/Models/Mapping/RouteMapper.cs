using AutoMapper;
using TiVerse.Application.DTO;
using TiVerse.Core.Entity;

namespace TiVerse.Application.Mapping
{
    public class RouteMapper : Profile
    {
        public RouteMapper() 
        {
            CreateMap<Trip, RouteDTO>();
        }
    }
}
