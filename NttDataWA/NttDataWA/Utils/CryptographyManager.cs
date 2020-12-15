using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;

namespace NttDataWA.Utils
{
    public class CryptographyManager
    {
        /// <summary>
        /// Calculates fingerprints
        /// </summary>
        /// <param name="stream">Byte[]</param>
        /// <returns>string</returns>
        public static string CalculatesFingerprint(byte[] stream)
        {
            try
            {
                SHA1 sha = new SHA1CryptoServiceProvider();

                byte[] impronta = sha.ComputeHash(stream);
                Console.WriteLine(impronta.Length);

                return BitConverter.ToString(impronta).Replace("-", "");
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return null;
            }
        }
    }
}