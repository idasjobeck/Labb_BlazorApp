using System.ComponentModel.DataAnnotations;

namespace Labb_BlazorApp.Components.Pages;

public partial class Users
{
    public class User
    {
        private int _userId;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phoneNumber;
        private string _website;
        private AddressInfo _address;
        private CompanyInfo _company;

        public int UserId { get => _userId; }

        [Required]
        public string FirstName { get => _firstName; set => _firstName = value; }

        [Required]
        public string LastName { get => _lastName; set => _lastName = value; }

        [Required]
        public string Email { get => _email; set => _email = value; }

        public string PhoneNumber { get => _phoneNumber; set => _phoneNumber = value; }

        public string Website { get => _website; set => _website = value; }

        public AddressInfo Address { get; set; }
        public CompanyInfo Company { get; set; }

        public User() //constructor with no parameters
        {
            _userId = 999; //should really generate an UserID, and would think that it should be unique for each user.
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            Website = string.Empty;
            Address = new AddressInfo();
            Company = new CompanyInfo();
        }

        public User(int userId, string firstName, string lastName, string email)
        {
            _userId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Website = string.Empty;
            Address = new AddressInfo();
            Company = new CompanyInfo();
        }

        public User(int userId, string firstName, string lastName, string email, string website, string street, string city, string zipcode, string companyName, string catchphrase)
        {
            _userId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Website = string.Empty;
            Address = new AddressInfo(street, city, zipcode);
            Company = new CompanyInfo(companyName, catchphrase);
        }
    }

    public class AddressInfo
    {
        private string _street;
        private string _city;
        private string _zipCode;

        public string Street { get => _street; set => _street = value; }
        public string City { get => _city; set => _city = value; }
        public string ZipCode { get => _zipCode; set => _zipCode = value; }

        public AddressInfo()
        {
            Street = string.Empty;
            City = string.Empty;
            ZipCode = string.Empty;
        }

        public AddressInfo(string street, string city, string zipCode)
        {
            Street = street;
            City = city;
            ZipCode = zipCode;
        }

    }

    public class CompanyInfo
    {
        private string _companyName;
        private string _catchphrase;
        public string CompanyName { get => _companyName; set => _companyName = value; }
        public string Catchphrase { get => _catchphrase; set => _companyName = value; }

        public CompanyInfo()
        {
            CompanyName = string.Empty;
            Catchphrase = string.Empty;
        }

        public CompanyInfo(string companyName, string catchphrase)
        {
            CompanyName = companyName;
            Catchphrase = catchphrase;
        }
    }
}