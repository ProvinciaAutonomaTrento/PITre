using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using InformaticaTrentinaPCL.Network;

namespace PinningSSL
{
    public class ServicePointConfig
    {
        public ServicePointConfig()
        {
        }

        public static void SetUp()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
        }

        private static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors
        )
        {
            if (null == certificate)
                return false;

            var pk = certificate.GetPublicKeyString();
			foreach (string currentKey in NetworkConstants.SupportedPublicKeyForPinning)
			{
				if (pk.Equals(currentKey))
				{
					return true;
				}
			}
            return false;
        }

    }
}
