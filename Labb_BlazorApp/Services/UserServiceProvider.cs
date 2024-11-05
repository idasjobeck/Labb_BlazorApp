using Labb_BlazorApp.Components.Pages;
using Labb_BlazorApp.Models;
using Labb_BlazorApp.Services;

namespace Labb_BlazorApp.Services;

public static class UserServiceProvider
{
    public static IUserService GetUserService(DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.Api:
                //get users from API
                return new UserServiceApi();
            case DataSource.Memory:
                //get users from memory
                return new UserServiceMemory();
            case DataSource.Csv:
                //get users from csv file
                return new UserServiceCsv();
            default:
                throw new InvalidOperationException("Invalid User Service requested.");
        }
    }
}