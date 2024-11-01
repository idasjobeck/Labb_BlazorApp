using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Labb_BlazorApp.Components.Pages;

namespace Labb_BlazorApp.Models;

public class User
{
    private int _userId;

    [JsonPropertyName("Id")]
    public int UserId
    {
        get => _userId;
        set => _userId = value;
    }

    [JsonPropertyName("Name")]
    public string FullName
    {
        get => $"{FirstName} {LastName}";
        set
        {
            var fullName = value.Split(' ');
            FirstName = fullName[0];
            LastName = fullName[1];
        }
    }

    [Required(ErrorMessage = "The first name field is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "The last name field is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "The email field is required.")]
    [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "The email entered must be in a valid format.")]
    public string Email { get; set; }

    [JsonPropertyName("Phone")]
    public string PhoneNumber { get; set; }

    public string Website { get; set; }

    public Address Address { get; set; }
    public Company Company { get; set; }

    public User()
    {
        _userId = 999; //should really generate an UserID, and would think that it should be unique for each user.
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
        Website = string.Empty;
        Address = new Address();
        Company = new Company();
    }

    public User(int userId, string firstName, string lastName, string email)
    {
        _userId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = string.Empty;
        Website = string.Empty;
        Address = new Address();
        Company = new Company();
    }

    public User(int userId, string firstName, string lastName, string email, string phoneNumber, string website, string street, string city, string zipcode, string companyName, string catchphrase)
    {
        _userId = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Website = website;
        Address = new Address(street, city, zipcode);
        Company = new Company(companyName, catchphrase);
    }
}