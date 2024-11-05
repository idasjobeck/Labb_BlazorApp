using Labb_BlazorApp.Models;
using System.Text.Json;

namespace Labb_BlazorApp.Services;

public class UserServiceApi :  IUserService
{
    public string Url { get; set; }

    public UserServiceApi()
    {
        Url = "https://jsonplaceholder.typicode.com/users";
    }

    public UserServiceApi(string url)
    {
        Url = url;
    }

    public IEnumerable<User> GetUsers()
    {
        Task<IEnumerable<User>> usersTask = Task.Run<IEnumerable<User>>(async() => await GetUsersAsync());
        return usersTask.Result;
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        var jsonData = await GetDataFromApi(Url);
        var users = await DeserializeJson(jsonData);

        return users;
    }

    public async Task<string> GetDataFromApi(string url)
    {
        using HttpClient client = new HttpClient();
        Task<string> dataFetched = client.GetStringAsync(url);
        
        var data = await dataFetched;

        return data;
    }

    public async Task<IEnumerable<User>> DeserializeJson(string jsonData)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<IEnumerable<User>>(jsonData, options)!;
    }
}