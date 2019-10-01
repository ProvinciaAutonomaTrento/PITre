using System;
using System.IO;
using Ionic.Zip;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compression
{
    public class CompressionManager
    {
        public bool DecompressArchive(string pathFolder, string fileName, bool deleteOnSuccess = false)
        {
            bool retVal = false;

            string zipToUnpack = pathFolder + @"\" + fileName;
            string unpackDirectory = pathFolder;
            using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
            {
                // here, we extract every entry, but we could extract conditionally
                // based on entry name, size, date, checkbox status, etc.  
                foreach (ZipEntry e in zip1)
                {
                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    retVal = true;
                }
            }

            return retVal;
        }

        public bool DecompressArchiveVolumes(string pathFolder, string fileNameUncompressed, string[] volumes, bool deleteOnSuccess = false)
        {
            bool retVal = false;

            string finalName = pathFolder  + @"\" + fileNameUncompressed;
            FileStream myStream = new FileStream(finalName, FileMode.Create, FileAccess.Write);

            foreach(string zipFile in volumes)
            {
                string zipToUnpack = pathFolder + @"\" + zipFile;
                //unpackDirectory = pathFolder;
                using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                {
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(myStream);
                        //e.Extract(unpackDirectory, ExtractExistingFileAction.DoNotOverwrite);
                    }
                }
            }

            if (myStream.Length != 0)
            {
                //myStream.Flush();
                myStream.Close();

                foreach (string zipFile in volumes)
                {
                    string zipToDelete = pathFolder + @"\" + zipFile;
                    File.Delete(zipToDelete);
                }
                
                retVal = true;

                if (Path.GetExtension(fileNameUncompressed).ToUpper() == "PDF")
                {
                    this.LinearizzaFilePdf(finalName);
                }
            }
            else
            {
                myStream.Close();
                File.Delete(finalName);
            }

            return retVal;
        }

        private void LinearizzaFilePdf(string filePathAndName)
        {
            //Linearizzare il file pdf.

            //Nel caso sia più grande di <<Chiave su web.config>> e abbia più di <<N>> pagine,
            //realizzare un file pdf di anteprima con le prime <<N>> pagine estratte dall'originale.
        }
    }
}
