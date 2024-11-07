using System.ComponentModel;

namespace Labb_BlazorApp.Services;

public enum NumberOfItemsToDisplay
{
    [Description("All")] DisplayAll = 0,
    Display05 = 5,
    Display10 = 10,
    Display25 = 25,
    Display50 = 50
}

public class DisplayOptions
{
    public NumberOfItemsToDisplay NumberOfItemsToDisplay { get; set; }
    public List<NumberOfItemsToDisplay> NumberOfItemsToDisplayList { get; set; }
    public DataSourceOptions DataSourceOptions { get; set; }
    public Messages Messages { get; set; }
    public bool DisplayLoadingMessage { get; set; }
    public bool DisplayErrorMessage { get; set; }
    public string ExceptionMessage { get; set; }

    public DisplayOptions()
    {
        NumberOfItemsToDisplay = NumberOfItemsToDisplay.Display05;
        NumberOfItemsToDisplayList = [NumberOfItemsToDisplay.Display05, NumberOfItemsToDisplay.Display10, NumberOfItemsToDisplay.Display25, NumberOfItemsToDisplay.Display50, NumberOfItemsToDisplay.DisplayAll];
        DataSourceOptions = new DataSourceOptions();
        Messages = new Messages();
        DisplayLoadingMessage = true;
        DisplayErrorMessage = false;
        ExceptionMessage = string.Empty;
    }

    public DisplayOptions(NumberOfItemsToDisplay numberOfItemsToDisplay, List<NumberOfItemsToDisplay> numberOfItemsToDisplayList, bool displayLoadingMessage = true, bool displayErrorMessage = false)
    {
        NumberOfItemsToDisplay = numberOfItemsToDisplay;
        NumberOfItemsToDisplayList = numberOfItemsToDisplayList;
        DataSourceOptions = new DataSourceOptions();
        Messages = new Messages();
        DisplayLoadingMessage = displayLoadingMessage;
        DisplayErrorMessage = displayErrorMessage;
        ExceptionMessage = string.Empty;
    }

    public DisplayOptions(NumberOfItemsToDisplay numberOfItemsToDisplay, List<NumberOfItemsToDisplay> numberOfItemsToDisplayList, DataSource dataSource, List<DataSource> dataSourceList, bool dataSourceDisabled, string loading, string loadingError, string allowOtherDataSourceOnLoadingError, bool displayLoadingMessage = true, bool displayErrorMessage = false)
    {
        NumberOfItemsToDisplay = numberOfItemsToDisplay;
        NumberOfItemsToDisplayList = numberOfItemsToDisplayList;
        DataSourceOptions = new DataSourceOptions(dataSource, dataSourceList, dataSourceDisabled);
        Messages = new Messages(loading, loadingError, allowOtherDataSourceOnLoadingError);
        DisplayLoadingMessage = displayLoadingMessage;
        DisplayErrorMessage = displayErrorMessage;
        ExceptionMessage = string.Empty;
    }

    public void ResetExceptionSettings()
    {
        DisplayLoadingMessage = true;
        DisplayErrorMessage = false;
        ExceptionMessage = string.Empty;
    }

    public void SetToDisplayException(string exceptionMessage)
    {
        DisplayLoadingMessage = false;
        DisplayErrorMessage = true;
        ExceptionMessage = exceptionMessage;
    }

    public void SetToDisplayException()
    {
        DisplayLoadingMessage = false;
        DisplayErrorMessage = true;
    }
}