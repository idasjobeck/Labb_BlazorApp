﻿using Labb_BlazorApp.Extensions;
using Labb_BlazorApp.Models;
using Labb_BlazorApp.Services;

namespace Labb_BlazorApp.Components.Pages;

public partial class Users
{
    private List<User>? _users; //original list of users fetched from data source
    public List<User>? UsersToDisplay { get; set; } //list of users being manipulated and displayed
    public DisplayOptions DisplayOptions = new();
    private UserSortOrderIndicators _sortOrderIndicator = new();
    public UserDataProcessing DataProcessing = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await Task.Delay(2500);

                var userService = UserServiceProvider.GetUserService(DisplayOptions.DataSourceOptions.DataSource);
                _users = userService.GetUsers().ToList();
                SetUsersToDisplay();
                _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
                DisplayOptions.DataSourceOptions.DataSourceDisabled = false;

                StateHasChanged();
            }
        }
        catch (AggregateException ae)
        {
            AggregateExceptionHandling(ae);
        }
        catch (Exception e)
        {
            ExceptionHandling(e);
        }
    }

    private void SetUsersToDisplay()
    {
        DataProcessing.SortBy = SortByAttribute.FirstName;
        DataProcessing.SortOrder = SortOrder.Ascending;

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (_users.IsNumberToDisplayGreaterThanUsersAvailable((int)DisplayOptions.NumberOfItemsToDisplay))
            DisplayOptions.NumberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        UsersToDisplay = _users?.OrderBy(user => user.FirstName).Take((int)DisplayOptions.NumberOfItemsToDisplay).ToList();
    }

    private void DataSourceIsChanged(DataSource selectedItem)
    {
        try
        {
            DisplayOptions.DataSourceOptions.DataSource = selectedItem; //set data source to selected item
            DisplayOptions.NumberOfItemsToDisplay = NumberOfItemsToDisplay.Display05; //set number of users to display to 5
            DisplayOptions.ResetExceptionSettings(); //clear potential error messages and set loading message to display

            var userService = UserServiceProvider.GetUserService(selectedItem);
            _users = userService.GetUsers().ToList();
            SetUsersToDisplay();
            _sortOrderIndicator.SetSortOrderIndicator(DataProcessing.SortOrder, DataProcessing.SortBy);
            DataProcessing.ResetSearchOptions();
        }
        catch (AggregateException ae)
        {
            AggregateExceptionHandling(ae);
        }
        catch (Exception e)
        {
            ExceptionHandling(e);
        }
    }

    private void AllowDataSourceSelectionOnError()
    {
        DisplayOptions.DataSourceOptions.DataSourceDisabled = false;
        StateHasChanged();
    }

    private void FilterUsers(NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        DisplayOptions.NumberOfItemsToDisplay = numberOfItemsToDisplay;

        //reset UsersToDisplay to whole list of all users, ensuring that option to increase number of items to display works.
        UsersToDisplay = _users?.ToList();

        //if "number of items to display" is greater than the count of users, then set "number of items to display" to all.
        if (UsersToDisplay.IsNumberToDisplayGreaterThanUsersAvailable((int)DisplayOptions.NumberOfItemsToDisplay))
            DisplayOptions.NumberOfItemsToDisplay = NumberOfItemsToDisplay.DisplayAll;

        SortUsers(DataProcessing.SortBy, false); //maintain sort order
        UsersToDisplay = DataProcessing.Filter(UsersToDisplay!, DisplayOptions.NumberOfItemsToDisplay).ToList();
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

    private void ExceptionHandling(Exception e, bool isAggregateException = false, bool hasUserFriendlyErrorMessage = false, string userFriendlyErrorMessage = "An error has occurred.")
    {
        //ideally log the exception, e.Message and StackTrace
        //(can expand the method to take different actions, as needed, depending on type of error)
        var exceptionMessage = hasUserFriendlyErrorMessage ? userFriendlyErrorMessage : e.Message;

        if (isAggregateException)
        {
            DisplayOptions.ExceptionMessage += $" {exceptionMessage}";
        }
        else if (!isAggregateException)
        {
            DisplayOptions.SetToDisplayException(exceptionMessage);
            AllowDataSourceSelectionOnError();
        }
    }

    private void AggregateExceptionHandling(AggregateException ae)
    {
        //ideally log the exception, ae.InnerException.Message and StackTrace
        DisplayOptions.ExceptionMessage = "One or more errors have occurred.";

        foreach (var exception in ae.InnerExceptions)
        {
            ExceptionHandling(exception, true);
        }
        DisplayOptions.SetToDisplayException();
        AllowDataSourceSelectionOnError();
    }
}