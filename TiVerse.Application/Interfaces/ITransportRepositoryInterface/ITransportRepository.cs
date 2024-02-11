using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TiVerse.Application.ViewModels;
using TiVerse.Core.Entity;

namespace TiVerse.Application.Interfaces.ITransportRepositoryInterface
{
    public interface ITransportRepository
    {
        IQueryable<Trip> GetAllRoutes();
        IQueryable<Trip> GetRoutesByTransport(string transport);
        bool CreateRoute(RouteViewModel viewModel);
        Task<bool> UpdateRoute(Guid id, RouteViewModel viewModel);
        Task<bool> DeleteRoute(Guid id);

    }
}
