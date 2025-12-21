using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

public class Traveler
{
    public string Id { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }
    public Contact Contact { get; set; }
    public Name Name { get; set; }
}

public class Contact
{
    public string EmailAddress { get; set; }
    public Phone[] Phones { get; set; }
}

public class Phone
{
    public string DeviceType { get; set; }
    public string CountryCallingCode { get; set; }
    public string Number { get; set; }
}

public class Name
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}