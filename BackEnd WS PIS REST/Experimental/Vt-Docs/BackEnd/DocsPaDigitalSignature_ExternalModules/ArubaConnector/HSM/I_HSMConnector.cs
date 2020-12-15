using System;
namespace ArubaConnector
{
    interface I_HSMConnector
    {
        object createClient(string endPoindAddress);
        byte[] ControFirmaFileCADES(byte[] fileDafirmare, string aliasCertificatoDaControfirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, object client);
        byte[] FirmaFileCADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, bool cofirma, object client);
        byte[] FirmaFilePADES(byte[] fileDafirmare, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, bool marcaTemporale, object client);
        string OpenMultiSignSession(bool cosign, bool timestamp, int Type);
        bool richiediOTP(string aliasCertificato, string dominioCertificato, object client);
        bool Session_CloseMultiSign(string SessionToken);
        string Session_GetManifest(string SessionToken);
        string Session_GetSessions();
        byte[] Session_GetSignedFile(string SessionToken, string hashFileDaFirmare);
        string Session_PutFileToSign(string SessionToken, byte[] FileDafirmare, string FileName);
        bool Session_RemoteSign(string SessionToken, string aliasCertificato, string dominioCertificato, string pinCertificato, string otpFirma, object client);
        string VisualizzaCertificatoHSM(string aliasCertificato, string dominioCertificato, object client);
    }
}
