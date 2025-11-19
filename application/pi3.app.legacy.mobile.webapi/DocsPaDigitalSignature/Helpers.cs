// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Tsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Documenti.DigitalSignature
{
    public class Helpers
    {
        public static int IndexOfInArray(byte[] array, byte[] pattern)
        {
            bool found = false;
            if (pattern.Length > array.Length)
                return -1;
            int i, j;

            for (i = 0, j = 0; i < array.Length;)
            {
                if (array[i++] != pattern[j++])
                {
                    j = 0;
                    continue;
                }

                if (j == pattern.Length)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                return -1;
            else
                return i - pattern.Length;
        }

        public static byte[] sbustaFileTimstamped(byte[] fileContents)
        {
            TsType tipo = getFileType(fileContents);
            if (tipo == TsType.TSD)
            {
                PKCS_Utils.tsd tsd = new PKCS_Utils.tsd();
                tsd.explode(fileContents);
                fileContents = tsd.Data.Content;
            }

            if (tipo == TsType.M7M)
            {
                PKCS_Utils.m7m m7m = new PKCS_Utils.m7m();
                m7m.explode(fileContents);
                fileContents = m7m.Data.Content;
            }
            return fileContents;
        }

        public static TsType getFileType(byte[] fileContents)
        {
            try
            {
                Asn1Sequence sequenza = Asn1Sequence.GetInstance(fileContents);
                DerObjectIdentifier FileOID = sequenza[0] as DerObjectIdentifier;
                if (FileOID != null)
                {
                    if (FileOID.Id == CmsObjectIdentifiers.timestampedData.Id)   //TSD
                        return TsType.TSD;

                    if (FileOID.Id == CmsObjectIdentifiers.SignedData.Id)   //P7M
                        return TsType.PKCS;

                }
            }
            catch { }
            //provare per vedere se è TSR
            try
            {
                TimeStampResponse TSR = new TimeStampResponse(fileContents);
                if (TSR != null)
                    return TsType.TSR;
            }
            catch { };


            int posi = BusinessLogic.Documenti.DigitalSignature.Helpers.IndexOfInArray(fileContents, System.Text.ASCIIEncoding.ASCII.GetBytes("Mime-Version:"));
            if (posi == 0) //E' un mime m7m
                return TsType.M7M;

            return TsType.UNKNOWN;
        }

        /// <summary>
        /// Sbusta un file timstamped e firmato, fino arrivare al file originale (payload)
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns>bytearray del file tsd, m7m, o p7m</returns>
        public static byte[] sbustaFileFirmato(byte[] fileContents)
        {
            //controlla se è base64
            String strfileContents = System.Text.ASCIIEncoding.ASCII.GetString(fileContents);
            if (IsBase64Encoded(strfileContents))
                fileContents = Convert.FromBase64String(strfileContents);

            TsType tipo = getFileType(fileContents);
            if (tipo == TsType.PKCS)
            {
                CmsSignedData cms = new CmsSignedData(fileContents);
                fileContents = (byte[])cms.SignedContent.GetContent();
            }

            if (tipo == TsType.TSD)
            {
                PKCS_Utils.tsd tsd = new PKCS_Utils.tsd();
                tsd.explode(fileContents);
                fileContents = tsd.Data.Content;
            }

            if (tipo == TsType.M7M)
            {
                PKCS_Utils.m7m m7m = new PKCS_Utils.m7m();
                m7m.explode(fileContents);
                fileContents = m7m.Data.Content;
            }

            //non conosco il tipo, esco
            if (tipo == TsType.UNKNOWN)
                return fileContents;


            //ricorsione per arrivare al singolo documento
            return sbustaFileFirmato(fileContents);
        }

        /// <summary>
        /// riconosce se un file è base64 o meno
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64Encoded(String str)
        {
            try
            {
                // If no exception is caught, then it is possibly a base64 encoded string
                byte[] data = Convert.FromBase64String(str);
                // The part that checks if the string was properly padded to the
                // correct length was borrowed from d@anish's solution
                return (str.Replace(" ", "").Replace("\r", "").Replace("\n", "").Length % 4 == 0);
            }
            catch
            {
                // If exception is caught, then it is not a base64 encoded string
                return false;
            }
        }

    }
}
