using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Components.Pages;

public partial class NewUser
{
    public bool displayForm = true;
    private User _user = new User();
    private string _defaultIfFieldNotCompleted = "unknown";
    
    private void AddUserToDb()
    {
        //would have code to add user to database.

        //for the time being it just hides the form by setting displayForm to false.
        displayForm = false;
    }
}

public static class StringExtensions
{
    public static string OrIfEmpty(this string? str, string defaultValue)
    {
        return !string.IsNullOrEmpty(str) ? str : defaultValue;
    }

    public static string OrIfEmpty(this string? self, Func<string> defaultValue)
    {
        return !string.IsNullOrEmpty(self) ? self : defaultValue();
    }
}
