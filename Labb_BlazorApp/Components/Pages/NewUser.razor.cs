namespace Labb_BlazorApp.Components.Pages;

public partial class NewUser
{
    public bool displayForm = true;
    private User _user = new User();
    
    private void AddUserToDb()
    {
        //would have code to add user to database.

        //for the time being it just hides the form by setting displayForm to false.
        displayForm = false;
    }
}
