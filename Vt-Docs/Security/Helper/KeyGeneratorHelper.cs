using System;
using System.Security.Cryptography;

namespace Security.Core.Helper
{
    /// <summary>
    /// Questa classe fornisce funzioni di utilità per il calcolo
    /// di chiavi per l'encryption e di vettori di inizializzazione per
    /// gli algoritmi di encrypting
    /// </summary>
    public class KeyGeneratorHelper
    {
        
        /// <summary>
        /// Lunghezza della chiave da generare
        /// </summary>
        private static int KeyLength { get; set; }

        /// <summary>
        /// Funzione per la generazione della chiave random
        /// </summary>
        /// <param name="keyLength">Lunghezza della chiave da generare</param>
        /// <returns>Chiave generata</returns>
        public static string BetterRandomString(int keyLength)
        {
            // Salvataggio del numero di caratteri costituenti la chiave
            KeyLength = keyLength;

            // Create a stronger hash code using RNGCryptoServiceProvider
            byte[] random = new byte[KeyLength];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // Populate with random bytes
            rng.GetBytes(random);

            // convert random bytes to string
            string randomBase64 = Convert.ToBase64String(random);

            // Se la stringa casuale è più lunga della dimensione richiesta, viene tagliata
            // alla dimensione richiesta
            if (randomBase64.Length > KeyLength)
                randomBase64 = randomBase64.Substring(0, KeyLength);

            // Return
            return randomBase64;
        }

    }
}
