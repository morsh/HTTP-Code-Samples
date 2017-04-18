using System;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Web;

namespace Microsoft.Translator.Samples
{
    class TranslateSample
    {
        public static void Run(string authToken)
        {
            string text = "Use pixels to express measurements for padding and margins.";
            string from = "en";
            string to = "de";
            string uri = "https://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + HttpUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            using (WebResponse response = httpWebRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                DataContractSerializer dcs = new DataContractSerializer(Type.GetType("System.String"));
                string translation = (string)dcs.ReadObject(stream);
                Console.WriteLine("Translation for source text '{0}' from {1} to {2} is", text, "en", "de");
                Console.WriteLine(translation);
            }
        }
    }
}
