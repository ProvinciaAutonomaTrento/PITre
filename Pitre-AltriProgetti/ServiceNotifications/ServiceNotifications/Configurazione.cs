using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace ServiceNotifications
{
    public class Configurazione
    {

        #region service settings
        public static string getServiceName()
        {
            string retval = "Default Service";

            string value = ConfigurationManager.AppSettings["name_service"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }


        public static string getServiceDescription()
        {
            string retval = "Default Service Description";

            string value = ConfigurationManager.AppSettings["description_service"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }


        public static string getDisplayNameService()
        {
            string retval = "Default Service Description";

            string value = ConfigurationManager.AppSettings["display_name_service"];
            if (!string.IsNullOrEmpty(value))
                retval = value;

            return retval;
        }

        public static int getSleepServiceValue()
        {
            int defaultVal = 4000;

            string value = ConfigurationManager.AppSettings["sleep_service"];
            if (!string.IsNullOrEmpty(value))
            {
                int retval = 0;
                if (!Int32.TryParse(value, out retval))
                    return retval;
                else
                    return defaultVal;
            }
            else
            {
                return defaultVal;
            }
        }



        public static string getWorkFolder()
        {
            string retval = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;

            string value = ConfigurationManager.AppSettings["work_folder"];
            if (!string.IsNullOrEmpty(value))
            {
                if (Directory.Exists (value))
                    retval = value;
            }
            return retval;
        }

#endregion

        public static void CopyQueryList()
        {
            //***************************************
            //I copy the query list on the local disk
            //***************************************
            string SourceFolder = Directory.GetCurrentDirectory() + @"\xml";
            //target directory
            string TargetFolder = getWorkFolder();

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(TargetFolder))
                System.IO.Directory.CreateDirectory(TargetFolder);

            if (System.IO.Directory.Exists(SourceFolder))
            {
                string[] files = System.IO.Directory.GetFiles(SourceFolder);
                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(TargetFolder, fileName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            //***************************************
            //end copy
            //***************************************
        }
    }
}
