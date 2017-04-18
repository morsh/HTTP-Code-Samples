using System;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Translator.Samples
{
    class DetectArraySample
    {
        public static async Task Run(string authToken)
        {
            var textArray = new []
            {
                "les erreurs sont parfois amusantes",
                "you can try to enter a longer phrase"
            };

            string requestBody;
            using (var swriter = new StringWriter())
            using (var xwriter = new XmlTextWriter(swriter))
            {
                xwriter.WriteStartElement("ArrayOfstring");
                xwriter.WriteAttributeString("xmlns", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
                foreach (string text in textArray)
                {
                    xwriter.WriteStartElement("string");
                    xwriter.WriteString(text);
                    xwriter.WriteEndElement();
                }
                xwriter.WriteEndElement();
                requestBody = swriter.ToString();
            }

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri("https://api.microsofttranslator.com/v2/Http.svc/DetectArray");
                request.Content = new StringContent(requestBody, Encoding.UTF8, "text/xml");
                request.Headers.Add("Authorization", authToken);
                var response = await client.SendAsync(request);
                var s = await response.Content.ReadAsStringAsync();
                Console.WriteLine(s);
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var dcs = new DataContractSerializer(typeof(string[]));
                    var detectArray = (string[]) dcs.ReadObject(stream);
                    Console.WriteLine("The detected languages are: ");
                    for (int i = 0; i < detectArray.Length; i++)
                    {
                        Console.WriteLine("Text '{0}' is from Language {1}", textArray[i], detectArray[i]);
                    }
                }
            }
        }
    }
}
