using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulatorUI_sim.DomainUI
{
    internal class MunicipalitySelection
    {
        public string Name { get; set; }
        public int Percentage { get; set; } = 100; // Standard 100%

        public MunicipalitySelection(string name, int percentage)
        {
            Name = name;
            Percentage = percentage;
        }

        public override bool Equals(object? obj)
        {
            return obj is MunicipalitySelection selection &&
                   Name == selection.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
