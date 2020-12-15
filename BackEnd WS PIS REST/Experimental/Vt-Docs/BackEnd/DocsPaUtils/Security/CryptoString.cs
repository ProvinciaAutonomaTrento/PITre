using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DocsPaUtils.Security
{
    /// <summary>
    /// Classe di utility per l'encryption di una stringa
    /// </summary>
    public class CryptoString
    {
        /// <summary>
        /// 
        /// </summary>
        private CryptoString() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        public CryptoString(string key)
        {
            byte[] customKey = ASCIIEncoding.ASCII.GetBytes(GenerateCustomKey(key));

            this.Key = customKey;
            this.IV = customKey;
        }

        /// <summary>
        /// 
        /// </summary>
        protected byte[] Key
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        protected byte[] IV
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GenerateCustomKey(string key)
        {
            if (key.Length < 32)
                for (int i = key.Length; i < 32; i++)
                    key = "0" + key;
            else
                return key.Substring(0, 16);

            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        public string Encrypt(string originalStr)
        {
            // Encode data string to be stored in memory
            byte[] originalStrAsBytes = Encoding.ASCII.GetBytes(originalStr);
            byte[] originalBytes = { };

            // Create MemoryStream to contain output
            MemoryStream memStream = new MemoryStream(originalStrAsBytes.Length);

            RijndaelManaged rijndael = new RijndaelManaged();

            rijndael.KeySize = 256;
            rijndael.BlockSize = 256;
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
        /// 
        /// </summary>
        /// <param name="encryptedStr"></param>
        /// <returns></returns>
        public string Decrypt(string encryptedStr)
        {
            // Unconvert encrypted string
            byte[] encryptedStrAsBytes = Convert.FromBase64String(encryptedStr);
            byte[] initialText = new Byte[encryptedStrAsBytes.Length];

            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.KeySize = 256;
            rijndael.BlockSize = 256;
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
