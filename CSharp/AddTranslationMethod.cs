using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Translator.Samples
{
    class AddTranslationSample
    {
        public static async Task Run(string authToken)
        {
            string originaltext = "una importante contribuci?n a la rentabilidad de la empresa";
            string translatedtext = "an important contribution to the company profitability";
            string from = "es";
            string to = "en";
            string user = "TestUserId";
            string addTranslationuri = 
                "https://api.microsofttranslator.com/V2/Http.svc/AddTranslation?originaltext=" + originaltext
                + "&translatedtext=" + translatedtext
                + "&from=" + from
                + "&to=" + to
                + "&user=" + user;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(addTranslationuri);
                request.Headers.Add("Authorization", authToken);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Translation for {0} has been added successfully.", originaltext);
                }
                else
                {
                    Console.WriteLine("AddTranslationArray request failed.");
                    Console.WriteLine("  Request status code is: {0}.", response.StatusCode);
                    Console.WriteLine("  Request error message: {0}.", await response.Content.ReadAsStringAsync());
                }
            }
        }
    }
}
