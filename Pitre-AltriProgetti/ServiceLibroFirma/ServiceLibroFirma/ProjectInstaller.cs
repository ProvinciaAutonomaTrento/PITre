using System;
using System.Configuration;
using System.Reflection;
using System.Configuration.Install;
using System.ServiceProcess;
using System.IO;

namespace ServiceLibroFirma
{
    /// <summary>
    ///     Summary description for ProjectInstaller.
    /// </summary>
    [System.ComponentModel.RunInstaller(true)]
    public class ProjectInstaller : System.Configuration.Install.Installer
    {
        public static string ServiceName = "LibroFirma Services";

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;

        public ProjectInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            this.serviceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceProcessInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller});

        }

        /// <summary>
        /// handler for the service to start automatically after installation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            //ServiceController sc = new ServiceController(this.serviceProcessInstaller.ServiceName);
            //sc.Start();
        }

        private static string GetConfigurationValue(string key)
        {
            var service = Assembly.GetAssembly(typeof(ProjectInstaller));
            Configuration config = ConfigurationManager.OpenExeConfiguration(service.Location);
            if (config.AppSettings.Settings[key] == null)
            {
                throw new IndexOutOfRangeException("Settings collection does not contain the requested key:" + key);
            }
            return config.AppSettings.Settings[key].Value;
        }

        private void CopyQueryList()
        {

            //***************************************
            //I copy the query list on the local disk
            //***************************************
            string SourceFolder = Directory.GetCurrentDirectory() + @"\xml";
            //target directory
            string TargetFolder = @"C:\Servizi_LibroFirma/";

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(TargetFolder))
            {
                System.IO.Directory.CreateDirectory(TargetFolder);
            }

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

        private void serviceInstaller_AfterInstall_1(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}