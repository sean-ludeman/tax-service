using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
using TaxServiceLibrary;

namespace ClientUnitTests
{
    // need some constants for the tests
    // need moq to mock the TaxCalculator class

    // I'm not sure what you guys use for api unit testing.
    // MSTest and Moq are what I usually use but I understand that
    // this might not be what is typically used to tests apis.
    // 
    // I'm not too familiar with WireMock yet, so
    // here I'm sticking to what I know.
    [TestClass]
    public class TaxServiceTests
    {
        const string TestZip = "32207";
        const decimal TestTax = 1.50M;
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
        ITaxCalculator Calculator;

        // need some constructor tests
        [TestMethod]
        public void TaxService_GivenValidArgs_ShouldCreateObject()
        {
            // Arrange
            var calculator = Mock.Of<ITaxCalculator>();

            // Act
            var service = new TaxService(calculator);

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TaxService));
        }

        // should return rate
        // use zip
        // decimal TaxRateByLocation(string zip, object body)
        [TestMethod]
        public void TaxRateByLocation_GivenZip_ShouldReturnTaxRate()
        {
            // Arrange

            // Act
            var service = new TaxService(Calculator);
            ConstructorCheck(service);
            var rate = service.TaxRateByLocation(TestZip, null);

            // Assert
            Assert.IsInstanceOfType(rate, typeof(decimal));
            Assert.IsTrue(rate == TestRate);
        }

        // should return rate
        // use body
        // decimal TaxRateByLocation(string zip, object body)
        [TestMethod]
        public void TaxRateByLocation_GivenBody_ShouldReturnTaxRate()
        {
            // Arrange

            // Act
            var service = new TaxService(Calculator);
            ConstructorCheck(service);
            var rate = service.TaxRateByLocation(null, TestRateBody);

            // Assert
            RateCheck(rate);
        }

        // should return tax
        // use body
        // decimal CalculateTaxForOrder(object body)
        [TestMethod]
        public void CalculateTaxForOrder_GivenBody_ShouldReturnCollectedTax()
        {
            // Arrange

            // Act
            var service = new TaxService(Calculator);
            ConstructorCheck(service);
            var tax = service.CalculateTaxForOrder(TestTaxBody);

            // Assert
            TaxCheck(tax);
        }

        // Test helpers
        [TestInitialize]
        public void Init()
        {
            var calculatorMock = new Mock<ITaxCalculator>();
            calculatorMock
                .Setup(x => x.TaxRate(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(TestRate);
            calculatorMock
                .Setup(x => x.CalculateTax(It.IsAny<object>()))
                .Returns(TestTax);
            Calculator = calculatorMock.Object;
        }

        private void ConstructorCheck(object service)
        {
            Assert.IsNotNull(service);
            Assert.IsInstanceOfType(service, typeof(TaxService));
        }

        private void RateCheck(object rate)
        {
            Assert.IsInstanceOfType(rate, typeof(decimal));
            Assert.IsTrue((decimal)rate == TestRate);
        }

        private void TaxCheck(object tax)
        {
            Assert.IsInstanceOfType(tax, typeof(decimal));
            Assert.IsTrue((decimal)tax == TestTax);
        }
    }
}
