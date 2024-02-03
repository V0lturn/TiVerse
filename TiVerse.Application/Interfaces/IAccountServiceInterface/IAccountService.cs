using TiVerse.Core.Entity;
using TiVerse.WebUI.ViewModels;

namespace TiVerse.Application.Interfaces.IAccountServiceInterface
{
    public interface IAccountService
    {
        Task<(bool success, string message)> UpdateAccountInfo(PersonalInfoViewModel model, string userId);
        Task<(bool success, string message)> BuyTicket(Guid tripId, string userId);
        Task<List<Trip>> GetUserTripHistory(string userId);
        Task<List<Trip>> GetUserPlannedTrips(string userId);
        Task<(bool success, string message)> UpdateUserBalance(decimal money, string userId);
    }
}
