using AutoMapper;
using LiqPay.SDK;
using LiqPay.SDK.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TiVerse.Application.DTO;
using TiVerse.Application.Interfaces.IAccountServiceInterface;
using TiVerse.Application.Interfaces.IRepositoryInterface;
using TiVerse.Application.Interfaces.IRouteServiceInterface;
using TiVerse.Core.Entity;
using TiVerse.WebUI.CityLocalizer;
using TiVerse.WebUI.Models;
using TiVerse.WebUI.ViewModels;

namespace TiVerse.WebUI.Controllers
{
    [Authorize(Policy = "RequireAuth")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRouteService _routeService;
        private readonly ITiVerseIRepository<Account> _accountRepository;
        private readonly IMapper _mapper;
        private readonly ICityLocalization _cityLocalization;
        private readonly IConfiguration _config;

        public AccountController(IAccountService accountService, ITiVerseIRepository<Account> accountRepository,
            IRouteService routeService, IMapper mapper, ICityLocalization cityLocalization,
            IConfiguration config)
        {
            _accountService = accountService;
            _accountRepository = accountRepository;
            _routeService = routeService;
            _mapper = mapper;
            _cityLocalization = cityLocalization;
            _config = config;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;
            ViewData["UserBalance"] = await _routeService.GetUserBalance(userId);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadPartialViewAsync(string viewName)
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;

            if (viewName == "_PersonalInfo")
            {
                var existingUser = _accountRepository.GetAll<Account>().FirstOrDefault(u => u.UserId == userId);
                ViewData["ExistingUserData"] = existingUser;
            }

            if (viewName == "_PlannedTrip")
            {
                var routes = await _accountService.GetUserPlannedTrips(userId);
                var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);

                var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

                ViewData["PlannedTrip"] = localizedRoutes;
            }

            if (viewName == "_TripHistory")
            {
                var routes = await _accountService.GetUserTripHistory(userId);
                var routesToDTO = _mapper.Map<List<RouteDTO>>(routes);

                var localizedRoutes = _cityLocalization.GetLocalizedList(routesToDTO);

                ViewData["TripHistory"] = localizedRoutes;
            }

            if (viewName == "_AccountBalance")
            {
                ViewData["AccountBalance"] = await _routeService.GetUserBalance(userId);
            }

            return viewName switch
            {
                "_PersonalInfo" => PartialView("_PersonalInfo"),
                "_PlannedTrip" => PartialView("_PlannedTrip"),
                "_TripHistory" => PartialView("_TripHistory"),
                "_AccountBalance" => PartialView("_AccountBalance"),
                _ => new EmptyResult(),
            };
        }


        [HttpPost]
        public async Task<IActionResult> UpdatePersonalInfo([FromForm] PersonalInfoViewModel model)
        {
            string userId = HttpContext.User.FindFirst("sub")?.Value;

            var result = await _accountService.UpdateAccountInfo(model, userId);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> BuyTicket(Guid tripId)
        {
            TempData["SuccessMessage"] = string.Empty;
            TempData["ErrorMessage"] = string.Empty;

            string userId = HttpContext.User.FindFirst("sub")?.Value;

            var result = await _accountService.BuyTicket(tripId, userId);

            if (result.success)
            {
                TempData["SuccessMessage"] = result.message;
                return Json(new { success = true, message = result.message });
            }
            else
            {
                TempData["ErrorMessage"] = result.message;
                return Json(new { success = false, message = result.message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> TopUpBalance(decimal topUpAmount)
        //{
        //    string userId = HttpContext.User.FindFirst("sub")?.Value;

        //    var result = await _accountService.UpdateUserBalance(topUpAmount, userId);

        //    if (result.success)
        //    {
        //        TempData["SuccessMessage"] = result.message;
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = result.message;

        //    }
        //    return RedirectToAction("Index");
        //}


        [HttpPost]
        public ActionResult Pay([FromServices] IConfiguration config, decimal topUpAmount)
        {
            string publicKey = _config["LiqpayApi:PublicKey"];
            string privateKey = _config["LiqpayApi:PrivateKey"];

            LiqPayClient liqpay = new LiqPayClient(publicKey, privateKey);

            //Payment parameters
            var paymentParams = new LiqPayRequest
            {
                Version = 3, //Версия Api
                Action = LiqPay.SDK.Dto.Enums.LiqPayRequestAction.Pay, //Transaction type
                Amount = (double)topUpAmount, //Payment amount
                Currency = "UAH", //Payment currency
                Description = "Поповнення балансу", //Payment Description 
                OrderId = Guid.NewGuid().ToString(), //Unique order number on your website
                ResultUrl = "https://localhost:5002/account/success" //Link to successful payment page
            };

            var formCode = liqpay.CNBForm(paymentParams);

            PaymentViewModel pvm = new PaymentViewModel
            {
                PaymentParams = paymentParams,
                FormCode = formCode,
            };

            return View(pvm);
        }

        private string GetLiqPaySignature(string data)
        {
            string privateKey = _config["LiqpayApi:PrivateKey"];
            return Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(privateKey + data + privateKey)));
        }

        public IActionResult Success([FromForm] IFormCollection form)
        {
            //var request_dictionary = form.Keys.ToDictionary(key => key, key => form[key].ToString());

            //// --- Розшифровую параметр data відповіді LiqPay та перетворюю в Dictionary<string, string> для зручності:
            //byte[] request_data = Convert.FromBase64String(request_dictionary["data"]);
            //string decodedString = Encoding.UTF8.GetString(request_data);
            //var request_data_dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedString);

            //// --- Отримую сигнатуру для перевірки
            //var mySignature = GetLiqPaySignature(request_dictionary["data"]);

            //// --- Якщо сигнатура сервера не співпадає з сигнатурою відповіді LiqPay - щось пішло не так
            //if (request_data_dictionary["status"] == "sandbox" || request_data_dictionary["status"] == "success")
            //{
            //    // Тут можна оновити статус замовлення та зробити всі необхідні речі. Id замовлення можна взяти тут: request_data_dictionary[order_id]
            //    // ...

            //    return View();
            //}

            return View();
        }

    }
}
