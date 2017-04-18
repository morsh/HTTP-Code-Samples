using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.Translator.Samples
{
    class GetTranslationsArraySample
    {
        public static void Run(string authToken)
        {
            string uri = "https://api.microsofttranslator.com/v2/Http.svc/GetTranslationsArray";
            string requestBody = string.Format(
                "<GetTranslationsArrayRequest>" +
                "  <AppId>{0}</AppId>" +
                "  <From>{1}</From>" +
                "  <Options>" +
                "  <Category xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">general</Category>" +
                "  <ContentType xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">text/plain</ContentType>" +
                "  <ReservedFlags xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\"/>" +
                "  <State xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\"/>" +
                "  <Uri xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{2}</Uri>" +
                "  <User xmlns=\"http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2\">{3}</User>" +
                "  </Options>" +
                "  <Texts>{6}</Texts>" +
                "  <To>{4}</To>" +
                "  <MaxTranslations>{5}</MaxTranslations>" +
                "</GetTranslationsArrayRequest>", "", "es", "", "TestUserId", "en", "3", "{0}");

            string translationsCollection = string.Empty;
            string[] textTranslations =
            {
                "a veces los errores son divertidos",
                " una importante contribución a la rentabilidad de la empresa"
            };
            // build the Translations collection
            translationsCollection += string.Format("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", textTranslations[0]);
            translationsCollection += string.Format("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">{0}</string>", textTranslations[1]);
            // update the body
            requestBody = string.Format(requestBody, translationsCollection);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "text/xml";
            request.Headers.Add("Authorization", authToken);
            request.Method = "POST";
            using (Stream stream = request.GetRequestStream())
            {
                byte[] arrBytes = Encoding.ASCII.GetBytes(requestBody);
                stream.Write(arrBytes, 0, arrBytes.Length);
            }
            using (WebResponse response = request.GetResponse())
            using (Stream respStream = response.GetResponseStream())
            {
                StreamReader rdr = new StreamReader(respStream, Encoding.ASCII);
                string strResponse = rdr.ReadToEnd();
                XDocument doc = XDocument.Parse(@strResponse);
                XNamespace ns = "http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2";
                int i = 0;
                foreach (XElement xe in doc.Descendants(ns + "GetTranslationsResponse"))
                {
                    Console.WriteLine("\n\nSource Text: '{0}' Results", textTranslations[i++]);
                    int j = 1;
                    foreach (XElement xe2 in xe.Descendants(ns + "TranslationMatch"))
                    {
                        Console.WriteLine("\nCustom translation :{0} ", j++);
                        foreach (var node in xe2.Elements())
                        {
                            Console.WriteLine("{0} = {1}", node.Name.LocalName, node.Value);
                        }
                    }
                }
            }
        }
    }
}
