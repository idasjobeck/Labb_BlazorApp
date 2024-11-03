using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Services;

public interface IUserService
{
    public IEnumerable<User> GetUsers();

    public Task<IEnumerable<User>> GetUsers(string url);

    public IEnumerable<User> GetUsers(string filePath, char delimiter);
}