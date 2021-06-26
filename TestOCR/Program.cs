using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TestOCR
{
    class Program
    {

        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "585a63258bfe41d589f007db6cb05fc9";
        static string endpoint = "https://demo-billingacquisition.cognitiveservices.azure.com/";
        static string fatturePath = "C:\\Users\\l.maletti\\Documents\\Miei\\AcquisizioneFatture\\Fatture\\";
        //static string filename = "W2035888689.PDF";
        static string logFilename = "log.txt";
        static string csvFilename = "data.csv";

        static void Main(string[] args)
        {
            Console.WriteLine("BILLING ACQUISITION");
            var client = Authenticate(endpoint, subscriptionKey);

            using (StreamWriter swCsv = new StreamWriter(fatturePath + csvFilename, false))
            {
                swCsv.WriteLine("Numero Fattura;Data Emissione;Importo");
                DirectoryInfo dirInfo = new DirectoryInfo(fatturePath);
                FileInfo[] arrFi = dirInfo.GetFiles("*.PDF");
                Console.WriteLine("Number of files: " + arrFi.Length.ToString());

                foreach (FileInfo fi in arrFi)
                {
                    using (StreamWriter swLog = new StreamWriter(fatturePath + logFilename, true))
                    {
                        ReadFile(client, fi, swCsv, swLog).Wait();
                    }                       
                    Thread.Sleep(6000);
                }           
            }
            Console.WriteLine("END");
        }

        /*
         * AUTHENTICATE
         * Creates a Computer Vision client used by each example.
         */
        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
       
            return client;
        }


        /*
         * READ FILE - URL 
         * Extracts text. 
         */
        public static async Task ReadFile(ComputerVisionClient client, FileInfo fi, StreamWriter swCsv, StreamWriter swLog)
        {
            Console.WriteLine("READ FILE " + fi.Name);

            FileStream fsPdf = fi.OpenRead();
            var textHeaders = await client.ReadInStreamAsync(fsPdf, "it", new string[] { "1" });
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
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            Amount billingImport = new Amount();
            EmissionDate emissionDate = new EmissionDate();


            swLog.WriteLine("\n");
            swLog.WriteLine(fi.Name);
            foreach (ReadResult page in textUrlFileResults)
            {                    
                foreach (Line line in page.Lines)
                {
                    if (billingImport.Found && emissionDate.Found)
                        break;

                    string textvalue = line.Text.Trim();
                    billingImport.CheckValue(textvalue);
                    emissionDate.CheckValue(textvalue);
                    swLog.WriteLine(line.Text);
                }
            }
            
            string numeroFattura = fi.Name.Split('.')[0];
            swCsv.WriteLine(numeroFattura + ";" + emissionDate.Value.ToString("dd/MM/yyyy") + ";" + billingImport.Value.ToString("N2"));
            swCsv.Flush();
        }
    }

   

    


}
