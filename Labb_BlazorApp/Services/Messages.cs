namespace Labb_BlazorApp.Services;

public class Messages
{
    public string Loading { get; }
    public string LoadingError { get; }
    public string AllowOtherDataSourceOnLoadingError { get; }

    public Messages()
    {
        Loading = "Loading...";
        LoadingError = "Loading user data unsuccessful.";
        AllowOtherDataSourceOnLoadingError = "Please select an alternative data source from the drop-down in the top right-hand corner.";
    }

    public Messages(string loading, string loadingError, string allowOtherDataSourceOnLoadingError)
    {
        Loading = loading;
        LoadingError = loadingError;
        AllowOtherDataSourceOnLoadingError = allowOtherDataSourceOnLoadingError;
    }
}