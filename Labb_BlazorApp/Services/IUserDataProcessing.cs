using Labb_BlazorApp.Components.Pages;
using Labb_BlazorApp.Models;

namespace Labb_BlazorApp.Services;

public interface IUserDataProcessing
{
    public IEnumerable<User> Filter(IEnumerable<User> users, NumberOfItemsToDisplay numberOfItemsToDisplay);
    public IEnumerable<User> Sort(IEnumerable<User> users, SortByAttribute sortBy, SortOrder sortOrder);
    public IEnumerable<User> Search(IEnumerable<User> users, SearchCriteria searchCriteria, string searchTerm);
}