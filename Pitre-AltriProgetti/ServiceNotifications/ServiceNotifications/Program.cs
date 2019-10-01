using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.IO;

namespace ServiceNotifications
{
    static class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        {
                            try
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                                System.Windows.Forms.MessageBox.Show("Servizio Installato","ServiceInstaller");
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Errore durante l'installazione del servizio " + e.Message, "ServiceInstaller");
                                System.Windows.Forms.MessageBox.Show("Si ricorda di eseguire il Command Prompt con diritti di Amministratore e di fermare il servizio prima della installazione.", "ServiceInstaller");
                            };
                            break;
                        }
                    case "--uninstall":
                        {
                            try
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                                System.Windows.Forms.MessageBox.Show("Servizio Disinstallato","ServiceInstaller");
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Errore durante la disinstallazione del servizio " + e.Message,"ServiceInstaller");
                                System.Windows.Forms.MessageBox.Show("Si ricorda di eseguire il Command Prompt con diritti di Amministratore e di fermare il servizio prima della rimozione.","ServiceInstaller");
                            };
                            break;
                        }



                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("Installazione /disinstallazione --install / --uninstall","ServiceInstaller");
                            break;
                        }

                }

            }
            else
            {
                log4net.Config.XmlConfigurator.Configure();
                //Creation of windows service
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new ServiceNotify() };
                //execution of the service windows
                System.ServiceProcess.ServiceBase.Run(ServicesToRun);
            }
        }

      
    }
}
