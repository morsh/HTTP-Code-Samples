using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Translator.Samples
{
    class AddTranslationArraySample
    {
        public static async Task Run(string authToken)
        {
            string appId = "";
            string uri = "https://api.microsofttranslator.com/v2/Http.svc/AddTranslationArray";
            string originalText1 = "una importante contribución a la rentabilidad de la empresa";
            string translatedText1 = "a significant contribution tothe company profitability";
            string originalText2 = "a veces los errores son divertidos";
            string translatedText2 = "in some cases errors are fun";

            string body = GenerateAddtranslationRequestBody(appId, "es", "en", "general", "text/plain", "", "TestUserId");
            string translationsCollection = string.Format("{0}{1}",
                GenerateAddtranslationRequestElement(originalText1, 8, 0, translatedText1),
                GenerateAddtranslationRequestElement(originalText2, 6, 0, translatedText2));
            string requestBody = string.Format(body, translationsCollection);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(uri);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "text/xml");
                request.Headers.Add("Authorization", authToken);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Your translations for '{0}' and '{1}' has been added successfully.", originalText1, originalText2);
                }
                else
                {
                    Console.WriteLine("AddTranslationArray request failed.");
                    Console.WriteLine("  Request status code is: {0}.", response.StatusCode);
                    Console.WriteLine("  Request error message: {0}.", await response.Content.ReadAsStringAsync());
                }
            }
        }

        private static string GenerateAddtranslationRequestBody(string appId, string from, string to, string category, string contentType, string uri, string user)
        {
            string body = "<AddtranslationsRequest>" +
                             "<AppId>{0}</AppId>" +
                             "<From>{1}</From>" +
                             "<Options>" +
                               "<Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{2}</Category>" +
                               "<ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{3}</ContentType>" +
                               "<User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{4}</User>" +
                               "<Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{5}</Uri>" +
                             "</Options>" +
                             "<To>{6}</To>" +
                             "<Translations>{7}</Translations>" +
                           "</AddtranslationsRequest>";
            return string.Format(body, appId, from, category, contentType, user, uri, to, "{0}");
        }

        private static string GenerateAddtranslationRequestElement(string originalText, int rating, int sequence, string translatedText)
        {
            string element = "<Translation xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">" +
                "<OriginalText>{0}</OriginalText>" +
                "<Rating>{1}</Rating>" +
                "<TranslatedText>{2}</TranslatedText>" +
                "<Sequence>{3}</Sequence>" +
                "</Translation>";
            return string.Format(element, originalText, rating.ToString(), translatedText, sequence.ToString());
        }
    }
}
