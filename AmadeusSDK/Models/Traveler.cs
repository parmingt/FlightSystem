using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

public class Traveler
{
    public string id { get; set; }
    public string dateOfBirth { get; set; }
    public string gender { get; set; }
    public Contact contact { get; set; }
    public Name name { get; set; }
}

public class Contact
{
    public string emailAddress { get; set; }
    public Phone[] phones { get; set; }
}

public class Phone
{
    public string deviceType { get; set; }
    public string countryCallingCode { get; set; }
    public string number { get; set; }
}

public class Name
{
    public string firstName { get; set; }
    public string lastName { get; set; }
}