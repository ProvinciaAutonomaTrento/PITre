// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox
{
    public class Crypter
    {
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
                }
                return encode;

            }
            catch (Exception e)
            {
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
                }
                return decode;
            }
            catch (Exception e)
            {
                return password;
            }
        }
    }
}
