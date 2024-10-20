using System.ComponentModel.DataAnnotations;

namespace Labb_BlazorApp.Components.Pages;

public partial class Users
{
    public class User
    {
        private int _userId;
        
        public int UserId { get => _userId; }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
            set
            {
                var fullName = value.Split(' ');
                FirstName = fullName[0];
                LastName = fullName[1];
            }
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Website { get; set; }

        public Address Address { get; set; }
        public Company Company { get; set; }

        public User() //constructor with no parameters
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
            Website = string.Empty;
            Address = new Address();
            Company = new Company();
        }

        public User(int userId, string firstName, string lastName, string email, string website, string street, string city, string zipcode, string companyName, string catchphrase)
        {
            _userId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Website = string.Empty;
            Address = new Address(street, city, zipcode);
            Company = new Company(companyName, catchphrase);
        }
    }

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

    public class Company
    {
        public string CompanyName { get; set; }
        public string Catchphrase { get; set; }

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
}