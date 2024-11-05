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
using System.Xml.Linq;

namespace Labb_BlazorApp.Components.Pages;

public enum NumberOfItemsToDisplay
{
    [Description("All")] DisplayAll = 0,
    Display05 = 5,
    Display10 = 10,
    Display25 = 25,
    Display50 = 50
}

public enum DataSource
{
    [Description("API")] Api,
    Memory,
    [Description("CSV")] Csv
}

public partial class Users
{
    private List<User>? _users; //original list of users fetched from data source
    public List<User>? UsersToDisplay { get; set; } //list of users being manipulated and displayed
    private UserSortOrderIndicators _sortOrderIndicator = new UserSortOrderIndicators();
    public UserDataProcessing DataProcessing = new UserDataProcessing();


    private NumberOfItemsToDisplay _numberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
    public NumberOfItemsToDisplay NumberOfItemsToDisplay { get => _numberOfItemsToDisplay; set => _numberOfItemsToDisplay = value; }
    public List<NumberOfItemsToDisplay> NumberOfItemsToDisplayList = [NumberOfItemsToDisplay.Display05, NumberOfItemsToDisplay.Display10, NumberOfItemsToDisplay.Display25, NumberOfItemsToDisplay.Display50, NumberOfItemsToDisplay.DisplayAll];
    

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

                var userService = UserServiceProvider.GetUserService(DataSource);
                _users = userService.GetUsers().ToList();
                SetUsersToDisplay();
                _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
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
        catch (AggregateException e)
        {
            ErrorHandling(e); //error messages vary, so not providing a "user friendly" error message for this exception.
        }
        catch (Exception e)
        {
            ErrorHandling(e, "Unknown error.");
        }
    }

    private void SetUsersToDisplay()
    {
        DataProcessing.SortBy = SortByAttribute.FirstName;
        DataProcessing.SortOrder = SortOrder.Ascending;

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        UsersToDisplay = _users?.OrderBy(user => user.FirstName).Take((int)_numberOfItemsToDisplay).ToList();
    }

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
            _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
            DataProcessing.ResetSearchOptions();
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
        catch (AggregateException e)
        {
            ErrorHandling(e); //error messages vary, so not providing a "user friendly" error message for this exception.
        }
        catch (Exception e)
        {
            ErrorHandling(e, "Unknown error.");
        }
    }

    private void AllowDataSourceSelectionOnError()
    {
        _dataSourceDisabled = false;
        StateHasChanged();
    }

    private void FilterUsers(NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        NumberOfItemsToDisplay = numberOfItemsToDisplay;

        //reset UsersToDisplay to whole list of all users, ensuring that option to increase number of items to display works.
        UsersToDisplay = _users?.ToList();

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (UsersToDisplay.IsNumberToDisplayGreaterThanUsersAvailable((int)_numberOfItemsToDisplay))
            _numberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        SortUsers(DataProcessing.SortBy, false); //maintain sort order
        UsersToDisplay = DataProcessing.Filter(UsersToDisplay!, _numberOfItemsToDisplay).ToList();
    }

    private void SortUsers(SortByAttribute sortBy, bool changeSortDirection)
    {
        DataProcessing.SortBy = sortBy;

        if (changeSortDirection)
            DataProcessing.ChangeSortDirection();

        UsersToDisplay = DataProcessing.Sort(UsersToDisplay!).ToList();
        _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
    }

    private void SearchUsers()
    {
        UsersToDisplay = DataProcessing.Search(UsersToDisplay!).ToList();
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
}