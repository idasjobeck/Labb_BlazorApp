using Labb_BlazorApp.Components.Pages;
using Labb_BlazorApp.Models;
using System.ComponentModel;
using CsvHelper;
using System.Xml;

namespace Labb_BlazorApp.Services;

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

public class UserDataProcessing
{
    public SortOrder SortOrder { get; set; }
    public SortByAttribute SortBy { get; set; }
    public SearchCriteria SearchCriteria { get; set; }

    public List<SearchCriteria> SearchCriteriaList { get; set; }
    public bool SearchDisabled { get; set; }
    public string SearchTerm { get; set; }

    public UserDataProcessing()
    {
        SortOrder = SortOrder.Ascending;
        SortBy = SortByAttribute.FirstName;
        SearchCriteria = SearchCriteria.None;
        SearchCriteriaList = [ Services.SearchCriteria.None, Services.SearchCriteria.UserId, Services.SearchCriteria.FirstName, Services.SearchCriteria.LastName,
            Services.SearchCriteria.Email, Services.SearchCriteria.Company ];
        SearchDisabled = true;
        SearchTerm = string.Empty;
    }

    public UserDataProcessing(SortOrder sortOrder, SortByAttribute sortBy)
    {
        SortOrder = sortOrder;
        SortBy = sortBy;
        SearchCriteriaList = [ Services.SearchCriteria.None, Services.SearchCriteria.UserId, Services.SearchCriteria.FirstName, Services.SearchCriteria.LastName,
            Services.SearchCriteria.Email, Services.SearchCriteria.Company ];
        SearchDisabled = true;
        SearchTerm = string.Empty;
    }

    public IEnumerable<User> Filter(IEnumerable<User> users, NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        users = numberOfItemsToDisplay switch
        {
            NumberOfItemsToDisplay.Display05 => users.Take((int)NumberOfItemsToDisplay.Display05),
            NumberOfItemsToDisplay.Display10 => users.Take((int)NumberOfItemsToDisplay.Display10),
            NumberOfItemsToDisplay.Display25 => users.Take((int)NumberOfItemsToDisplay.Display25),
            NumberOfItemsToDisplay.Display50 => users.Take((int)NumberOfItemsToDisplay.Display50),
            NumberOfItemsToDisplay.DisplayAll => users,
            _ => throw new InvalidOperationException("Invalid number of users to display provided.")
        };

        return users;
    }

    public IEnumerable<User> Sort(IEnumerable<User> users)
    {
        users = SortBy switch
        {
            SortByAttribute.UserId => SortOrder == SortOrder.Ascending ? users.OrderBy(users => users.UserId) : users.OrderByDescending(users => users.UserId),
            SortByAttribute.FirstName => SortOrder == SortOrder.Ascending ? users.OrderBy(users => users.FirstName) : users.OrderByDescending(users => users.FirstName),
            SortByAttribute.LastName => SortOrder == SortOrder.Ascending ? users.OrderBy(users => users.LastName) : users.OrderByDescending(users => users.LastName),
            SortByAttribute.Email => SortOrder == SortOrder.Ascending ? users.OrderBy(users => users.Email) : users.OrderByDescending(users => users.Email),
            SortByAttribute.Company => SortOrder == SortOrder.Ascending ? users.OrderBy(users => users.Company.CompanyName) : users.OrderByDescending(users => users.Company.CompanyName),
            _ => throw new InvalidOperationException("Invalid sort order provided.") //users
        };

        return users;
    }

    public void ChangeSortDirection() => SortOrder = SortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

    public IEnumerable<User> Search(IEnumerable<User> users)
    {
        users = SearchCriteria switch
        {
            Services.SearchCriteria.UserId => users.Where(users =>
                users.UserId.ToString().Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)),
            Services.SearchCriteria.FirstName => users.Where(users =>
                users.FirstName.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)),
            Services.SearchCriteria.LastName => users.Where(users =>
                users.LastName.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)),
            Services.SearchCriteria.Email => users.Where(users =>
                users.Email.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)),
            Services.SearchCriteria.Company => users.Where(users =>
                users.Company.CompanyName!.Contains(SearchTerm, StringComparison.CurrentCultureIgnoreCase)),
            _ => throw new InvalidOperationException("Invalid search criteria provided.")
        };

        return users;
    }

    public void SearchCriteriaIsChanged(SearchCriteria selectedItem)
    {
        SearchCriteria = selectedItem; //set search criteria to selected item
        SearchTerm = string.Empty; //clear search input box

        //check if searchable criteria is selected, and enable/disable the search button accordingly
        SearchDisabled = SearchCriteria == SearchCriteria.None ? SearchDisabled = true : SearchDisabled = false;
    }

    public void ResetSearchOptions()
    {
        //clear search input box
        SearchTerm = string.Empty;

        //set search criteria to none
        SearchCriteria = SearchCriteria.None;

        //disable search button
        SearchDisabled = true;
    }
}