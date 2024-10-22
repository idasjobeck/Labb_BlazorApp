using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using static Labb_BlazorApp.Components.Pages.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Labb_BlazorApp.Components.Pages;

public enum NumberOfItemsToDisplay
{
    DisplayAll = 0,
    Display05 = 5,
    Display10 = 10,
    Display25 = 25,
    Display50 = 50
}

public partial class Users
{
    private List<User>? _users;
    public List<User>? UsersToDisplay { get; set; }
    public UserService MyUserService = new UserService();
    private NumberOfItemsToDisplay _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(3500); //change to 3500 later. Set to 500 atm to shorten wait time when running app to test it.

            _users = MyUserService.GetUsers().ToList();
            
            if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
            {
                ResetUsersToDisplayToAll();
                UsersToDisplay = _users.OrderBy(user => user.FirstName).ToList();
            }
            else
            {
                UsersToDisplay = _users.OrderBy(user => user.FirstName).Take((int)_numberOfItemsToDisplay).ToList();
            }
                
            StateHasChanged();
        }
    }

    private void ResetUsersToDisplayToAll()
    {
        UsersToDisplay = _users?.ToList();
        _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;
    }

    private void FilterUsers(NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        _numberOfItemsToDisplay = numberOfItemsToDisplay;
        UsersToDisplay = _users.ToList();

        if (UsersToDisplay.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
        {
            //set to display all
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;
        }

        UsersToDisplay = UsersToDisplay.Filter(_numberOfItemsToDisplay);
    }


}

public class User
{
    private int _userId;

    public int UserId { get => _userId; }

    public string FullName
    {
        get { return $"{FirstName} {LastName}"; }
        set
        {
            var fullName = value.Split(' ');
            FirstName = fullName[0];
            LastName = fullName[1];
        }
    }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Website { get; set; }

    public Address Address { get; set; }
    public Company Company { get; set; }

    public User()
    {
        _userId = 999; //should really generate an UserID, and would think that it should be unique for each user.
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
        Website = string.Empty;
        Address = new Address();
        Company = new Company();
    }

    public User(int userId, string firstName, string lastName, string email)
    {
        _userId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = string.Empty;
        Website = string.Empty;
        Address = new Address();
        Company = new Company();
    }

    public User(int userId, string firstName, string lastName, string email, string phoneNumber, string website, string street, string city, string zipcode, string companyName, string catchphrase)
    {
        _userId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Website = website;
        Address = new Address(street, city, zipcode);
        Company = new Company(companyName, catchphrase);
    }
}

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }

    public Address()
    {
        Street = string.Empty;
        City = string.Empty;
        ZipCode = string.Empty;
    }

    public Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

}

public class Company
{
    public string CompanyName { get; set; }
    public string Catchphrase { get; set; }

    public Company()
    {
        CompanyName = string.Empty;
        Catchphrase = string.Empty;
    }

    public Company(string companyName, string catchphrase)
    {
        CompanyName = companyName;
        Catchphrase = catchphrase;
    }
}

public class UserService
{
    public IEnumerable<User> GetUsers()
    {
        //random user data obtained from https://www.rndgen.com/data-generator; catchphrases from various Reddit threads.

        return new List<User>()
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
}

public static class UserExtensions
{
    public static bool IsNumberToDisplayGreaterThanUsersAvailable(this IEnumerable<User> users,
        int numberOfItemsToDisplay)
    {
        return numberOfItemsToDisplay > users.Count();
    }

    public static List<User> Filter(this IEnumerable<User> users, NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        switch (numberOfItemsToDisplay)
        {
            case NumberOfItemsToDisplay.Display05:
                return users.Take((int)NumberOfItemsToDisplay.Display05).ToList();
            case NumberOfItemsToDisplay.Display10:
                return users.Take((int)NumberOfItemsToDisplay.Display10).ToList();
            case NumberOfItemsToDisplay.Display25:
                return users.Take((int)NumberOfItemsToDisplay.Display25).ToList();
            case NumberOfItemsToDisplay.Display50:
                return users.Take((int)NumberOfItemsToDisplay.Display50).ToList();
            case NumberOfItemsToDisplay.DisplayAll:
            default:
                return users.ToList();
        }
    }
}