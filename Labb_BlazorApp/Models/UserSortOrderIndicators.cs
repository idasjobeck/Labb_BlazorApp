using Labb_BlazorApp.Components.Pages;
using Labb_BlazorApp.Services;

namespace Labb_BlazorApp.Models;

public class UserSortOrderIndicators
{
    public string? SortOrderIndicatorUserId { get; set; }
    public string? SortOrderIndicatorFirstName { get; set; }
    public string? SortOrderIndicatorLastName { get; set; }
    public string? SortOrderIndicatorEmail { get; set; }
    public string? SortOrderIndicatorCompanyName { get; set; }

    private readonly string _sortOrderIndicatorNotSortedDblArrow = "<i class=\"fa-solid fa-sort\"></i>";
    private readonly string _sortOrderIndicatorAscendingAZ = "<i class=\"fa-solid fa-arrow-down-a-z\"></i>";
    private readonly string _sortOrderIndicatorDescendingZA = "<i class=\"fa-solid fa-arrow-up-z-a\"></i>";
    private readonly string _sortOrderIndicatorAscending19 = "<i class=\"fa-solid fa-arrow-down-1-9\"></i>";
    private readonly string _sortOrderIndicatorDescending91 = "<i class=\"fa-solid fa-arrow-up-9-1\"></i>";

    public UserSortOrderIndicators()
    {
        SetAllToNotSorted();
    }

    public void SetSortOrderIndicator(SortOrder sortOrder, SortByAttribute sortBy)
    {
        SetAllToNotSorted();

        switch (sortBy)
        {
            case SortByAttribute.UserId:
                SortOrderIndicatorUserId = sortOrder == SortOrder.Ascending ? _sortOrderIndicatorAscending19 : _sortOrderIndicatorDescending91;
                break;
            case SortByAttribute.FirstName:
                SortOrderIndicatorFirstName = sortOrder == SortOrder.Ascending ? _sortOrderIndicatorAscendingAZ : _sortOrderIndicatorDescendingZA;
                break;
            case SortByAttribute.LastName:
                SortOrderIndicatorLastName = sortOrder == SortOrder.Ascending ? _sortOrderIndicatorAscendingAZ : _sortOrderIndicatorDescendingZA;
                break;
            case SortByAttribute.Email:
                SortOrderIndicatorEmail = sortOrder == SortOrder.Ascending ? _sortOrderIndicatorAscendingAZ : _sortOrderIndicatorDescendingZA;
                break;
            case SortByAttribute.Company:
                SortOrderIndicatorCompanyName = sortOrder == SortOrder.Ascending ? _sortOrderIndicatorAscendingAZ : _sortOrderIndicatorDescendingZA;
                break;
            default:
                break;
        }
    }

    public void SetAllToNotSorted()
    {
        SortOrderIndicatorUserId = _sortOrderIndicatorNotSortedDblArrow;
        SortOrderIndicatorFirstName = _sortOrderIndicatorNotSortedDblArrow;
        SortOrderIndicatorLastName = _sortOrderIndicatorNotSortedDblArrow;
        SortOrderIndicatorEmail = _sortOrderIndicatorNotSortedDblArrow;
        SortOrderIndicatorCompanyName = _sortOrderIndicatorNotSortedDblArrow;
    }
}