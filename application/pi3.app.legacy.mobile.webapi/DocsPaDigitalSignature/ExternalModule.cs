// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace BusinessLogic.Documenti.DigitalSignature;



public enum EsitoVerificaStatus
{
    Valid = 0,         //OK
    NotTimeValid = 1,  //Scaduto
    Revoked = 4,       //Revocato
    CtlNotTimeValid = 131072, //Data non corretta
    ErroreGenerico = -1,
    SHA1NonSupportato = -2
}

[Serializable]
public class EsitoVerifica
{
    public EsitoVerificaStatus status;
    public string message;
    public string errorCode;
    public DateTime? dataRevocaCertificato;
    public string SubjectDN;
    public string SubjectCN;
    public string[] additionalData;
    public DocsPaVO.documento.VerifySignatureResult VerifySignatureResult;
    public byte[] content;
}

class ExternalModule
{
    private ILogger logger = Serilog.Log.ForContext(typeof(ExternalModule));

    private EsitoVerifica esito;
    public EsitoVerifica Esito
    {
        get
        {
            return esito;
        }
    }

    /*
    public DocsPaVO.documento.Certificate[] GetHSMCertificateList(string aliasCertificato, string dominioCertificato)
    {
        DocsPaVO.documento.Certificate[] retValue = null;
        try
        {
            string certificate = RemoteSignature.GetHSMCertificateList(aliasCertificato, dominioCertificato);
            if (!string.IsNullOrEmpty(certificate))
            {
                retValue = DeserializeObject<DocsPaVO.documento.Certificate[]>(certificate);
            }
        }
        catch (Exception e)
        {
            logger.Error("Errore in GetHSMCertificateList: " + e.Message);
            retValue = null;
        }

        return retValue;
    }
    */

    public bool externalClrCheckWS(string CLRWsUrl, string fileToverify, DateTime? dateVerifica, bool isPadesAndCades = false)
    {
        logger.Debug("START");
        if (esito == null)
            esito = new EsitoVerifica();

        if (string.IsNullOrEmpty(CLRWsUrl))
        {
            logger.Error(String.Format("l'Url del componente di verifica firma non è stato specificato"));
            throw new Exception(String.Format("l'Url del componente di verifica firma non è stato specificato"));
        }

        try
        {
#if false   // chiamata a servizio esterno
            externalService.ClrVerification ver = new externalService.ClrVerification();
            ver.Url = CLRWsUrl;
            ver.Timeout = 100000;
            String retval = null;

            if (dateVerifica == DateTime.MinValue)
                dateVerifica = null;

            if (ver.FileLocale(fileToverify))  //Controllo l'sistenza del file locale al server
                retval = ver.VerificaCertificatoFileLocale(fileToverify, dateVerifica, false, isPadesAndCades);
            else //nel caso non ci fosse lo mando...
                retval = ver.VerificaCertificato(System.IO.File.ReadAllBytes(fileToverify), dateVerifica, false, isPadesAndCades);


            esito = DeserializeObject<EsitoVerifica>(retval);
            if (esito.status == EsitoVerificaStatus.Valid)
            {
                logger.Info("externalClrCheck Certificato OK");
                return true;
            }
            if (esito.status == EsitoVerificaStatus.NotTimeValid)
            {
                logger.Debug("externalClrCheck Certificato SCADUTO o REVOCATO ");
                return false;
            }
            if (esito.status == EsitoVerificaStatus.SHA1NonSupportato)
            {
                logger.Debug("externalClrCheck Certificato IN SHA1 non supportato");
                return false;
            }
            if (esito.status == EsitoVerificaStatus.Revoked)
            {
                logger.Debug(String.Format("Certificato Revocato in data:{0} per {1} : {2} ", esito.dataRevocaCertificato, esito.SubjectCN, esito.SubjectDN));
                return false;
            }
            if (esito.status == EsitoVerificaStatus.ErroreGenerico)
            {
                logger.Debug(String.Format("ErroreGenerico {0}", retval));
                //esito.message = retval;
                return false;
            }

#endif
        }
        catch (Exception e)
        {
            string msgerr = String.Format("eccezione in externalClrCheck: {0} \r\n {1}", e.Message, e.StackTrace);
            logger.Error(msgerr);
            esito.message = msgerr;
            throw e;
        }
        return false;
    }

    public bool externalCertCheckWsBatch(string CLRWsUrl, List<DocsPaVO.documento.CertificateInfo> certificateToVerifyLst)
    {
        logger.Debug("START");

        if (string.IsNullOrEmpty(CLRWsUrl))
        {
            logger.Error(String.Format("l'Url del componente di verifica firma non è stato specificato"));
            throw new Exception(String.Format("l'Url del componente di verifica firma non è stato specificato"));
        }

        try
        {
#if false   // chiamata a servizio esterno
            externalService.ClrVerification ver = new externalService.ClrVerification();
            ver.Url = CLRWsUrl;
            ver.Timeout = 100000;

            String retval = null;
            for (int i = 0; i < certificateToVerifyLst.Count; i++)
            {
                DocsPaVO.documento.CertificateInfo cinfo = certificateToVerifyLst[i];
                retval = ver.DettaglioCertificato(cinfo.X509Certificate, null);
                cinfo = DeserializeObject<DocsPaVO.documento.CertificateInfo>(retval);
            }
#endif                
            return true;
        }
        catch (Exception e)
        {
            string msgerr = String.Format("eccezione in externalCertCheckWsBatch: {0} \r\n {1}", e.Message, e.StackTrace);
            logger.Error(msgerr);
            esito.message = msgerr;
            return false;
            throw e;
        }
    }

    public EsitoVerifica externalCertCheckWs(string CLRWsUrl, DocsPaVO.documento.CertificateInfo certificateToVerify)
    {
        logger.Debug("START");
        if (esito == null)
            esito = new EsitoVerifica();

        if (string.IsNullOrEmpty(CLRWsUrl))
        {
            logger.Error(String.Format("l'Url del componente di verifica firma non è stato specificato"));
            throw new Exception(String.Format("l'Url del componente di verifica firma non è stato specificato"));
        }

        try
        {
#if false   // chiamata a servizio esterno
            externalService.ClrVerification ver = new externalService.ClrVerification();
            ver.Url = CLRWsUrl;
            ver.Timeout = 100000;
            String retval = null;

            retval = ver.DettaglioCertificato(certificateToVerify.X509Certificate, null);
            logger.Debug("VERIFY_SIGNATURE: RETVAL " + retval);
            esito = DeserializeObject<EsitoVerifica>(retval);

#endif                
            return esito;
        }
        catch (Exception e)
        {
            string msgerr = String.Format("eccezione in externalCertCheckWs: {0} \r\n {1}", e.Message, e.StackTrace);
            logger.Error(msgerr);
            esito.message = msgerr;
            return esito;
            throw e;
        }
    }

    static public t DeserializeObject<t>(String pXmlizedString)
    {
        XmlSerializer xs = new XmlSerializer(typeof(t));
        MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
        XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

        try
        {
            return (t)xs.Deserialize(memoryStream);
        }
        catch (Exception e) { System.Console.WriteLine(e); return default(t); }

    }

    static Byte[] StringToUTF8ByteArray(String pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        Byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    static String UTF8ByteArrayToString(Byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        String constructedString = encoding.GetString(characters);
        return (constructedString);
    }


}
