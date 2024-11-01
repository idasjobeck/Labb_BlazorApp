using Microsoft.AspNetCore.Routing;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Html;
using static Labb_BlazorApp.Components.Pages.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Labb_BlazorApp.Extensions;
using Labb_BlazorApp.Models;
using Labb_BlazorApp.Services;

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
    private readonly string _url = "https://jsonplaceholder.typicode.com/users";
    public IUserDataProcessing DataProcessing = new DataProcessing();
    private SortOrder _sortOrder = SortOrder.Ascending;
    private SortByAttribute _sortBy = SortByAttribute.FirstName;
    private SearchCriteria _searchCriteria = SearchCriteria.None;
    private bool _searchDisabled = true;
    //private string _searchButtonClass = "btn btn-outline-secondary";
    private string _searchTerm = string.Empty;
    private readonly string _sortOrderIndicatorNotSortedDblArrow = "<i class=\"fa-solid fa-sort\"></i>";
    private readonly string _sortOrderIndicatorAscendingAZ = "<i class=\"fa-solid fa-arrow-down-a-z\"></i>";
    private readonly string _sortOrderIndicatorDescendingZA = "<i class=\"fa-solid fa-arrow-up-z-a\"></i>";
    private readonly string _sortOrderIndicatorAscending19 = "<i class=\"fa-solid fa-arrow-down-1-9\"></i>";
    private readonly string _sortOrderIndicatorDescending91 = "<i class=\"fa-solid fa-arrow-up-9-1\"></i>";
    private bool _dataSourceDisabled = true;
    private string _resetSearchCriteriaDropdown = "<option value=\"none\" selected>Please select</option>";
    //private string _testReset; //Was testing resetting the search criteria dropdown when changing data source. Binding a variable will reset it, but can't have the onchange at the same time.
    private string _loadingUserdataMessage = "Loading...";
    private readonly string _selectOtherDataSourceOnErrorMessage = "Please select an alternative data source from the drop-down in the top right-hand corner.";

    private string _sortOrderIndicatorUserId = "<i class=\"fa-solid fa-sort\"></i>";
    private string _sortOrderIndicatorFirstName = "<i class=\"fa-solid fa-sort\"></i>";
    private string _sortOrderIndicatorLastName = "<i class=\"fa-solid fa-sort\"></i>";
    private string _sortOrderIndicatorEmail = "<i class=\"fa-solid fa-sort\"></i>";
    private string _sortOrderIndicatorCompanyName = "<i class=\"fa-solid fa-sort\"></i>"; //want to set them to _sortOrderIndicatorNotSortedDblArrow, but get error message that it can't set it to a non-static field.

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await Task.Delay(3500);

                var usersFromApi = await UserService.GetUsers(_url);
                _users = usersFromApi.ToList();
                SetUsersToDisplay();
                SetSortOrderIndicator();
                _dataSourceDisabled = false;

                StateHasChanged();
            }
        }
        catch (HttpRequestException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "404 (Not Found)");
        }
        catch (InvalidOperationException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The request URL is in an invalid format.");
        }
        catch (TaskCanceledException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The request failed due to timeout.");
        }
        catch (UriFormatException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The provided request URI is not valid relative or absolute URI.");
        }
        catch (Exception e)
        {
            ErrorHandling(e, "Unknown error.");
        }
    }

    private void SetUsersToDisplay()
    {
        _sortBy = SortByAttribute.FirstName;
        _sortOrder = SortOrder.Ascending;
        
        if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
        {
            ResetUsersToDisplayToAll();
            UsersToDisplay = _users?.OrderBy(user => user.FirstName).ToList();
        }
        else
        {
            UsersToDisplay = _users?.OrderBy(user => user.FirstName).Take((int)_numberOfItemsToDisplay).ToList();
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
        UsersToDisplay = _users?.ToList();

        if (UsersToDisplay.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
        {
            //set to display all
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;
        }

        SortUsers(_sortBy, false);
        UsersToDisplay = DataProcessing.Filter(UsersToDisplay!, _numberOfItemsToDisplay).ToList();
    }

    private async Task DataSourceIsChanged(ChangeEventArgs args)
    {
        try
        {
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
            _loadingUserdataMessage = "Loading...";

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
            ResetSearchOptions();
        }
        catch (ArgumentNullException e) //can be thrown by the JsonSerializer.Deserialize method
        {
            ErrorHandling(e, "utf8Json or jsonTypeInfo is null.");
        }
        catch (JsonException e) //can be thrown by the JsonSerializer.Deserialize method
        {
            ErrorHandling(e, "The JSON is invalid.");
        }
        catch (HttpRequestException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "404 (Not Found)");
        }
        catch (InvalidOperationException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The request URL is in an invalid format.");
        }
        catch (TaskCanceledException e)  //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The request failed due to timeout.");
        }
        catch (UriFormatException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The provided request URI is not valid relative or absolute URI.");
        }
        catch (Exception e)
        {
            ErrorHandling(e, "Unknown error.");
        }
    }

    private void ChangeSortDirection() => _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

    private void SetSortOrderIndicator()
    {
        _sortOrderIndicatorUserId = _sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;

        if (_sortOrder == SortOrder.Ascending)
        {
            switch (_sortBy)
            {
                case SortByAttribute.UserId:
                    _sortOrderIndicatorUserId = _sortOrderIndicatorAscending19;
                    break;
                case SortByAttribute.FirstName:
                    _sortOrderIndicatorFirstName = _sortOrderIndicatorAscendingAZ;
                    break;
                case SortByAttribute.LastName:
                    _sortOrderIndicatorLastName = _sortOrderIndicatorAscendingAZ;
                    break;
                case SortByAttribute.Email:
                    _sortOrderIndicatorEmail = _sortOrderIndicatorAscendingAZ;
                    break;
                case SortByAttribute.Company:
                    _sortOrderIndicatorCompanyName = _sortOrderIndicatorAscendingAZ;
                    break;
                default:
                    break;
            }
        }
        else if (_sortOrder == SortOrder.Descending)
        {
            switch (_sortBy)
            {
                case SortByAttribute.UserId:
                    _sortOrderIndicatorUserId = _sortOrderIndicatorDescending91;
                    break;
                case SortByAttribute.FirstName:
                    _sortOrderIndicatorFirstName = _sortOrderIndicatorDescendingZA;
                    break;
                case SortByAttribute.LastName:
                    _sortOrderIndicatorLastName = _sortOrderIndicatorDescendingZA;
                    break;
                case SortByAttribute.Email:
                    _sortOrderIndicatorEmail = _sortOrderIndicatorDescendingZA;
                    break;
                case SortByAttribute.Company:
                    _sortOrderIndicatorCompanyName = _sortOrderIndicatorDescendingZA;
                    break;
                default:
                    break;
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

        UsersToDisplay = DataProcessing.Sort(UsersToDisplay!, _sortBy, _sortOrder).ToList();
        SetSortOrderIndicator();
    }

    private void SearchCriteriaIsChanged(ChangeEventArgs args)
    {
        _searchTerm = string.Empty; //clear search input box

        _searchCriteria = args.Value?.ToString() switch
        {
            "none" => SearchCriteria.None,
            "userId" => SearchCriteria.UserId,
            "firstName" => SearchCriteria.FirstName,
            "lastName" => SearchCriteria.LastName,
            "email" => SearchCriteria.Email,
            "company" => SearchCriteria.Company,
            _ => SearchCriteria.None
        };

        if (_searchCriteria == SearchCriteria.None)
            _searchDisabled = true;
        else
            _searchDisabled = false;
    }

    private void SearchUsers()
    {
        UsersToDisplay = DataProcessing.Search(UsersToDisplay!, _searchCriteria, _searchTerm).ToList();
    }

    private void ResetSearchOptions()
    {
        //clear search input box
        _searchTerm = string.Empty;
        //set search criteria to none
        _searchCriteria = SearchCriteria.None; //This does not reset the drop-down for search criteria though.
        //disable search button
        _searchDisabled = true;

        //reset drop-down for search criteria to "please select"
        //could not find a way to change the drop-down selection from C#, so adding/removing a space in the label for the drop-down option as below.
        _resetSearchCriteriaDropdown = _resetSearchCriteriaDropdown == "<option value=\"none\" selected>Please select </option>"
            ? "<option value=\"none\" selected>Please select</option>"
            : "<option value=\"none\" selected>Please select </option>";

        //_testReset = "none"; //Was testing resetting the search criteria dropdown when changing data source. Binding a variable will reset it, but can't have the onchange at the same time.
    }

    private void ErrorHandling(Exception e)
    {
        //ideally log the exception, e.Message
        _loadingUserdataMessage = $"An error has occured: {e.Message}";
        AllowDataSourceSelectionOnError();
    }

    private void ErrorHandling(Exception e, string userFriendlyErrorMessage)
    {
        //ideally log the exception, e.Message
        _loadingUserdataMessage = $"An error has occured: {userFriendlyErrorMessage}";
        AllowDataSourceSelectionOnError();
    }

    private void AllowDataSourceSelectionOnError()
    {
        _dataSourceDisabled = false;
        StateHasChanged();
    }
}