using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Extensions;

public static class UserExtensions
{
    public static bool IsNumberToDisplayGreaterThanUsersAvailable(this IEnumerable<User>? users,
        int numberOfItemsToDisplay)
    {
        return numberOfItemsToDisplay >= users?.Count();
    }
}