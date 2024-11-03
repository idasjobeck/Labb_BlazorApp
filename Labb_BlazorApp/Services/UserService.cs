using System.Globalization;
using System.Text.Json;
using CsvHelper;
using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Services;

public class UserService : IUserService
{
    public IEnumerable<User> GetUsers()
    {
        //random user data obtained from https://www.rndgen.com/data-generator; catchphrases from various Reddit threads.

        return new List<User>
        {
            new User(1, "James", "Kohler", "James.Kohler73@hotmail.com",  "+1 (322) 332-3404", "https://unwritten-lycra.info/", "313 Sawayn Street", "Lake Katelyntown", "6032", "Dibbert, Treutel and Conroy", "Feedback is the breakfast of champions."),
            new User(2, "Doreen", "Glover", "Doreen92@yahoo.com", "+1 (560) 818-0710", "https://sophisticated-browser.org", "6374 Velma Trace", "Juliestead", "3803", "Cruickshank - Kub", "There's no traffic on the extra mile."),
            new User(3, "Emilio", "VonRueden", "Emilio.VonRueden66@yahoo.com", "+1 (927) 740-1076", "https://crowded-processor.com/", "185 Ullrich Burgs", "Rosettabury", "8598", "Williamson, Walsh and Muller", "Teamwork makes the dream work."),
            new User(4, "Robert", "Welch", "Robert.Welch80@gmail.com", "+1 (920) 972-7585", "https://thoughtful-honey.org", "1951 Cormier Junctions", "Arjunstad", "0307", "Miller - Ryan", "If you've got time to lean, you've got time to clean!"),
            new User(5, "Sharon", "McLaughlin", "Sharon11@hotmail.com", "+1 (660) 225-8094", "https://overlooked-wheat.net", "9655 Gunnar Center", "Portsmouth", "5145", "Rutherford - Bins", "There's no I in Team."),
            new User(6, "Antonia", "Kutch", "Antonia_Kutch81@hotmail.com", "+1 (338) 974-5798", "https://tempting-tandem.biz", "844 Hazle Skyway", "Laishabury", "6155", "Hills Group", "It is what it is and it isn't what it isn't."),
            new User(7, "Oscar", "Barrows", "Oscar_Barrows67@gmail.com", "+1 (127) 057-0893", "https://several-association.org", "646 Savannah Orchard", "Harveyside", "3153", "Boyle - Beer", "Great should not be the enemy of good."),
            new User(8, "Myra", "Daniel", "Myra41@yahoo.com", "+1 (435) 514-3378", "https://proper-clinic.info", "253 O'Kon Freeway", "Walkerfield", "2080", "Daniel - Predovic", "Better to have and not need, than need and not have."),
            new User(9, "Jeanne", "Mueller", "Jeanne.Mueller22@yahoo.com", "+1 (736) 489-1681", "https://astonishing-parchment.com/", "70933 Glenda Brooks", "South Gate", "6265", "Brakus Group", "If you don't have time to do it right, you definitely don't have time to do it over."),
            new User(10, "Beulah", "Spencer", "Beulah34@yahoo.com", "+1 (461) 852-2303", "https://frail-productivity.org", "23731 Hackett Parks", "Prudenceboro", "4294", "Schmitt, Ferry and Fadel", "Fail to plan, plan to fail.")
        };
    }

    public async Task<IEnumerable<User>> GetUsers(string url)
    {
        var jsonData = await GetDataFromApi(url);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var users = JsonSerializer.Deserialize<List<User>>(jsonData, options);

        return users!;
    }

    public async Task<string> GetDataFromApi(string url)
    {
        using HttpClient client = new HttpClient();
        Task<string> dataFetched = client.GetStringAsync(url);

        var data = await dataFetched;

        return data;
    }

    public IEnumerable<User> GetUsers(string filePath, char delimiter = ',')
    {
        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            //var users = csv.GetRecords<User>().ToList();
            //return users;
            csv.Context.RegisterClassMap<UserMap>();
            var users = csv.GetRecords<User>().ToList();
            return users;
        }
    }
}