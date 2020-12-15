using System;
using System.Collections;
using System.ComponentModel;
using System.Web.Services;
using System.Xml.Serialization;
using System.Xml;
using System.Configuration;
using System.IO;
using System.Data;
using log4net;

namespace DocsPaWS
{
    /// <summary>
    /// WEB SERVICES utilizzati dall'applicazione RDE
    /// </summary>   
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [WebService(Namespace = "http://localhost")]
    public class DocsPaWSRDE : System.Web.Services.WebService
    {
        private ILog logger = LogManager.GetLogger(typeof(DocsPaWSRDE));
        [WebMethod]
        public DocsPaVO.utente.Utente LoginRDE(DocsPaVO.utente.Login login)
        {
            DocsPaVO.utente.Utente utente = null;
            try
            {
                DocsPaVO.utente.UserLogin userLogin = new DocsPaVO.utente.UserLogin(login.userName,
                    login.password,
                    login.idAmministrazione,
                    login.dominio,
                    null,
                    login.update);

                DocsPaVO.utente.UserLogin.LoginResult loginResult;
                string ipaddress = "";
                //utente = BusinessLogic.Utenti.Login.loginMethod(userLogin, out loginResult, true,
                //    null, out ipaddress);
                utente = BusinessLogic.RDE.Rde.loginMethod(userLogin, out loginResult, true, null, out ipaddress);
            }
            catch (Exception e)
            {
                logger.Error("RDE - Errore durante la login.", e);
                utente = null;
            }
            return utente;
        }

        [WebMethod]
        public bool LogoffRDE(DocsPaVO.utente.InfoUtente infoUtente)
        {
            bool result = false;
            try
            {
                BusinessLogic.Utenti.Login.logoff_RDE(infoUtente.userId, "");		
                result = true;
            }
            catch (Exception e)
            {
                logger.Error("RDE - Errore durante il logoff.", e);
                result = false;               
            }
            return result;
        }

        [WebMethod]
        public string EsportaUtenti()
        {
            string xmlDoc = string.Empty;
            try
            {
                xmlDoc = BusinessLogic.RDE.Rde.EsportaUtenti();
            }
            catch (Exception e)
            {
                logger.Error("RDE - Errore durante l'esportazione degli utenti.", e);
            }
            return xmlDoc;
        }

        [WebMethod]
        public string EsportaRegistri()
        {
            string xmlDoc = string.Empty;
            try
            {
                xmlDoc = BusinessLogic.RDE.Rde.EsportaRegistri();
            }
            catch (Exception e)
            {
                logger.Error("RDE - Errore durante l'esportazione dei registri.", e);
            }
            return xmlDoc;
        }

        [WebMethod]
        public string EsportaAutorizzazioni()
        {
            string xmlDoc = string.Empty;
            try
            {
                xmlDoc = BusinessLogic.RDE.Rde.EsportaAutorizzazioni();
            }
            catch (Exception e)
            {
                logger.Error("RDE - Errore durante l'esportazione delle autorizzazioni.", e);
            }
            return xmlDoc;
        }

        [WebMethod]
        public DocsPaVO.documento.resultProtoEmergenza documentoImportaProtocolloEmergenza(DocsPaVO.documento.ProtocolloEmergenza protoEmergenza, DocsPaVO.utente.InfoUtente infoUtente)
        {

            DocsPaVO.documento.resultProtoEmergenza res = null;
            try
            {
                res = BusinessLogic.Documenti.ProtocolloEmergenza.importaProtoEmergenza(protoEmergenza, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("RDE  - Errore durante l'importazione del Protocollo di Emergenza", e);
                return res;
            }
            return res;
        }

        [WebMethod]
        public DocsPaVO.documento.resultProtoEmergenza documentoImportaProtocolloEmergenzaGrigi(DocsPaVO.documento.ProtocolloEmergenzaGrigi documento, DocsPaVO.utente.InfoUtente infoUtente)
        {
            DocsPaVO.documento.resultProtoEmergenza res = null;
            try
            {
                res = BusinessLogic.Documenti.ProtocolloEmergenza.importaProtoEmergenzaGrigi(documento, infoUtente);
            }
            catch (Exception e)
            {
                logger.Error("RDE  - Errore durante l'importazione del Protocollo di Emergenza (documenti grigi)", e);
                return res;
            }
            return res;
        }
    }
}
