using System.ComponentModel;

namespace Labb_BlazorApp.Services;

public enum DataSource
{
    [Description("API")] Api,
    Memory,
    [Description("CSV")] Csv
}

public class DataSourceOptions
{
    public DataSource DataSource { get; set; }
    public List<DataSource> DataSourceList { get; set; }
    public bool DataSourceDisabled { get; set; }

    public DataSourceOptions()
    {
        DataSource = DataSource.Api;
        DataSourceList = [DataSource.Api, DataSource.Memory, DataSource.Csv];
        DataSourceDisabled = true;
    }

    public DataSourceOptions(DataSource dataSource, List<DataSource> dataSourceList, bool dataSourceDisabled = true)
    {
        DataSource = dataSource;
        DataSourceList = dataSourceList;
        DataSourceDisabled = dataSourceDisabled;
    }
}