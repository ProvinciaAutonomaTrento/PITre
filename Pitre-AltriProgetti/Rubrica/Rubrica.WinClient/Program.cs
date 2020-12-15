using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rubrica.WinClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Login login = new Login())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new frmGestioneRubrica(login.Credentials, login.Amministratore));
                }
            }
        }
    }
}
