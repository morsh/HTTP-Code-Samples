using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Translator.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            if ((args == null) || (args.Length < 2))
            {
                PrintUsage();
                return;
            }

            string subscriptionKey = args[0];
            string sampleName = args[1];
            try
            {
                await RunSample(subscriptionKey, sampleName);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Error:\n  {0}\n", ex.Message);
                PrintUsage();
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private static async Task RunSample(string key, string name)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The subscription key has not been provided.");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The sample name has not been provided.");
            }

            var authTokenSource = new AzureAuthToken(key.Trim());
            string authToken;
            try
            {
                authToken = await authTokenSource.GetAccessTokenAsync();
            }
            catch (HttpRequestException)
            {
                if (authTokenSource.RequestStatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Request to token service is not authorized (401). Check that the Azure subscription key is valid.");
                    return;
                }
                if (authTokenSource.RequestStatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Request to token service is not authorized (403). For accounts in the free-tier, check that the account quota is not exceeded.");
                    return;
                }
                throw;
            }

            if (string.Compare(name, "AddTranslation", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await AddTranslationSample.Run(authToken);
            }
            else if (string.Compare(name, "AddTranslationArray", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await AddTranslationArraySample.Run(authToken);
            }
            else if (string.Compare(name, "BreakSentences", StringComparison.OrdinalIgnoreCase) == 0)
            {
                BreakSentencesSample.Run(authToken);
            }
            else if (string.Compare(name, "Detect", StringComparison.OrdinalIgnoreCase) == 0)
            {
                DetectSample.Run(authToken);
            }
            else if (string.Compare(name, "DetectArray", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await DetectArraySample.Run(authToken);
            }
            else if (string.Compare(name, "GetLanguageNames", StringComparison.OrdinalIgnoreCase) == 0)
            {
                GetLanguageNamesSample.Run(authToken);
            }
            else if (string.Compare(name, "GetLanguagesForTranslate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                GetLanguagesForTranslateSample.Run(authToken);
            }
            else if (string.Compare(name, "GetLanguagesForSpeak", StringComparison.OrdinalIgnoreCase) == 0)
            {
                GetLanguagesForSpeakSample.Run(authToken);
            }
            else if (string.Compare(name, "GetTranslations", StringComparison.OrdinalIgnoreCase) == 0)
            {
                GetTranslationsSample.Run(authToken);
            }
            else if (string.Compare(name, "GetTranslationsArray", StringComparison.OrdinalIgnoreCase) == 0)
            {
                GetTranslationsArraySample.Run(authToken);
            }
            else if (string.Compare(name, "Speak", StringComparison.OrdinalIgnoreCase) == 0)
            {
                SpeakSample.Run(authToken);
            }
            else if (string.Compare(name, "TranslateArray", StringComparison.OrdinalIgnoreCase) == 0)
            {
                await TranslateArraySample.Run(authToken);
            }
            else if (string.Compare(name, "Translate", StringComparison.OrdinalIgnoreCase) == 0)
            {
                TranslateSample.Run(authToken);
            }
            else
            {
                throw new ArgumentException($"The sample name '{name}' is not valid.");
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  TextTranslationSample.exe <client_secret> <sample_name>");
            Console.WriteLine("  client_secret: Value of your Text Translation API subscription secret key.");
            Console.WriteLine("  sample_name  : Name of sample. Available names:");
            Console.WriteLine("                 AddTranslation");
            Console.WriteLine("                 AddTranslationArray");
            Console.WriteLine("                 BreakSentences");
            Console.WriteLine("                 Detect");
            Console.WriteLine("                 DetectArray");
            Console.WriteLine("                 GetLanguageNames");
            Console.WriteLine("                 GetLanguagesForTranslate");
            Console.WriteLine("                 GetLanguagesForSpeak");
            Console.WriteLine("                 GetTranslations");
            Console.WriteLine("                 GetTranslationsArray");
            Console.WriteLine("                 Speak");
            Console.WriteLine("                 Translate");
            Console.WriteLine("                 TranslateArray");
        }
    }
}
