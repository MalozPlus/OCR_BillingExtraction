using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestOCR
{
    class OCRClient
    {
        private const string subscriptionKey = "585a63258bfe41d589f007db6cb05fc9";
        private const string endpoint = "https://demo-billingacquisition.cognitiveservices.azure.com/";
        private ComputerVisionClient _client = null;

        /*
         * AUTHENTICATE
         * Creates a Computer Vision client used by each example.
         */
        public void Authenticate()
        {
            this._client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
              { Endpoint = endpoint };
        }



        /*
        * READ FILE - URL 
        * Extracts text. 
        */
        public  async Task ReadFile(FileInfo fi, StreamWriter swCsv)
        {
            var _log4net = log4net.LogManager.GetLogger(typeof(Program));
            Console.WriteLine("READ FILE " + fi.Name);

            FileStream fsPdf = fi.OpenRead();
            var textHeaders = await this._client.ReadInStreamAsync(fsPdf, "it", new string[] { "1" });
            // After the request, get the operation location (operation ID)
            fsPdf.Close();
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            Console.WriteLine($"Extracting text from URL file...");
            do
            {
                results = await this._client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            Amount billingImport = new Amount();
            EmissionDate emissionDate = new EmissionDate();

            _log4net.Info("Filename: " + fi.Name);
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    if (billingImport.Found && emissionDate.Found)
                        break;

                    string textvalue = line.Text.Trim();
                    billingImport.CheckValue(textvalue);
                    emissionDate.CheckValue(textvalue);
                    _log4net.Info("Extracted: " + line.Text);
                }
            }

            string numeroFattura = fi.Name.Split('.')[0];
            swCsv.WriteLine(numeroFattura + ";" + emissionDate.Value.ToString("dd/MM/yyyy") + ";" + billingImport.Value.ToString("N2"));
            swCsv.Flush();
        }
    }
}
