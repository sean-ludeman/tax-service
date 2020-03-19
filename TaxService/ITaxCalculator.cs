using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxServiceLibrary
{
    public interface ITaxCalculator
    {
        // I really feel like this should be generic but idk...
        decimal TaxRate(string zip, object body);
        decimal CalculateTax(object body);
    }
}
