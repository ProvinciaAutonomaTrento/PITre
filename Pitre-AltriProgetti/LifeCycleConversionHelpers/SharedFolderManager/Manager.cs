using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace SharedFolderManager
{
    public class Manager
    {
        public byte[] convertToPdf(string fileName, byte[] inputContent, string inputFolder,string outputFolder, string errorFolder)
        {
            byte[] retval;
            string tempfile = Guid.NewGuid().ToString().Replace("-", string.Empty) + "_" + fileName;

            File.WriteAllBytes(Path.Combine(inputFolder, tempfile), inputContent);

            string outFile = Path.Combine(outputFolder, tempfile);
            string errFile = Path.Combine(errorFolder, tempfile);
            while (true)
            {
                if (File.Exists(errFile))
                {
                    File.Delete(errFile);
                    return null;
                }
                if (File.Exists(outFile + ".pdf"))
                {
                    retval = readAndDeletefile(outFile + ".pdf");
                    break;
                }
            }

            readAndDeletefile(outFile); //lo cancello e basta
            return retval;
        }

        private byte[] readAndDeletefile(string filePath)
        {
            int counter = 0;

            byte[] content = null;
            while (true)
            {
                if (counter > 100) // 100 tentativi.
                    break;
                try
                {
                    if (File.Exists(filePath))
                    {
                        content = File.ReadAllBytes(filePath);
                        File.Delete(filePath);
                        break;
                    }
                }
                catch
                {
                    //wait a while
                    System.Threading.Thread.Sleep(500);
                    counter++;
                }

            }
            return content;
        }
    }
}
