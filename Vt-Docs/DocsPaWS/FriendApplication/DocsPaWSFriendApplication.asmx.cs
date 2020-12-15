using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Collections;
using log4net;

namespace DocsPaWS
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    
    public class DocsPaWSFriendApplication : System.Web.Services.WebService
    {
        private ILog logger = LogManager.GetLogger(typeof(DocsPaWSFriendApplication));
        private DocsPaWebService WS = new DocsPaWebService();
        private DocsPaWS.SmartServices.DocsPaServices WS_Smart = new DocsPaWS.SmartServices.DocsPaServices();

        #region WebMethod
        [WebMethod]
        public DocsPaVO.FriendApplication.SchedaDocumento DocumentoProtocolla(string friendApplication, string codiceRegistro, DocsPaVO.FriendApplication.SchedaDocumento schedaDocumento)
        {
            try
            {
                DocsPaVO.documento.SchedaDocumento schedaDocumentoDocsPaResult = null;
                DocsPaVO.FriendApplication.FriendApplication objFriendApplication = BusinessLogic.FriendApplication.FriendApplicationManager.getFriendApplication(friendApplication, codiceRegistro);
                
                if (objFriendApplication == null)
                    throw new Exception("Friend application non censita in docspa");

                string token = WS_Smart.GetToken(objFriendApplication.utente, objFriendApplication.ruolo);
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaWS.SmartServices.Utils.getInfoUtenteFromToken(token);
                DocsPaVO.documento.SchedaDocumento schedaDocumentoTrasformata = this.getSchedaDocumentoDocsPa(schedaDocumento, objFriendApplication, infoUtente);

                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
                schedaDocumentoDocsPaResult = WS_Smart.DocumentoProtocolla(token, schedaDocumentoTrasformata, out risultatoProtocollazione);

                if (schedaDocumentoDocsPaResult != null)
                {
                    DocsPaVO.FriendApplication.SchedaDocumento schedaResultLite = this.getSchedaDocumentoLite(schedaDocumentoDocsPaResult, schedaDocumento);
                    return schedaResultLite;
                }
                else
                {
                    logger.Debug("Errore in DocsPaWSFriendApplication - metodo : DocumentoProtocolla " + risultatoProtocollazione.ToString());
                    throw new Exception("ERRORE - Creazione protocollo " + risultatoProtocollazione.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWSFriendApplication - metodo : DocumentoProtocolla", ex);
                throw ex;
            }
        }

        [WebMethod]
        public bool DocumentoAnnullaProtocollo(string friendApplication, string codiceRegistro, string segnatura, string noteAnnullamento)
        {
            try
            {
                DocsPaVO.FriendApplication.FriendApplication objFriendApplication = BusinessLogic.FriendApplication.FriendApplicationManager.getFriendApplication(friendApplication, codiceRegistro);
                
                if (objFriendApplication == null)
                    throw new Exception("Friend application non censita in docspa");

                string token = WS_Smart.GetToken(objFriendApplication.utente, objFriendApplication.ruolo);
                DocsPaVO.utente.InfoUtente infoUtente = DocsPaWS.SmartServices.Utils.getInfoUtenteFromToken(token);

                DocsPaVO.documento.SchedaDocumento schedaDocumentoDocsPa = BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infoUtente);

                if (schedaDocumentoDocsPa != null && schedaDocumentoDocsPa.accessRights != null && schedaDocumentoDocsPa.accessRights != "")
                {
                    if (HMDiritti(schedaDocumentoDocsPa.accessRights))
                    {
                        logger.Debug("Errore in DocsPaWSFriendApplication - metodo : DocumentoAnnullaProtocollo : l'utente non possiede i diritti necessari per annullare il protocollo");
                        throw new Exception("ERRORE - Annullamento protocollo : l'utente non possiede i diritti necessari per annullare il protocollo");
                    }
                }

                bool annullato = false;
                if (schedaDocumentoDocsPa != null)
                {

                    DocsPaVO.documento.ProtocolloAnnullato protoAnnullato = new DocsPaVO.documento.ProtocolloAnnullato();
                    protoAnnullato.autorizzazione = noteAnnullamento;
                    annullato = this.WS.DocumentoExecAnnullaProt(infoUtente, ref schedaDocumentoDocsPa, protoAnnullato);
                }
                else
                {
                    logger.Debug("Errore in DocsPaWSFriendApplication - metodo : DocumentoAnnullaProtocollo : protocollo da annulla non trovato");
                    throw new Exception("ERRORE - Annullamento protocollo : protocollo da annullare non trovato");
                }

                return annullato;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWSFriendApplication - metodo : DocumentoAnnullaProtocollo", ex);
                throw ex;
            }
        }
        #endregion WebMethod

        #region Utility
        private DocsPaVO.utente.Corrispondente ricercaCorr(DocsPaVO.FriendApplication.CorrLite corrispondente, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                DocsPaVO.utente.Corrispondente corr = null;
                //Verifica preesistenza della descrizione del corrispondente
                if (corrispondente.codice != null && corrispondente.codice != "")
                {
                    DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                    qco.codiceRubrica = corrispondente.codice;
                    qco.getChildren = false;
                    qco.idAmministrazione = infoUtente.idAmministrazione;
                    qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;
                    ArrayList corrls = this.WS.AddressbookGetListaCorrispondenti(qco);
                    if (corrls != null && corrls.Count > 0)
                        corr = (DocsPaVO.utente.Corrispondente)corrls[0];
                }
                if (corr != null && corr.systemId != null && corr.systemId != "")
                {
                    //Trovato, cotrollo se la descrizione è quella inserita dall'utente
                    if (corrispondente.descrizione.ToUpper() == corr.descrizione.ToUpper())
                    {
                        //OK, è lo stesso, quindi si usa quello della rubrica di docspa, uso  corr	
                    }
                    else
                    {
                        //Creo un OCC con la descrizione passata dall'utente
                        corr = new DocsPaVO.utente.Corrispondente();
                        corr.descrizione = corrispondente.descrizione;
                        corr.tipoCorrispondente = "O";
                        corr.idAmministrazione = infoUtente.idAmministrazione;

                    }
                }
                else
                {
                    //Non trovato, creo OCC
                    corr = new DocsPaVO.utente.Corrispondente();
                    corr.descrizione = corrispondente.descrizione;
                    corr.tipoCorrispondente = "O";
                    corr.idAmministrazione = infoUtente.idAmministrazione;

                }

                return corr;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWSFriendApplication - metodo : ricercaCorr", ex);
                throw ex;
            }
        }

        private DocsPaVO.documento.SchedaDocumento getSchedaDocumentoDocsPa(DocsPaVO.FriendApplication.SchedaDocumento schedaDocumentoLite, DocsPaVO.FriendApplication.FriendApplication objFriendApplication, DocsPaVO.utente.InfoUtente infoUtente)
        {
            try
            {
                //Creo e valorizzo una Scheda Documento
                DocsPaVO.documento.SchedaDocumento schedaDocumentoTrasformata = new DocsPaVO.documento.SchedaDocumento();
                schedaDocumentoTrasformata.appId = "ACROBAT";
                schedaDocumentoTrasformata.idPeople = objFriendApplication.idPeopleFactory;
                schedaDocumentoTrasformata.typeId = "LETTERA";
                schedaDocumentoTrasformata.userId = objFriendApplication.utente.userId;
                schedaDocumentoTrasformata.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                schedaDocumentoTrasformata.noteDocumento.Add(new DocsPaVO.Note.InfoNota(schedaDocumentoLite.note, objFriendApplication.utente.systemId, objFriendApplication.ruolo.systemId, null));
                DocsPaVO.documento.Oggetto oggetto = new DocsPaVO.documento.Oggetto();
                oggetto.descrizione = schedaDocumentoLite.oggetto;
                schedaDocumentoTrasformata.oggetto = oggetto;
                
                //Protocollo in Partenza
                if (schedaDocumentoLite.tipoProto.ToUpper() == "P".ToUpper())
                {
                    DocsPaVO.documento.ProtocolloUscita protoPartenza = new DocsPaVO.documento.ProtocolloUscita();
                    
                    //Aggiungo il mittente
                    if (schedaDocumentoLite.mittente != null)
                    {
                        DocsPaVO.utente.Corrispondente mittente = ricercaCorr(schedaDocumentoLite.mittente, infoUtente);
                        if (mittente != null)
                            protoPartenza.mittente = mittente;
                    }
                    
                    //Aggiungo i destinatari
                    protoPartenza.destinatari = new ArrayList();
                    foreach (DocsPaVO.FriendApplication.CorrLite corrLite in schedaDocumentoLite.destinatari)
                    {
                        protoPartenza.destinatari.Add(ricercaCorr(corrLite,infoUtente));

                    }

                    schedaDocumentoTrasformata.protocollo = protoPartenza;
                    schedaDocumentoTrasformata.tipoProto = "P";
                }

                //Protocollo in Arrivo
                if (schedaDocumentoLite.tipoProto.ToUpper() == "A".ToUpper())
                {
                    DocsPaVO.documento.ProtocolloEntrata protoArrivo = new DocsPaVO.documento.ProtocolloEntrata();

                    //Aggiungo il mittente
                    DocsPaVO.utente.Corrispondente mittente = ricercaCorr(schedaDocumentoLite.mittente, infoUtente);
                    protoArrivo.mittente = mittente;

                    schedaDocumentoTrasformata.protocollo = protoArrivo;
                    schedaDocumentoTrasformata.tipoProto = "A";
                }

                //Protocollo Interno
                if (schedaDocumentoLite.tipoProto.ToUpper() == "I".ToUpper())
                {
                    DocsPaVO.documento.ProtocolloInterno protoInterno = new DocsPaVO.documento.ProtocolloInterno();

                    //Aggiungo il mittente
                    DocsPaVO.utente.Corrispondente mittente = ricercaCorr(schedaDocumentoLite.mittente, infoUtente);
                    protoInterno.mittente = mittente;

                    //Aggiungo i destinatari
                    protoInterno.destinatari = new ArrayList();
                    foreach (DocsPaVO.FriendApplication.CorrLite corrLite in schedaDocumentoLite.destinatari)
                    {
                        protoInterno.destinatari.Add(ricercaCorr(corrLite, infoUtente));

                    }

                    schedaDocumentoTrasformata.protocollo = protoInterno;
                    schedaDocumentoTrasformata.tipoProto = "I";
                }

                schedaDocumentoTrasformata.registro = objFriendApplication.registro;
                return schedaDocumentoTrasformata;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWSFriendApplication - metodo : setSchedaDocumento", ex);
                throw ex;
            }
        }

        private DocsPaVO.FriendApplication.SchedaDocumento getSchedaDocumentoLite(DocsPaVO.documento.SchedaDocumento schedaDocumento, DocsPaVO.FriendApplication.SchedaDocumento schedaDocumentoLite)
        {
            try
            {
                schedaDocumentoLite.systemId = schedaDocumento.systemId;
                schedaDocumentoLite.docNumber = schedaDocumento.docNumber;
                schedaDocumentoLite.codiceRegistro = schedaDocumento.registro.codRegistro;
                schedaDocumentoLite.segnatura = schedaDocumento.protocollo.segnatura;
                schedaDocumentoLite.dataCreazione = schedaDocumento.dataCreazione;

                //Protocollo in Uscita
                if (schedaDocumentoLite.tipoProto.ToUpper() == "P".ToUpper())
                {
                    DocsPaVO.documento.ProtocolloUscita protoPartenza = (DocsPaVO.documento.ProtocolloUscita) schedaDocumento.protocollo;
                    schedaDocumentoLite.dataProtocollo = protoPartenza.dataProtocollazione;
                    schedaDocumentoLite.numeroProtocollo = protoPartenza.numero;                    
                }

                //Protocollo in Arrivo
                if (schedaDocumentoLite.tipoProto.ToUpper() == "A".ToUpper())
                {
                    DocsPaVO.documento.ProtocolloEntrata protoArrivo = (DocsPaVO.documento.ProtocolloEntrata)schedaDocumento.protocollo;
                    schedaDocumentoLite.dataProtocollo = protoArrivo.dataProtocollazione;
                    schedaDocumentoLite.numeroProtocollo = protoArrivo.numero;                    
                }

                //Protocollo Interno
                if (schedaDocumentoLite.tipoProto.ToUpper() == "I".ToUpper())
                {
                    DocsPaVO.documento.ProtocolloInterno protoInterno = (DocsPaVO.documento.ProtocolloInterno)schedaDocumento.protocollo;
                    schedaDocumentoLite.dataProtocollo = protoInterno.dataProtocollazione;
                    schedaDocumentoLite.numeroProtocollo = protoInterno.numero;                    
                }

                return schedaDocumentoLite;
            }
            catch (Exception ex)
            {
                logger.Debug("Errore in DocsPaWSFriendApplication - metodo : getSchedaDocumentoLite", ex);
                throw ex;
            }
        }

        private static bool HMDiritti(string accessRights)
        {
            bool disabilita = false;
            DocsPaVO.HMDiritti.HMdiritti HMD = new DocsPaVO.HMDiritti.HMdiritti();
            if ((Convert.ToInt32(accessRights) < HMD.HMdiritti_Write) || (Convert.ToInt32(accessRights) < HMD.HMdiritti_Eredita))
            {
                disabilita = true;
            }
            return disabilita;
        }
        #endregion Utility
    }
}
