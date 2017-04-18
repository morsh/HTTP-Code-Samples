using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace Microsoft.Translator.Samples
{
    class BreakSentencesSample
    {
        public static void Run(string authToken)
        {
            string text = "Use the Microsoft Translator webpage widget to deliver your site in the visitor’s language. The visitor never leaves your site, and the widget seamlessly translates each page as they navigate.";
            string uri = "https://api.microsofttranslator.com/v2/Http.svc/BreakSentences?text=" + text + "&language=en";
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", authToken);
            using (WebResponse response = httpWebRequest.GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<int>));
                List<int> result = (List<int>)dcs.ReadObject(stream);
                Console.WriteLine(text);
                Console.WriteLine("BreakSentences broke up the above sentence into " + result.Count + " sentences.");

                int curPos = 0;
                for (int i = 0; i < result.Count; i++)
                {
                    Console.WriteLine("Sentence {0} -> {1}", i+1, text.Substring(curPos, result[i] - 1));
                    curPos += result[i];
                }
            }
        }
    }
}
