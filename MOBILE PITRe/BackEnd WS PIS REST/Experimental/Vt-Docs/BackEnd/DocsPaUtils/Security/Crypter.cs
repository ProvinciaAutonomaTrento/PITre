using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using log4net;

namespace DocsPaUtils.Security
{
    public class Crypter
    {
        private static ILog logger = LogManager.GetLogger(typeof(Crypter));
        private const string chiave = "AxTYQWCvGTFRbgLL"; //16 byte
        private const string iv = "QWExcfTyUxxLOafO"; //16 byte

        public static string id_registo { get; set;}

        public Crypter()
        {
        }

        private static string genera_key_16(string stringa)
        {
            if (stringa.Length < 16)
                for (int i = stringa.Length; i < 16; i++)
                    stringa = "0" + stringa;
            else
                return stringa.Substring(0, 16);
            
            return stringa;
        }

        public static string Encode(string password, string key)
        {
            try
            {
                string encode = password;
                if (!string.IsNullOrEmpty(key))
                {
                    logger.Debug("inizio encode password");
                    RijndaelManaged rjm = new RijndaelManaged();
                    rjm.KeySize = 128;
                    rjm.BlockSize = 128;
                    rjm.Key = ASCIIEncoding.ASCII.GetBytes(genera_key_16(key));//chiave);
                    rjm.IV = ASCIIEncoding.ASCII.GetBytes(genera_key_16(key));//iv);
                    if (password == string.Empty)
                        password = " ";
                    Byte[] input = Encoding.UTF8.GetBytes(password);
                    Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
                    encode = Convert.ToBase64String(output);
                    logger.Debug("fine encode password");
                }
                return encode;

            }
            catch (Exception e)
            {
                if (key == string.Empty)
                    logger.Debug("kiave non presente");
                else
                    logger.Debug("errore nell'encript della password- errore:" + e.Message);
                return password;
            }
        }

        public static string Decode(string password, string key)
        {
            try
            {
                string decode = password;

                if (!string.IsNullOrEmpty(key))
                {
                    logger.Debug("Inizio Decode");
                    RijndaelManaged rjm = new RijndaelManaged();
                    rjm.KeySize = 128;
                    rjm.BlockSize = 128;
                    
                    rjm.Key = ASCIIEncoding.ASCII.GetBytes(genera_key_16(key));//chiave);
                    rjm.IV = ASCIIEncoding.ASCII.GetBytes(genera_key_16(key));//iv);

                    Byte[] input = Convert.FromBase64String(password);
                    Byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                    decode = Encoding.UTF8.GetString(output);
                    //caso di mancanza di password
                    if (decode.Equals(" "))
                        decode = string.Empty;
                    logger.Debug("End Decode");
                }
                return decode;
            }
            catch(Exception e)
            {
                if (key == string.Empty)
                    logger.Debug("kiave non presente");
                else
                    logger.Debug("errore durante il decode della password - errore: "+e.Message);
                return password;
            }
        }

    }
}
