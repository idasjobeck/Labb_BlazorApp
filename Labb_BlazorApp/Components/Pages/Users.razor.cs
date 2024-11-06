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

public partial class Users
{
    private List<User>? _users; //original list of users fetched from data source
    public List<User>? UsersToDisplay { get; set; } //list of users being manipulated and displayed
    public DisplayProperties DisplayProperties = new DisplayProperties();
    private UserSortOrderIndicators _sortOrderIndicator = new UserSortOrderIndicators();
    public UserDataProcessing DataProcessing = new UserDataProcessing();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await Task.Delay(2500);

                var userService = UserServiceProvider.GetUserService(DisplayProperties.DataSourceProperties.DataSource);
                _users = userService.GetUsers().ToList();
                SetUsersToDisplay();
                _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
                DisplayProperties.DataSourceProperties.DataSourceDisabled = false;

                StateHasChanged();
            }
        }
        catch (Exception e)
        {
            ErrorHandling(e);
        }
    }

    private void SetUsersToDisplay()
    {
        DataProcessing.SortBy = SortByAttribute.FirstName;
        DataProcessing.SortOrder = SortOrder.Ascending;

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)DisplayProperties.NumberOfItemsToDisplay))
            DisplayProperties.NumberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        UsersToDisplay = _users?.OrderBy(user => user.FirstName).Take((int)DisplayProperties.NumberOfItemsToDisplay).ToList();
    }

    private void DataSourceIsChanged(DataSource selectedItem)
    {
        try
        {
            DisplayProperties.DataSourceProperties.DataSource = selectedItem; //set data source to selected item
            DisplayProperties.NumberOfItemsToDisplay = NumberOfItemsToDisplay.Display05; //set number of users to display to 5
            DisplayProperties.DisplayErrorMessage = false;
            DisplayProperties.DisplayLoadingMessage = true;

            var userService = UserServiceProvider.GetUserService(selectedItem);
            _users = userService.GetUsers().ToList();
            SetUsersToDisplay();
            _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
            DataProcessing.ResetSearchOptions();
        }
        catch (Exception e)
        {
            ErrorHandling(e);
        }
    }

    private void AllowDataSourceSelectionOnError()
    {
        DisplayProperties.DataSourceProperties.DataSourceDisabled = false;
        StateHasChanged();
    }

    private void FilterUsers(NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        DisplayProperties.NumberOfItemsToDisplay = numberOfItemsToDisplay;

        //reset UsersToDisplay to whole list of all users, ensuring that option to increase number of items to display works.
        UsersToDisplay = _users?.ToList();

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (UsersToDisplay.IsNumberToDisplayGreaterThanUsersAvailable((int)DisplayProperties.NumberOfItemsToDisplay))
            DisplayProperties.NumberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        SortUsers(DataProcessing.SortBy, false); //maintain sort order
        UsersToDisplay = DataProcessing.Filter(UsersToDisplay!, DisplayProperties.NumberOfItemsToDisplay).ToList();
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
        //ideally log the exception, e.Message and StackTrace
        DisplayProperties.DisplayLoadingMessage = false;
        DisplayProperties.DisplayErrorMessage = true;
        DisplayProperties.ExceptionMessage = e.GetType() == typeof(AggregateException) ? e.InnerException!.Message : e.Message;
        AllowDataSourceSelectionOnError();
    }

    private void ErrorHandling(Exception e, string userFriendlyErrorMessage)
    {
        //ideally log the exception, e.Message and StackTrace
        DisplayProperties.DisplayLoadingMessage = false;
        DisplayProperties.DisplayErrorMessage = true;
        DisplayProperties.ExceptionMessage = userFriendlyErrorMessage;
        AllowDataSourceSelectionOnError();
    }
}