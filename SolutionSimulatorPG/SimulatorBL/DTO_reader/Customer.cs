using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;

namespace SimulatorBL.Domain
{
    public class Customer
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public Gender Gender { get; set; }
        public string Street { get; set; }
        public string Municipality { get; set; }
        public string Country { get; set; }
        public string HouseNumber { get; set; }
        public DateTime BirthDate { get; set; }

        public Customer(string name, string lastname, Gender gender, string street, string municipality, string country, string houseNumber, DateTime birthDate)
        {
            Name = name;
            Lastname = lastname;
            Gender = gender;
            Street = street;
            Municipality = municipality;
            Country = country;
            HouseNumber = houseNumber;
            BirthDate = birthDate;
        }
    }
}
