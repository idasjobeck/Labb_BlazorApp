using Labb_BlazorApp.Components.Pages;
using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Services;

public class DataProcessing : IUserDataProcessing
{
    public IEnumerable<User> Filter(IEnumerable<User> users, NumberOfItemsToDisplay numberOfItemsToDisplay)
    {
        users = numberOfItemsToDisplay switch
        {
            NumberOfItemsToDisplay.Display05 => users.Take((int)NumberOfItemsToDisplay.Display05),
            NumberOfItemsToDisplay.Display10 => users.Take((int)NumberOfItemsToDisplay.Display10),
            NumberOfItemsToDisplay.Display25 => users.Take((int)NumberOfItemsToDisplay.Display25),
            NumberOfItemsToDisplay.Display50 => users.Take((int)NumberOfItemsToDisplay.Display50),
            NumberOfItemsToDisplay.DisplayAll => users,
            _ => users
        };

        return users;
    }

    public IEnumerable<User> Sort(IEnumerable<User> users, SortByAttribute sortBy, SortOrder sortOrder)
    {
        if (sortOrder == SortOrder.Ascending)
        {
            users = sortBy switch
            {
                SortByAttribute.UserId => users.OrderBy(users => users.UserId),
                SortByAttribute.FirstName => users.OrderBy(users => users.FirstName),
                SortByAttribute.LastName => users.OrderBy(users => users.LastName),
                SortByAttribute.Email => users.OrderBy(users => users.Email),
                SortByAttribute.Company => users.OrderBy(users => users.Company.CompanyName),
                _ => users
            };
        }
        else if (sortOrder == SortOrder.Descending)
        {
            users = sortBy switch
            {
                SortByAttribute.UserId => users.OrderByDescending(users => users.UserId),
                SortByAttribute.FirstName => users.OrderByDescending(users => users.FirstName),
                SortByAttribute.LastName => users.OrderByDescending(users => users.LastName),
                SortByAttribute.Email => users.OrderByDescending(users => users.Email),
                SortByAttribute.Company => users.OrderByDescending(users => users.Company.CompanyName),
                _ => users
            };
        }

        return users;
    }

    public IEnumerable<User> Search(IEnumerable<User> users, SearchCriteria searchCriteria, string searchTerm)
    {
        users = searchCriteria switch
        {
            SearchCriteria.UserId => users.Where(users =>
                users.UserId.ToString().Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)),
            SearchCriteria.FirstName => users.Where(users =>
                users.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)),
            SearchCriteria.LastName => users.Where(users =>
                users.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)),
            SearchCriteria.Email => users.Where(users =>
                users.Email.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)),
            SearchCriteria.Company => users.Where(users =>
                users.Company.CompanyName!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)),
            _ => users
        };

        return users;
    }
}