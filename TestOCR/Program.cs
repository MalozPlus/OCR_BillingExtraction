
using System;
using System.Linq;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

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
                    Task<List<EntityToFind>> tskReadFile = ocrClient.ReadFile(fi);
                    tskReadFile.Wait();

                    StringBuilder sb = new StringBuilder(fi.Name.Split('.')[0]);
                    tskReadFile.Result.ForEach(x => sb.Append(";" + x.ValueToString()));
                    swCsv.WriteLine(sb.ToString());
                    swCsv.Flush();
                    Thread.Sleep(6000);
                }           
            }
            Console.WriteLine("END");
        }

        


       
    }

   

    


}
