using System;
using System.Net;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.Translator.Samples
{
    class GetLanguageNamesSample
    {
        public static void Run(string authToken)
        {
            // Language codes for which to retrieve language names.
            string[] languageCodes = { "en", "fr", "uk" };

            string uri = "https://api.microsofttranslator.com/v2/Http.svc/GetLanguageNames?locale=en";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers.Add("Authorization", authToken);
            request.ContentType = "text/xml";
            request.Method = "POST";
            DataContractSerializer dcs = new DataContractSerializer(Type.GetType("System.String[]"));
            using (Stream stream = request.GetRequestStream())
            {
                dcs.WriteObject(stream, languageCodes);
            }
            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                string[] languageNames = (string[])dcs.ReadObject(stream);

                Console.WriteLine("Language codes = Language name");
                for (int i = 0; i < languageNames.Length; i++)
                {
                    Console.WriteLine("      {0}       = {1}", languageCodes[i], languageNames[i]);
                }
            }
        }
    }
}
