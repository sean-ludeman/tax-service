using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TaxServiceLibrary
{
    // This could be swapped out with some other api that you use.
    // You'd just create your own SomeApiCalculator, and implement
    // the ITaxCalculator interface.
    // 
    // I think the next step would be to make this more generic to 
    // allow for different request/response models but it was becoming 
    // pretty complicated so I just made the interface assuming 
    // other TaxCalulators would have similar request parameters and
    // they would just have their own models for the response. 
    public class TaxJarCalculator : ITaxCalculator
    {
        public string BaseAddress = "https://api.taxjar.com/v2/";
        public string Token { get; set; } = "TOKEN";    // Need to use your own token here,
                                                        // or pass it in the constructor.
        public int Timeout { get; set; } = 0; // Milliseconds
        internal RestClient _client;

        public TaxJarCalculator()
        {
            _client = new RestClient(BaseAddress);
        }

        public TaxJarCalculator(string token, string baseAddress, int timeout)
        {
            Token = token;
            BaseAddress = baseAddress;
            Timeout = timeout;
            _client = new RestClient(BaseAddress);
        }

        public decimal TaxRate(string zip, object body = null)
        {
            if (zip == null && body == null)
            {
                throw new ArgumentNullException($"Must either have a { nameof(zip) } or { nameof(body) }");
            }

            string url = body != null ? $"{ BaseAddress }rates" : $"{ BaseAddress }rates/{ zip }";

            var request = new RestRequest(url, Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", $"Bearer {Token}");
            request.Timeout = Timeout;

            // request body can't be added as json for a GET, so we add the props as parameters in th url.
            if (body != null)
            {
                // might be a better way to handle this...
                foreach (var prop in body.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    request.AddQueryParameter(prop.Name, prop.GetValue(body).ToString());
                }
            }

            var response = _client.Execute(request);

            if ((int)response.StatusCode >= 400)
            {
                throw new Exception(response.Content);
            }

            if (response.ErrorException != null)
            {
                throw new Exception($"{ response.ErrorMessage } - { response.ErrorException }");
            }

            var rateModel = JsonConvert.DeserializeObject<RateResponseModel>(response.Content);

            // Again, assuming this is what needs to be returned but should be able to pull whatever
            // you need here.
            return rateModel.Rate.CombinedRate;
        }

        // Could probably pull out the Execute(request) and make it generic for the response type.
        // T Execute<T>(object request) or something like that.
        // 
        // Could do this for the TaxRate method as well..
        //
        // This is another place where it could be async, just depends on implementation.
        public decimal CalculateTax(object body)
        {
            if (body == null)
            {
                throw new ArgumentNullException($"{ nameof(body) } cannot be null.");
            }

            string url = $"{ BaseAddress }taxes";

            var request = new RestRequest(url, Method.POST, DataFormat.Json);
            request.AddHeader("Authorization", $"Bearer {Token}");
            request.Timeout = Timeout;

            request.AddJsonBody(body);

            var response = _client.Execute(request);

            if ((int)response.StatusCode >= 400)
            {
                throw new Exception(response.Content);
            }

            if (response.ErrorException != null)
            {
                throw new Exception($"{ response.ErrorMessage } - { response.ErrorException }");
            }

            var taxModel = JsonConvert.DeserializeObject<TaxResponseModel>(response.Content);

            // I'm assuming this is what needs to be returned for TaxJar's implementation.
            return taxModel.Tax.AmountToCollect;
        }
    }
}
