namespace Labb_BlazorApp.Models;

public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }

    public Address()
    {
        Street = string.Empty;
        City = string.Empty;
        ZipCode = string.Empty;
    }

    public Address(string street, string city, string zipCode)
    {
        Street = street;
        City = city;
        ZipCode = zipCode;
    }

}