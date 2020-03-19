using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxServiceLibrary
{
    public class TaxService
    {
        private readonly ITaxCalculator _taxCalculator;
        
        // this is what will allow you to inject a specific calc.
        public TaxService(ITaxCalculator taxCalculator)
        {
            _taxCalculator = taxCalculator;
        }

        // this could definitely be async
        public decimal TaxRateByLocation(string zip, object body)
        {
            return _taxCalculator.TaxRate(zip, body);
        }

        // this could be async too
        public decimal CalculateTaxForOrder(object body)
        {
            return _taxCalculator.CalculateTax(body);
        }
    }
}