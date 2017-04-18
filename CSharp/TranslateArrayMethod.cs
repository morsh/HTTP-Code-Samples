using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Translator.Samples
{
    class TranslateArraySample
    {
        public static async Task Run(string authToken)
        {
            var from = "en";
            var to = "es";
            var translateArraySourceTexts = new []
            {
                "The answer lies in machine translation.",
                "the best machine translation technology cannot always provide translations tailored to a site or users like a human ",
                "Simply copy and paste a code snippet anywhere "
            };
            var uri = "https://api.microsofttranslator.com/v2/Http.svc/TranslateArray";
            var body = "<TranslateArrayRequest>" +
                           "<AppId />" +
                           "<From>{0}</From>" +
                           "<Options>" +
                           " <Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                               "<ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{1}</ContentType>" +
                               "<ReservedFlags xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                               "<State xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                               "<Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                               "<User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\" />" +
                           "</Options>" +
                           "<Texts>" +
                               "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{2}</string>" +
                               "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{3}</string>" +
                               "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{4}</string>" +
                           "</Texts>" +
                           "<To>{5}</To>" +
                       "</TranslateArrayRequest>";
            string requestBody = string.Format(body, from, "text/plain", translateArraySourceTexts[0], translateArraySourceTexts[1], translateArraySourceTexts[2], to);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "text/xml");
                request.Headers.Add("Authorization", authToken);
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Console.WriteLine("Request status is OK. Result of translate array method is:");
                        var doc = XDocument.Parse(responseBody);
                        var ns = XNamespace.Get("http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2");
                        var sourceTextCounter = 0;
                        foreach (XElement xe in doc.Descendants(ns + "TranslateArrayResponse"))
                        {
                            foreach (var node in xe.Elements(ns + "TranslatedText"))
                            {
                                Console.WriteLine("\n\nSource text: {0}\nTranslated Text: {1}", translateArraySourceTexts[sourceTextCounter], node.Value);
                            }
                            sourceTextCounter++;
                        }
                        break;
                    default:
                        Console.WriteLine("Request status code is: {0}.", response.StatusCode);
                        Console.WriteLine("Request error message: {0}.", responseBody);
                        break;
                }
            }
        }
    }
}
