namespace Labb_BlazorApp.Services;

public static class UserServiceProvider
{
    public static IUserService GetUserService(DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.Api:
                //provide user service to get users from API
                return new UserServiceApi();
            case DataSource.Memory:
                //provide user service to get users from memory
                return new UserServiceMemory();
            case DataSource.Csv:
                //provide user service to get users from csv file
                return new UserServiceCsv();
            default:
                throw new InvalidOperationException("Invalid User Service requested.");
        }
    }
}