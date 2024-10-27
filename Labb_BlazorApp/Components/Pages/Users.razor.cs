using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
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
public enum SortOrder
{
    Ascending,
    Descending
}

public enum SortByAttribute
{
    UserId,
    FirstName,
    LastName,
    Email,
    Company
}

public enum SearchCriteria
{
    None,
    UserId,
    FirstName,
    LastName,
    Email,
    Company
}

public partial class Users
{
    private List<User>? _users;
    public List<User>? UsersToDisplay { get; set; }
    private NumberOfItemsToDisplay _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
    public IUserService UserService = new UserService();
    private string _url = "https://jsonplaceholder.typicode.com/users";
    public IUserDataProcessing DataProcessing = new DataProcessing();
    private SortOrder _sortOrder = SortOrder.Ascending;
    private SortByAttribute _sortBy = SortByAttribute.FirstName;
    private SearchCriteria _searchCriteria = SearchCriteria.None;
    private bool _searchDisabled = true;
    private string _searchTerm;
    private readonly string _sortOrderIndicatorNotSortedDblArrow = "<i class=\"fa-solid fa-sort\"></i>";
    private readonly string _sortOrderIndicatorAscendingAZ = "<i class=\"fa-solid fa-arrow-down-a-z\"></i>";
    private readonly string _sortOrderIndicatorDescendingZA = "<i class=\"fa-solid fa-arrow-up-z-a\"></i>";
    private readonly string _sortOrderIndicatorAscending19 = "<i class=\"fa-solid fa-arrow-down-1-9\"></i>";
    private readonly string _sortOrderIndicatorDescending91 = "<i class=\"fa-solid fa-arrow-up-9-1\"></i>";

    private string _sortOrderIndicatorUserID,
        _sortOrderIndicatorFirstName,
        _sortOrderIndicatorLastName,
        _sortOrderIndicatorEmail,
        _sortOrderIndicatorCompanyName = "<i class=\"fa-solid fa-sort\"></i>"; //want to set them to _sortOrderIndicatorNotSortedDblArrow, but get error message that it can't set it to a non-static field.

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Task.Delay(3500);

            var usersFromApi = await UserService.GetUsers(_url);
            _users = usersFromApi.ToList();
            SetUsersToDisplay();
            SetSortOrderIndicator();

            StateHasChanged();
        }
    }

    private void SetUsersToDisplay()
    {
        _sortBy = SortByAttribute.FirstName;
        _sortOrder = SortOrder.Ascending;
        
        if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
        {
            ResetUsersToDisplayToAll();
            UsersToDisplay = _users.OrderBy(user => user.FirstName).ToList();
        }
        else
        {
            UsersToDisplay = _users.OrderBy(user => user.FirstName).Take((int)_numberOfItemsToDisplay).ToList();
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

        SortUsers(_sortBy, false);
        UsersToDisplay = DataProcessing.Filter(UsersToDisplay, _numberOfItemsToDisplay);
    }

    private async Task DataSourceIsChanged(ChangeEventArgs args)
    {
        _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;

        switch (args.Value?.ToString())
        {
            case "api":
                //get users from API
                var usersFromApi = await UserService.GetUsers(_url);
                _users = usersFromApi.ToList();
                break;
            case "memory":
                //get users from memory
                _users = UserService.GetUsers().ToList();
                break;
            default:
                break;
        }

        SetUsersToDisplay();
        SetSortOrderIndicator();
    }

    private void ChangeSortDirection()
    {
        _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
    }

    private void SetSortOrderIndicator()
    {
        if (_sortOrder == SortOrder.Ascending)
        {
            if (_sortBy == SortByAttribute.UserId)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorAscending19;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.FirstName)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorAscendingAZ;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.LastName)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorAscendingAZ;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.Email)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorAscendingAZ;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.Company)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorAscendingAZ;
            }
        }
        else if (_sortOrder == SortOrder.Descending)
        {
            if (_sortBy == SortByAttribute.UserId)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorDescending91;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.FirstName)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorDescendingZA;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.LastName)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorDescendingZA;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.Email)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorDescendingZA;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
            }
            else if (_sortBy == SortByAttribute.Company)
            {
                _sortOrderIndicatorUserID = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
                _sortOrderIndicatorCompanyName = _sortOrderIndicatorDescendingZA;
            }
        }
    }

    private void SortUsers(SortByAttribute sortBy, bool changeSortDirection)
    {
        _sortBy = sortBy;

        if (changeSortDirection)
        {
            ChangeSortDirection();
        }

        UsersToDisplay = DataProcessing.Sort(UsersToDisplay, _sortBy, _sortOrder);
        SetSortOrderIndicator();
    }

    private void SearchCriteriaIsChanged(ChangeEventArgs args)
    {
        //clear search input box
        _searchTerm = string.Empty;

        switch (args.Value?.ToString()) //change to switch expression?
        {
            case "none":
                _searchCriteria = SearchCriteria.None;
                break;
            case "userId":
                _searchCriteria = SearchCriteria.UserId;
                break;
            case "firstName":
                _searchCriteria = SearchCriteria.FirstName;
                break;
            case "lastName":
                _searchCriteria = SearchCriteria.LastName;
                break;
            case "email":
                _searchCriteria = SearchCriteria.Email;
                break;
            case "company":
                _searchCriteria = SearchCriteria.Company;
                break;
            default:
                break;
        }

        if (_searchCriteria == SearchCriteria.None)
            _searchDisabled = true;
        else
            _searchDisabled = false;
    }

    private void SearchUsers()
    {
        UsersToDisplay = DataProcessing.Search(UsersToDisplay, _searchCriteria, _searchTerm);
    }
}

