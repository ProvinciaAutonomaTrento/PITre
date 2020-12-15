using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace ArubaConnector
{
    public class HsmConnectorFactory : I_HSMConnector
    {

        I_HSMConnector connector = null;
        public HsmConnectorFactory()
        {

            string connectorType=  ConfigurationManager.AppSettings["HSMSERVICETYPE"];

            //default
            if (string.IsNullOrEmpty ( connectorType ))
            {
                connector = new HSMConnector_arsss();
                return;
            }

            //è specificato.
            if (connectorType == "arsss")
                connector = new HSMConnector_arsss();
            else if (connectorType == "asspbQSS")
                connector = new HSMConnector_asspbQSS();
            else  //default
                connector = new HSMConnector_arsss();
            
        }


        public object createClient(string endPoindAddress)
        {
            return connector.createClient(endPoindAddress);
        }
    

        byte[] I_HSMConnector.ControFirmaFileCADES(byte[] fileDafirmare, string aliasCertificatoDaControfirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, object client)
        {
            return  connector.ControFirmaFileCADES(fileDafirmare, aliasCertificatoDaControfirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale, client);
        }

        byte[] I_HSMConnector.FirmaFileCADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma, object client)
        {
            return connector.FirmaFileCADES(fileDafirmare, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, marcaTemporale, cofirma, client);
        }

        byte[] I_HSMConnector.FirmaFilePADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, object client)
        {
            return connector.FirmaFilePADES(fileDafirmare,aliasCertificato,dominioCertificato,pinCertificato,otpFirma, marcaTemporale,client);
        }

        string I_HSMConnector.OpenMultiSignSession(bool cosign, bool timestamp, int Type)
        {
            return connector.OpenMultiSignSession(cosign, timestamp, Type);
        }

        bool I_HSMConnector.richiediOTP(string aliasCertificato, string dominioCertificato, object client)
        {
            return connector.richiediOTP(aliasCertificato, dominioCertificato, client);
        }

        bool I_HSMConnector.Session_CloseMultiSign(string SessionToken)
        {
            return connector.Session_CloseMultiSign(SessionToken);
        }

        string I_HSMConnector.Session_GetManifest(string SessionToken)
        {
            return connector.Session_GetManifest(SessionToken);
        }

        string I_HSMConnector.Session_GetSessions()
        {
            return connector.Session_GetSessions();
        }

        byte[] I_HSMConnector.Session_GetSignedFile(string SessionToken, string hashFileDaFirmare)
        {
            return connector.Session_GetSignedFile(SessionToken, hashFileDaFirmare);
        }

        string I_HSMConnector.Session_PutFileToSign(string SessionToken, byte[] FileDafirmare, string FileName)
        {
            return connector.Session_PutFileToSign(SessionToken, FileDafirmare, FileName);
        }

        bool I_HSMConnector.Session_RemoteSign(string SessionToken, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, object client)
        {
            return connector.Session_RemoteSign(SessionToken, aliasCertificato, dominioCertificato, pinCertificato, otpFirma, client); 
        }

        string I_HSMConnector.VisualizzaCertificatoHSM(string aliasCertificato, string dominioCertificato, object client)
        {
            return connector.VisualizzaCertificatoHSM( aliasCertificato,  dominioCertificato, client);
        }


    }
}