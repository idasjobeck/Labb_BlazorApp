using System.Text.Json.Serialization;

namespace Labb_BlazorApp.Models;

public class Company
{
    [JsonPropertyName("Name")]
    public string? CompanyName { get; set; }
    public string? Catchphrase { get; set; }

    public Company()
    {
        CompanyName = string.Empty;
        Catchphrase = string.Empty;
    }

    public Company(string companyName, string catchphrase)
    {
        CompanyName = companyName;
        Catchphrase = catchphrase;
    }
}