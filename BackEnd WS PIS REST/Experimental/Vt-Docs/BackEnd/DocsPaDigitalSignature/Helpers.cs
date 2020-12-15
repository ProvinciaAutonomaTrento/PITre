using System;
using System.Text;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Tsp;
using DocsPaVO.documento;
using Org.BouncyCastle.Cms;

namespace BusinessLogic.Documenti.DigitalSignature
{
    public class Helpers
    {
        /// <summary>
        /// Sbusta un file timestampato, estraendo l'eventuale file firmato (pkcs) o il payload nel caso non fosse firmato.
        /// </summary>
        /// <param name="fileContents">bytearray del file tsd o m7m</param>
        /// <returns></returns>
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

        /// <summary>
        /// Sbusta un file timstamped e firmato, fino arrivare al file originale (payload)
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns>bytearray del file tsd, m7m, o p7m</returns>
        public static  byte[] sbustaFileFirmato(byte[] fileContents)
        {
            //controlla se è base64
            String strfileContents = System.Text.ASCIIEncoding.ASCII.GetString(fileContents);
            if (IsBase64Encoded(strfileContents))
                fileContents = Convert.FromBase64String (strfileContents);
            
            TsType tipo = getFileType (fileContents);
            if (tipo == TsType.PKCS)
            {
                CmsSignedData cms = new CmsSignedData(fileContents);
                fileContents=  (byte[]) cms.SignedContent.GetContent();
            }

            if (tipo == TsType.TSD)
            {
                PKCS_Utils.tsd tsd = new PKCS_Utils.tsd();
                tsd.explode (fileContents);
                fileContents  =tsd.Data.Content; 
            }

            if (tipo == TsType.M7M)
            {
                PKCS_Utils.m7m m7m = new PKCS_Utils.m7m();
                m7m.explode (fileContents);
                fileContents = m7m.Data.Content;
            }

            //non conosco il tipo, esco
            if (tipo == TsType.UNKNOWN)
                return fileContents;

            
            //ricorsione per arrivare al singolo documento
            return sbustaFileFirmato(fileContents);
        }

        /// <summary>
        /// Riconosce il tipo di file firmato o timestampato (tsd/m7m/p7m)
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
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
            catch{}
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

        public static int IndexOfInArray(byte[] array, byte[] pattern)
        {
            bool found = false;
            if (pattern.Length > array.Length)
                return -1;
            int i, j;

            for (i = 0, j = 0; i < array.Length; )
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


    }
    //public class CryptoHelper
    //{
    //    [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    public static extern int RetrieveCRL(IntPtr pContext, int timeout);

    //    [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    public static extern bool
    //        IsPKCS7CounterSigned([MarshalAs(UnmanagedType.AsAny)] object PKCS7Message, int messageLength);

    //    [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    public static extern void
    //        GetASN1UnstructuredName(StringBuilder uName, int uNameMaxLen, [MarshalAs(UnmanagedType.AsAny)] object PKCS7Message, int messageLength);

    //    [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //    public static extern int GetCertSignatureAlgorithm(IntPtr pContext, StringBuilder uName, int uNameMaxLen);
    //}

    //public class CAPICOMHelper
    //{
    //    public enum CAPICOM_CHAIN_STATUS
    //    {
    //        CAPICOM_TRUST_IS_NOT_TIME_VALID = 0x00000001,
    //        CAPICOM_TRUST_IS_NOT_TIME_NESTED = 0x00000002,
    //        CAPICOM_TRUST_IS_REVOKED = 0x00000004,
    //        CAPICOM_TRUST_IS_NOT_SIGNATURE_VALID = 0x00000008,
    //        CAPICOM_TRUST_IS_NOT_VALID_FOR_USAGE = 0x00000010,
    //        CAPICOM_TRUST_IS_UNTRUSTED_ROOT = 0x00000020,
    //        CAPICOM_TRUST_REVOCATION_STATUS_UNKNOWN = 0x00000040,
    //        CAPICOM_TRUST_IS_CYCLIC = 0x00000080,
    //        CAPICOM_TRUST_INVALID_EXTENSION = 0x00000100,
    //        CAPICOM_TRUST_INVALID_POLICY_CONSTRAINTS = 0x00000200,
    //        CAPICOM_TRUST_INVALID_BASIC_CONSTRAINTS = 0x00000400,
    //        CAPICOM_TRUST_INVALID_NAME_CONSTRAINTS = 0x00000800,
    //        CAPICOM_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT = 0x00001000,
    //        CAPICOM_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT = 0x00002000,
    //        CAPICOM_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT = 0x00004000,
    //        CAPICOM_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT = 0x00008000,
    //        CAPICOM_TRUST_IS_OFFLINE_REVOCATION = 0x01000000,
    //        CAPICOM_TRUST_NO_ISSUANCE_CHAIN_POLICY = 0x02000000,
    //        CAPICOM_TRUST_IS_PARTIAL_CHAIN = 0x00010000,
    //        CAPICOM_TRUST_CTL_IS_NOT_TIME_VALID = 0x00020000,
    //        CAPICOM_TRUST_CTL_IS_NOT_SIGNATURE_VALID = 0x00040000,
    //        CAPICOM_TRUST_CTL_IS_NOT_VALID_FOR_USAGE = 0x00080000,
    //    }
    //}
}
