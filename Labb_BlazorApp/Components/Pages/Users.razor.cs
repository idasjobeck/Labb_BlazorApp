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
using System.ComponentModel;
using Microsoft.Extensions.Primitives;

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
    [Description("Please Select")] None,
    [Description("User ID")] UserId,
    [Description("First name")] FirstName,
    [Description("Last name")] LastName,
    Email,
    [Description("Company name")] Company
}

public enum DataSource
{
    [Description("API")] Api,
    Memory,
    [Description("CSV")] Csv
}

public partial class Users
{
    private List<User>? _users;
    public List<User>? UsersToDisplay { get; set; }
    private NumberOfItemsToDisplay _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
    private SortOrder _sortOrder = SortOrder.Ascending;
    private SortByAttribute _sortBy = SortByAttribute.FirstName;
    private UserSortOrderIndicators _sortOrderIndicator = new UserSortOrderIndicators();
    private SearchCriteria _searchCriteria = SearchCriteria.None;
    public SearchCriteria SearchCriteria
    {
        get => _searchCriteria;
        set => _searchCriteria = value;
    }
    public List<SearchCriteria> SearchCriteriaList = [ SearchCriteria.None, SearchCriteria.UserId, SearchCriteria.FirstName, SearchCriteria.LastName,
        SearchCriteria.Email, SearchCriteria.Company ];
    private bool _searchDisabled = true;
    private string _searchTerm = string.Empty;
    private DataSource _dataSource;
    public DataSource DataSource { get => _dataSource; set => _dataSource = value; }
    public List<DataSource> DataSourceList = [ DataSource.Api, DataSource.Memory, DataSource.Csv ];
    private bool _dataSourceDisabled = true;
    private string _loadingUserdataMessage = "Loading...";
    private readonly string _selectOtherDataSourceOnErrorMessage = "Please select an alternative data source from the drop-down in the top right-hand corner.";


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await Task.Delay(3500); //set back to 3500 later

                IUserService userService = new UserServiceApi();
                _users = userService.GetUsers().ToList();
                SetUsersToDisplay();
                _sortOrderIndicator.SetSortOrderIndicator(_sortOrder, _sortBy);
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

    //private async Task DataSourceIsChanged(ChangeEventArgs args)
    private void DataSourceIsChanged(DataSource selectedItem)
    {
        try
        {
            DataSource = selectedItem; //set data source to selected item
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
            _loadingUserdataMessage = "Loading...";

            var userService = UserServiceProvider.GetUserService(selectedItem);
            _users = userService.GetUsers().ToList();
            SetUsersToDisplay();
            _sortOrderIndicator.SetSortOrderIndicator(_sortOrder, _sortBy);
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

    private void SortUsers(SortByAttribute sortBy, bool changeSortDirection)
    {
        _sortBy = sortBy;

        if (changeSortDirection)
            ChangeSortDirection();

        IUserDataProcessing DataProcessing = new DataProcessing();
        UsersToDisplay = DataProcessing.Sort(UsersToDisplay!, _sortBy, _sortOrder).ToList();
        _sortOrderIndicator.SetSortOrderIndicator(_sortOrder, sortBy);
    }

    private void SearchCriteriaIsChanged (SearchCriteria selectedItem)
    {
        SearchCriteria = selectedItem; //set search criteria to selected item
        _searchTerm = string.Empty; //clear search input box

        //check if searchable criteria is selected, and enable/disable the search button accordingly
        _searchDisabled = _searchCriteria == SearchCriteria.None ? _searchDisabled = true : _searchDisabled = false;
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
        SearchCriteria = SearchCriteria.None;
        
        //disable search button
        _searchDisabled = true;
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