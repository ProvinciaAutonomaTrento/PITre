using System;
using System.Data;
using System.Configuration;
using System.Text;
using System.Runtime.InteropServices;
using CAPICOM;

namespace ConservazioneWA.Utils
{
    public class CryptoHelper
    {
        [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int RetrieveCRL(IntPtr pContext, int timeout);

        [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool
            IsPKCS7CounterSigned([MarshalAs(UnmanagedType.AsAny)] object PKCS7Message, int messageLength);

        [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void
            GetASN1UnstructuredName(StringBuilder uName, int uNameMaxLen, [MarshalAs(UnmanagedType.AsAny)] object PKCS7Message, int messageLength);

        [DllImport("CryptoHelper.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetCertSignatureAlgorithm(IntPtr pContext, StringBuilder uName, int uNameMaxLen);   
    }
    public class CAPICOMHelper
    {
        public enum CAPICOM_CHAIN_STATUS
        {
            CAPICOM_TRUST_IS_NOT_TIME_VALID = 0x00000001,
            CAPICOM_TRUST_IS_NOT_TIME_NESTED = 0x00000002,
            CAPICOM_TRUST_IS_REVOKED = 0x00000004,
            CAPICOM_TRUST_IS_NOT_SIGNATURE_VALID = 0x00000008,
            CAPICOM_TRUST_IS_NOT_VALID_FOR_USAGE = 0x00000010,
            CAPICOM_TRUST_IS_UNTRUSTED_ROOT = 0x00000020,
            CAPICOM_TRUST_REVOCATION_STATUS_UNKNOWN = 0x00000040,
            CAPICOM_TRUST_IS_CYCLIC = 0x00000080,
            CAPICOM_TRUST_INVALID_EXTENSION = 0x00000100,
            CAPICOM_TRUST_INVALID_POLICY_CONSTRAINTS = 0x00000200,
            CAPICOM_TRUST_INVALID_BASIC_CONSTRAINTS = 0x00000400,
            CAPICOM_TRUST_INVALID_NAME_CONSTRAINTS = 0x00000800,
            CAPICOM_TRUST_HAS_NOT_SUPPORTED_NAME_CONSTRAINT = 0x00001000,
            CAPICOM_TRUST_HAS_NOT_DEFINED_NAME_CONSTRAINT = 0x00002000,
            CAPICOM_TRUST_HAS_NOT_PERMITTED_NAME_CONSTRAINT = 0x00004000,
            CAPICOM_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT = 0x00008000,
            CAPICOM_TRUST_IS_OFFLINE_REVOCATION = 0x01000000,
            CAPICOM_TRUST_NO_ISSUANCE_CHAIN_POLICY = 0x02000000,
            CAPICOM_TRUST_IS_PARTIAL_CHAIN = 0x00010000,
            CAPICOM_TRUST_CTL_IS_NOT_TIME_VALID = 0x00020000,
            CAPICOM_TRUST_CTL_IS_NOT_SIGNATURE_VALID = 0x00040000,
            CAPICOM_TRUST_CTL_IS_NOT_VALID_FOR_USAGE = 0x00080000,
        }
    }
}