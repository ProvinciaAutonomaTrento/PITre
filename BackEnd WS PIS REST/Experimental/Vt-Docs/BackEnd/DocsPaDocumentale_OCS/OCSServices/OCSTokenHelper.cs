using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace DocsPaDocumentale_OCS.OCSServices
{
  
    /// </summary>
    public sealed class OCSTokenHelper
    {
        // per ora lasciamo le chiavi fisse
        static byte[] key = { 145, 12, 32, 245, 98, 132, 98, 214, 6, 77, 131, 44, 221, 3, 9, 50 };
        static byte[] iv = { 15, 122, 132, 5, 93, 198, 44, 31, 9, 39, 241, 49, 250, 188, 80, 7 };



        public static string Encrypt(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return BytesToHex(Encrypt(encoding.GetBytes(str)));
        }

        public static byte[] Encrypt(byte[] data)
        {
            return Encrypt(data, key, iv);
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Rijndael algorithm = Rijndael.Create())
            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(key, iv))
                return Crypta(data, key, iv, encryptor);
        }

        public static string Decrypt(string hexstr)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetString(Decrypt(HexToBytes(hexstr)));
        }

        public static byte[] Decrypt(byte[] data)
        {
            return Decrypt(data, key, iv);
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            using (Rijndael algorithm = Rijndael.Create())
            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(key, iv))
                return Crypta(data, key, iv, decryptor);
        }

        public static byte[] Crypta(byte[] data, byte[] key, byte[] iv,
                             ICryptoTransform cryptor)
        {
            MemoryStream m = new MemoryStream();
            using (Stream c = new CryptoStream(m, cryptor, CryptoStreamMode.Write))
                c.Write(data, 0, data.Length);
            return m.ToArray();
        }

        public static String BytesToHex(byte[] data)
        {
            string st = "";
            for (int i = 0; i < data.Length; i++)
                st += data[i].ToString("X2");
            return st;
        }

        public static byte[] HexToBytes(string data)
        {
            int l = data.Length / 2;
            byte[] bytes = new byte[l];
            for (int i = 0; i < l; i++)
                bytes[i] = byte.Parse(data.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            return bytes;
        }
    }
}
