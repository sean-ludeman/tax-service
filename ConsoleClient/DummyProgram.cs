using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TaxServiceLibrary;

namespace ConsoleClient
{
    // This is just a quick and dirty sample program that makes use of the TaxService
    // and shows how you could pass some kind of ITaxCalculator in and use the methods
    // in the interface. This would allow you to create other tax calculators
    // other than TaxJar as long as they implement that ITaxCalculator interface.
    public class DummyProgram
    {
        private const string Token = "7715c49edd09a29e8e4b18d5cc4c4867";
        private const string BaseAddress = "https://api.taxjar.com/v2/";

        static void Main(string[] args)
        {
            // probably a good idea to put token in a config file
            // base address could also be there
            // api version, etc.
            var service = new TaxService(new TaxJarCalculator(Token, BaseAddress, 0));

            // Could also use the zip and leave the body out of the call but
            // in this case, we just use the request body.
            string zip = null;

            // Example request .
            object ratesRequestBody =
                new
                {
                    country = "US",
                    zip = "32207"
                };

            var rate = service.TaxRateByLocation(zip, ratesRequestBody);

            Console.WriteLine($"rate: { rate }");

            // Example request pulled off the TaxJar docs.
            object taxRequestBody = new
            {
                from_country = "US",
                from_zip = "92093",
                from_state = "CA",
                from_city = "La Jolla",
                from_street = "9500 Gilman Drive",
                to_country = "US",
                to_zip = "90002",
                to_state = "CA",
                to_city = "Los Angeles",
                to_street = "1335 E 103rd St",
                amount = 15,
                shipping = 1.5,
                nexus_addresses = new[] {
                        new {
                          id = "Main Location",
                          country = "US",
                          zip = "92093",
                          state = "CA",
                          city = "La Jolla",
                          street = "9500 Gilman Drive",
                        }
                    },
                line_items = new[] {
                        new {
                          id = "1",
                          quantity = 1,
                          product_tax_code = "20010",
                          unit_price = 15,
                          discount = 0
                        }
                    }
            };

            decimal tax = service.CalculateTaxForOrder(taxRequestBody);

            Console.WriteLine($"tax: { tax }");
            Console.ReadLine();
        }
    }
}
