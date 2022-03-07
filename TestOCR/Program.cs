
using System;

using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Extensions.Hosting;
using TestOCR.Model;

namespace TestOCR
{
    class Program
    {

        // Add your Computer Vision subscription key and endpoint
        static string fatturePath = "C:\\Users\\l.maletti\\Documents\\Miei\\AcquisizioneFatture\\Wind_LineaFissa\\";
        static string csvFilename = "data.csv";

        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            ExecuteMain();

            return host.RunAsync();        
        }


        static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args);


        static void ExecuteMain()
        {
            //Set up logging
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var _log4net = log4net.LogManager.GetLogger(typeof(Program));

            try
            {
                _log4net.Info("BILLING ACQUISITION -> START");

                //Start up client
                OCRClient ocrClient = new OCRClient();
                ocrClient.Authenticate();

                StringBuilder sbCsvContent = new StringBuilder();
                sbCsvContent.AppendLine("Numero Fattura;Importo;Data Emissione");

                //Get billing files
                DirectoryInfo dirInfo = new DirectoryInfo(fatturePath);
                FileInfo[] arrFi = dirInfo.GetFiles("*.PDF");
                _log4net.InfoFormat("Number of files: {0}", arrFi.Length);

                foreach (FileInfo fi in arrFi)
                {
                    //Read file and extract content
                    Task<List<EntityToFind>> tskReadFile = ocrClient.ReadFile(fi);
                    tskReadFile.Wait();

                    StringBuilder sbRow = new StringBuilder(fi.Name.Split('.')[0]);
                    tskReadFile.Result.ForEach(x => sbRow.Append(";" + x.ValueToString()));
                    sbCsvContent.AppendLine(sbRow.ToString());
                    Thread.Sleep(6000);
                }

                //Write output csv
                string csvFilePath = fatturePath + csvFilename;
                _log4net.InfoFormat("Writing final csv file: {0}", csvFilePath);
                using (StreamWriter swCsv = new StreamWriter(csvFilePath, false))
                {
                    swCsv.Write(sbCsvContent.ToString());
                }

                _log4net.Info("BILLING ACQUISITION -> END");
            }
            catch (Exception err)
            {
                _log4net.Error("Error during billing acquisition;", err);
            }
        }


    }

   

    


}
