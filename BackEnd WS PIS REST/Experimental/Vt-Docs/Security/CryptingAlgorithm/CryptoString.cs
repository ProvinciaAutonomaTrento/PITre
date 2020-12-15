using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Security.Core.CryptingAlgorithm
{
    /// <summary>
    /// Classe di utility per l'encryption di una stringa
    /// </summary>
    public class CryptoString
    {
        /// <summary>
        /// Costruttore per l'inizializzazione della classe responsabile del encrypt / decrypt
        /// del token
        /// </summary>
        /// <param name="key">Chiave da utlizzare per criptare / descriptare il token</param>
        /// <param name="initializationVector">Vettore di inizializzazione</param>
        public CryptoString(String key, String initializationVector) : this(key)
        {
            byte[] customIV = ASCIIEncoding.ASCII.GetBytes(GenerateCustomKey(initializationVector));
            this.IV = customIV;
        }

        /// <summary>
        /// Costruttore della classe responsabile dell'encrypting del token
        /// </summary>
        /// <param name="key">Chiave da utilizzare per l'encrypting</param>
        public CryptoString(string key)
        {
            byte[] customKey = ASCIIEncoding.ASCII.GetBytes(GenerateCustomKey(key));


            this.Key = customKey;
            this.IV = customKey;
        }

        /// <summary>
        /// Chiave da utilizzare per criptare il token
        /// </summary>
        protected byte[] Key
        {
            get;
            set;
        }

        /// <summary>
        /// Vettore di inizializzazione da utilizzare per criptare il token
        /// </summary>
        protected byte[] IV
        {
            get;
            set;
        }

        /// <summary>
        /// Funzione per la generazione di una chiave valida per l'algoritmo
        /// di criptazione CryptoString a partire da una chiave passata per parametro
        /// </summary>
        /// <param name="key">Chiave da validare</param>
        /// <returns>Chiave valida per l'algoritmo di criptazione CryptoString</returns>
        protected string GenerateCustomKey(string key)
        {
            if (key.Length > 32)
                return key.Substring(0, 32);
            else
                return key;
            
        }

        /// <summary>
        /// Funzione per l'encryption del token
        /// </summary>
        /// <param name="originalStr">Token da criptare</param>
        /// <returns>Token criptato</returns>
        public string Encrypt(string originalStr)
        {
            // Encode data string to be stored in memory
            byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
            byte[] originalBytes = { };

            // Create MemoryStream to contain output
            MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length);

            RijndaelManaged rijndael = new RijndaelManaged();

            rijndael.KeySize = this.Key.Length * 8;
            rijndael.BlockSize = this.IV.Length * 8;
            rijndael.Key = this.Key;
            rijndael.IV = this.IV;

            // Create encryptor, and stream objects
            ICryptoTransform rdTransform = rijndael.CreateEncryptor();
            CryptoStream cryptoStream = new CryptoStream(memStream, rdTransform, CryptoStreamMode.Write);

            // Write encrypted data to the MemoryStream
            cryptoStream.Write(originalStrAsBytes, 0, originalStrAsBytes.Length);
            cryptoStream.FlushFinalBlock();
            originalBytes = memStream.ToArray();

            // Release all resources
            memStream.Close();
            cryptoStream.Close();
            rdTransform.Dispose();
            rijndael.Clear();

            // Convert encrypted string
            string encryptedStr = Convert.ToBase64String(originalBytes);
            return (encryptedStr);
        }

        /// <summary>
        /// Funzione per decriptare il token
        /// </summary>
        /// <param name="encryptedStr">Token da decriptare</param>
        /// <returns>Token decriptato</returns>
        public string Decrypt(string encryptedStr)
        {
            // Unconvert encrypted string
            byte[] encryptedStrAsBytes = Convert.FromBase64String(encryptedStr);
            byte[] initialText = new Byte[encryptedStrAsBytes.Length];

            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.KeySize = this.Key.Length * 8;
            rijndael.BlockSize = this.IV.Length * 8;
            rijndael.Key = this.Key;
            rijndael.IV = this.IV;

            MemoryStream memStream = new MemoryStream(encryptedStrAsBytes);

            // Create decryptor, and stream objects
            ICryptoTransform rdTransform = rijndael.CreateDecryptor();
            CryptoStream cryptoStream = new CryptoStream(memStream, rdTransform, CryptoStreamMode.Read);

            // Read in decrypted string as a byte[]
            cryptoStream.Read(initialText, 0, initialText.Length);

            // Release all resources
            memStream.Close();
            cryptoStream.Close();
            rdTransform.Dispose();
            rijndael.Clear();

            // Convert byte[] to string
            string decryptedStr = Encoding.ASCII.GetString(initialText);
            return (decryptedStr);
        }
    }
}