public class User
{
    private int _userId;

    [JsonPropertyName("Id")]
    public int UserId { 
        get => _userId;
        set => _userId = value;
    }

    [JsonPropertyName("Name")]
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

    [Required(ErrorMessage = "The first name field is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "The last name field is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "The email field is required.")]
    [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "The email entered must be in a valid format.")]
    public string Email { get; set; }

    [JsonPropertyName("Phone")]
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
    [JsonPropertyName("Name")]
    public string? CompanyName { get; set; }
    public string? Catchphrase { get; set; }

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

        return users;
    }

    public async Task<string> GetDataFromApi(string url)
    {
        using HttpClient client = new HttpClient();
        Task<string> dataFetched = client.GetStringAsync(url);

        var data = await dataFetched;

        return data;
    }
}

public interface IUserService
{
    public IEnumerable<User> GetUsers();

    public Task<IEnumerable<User>> GetUsers(string url);
}

public class DataProcessing : IUserDataProcessing
{
    public List<User> Filter(IEnumerable<User> users, NumberOfItemsToDisplay numberOfItemsToDisplay)
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

    public List<User> Sort(IEnumerable<User> users, SortByAttribute sortBy, SortOrder sortOrder)
    {
        if (sortOrder == SortOrder.Ascending)
        {
            switch (sortBy)
            {
                case SortByAttribute.UserId:
                    return users.OrderBy(users => users.UserId).ToList();
                case SortByAttribute.FirstName:
                    return users.OrderBy(users => users.FirstName).ToList();
                case SortByAttribute.LastName:
                    return users.OrderBy(users => users.LastName).ToList();
                case SortByAttribute.Email:
                    return users.OrderBy(users => users.Email).ToList();
                case SortByAttribute.Company:
                    return users.OrderBy(users => users.Company.CompanyName).ToList();
                default:
                    return users.ToList();
            }
        }
        else if (sortOrder == SortOrder.Descending)
        {
            switch (sortBy)
            {
                case SortByAttribute.UserId:
                    return users.OrderByDescending(users => users.UserId).ToList();
                case SortByAttribute.FirstName:
                    return users.OrderByDescending(users => users.FirstName).ToList();
                case SortByAttribute.LastName:
                    return users.OrderByDescending(users => users.LastName).ToList();
                case SortByAttribute.Email:
                    return users.OrderByDescending(users => users.Email).ToList();
                case SortByAttribute.Company:
                    return users.OrderByDescending(users => users.Company.CompanyName).ToList();
                default:
                    return users.ToList();
            }
        }

        return users.ToList();
    }

    public List<User> Search(IEnumerable<User> users, SearchCriteria searchCriteria, string searchTerm)
    {
        switch (searchCriteria)
        {
            case SearchCriteria.UserId:
                users = users.Where(users =>
                    users.UserId.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
                break;
            case SearchCriteria.FirstName:
                users = users.Where(users =>
                    users.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
                break;
            case SearchCriteria.LastName:
                users = users.Where(users =>
                    users.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
                break;
            case SearchCriteria.Email:
                users = users.Where(users =>
                    users.Email.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
                break;
            case SearchCriteria.Company:
                users = users.Where(users =>
                    users.Company.CompanyName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
                break;
            case SearchCriteria.None:
            default:
                break;
        }

        return users.ToList();
    }
}

public interface IUserDataProcessing
{
    public List<User> Filter(IEnumerable<User> users, NumberOfItemsToDisplay numberOfItemsToDisplay);
    public List<User> Sort(IEnumerable<User> users, SortByAttribute sortBy, SortOrder sortOrder);
    public List<User> Search(IEnumerable<User> users, SearchCriteria searchCriteria, string searchTerm);
}

public static class UserExtensions
{
    public static bool IsNumberToDisplayGreaterThanUsersAvailable(this IEnumerable<User> users,
        int numberOfItemsToDisplay)
    {
        return numberOfItemsToDisplay > users.Count();
    }
}

