using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulatorBL.Enum;

namespace SimulatorBL.Domain
{
    public class NameRecord : Record
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public NameType nameType { get; set; }
        public int Frequency { get; set; }
        public Gender Gender { get; set; }
        public string Country { get; set; }
        public int Year { get; set; }

        public NameRecord(int? id, string name, NameType nameType, int frequency, Gender gender, string country, int year)
        {
            Id = id;
            Name = name;
            this.nameType = nameType;
            Frequency = frequency;
            Gender = gender;
            this.Country = country;
            Year = year;
        }

        public override string? ToString()
        {
            return $"{Name}| {Frequency} | {nameType} | {Country}";
        }
    }
}
