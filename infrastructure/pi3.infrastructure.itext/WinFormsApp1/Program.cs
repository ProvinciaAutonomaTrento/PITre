// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Reflection;
using System.Runtime.InteropServices;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            // Verifica che l'applicazione non sia gi� in esecuzione
            using (var mutex = new Mutex(true, $"SignatureClientSingleInstance-{Assembly.GetExecutingAssembly().GetName().Name}", out bool isNewInstance))
            {
                if (!isNewInstance)
                {
                    // L'app � gi� in esecuzione, invia un messaggio per mostrare l'istanza esistente
                    NativeMethods.PostMessage(
                        (IntPtr)NativeMethods.HWND_BROADCAST,
                        NativeMethods.WM_SHOWME,
                        IntPtr.Zero,
                        IntPtr.Zero);
                    return;
                }

                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }

        // Codice per la comunicazione tra istanze
        internal static class NativeMethods
        {
            public const int HWND_BROADCAST = 0xffff;
            public static readonly int WM_SHOWME = RegisterWindowMessage("WM_SHOWME_SIGNATURECLIENT");

            [DllImport("user32.dll")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImport("user32.dll")]
            public static extern int RegisterWindowMessage(string message);
        }
    }
}