using CsvHelper;
using CsvHelper.Configuration;
using Labb_BlazorApp.Models;
using System.Globalization;

namespace Labb_BlazorApp.Services;

public class UserServiceCsv : IUserService
{
    public string FilePath { get; set; }
    public string Delimiter { get; set; }

    public UserServiceCsv()
    {
        FilePath = "..\\Labb_BlazorApp\\wwwroot\\resources\\customers-100.csv";
        Delimiter = ",";
    }

    public UserServiceCsv(string filePath)
    {
        FilePath = filePath;
        Delimiter = ",";
    }

    public UserServiceCsv(string filePath, string delimiter)
    {
        FilePath = filePath;
        Delimiter = delimiter;
    }

    public IEnumerable<User> GetUsers()
    {
        var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = Delimiter };
        using (var reader = new StreamReader(FilePath))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Context.RegisterClassMap<UserMap>();
            var users = csv.GetRecords<User>().ToList();
            return users;
        }
    }
}