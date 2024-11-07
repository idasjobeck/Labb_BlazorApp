using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Components.Pages;

public partial class NewUser
{
    public bool DisplayForm = true;
    private User _user = new();
    private string _defaultIfFieldNotCompleted = "unknown";
    
    private void AddUserToDb()
    {
        //would have code to add user to database.

        //for the time being it just hides the form by setting DisplayForm to false.
        DisplayForm = false;
    }

    private void Reset()
    {
        //setting _user to new user to reset fields in the form as empty
        _user = new User();
    }
}