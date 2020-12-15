using System.ServiceProcess;
using System;
using System.Configuration.Install;
using System.Reflection;

namespace PublisherServiceMonitor
{
    internal static class Program
    {
        private static void Main(string[] args)
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
                                System.Windows.Forms.MessageBox.Show("Servizio Installato");
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Errore durante l'installazione del servizio " + e.Message);
                                System.Windows.Forms.MessageBox.Show("Si ricorda di eseguire il Command Prompt con diritti di Amministratore e di fermare il servizio prima della installazione.");
                            };
                            break;
                        }
                    case "--uninstall":
                        {
                            try
                            {
                                ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                                System.Windows.Forms.MessageBox.Show("Servizio Disinstallato");
                            }
                            catch (Exception e)
                            {
                                System.Windows.Forms.MessageBox.Show("Errore durante la disinstallazione del servizio " + e.Message);
                                System.Windows.Forms.MessageBox.Show("Si ricorda di eseguire il Command Prompt con diritti di Amministratore e di fermare il servizio prima della rimozione.");
                            };
                            break;
                        }



                    default:
                        {
                            System.Windows.Forms.MessageBox.Show("Installazione /disinstallazione --install / --uninstall");
                            break;
                        }

                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			{ 
				new PublisherServiceMonitor () 
			};
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
