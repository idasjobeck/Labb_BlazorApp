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
using CsvHelper;
using System;

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
    private SortOrder _sortOrder = SortOrder.Ascending;
    private SortByAttribute _sortBy = SortByAttribute.FirstName;
    private string _sortOrderIndicatorUserId, _sortOrderIndicatorFirstName, _sortOrderIndicatorLastName, _sortOrderIndicatorEmail, _sortOrderIndicatorCompanyName;
    private SearchCriteria _searchCriteria = SearchCriteria.None;
    private bool _searchDisabled = true;
    private string _searchTerm = string.Empty;
    private bool _dataSourceDisabled = true;
    private string _resetSearchCriteriaDropdown = "<option value=\"none\" selected>Please select</option>"; // this needs fixing with a better solution
    private string _loadingUserdataMessage = "Loading...";
    private readonly string _selectOtherDataSourceOnErrorMessage = "Please select an alternative data source from the drop-down in the top right-hand corner.";


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await Task.Delay(3500); //set back to 3500 later

                //instantiate relevant UserService here?
                IUserService userService = new UserService();
                var url = "https://jsonplaceholder.typicode.com/users";
                var usersFromApi = await userService.GetUsers(url);
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

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        UsersToDisplay = _users?.OrderBy(user => user.FirstName).Take((int)_numberOfItemsToDisplay).ToList();
    }

    private void FilterUsers(NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        _numberOfItemsToDisplay = numberOfItemsToDisplay;
        UsersToDisplay = _users?.ToList(); //reset UsersToDisplay to whole list of all users, ensuring that option to increase number of items to display works.

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (UsersToDisplay.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        SortUsers(_sortBy, false); //maintain sort order
        IUserDataProcessing DataProcessing = new DataProcessing();
        UsersToDisplay = DataProcessing.Filter(UsersToDisplay!, _numberOfItemsToDisplay).ToList();
    }

    private async Task DataSourceIsChanged(ChangeEventArgs args)
    {
        try
        {
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
            _loadingUserdataMessage = "Loading...";
            IUserService userService = new UserService();
            var url = "https://jsonplaceholder.typicode.com/users";
            var filePath = "..\\Labb_BlazorApp\\wwwroot\\resources\\customers-100.csv"; //C:\Users\idasj\source\repos\ITHS\Programmering_C#\Labb_BlazorApp\Labb_BlazorApp\wwwroot\resources\customers-100.csv
            var csvDelimiter = ',';

            switch (args.Value?.ToString())
            {
                case "api":
                    //get users from API
                    var usersFromApi = await userService.GetUsers(url);
                    _users = usersFromApi.ToList();
                    break;
                case "memory":
                    //get users from memory
                    _users = userService.GetUsers().ToList();
                    break;
                case "csv":
                    //get users from csv file
                    _users = userService.GetUsers(filePath, csvDelimiter).ToList();
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
        catch (InvalidOperationException e) //can be thrown by the HttpClient.GetStringAsync method and CsvHelper.CsvReader.GetRecords<T> method
        {
            ErrorHandling(e); //error messages vary, so not providing a "user friendly" error message for this exception.
        }
        catch (TaskCanceledException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The request failed due to timeout.");
        }
        catch (UriFormatException e) //can be thrown by the HttpClient.GetStringAsync method
        {
            ErrorHandling(e, "The provided request URI is not valid relative or absolute URI.");
        }
        catch (FileNotFoundException e) //can be thrown by the CsvHelper.CsvReader.GetRecords<T> method
        {
            ErrorHandling(e, "Could not find the file.");
        }
        catch (DirectoryNotFoundException e) //can be thrown by the CsvHelper.CsvReader.GetRecords<T> method
        {
            ErrorHandling(e, "Could not find a part of the path"); 
        }
        catch (HeaderValidationException e) //can be thrown by the CsvHelper.CsvReader.GetRecords<T> method
        {
            ErrorHandling(e);
        }
        catch (Exception e)
        {
            ErrorHandling(e, "Unknown error.");
        }
    }

    private void ChangeSortDirection() => _sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

    private void SetSortOrderIndicator()
    {
        var sortOrderIndicatorNotSortedDblArrow = "<i class=\"fa-solid fa-sort\"></i>";
        var sortOrderIndicatorAscendingAZ = "<i class=\"fa-solid fa-arrow-down-a-z\"></i>";
        var sortOrderIndicatorDescendingZA = "<i class=\"fa-solid fa-arrow-up-z-a\"></i>";
        var sortOrderIndicatorAscending19 = "<i class=\"fa-solid fa-arrow-down-1-9\"></i>";
        var sortOrderIndicatorDescending91 = "<i class=\"fa-solid fa-arrow-up-9-1\"></i>";

        _sortOrderIndicatorUserId = sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorFirstName = sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorLastName = sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorEmail = sortOrderIndicatorNotSortedDblArrow;
        _sortOrderIndicatorCompanyName = sortOrderIndicatorNotSortedDblArrow;

        if (_sortOrder == SortOrder.Ascending)
        {
            switch (_sortBy)
            {
                case SortByAttribute.UserId:
                    _sortOrderIndicatorUserId = sortOrderIndicatorAscending19;
                    break;
                case SortByAttribute.FirstName:
                    _sortOrderIndicatorFirstName = sortOrderIndicatorAscendingAZ;
                    break;
                case SortByAttribute.LastName:
                    _sortOrderIndicatorLastName = sortOrderIndicatorAscendingAZ;
                    break;
                case SortByAttribute.Email:
                    _sortOrderIndicatorEmail = sortOrderIndicatorAscendingAZ;
                    break;
                case SortByAttribute.Company:
                    _sortOrderIndicatorCompanyName = sortOrderIndicatorAscendingAZ;
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
                    _sortOrderIndicatorUserId = sortOrderIndicatorDescending91;
                    break;
                case SortByAttribute.FirstName:
                    _sortOrderIndicatorFirstName = sortOrderIndicatorDescendingZA;
                    break;
                case SortByAttribute.LastName:
                    _sortOrderIndicatorLastName = sortOrderIndicatorDescendingZA;
                    break;
                case SortByAttribute.Email:
                    _sortOrderIndicatorEmail = sortOrderIndicatorDescendingZA;
                    break;
                case SortByAttribute.Company:
                    _sortOrderIndicatorCompanyName = sortOrderIndicatorDescendingZA;
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
            ChangeSortDirection();

        IUserDataProcessing DataProcessing = new DataProcessing();
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
        IUserDataProcessing DataProcessing = new DataProcessing();
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