using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using TaxServiceLibrary;

namespace ClientUnitTests
{
    [TestClass]
    public class TaxJarCalculatorTests
    {
        const string TestToken = "7715c49edd09a29e8e4b18d5cc4c4867";
        const string TestBaseAddress = "https://api.taxjar.com/v2/";
        const int TestTimeout = 0;
        const string TestZip = "32207";
        const decimal TestTax = 1.43M;
        const decimal TestRate = 0.07M;
        static object TestRateBody => new
        {
            country = "US",
            zip = "32207"
        };
        static object TestTaxBody => new
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

        [TestMethod]
        public void Constructor_GivenDefaultArgs_ShouldCreateObject()
        {
            // Arrange

            // Act
            var calculator = new TaxJarCalculator();

            // Assert
            ConstructorCheck(calculator);
        }

        // should return rate
        // use zip
        // decimal TaxRate(string zip, object body = null)
        [TestMethod]
        public void Constructor_GivenValidArgs_ShouldReturnRate()
        {
            // Arrange

            // Act
            var calculator = new TaxJarCalculator(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsInRange(0, int.MaxValue, Range.Inclusive));

            // Assert
            ConstructorCheck(calculator);
        }

        // This is more of an api test since I didn't pull the request
        // and Execute() parts out for better testability.
        // It could definitely be done, it just would have taken a bit
        // longer to figure out the best way to approach that.
        //
        // decimal TaxRate(string zip, object body = null)
        [TestMethod]
        public void TaxRate_GivenZip_ShouldReturnRate()
        {
            // Arrange

            // Act
            var calculator = new TaxJarCalculator(TestToken, TestBaseAddress, TestTimeout);
            ConstructorCheck(calculator);
            var rate = calculator.TaxRate(TestZip);

            // Assert
            AssertRate(rate);
        }

        [TestMethod]
        public void TaxRate_GivenBody_ShouldReturnRate()
        {
            // Arrange

            // Act
            var calculator = new TaxJarCalculator(TestToken, TestBaseAddress, TestTimeout);
            ConstructorCheck(calculator);
            var rate = calculator.TaxRate(null, TestRateBody);

            // Assert
            AssertRate(rate);
        }


        // should return tax
        // use body
        // decimal CalculateTax(object body)
        [TestMethod]
        public void CalculateTax_GivenBody_ShouldReturnTax()
        {
            // Arrange

            // Act
            var calculator = new TaxJarCalculator(TestToken, TestBaseAddress, TestTimeout);
            ConstructorCheck(calculator);
            var tax = calculator.CalculateTax(TestTaxBody);

            // Assert
            AssertTax(tax);
        }

        // Test helpers:

        private void ConstructorCheck(object calculator)
        {
            Assert.IsNotNull(calculator);
            Assert.IsInstanceOfType(calculator, typeof(ITaxCalculator));
            Assert.IsInstanceOfType(calculator, typeof(TaxJarCalculator));
        }
        private void AssertRate(object rate)
        {
            Assert.IsNotNull(rate);
            Assert.IsInstanceOfType(rate, typeof(decimal));
            Assert.AreEqual(TestRate, rate);
        }
        private void AssertTax(object tax)
        {
            Assert.IsNotNull(tax);
            Assert.IsInstanceOfType(tax, typeof(decimal));
            Assert.AreEqual(TestTax, tax);
        }
    }
}
