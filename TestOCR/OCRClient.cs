using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestOCR.Model;

namespace TestOCR
{
    class OCRClient
    {
        private const string subscriptionKey = "585a63258bfe41d589f007db6cb05fc9";
        private const string endpoint = "https://demo-billingacquisition.cognitiveservices.azure.com/";
        private ComputerVisionClient _client = null;
        private log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(Program));

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
        public  async Task<List<EntityToFind>> ReadFile(FileInfo fi)
        {
            try 
            {
                _log4net.InfoFormat("Reading file: {0}", fi.Name);

                List<EntityToFind> lstEntities = new List<EntityToFind>();
                lstEntities.Add(new Amount());
                lstEntities.Add(new EmissionDate());

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
                _log4net.Info("Extracting text from URL file...");
                do
                {
                    results = await this._client.GetReadResultAsync(Guid.Parse(operationId));
                }
                while ((results.Status == OperationStatusCodes.Running ||
                    results.Status == OperationStatusCodes.NotStarted));

                // Display the found text.
                var textUrlFileResults = results.AnalyzeResult.ReadResults;

                foreach (ReadResult page in textUrlFileResults)
                {
                    foreach (Line line in page.Lines)
                    {                     
                        if (lstEntities.Where(x => x.Found == true).Count() == lstEntities.Count())
                            break;

                        string textvalue = line.Text.Trim();
                        lstEntities.ForEach(x => x.CheckValue(textvalue));
                        _log4net.Info("Extracted: " + line.Text);
                    }
                }

                return lstEntities;
            }
            catch(Exception err)
            {
                _log4net.Error("Error reading file;", err);
                throw err;
            }
            
        }
    }
}
