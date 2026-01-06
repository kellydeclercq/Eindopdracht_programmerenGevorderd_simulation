using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;

namespace SimulatorBL.Domain
{
    public class AddressRecord : Record
    {
        public int? Id { get; set; }
        public string? Municipality { get; set; }
        public string? Street { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }

        public AddressRecord(int? id, string municipality, string street, string country, int year)
        {
            Id = id;
            Municipality = municipality;
            Street = street;
            Country = country;
            Year = year;
        }

        public override bool Equals(object? obj)
        {
            return obj is AddressRecord record &&
                   Municipality == record.Municipality &&
                   Street == record.Street &&
                   Country == record.Country &&
                   Year == record.Year;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Municipality, Street, Country, Year);
        }
    }
}
