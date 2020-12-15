using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using BusinessLogic.Application;
using DocsPaUtils.Configuration;
using Publisher;
using INOUT = System.IO;

namespace DocsPaWS
{
    /// <summary>
    /// Summary description for Global.
    /// </summary>

    public class Global : HttpApplication
    {
        /// <summary>
        /// 
        /// </summary>
        public Global()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Indica se le pubblicazioni sono abilitate o meno per l'istanza
        /// </summary>
        private bool PublisherEnabled
        {
            get
            {
                const string KEY = "BE_PUBBLICAZIONI";

                var value = InitConfigurationKeys.GetValue("0", KEY);

                return value == "1";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            //ChilkatDot();
            InitializeDBConfigurations();

            Startup.ResetSemaforo();
            try
            {
                RestartUnexpectedStoppedChannels();
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Lettura delle configurazioni da web.config necessarie
        /// per l'interfacciamento con il database.
        /// Impostazione delle configurazioni nel contesto dell'applicazione web.
        /// </summary>
        private void InitializeDBConfigurations()
        {
            var dbtype = ConfigurationManager.AppSettings["DBType"];
            var connectionString = ConfigurationManager.AppSettings["connectionString"];

            HttpContext.Current.Application.Add("DBTYPE", dbtype);
            HttpContext.Current.Application.Add("CONNECTIONSTRING", connectionString);
        }

        /// <summary>
        /// Ripristino dei servizi di pubblicazione
        /// </summary>
        private void RestartUnexpectedStoppedChannels()
        {
            if (PublisherEnabled)
                PublisherServiceControl.RestartUnexpectedStoppedChannels();
        }

        /// <summary>
        /// Provvede a copiare il corretto ChilkatDotNet2.dll nella bin..
        /// E' necessario che pero' non sia presente tale DLL nella bin
        /// </summary>
        public void ChilkatDot()
        {
            var currentPath = HttpContext.Current.Server.MapPath("~/");
            var fileName = @"ChilkatDotNet2.dll";
            var destFile = currentPath + "bin\\" + fileName;
            try
            {
                var assembly = Assembly.LoadFrom(destFile);
            }
            catch
            {
                try
                {
                    if (INOUT.File.Exists(destFile))
                        INOUT.File.SetAttributes(destFile, INOUT.FileAttributes.Normal);
                    if (IntPtr.Size == 4)
                        INOUT.File.Copy(currentPath + @"ChilkatDotNet\ChilkatDotNet2_32.dll", destFile, true);
                    else
                        INOUT.File.Copy(currentPath + @"ChilkatDotNet\ChilkatDotNet2_64.dll", destFile, true);


                    if (INOUT.File.Exists(destFile))
                        INOUT.File.SetAttributes(destFile, INOUT.FileAttributes.Normal);


                    if (INOUT.File.Exists(currentPath + "bin\\" + "ChilkatDotNet2.exe"))
                    {
                        INOUT.File.SetAttributes(currentPath + "bin\\" + "ChilkatDotNet2.exe",
                            INOUT.FileAttributes.Normal);
                        INOUT.File.Delete(currentPath + "bin\\" + "ChilkatDotNet2.exe");
                    }
                }
                catch
                {
                    // Ignored
                }
            }
        }

        #region Web Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        #endregion
    }
}