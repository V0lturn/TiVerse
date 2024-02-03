using Microsoft.EntityFrameworkCore;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRepositoryInterface;
using TiVerse.Core.Entity;
using TiVerse.Infrastructure.AppDbContext;
using TiVerse.WebUI.ViewModels;

namespace TiVerse.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly ITiVerseDbContext _dbContext;
        private readonly ITiVerseIRepository<Account> _accountRepository;
        private readonly ITiVerseIRepository<Trip> _tripRepository;
        private readonly ITiVerseIRepository<UserRouteHistory> _userRouteHistoryRepository;

        //private readonly string UserId = "200958d2-0a14-40e1-96a1-1e3391ea8594";

        public AccountService(ITiVerseDbContext dbContext, ITiVerseIRepository<Account> accountRepository,
            ITiVerseIRepository<Trip> tripRepository, ITiVerseIRepository<UserRouteHistory> userRouteHistoryRepository)
        {
            _dbContext = dbContext;
            _accountRepository = accountRepository;
            _tripRepository = tripRepository;
            _userRouteHistoryRepository = userRouteHistoryRepository;
        }

        public async Task<(bool success, string message)> UpdateAccountInfo(PersonalInfoViewModel model, string userId)
        {
            if (model.BirthDate.HasValue)
            {
                DateTime currentDate = DateTime.Now;
                int age = currentDate.Year - model.BirthDate.Value.Year;

                if (age < 14 || model.BirthDate > currentDate || model.BirthDate == null)
                {
                    return (false, "Дата народження вказана невірно. Вам має бути 14 років.");
                }
            }

            bool userExists = await _accountRepository.GetAll<Account>().AnyAsync(u => u.UserId == userId);

            if (!userExists)
            {
                Account newAccount = new Account(
                    userId,
                    model.FirstName,
                    model.LastName,
                    model.BirthDate ?? DateTime.MinValue,
                    model.City,
                    0);

                _accountRepository.Add(newAccount);
                _accountRepository.SaveChanges();

                return (true, "Інформація додана успішно");
            }
            else
            {
                var user = await _accountRepository.GetAll<Account>().FirstOrDefaultAsync(u => u.UserId == userId);

                if (!string.IsNullOrEmpty(model.FirstName))
                {
                    user.FirstName = model.FirstName;
                }

                if (!string.IsNullOrEmpty(model.LastName))
                {
                    user.LastName = model.LastName;
                }

                if (!string.IsNullOrEmpty(model.City))
                {
                    user.City = model.City;
                }

                if (model.BirthDate.HasValue)
                {
                    user.BirthDate = model.BirthDate.Value;
                }

                _accountRepository.SaveChanges();
                return (true, "Інформація оновлена успішно");
            }
        }

        public async Task<(bool success, string message)> BuyTicket(Guid tripId, string userId)
        {
            var trip = await _tripRepository.GetById<Trip>(tripId);

            if (trip == null)
            {
                return (false, "Помилка. Білет для даної подорожі не знайдено.");
            }
            else
            {
                var user = await _accountRepository.GetAll<Account>().FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return (false, "Помилка з акаунтом користувача. Спробуйте пізніше");
                }
                else
                {
                    if (user.CashBalance < trip.TicketCost)
                    {
                        return (false, "Не вистачає коштів для покупки білета.");
                    }
                    else
                    {
                        trip.Places--;
                        user.CashBalance -= trip.TicketCost;

                        UserRouteHistory userRouteHistory = new UserRouteHistory(user.UserId, trip.TripID, trip.Date);
                        _userRouteHistoryRepository.Add(userRouteHistory);
                        _userRouteHistoryRepository.SaveChanges();
                        return (true, "Білет успішно куплено! Можна переглянути у \"Заплановані поїздки\"");
                    }
                }
            }
        }

        public async Task<List<Trip>> GetUserTripHistory(string userId)
        {
            DateTime today = DateTime.Now.Date;

            var trips = await _dbContext.UserRouteHistories
                .Where(urh => urh.UserId == userId && urh.Date < today)
                .Include(urh => urh.Trip)
                .Select(urh => urh.Trip)
                .OrderBy(urh => urh.Date)
                .ToListAsync();

            return trips;
        }

        public async Task<List<Trip>> GetUserPlannedTrips(string userId)
        {
            DateTime today = DateTime.Now.Date;

            var trips = await _dbContext.UserRouteHistories
                .Where(urh => urh.UserId == userId && urh.Date >= today)
                .Include(urh => urh.Trip)
                .Select(urh => urh.Trip)
                .OrderBy(urh => urh.Date)
                .ToListAsync();

            return trips;
        }

        public async Task<(bool success, string message)> UpdateUserBalance(decimal money, string userId)
        {
            if (money <= 0)
            {
                return (false, "Помилка з поповненням на задану суму. Перевірте та спробуйте ще раз.");
            }
            else
            {
                var user = await _accountRepository.GetAll<Account>().FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return (false, "Помилка з акаунтом користувача. Спробуйте пізніше");
                }
                else
                {
                    user.CashBalance += money;
                    _accountRepository.SaveChanges();

                    return (true, "Баланс успішно поповнено! Насолоджуйтесь подорожами! ");
                }
            }
        }
    }
}
