
using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using System.Reflection;

namespace TestOCR
{
    class Program
    {

        // Add your Computer Vision subscription key and endpoint
        static string fatturePath = "C:\\Users\\l.maletti\\Documents\\Miei\\AcquisizioneFatture\\Fatture\\";
        static string csvFilename = "data.csv";

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var _log4net = log4net.LogManager.GetLogger(typeof(Program));

            Console.WriteLine("BILLING ACQUISITION");
            OCRClient ocrClient = new OCRClient();
            ocrClient.Authenticate();

            using (StreamWriter swCsv = new StreamWriter(fatturePath + csvFilename, false))
            {
                swCsv.WriteLine("Numero Fattura;Data Emissione;Importo");
                DirectoryInfo dirInfo = new DirectoryInfo(fatturePath);
                FileInfo[] arrFi = dirInfo.GetFiles("*.PDF");
                Console.WriteLine("Number of files: " + arrFi.Length.ToString());

                foreach (FileInfo fi in arrFi)
                {
                    ocrClient.ReadFile(fi, swCsv).Wait();                                       
                    Thread.Sleep(6000);
                }           
            }
            Console.WriteLine("END");
        }

        


       
    }

   

    


}
