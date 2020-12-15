using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Rubrica.ConfigCustomAction
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WebConfigurations(args));
        }
    }
}
