using Microsoft.EntityFrameworkCore;
using TiVerse.Core.Entity;
namespace TiVerse.Application.SeedDB
{
    public static class SeedDb
    {
        private static Dictionary<string, string> ukraineCities = new Dictionary<string, string>
                {
                    {"Київ", "Україна"}, {"Харків", "Україна"}, {"Одеса", "Україна"}, {"Дніпро", "Україна"}, {"Донецьк", "Україна"},
                    {"Запоріжжя", "Україна"}, {"Львів", "Україна"}, {"Кривий Ріг", "Україна"}, {"Миколаїв", "Україна"}, {"Маріуполь", "Україна"},
                    {"Вінниця", "Україна"}, {"Херсон", "Україна"}, {"Полтава", "Україна"}, {"Чернігів", "Україна"}, {"Черкаси", "Україна"},
                    {"Житомир", "Україна"}, {"Суми", "Україна"}, {"Рівне", "Україна"}, {"Кам'янець-Подільський", "Україна"}, {"Івано-Франківськ", "Україна"}
                };

        private static Dictionary<string, string> otherCountries = new Dictionary<string, string>
                {
                    {"Лондон", "Велика Британія"}, {"Париж", "Франція"}, {"Берлін", "Німеччина"}, {"Нью-Йорк", "США"}, {"Токіо", "Японія"},
                    {"Рим", "Італія"}, {"Пекін", "Китай"}, {"Сідней", "Австралія"}, {"Мадрид", "Іспанія"}, {"Лос-Анджелес", "США"},
                    {"Даллас", "США" }, {"Дубай", "ОАЕ "}, {"Амстердам", "Нідерланди"}, {"Мюнхен", "Німеччина"}, {"Барселона", "Іспанія"},
                    {"Істанбул", ""}, {"Торонто", "Канада"}, {"Гонконг", " Китай"}, {"Сеул", "Південна Корея"}, {"Сан-Франциско", "США"},
                    {"Флоренція", "Італія"}, {"Прага", "Чехія"}, {"Мельбурн", "Австралія"},
                };


        public static void SeedLocations(ModelBuilder modelBuilder)
        {
            var locations = new List<Location>();
            var random = new Random();

            foreach (var city in ukraineCities)
            {
                var newLocation = new Location(city.Key, city.Value,
                    busStation: random.Next(0, 2) == 0,
                    railwayStation: random.Next(0, 2) == 0,
                    airport: random.Next(0, 2) == 0);

                locations.Add(newLocation);
            }

            foreach (var city in otherCountries)
            {
                var newLocation = new Location(city.Key, city.Value, false, false, true);

                locations.Add(newLocation);
            }

            modelBuilder.Entity<Location>().HasData(locations);
        }

        public static void SeedTrips(ModelBuilder modelBuilder)
        {
            var random = new Random();
            var trips = new List<Trip>();

            foreach (var departureCity in ukraineCities.Keys)
            {
                foreach (var destinationCity in ukraineCities.Keys)
                {
                    if (departureCity != destinationCity)
                    {
                        var numberOfTrips = random.Next(1, 4);

                        for (int i = 0; i < numberOfTrips; i++)
                        {
                            var date = DateTime.Now.AddDays(random.Next(1, 30));
                            var transportOptions = new List<string> { "Plane", "Train", "Bus" };
                            var transport = transportOptions[random.Next(0, transportOptions.Count)];
                            var company = "Any Company: " + random.Next(0, 10).ToString();
                            var places = random.Next(50, 200);
                            var ticketCost = Math.Round((decimal)random.NextDouble() * 500, 2);

                            if (ukraineCities[departureCity] != ukraineCities[destinationCity])
                            {
                                transport = "Plane";
                            }

                            var trip = new Trip(departureCity, destinationCity, date, transport, company, places, ticketCost);
                            trips.Add(trip);
                        }
                    }
                }
            }

            foreach (var departureCity in ukraineCities.Keys)
            {
                foreach (var destinationCity in otherCountries.Keys)
                {
                    var numberOfTrips = random.Next(1, 4); 

                    for (int i = 0; i < numberOfTrips; i++)
                    {
                        var date = DateTime.Now.AddDays(random.Next(1, 30));
                        var transport = "Plane"; 
                        var company = "Any Company";
                        var places = random.Next(50, 200);
                        var ticketCost = Math.Round((decimal)random.NextDouble() * 500, 2);

                        var trip = new Trip(departureCity, destinationCity, date, transport, company, places, ticketCost);
                        trips.Add(trip);
                    }
                }
            }

            modelBuilder.Entity<Trip>().HasData(trips);
        }
    }
}
