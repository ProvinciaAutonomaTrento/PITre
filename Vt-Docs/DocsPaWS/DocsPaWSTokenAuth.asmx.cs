using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using log4net;

namespace DocsPaWS
{
    /// <summary>
    /// Summary description for DocsPaWSTokenAuth
    /// </summary>
    [WebService(Namespace = "http://www.etnoteam.it/docspawstokenauth")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class DocsPaWSTokenAuth : System.Web.Services.WebService
    {
        private static ILog logger = LogManager.GetLogger(typeof(DocsPaWSTokenAuth));
        private DocsPaVO.utente.Utente Utente;
        private DocsPaVO.utente.Ruolo Ruolo;
        private DocsPaVO.utente.Registro Registro;
        private DocsPaWS.DocsPaWebService WS;
        private string UserID;
        private string CodRuolo;
        private string CodReg;
        private string Token;
        private string IdAddress;
        private string SessionIdWSTokenAuth = "WSTokenAuth";
        protected DocsPaVO.utente.UserLogin userLogin;

        #region protocollazioneCompleta
        [WebMethod]
        public virtual bool protocollazioneCompleta(string token, DocsPaVO.documento.SchedaDocumento schedaDoc, DocsPaVO.documento.ProtocolloEntrata protoE,
                                                    DocsPaVO.documento.ProtocolloUscita protoU,
                                                    DocsPaVO.documento.ProtocolloInterno protoI,
                       out int numProto, out int AnnoProto, out string segnatura, out string dataProtocollo)
        {
            bool result;
            if (schedaDoc == null)
            {
                result = false;
                throw (new Exception("scheda documento vuota"));
            }
            schedaDoc.protocollo = null;
            if (schedaDoc.tipoProto.ToUpper().Equals("A"))
            {
                schedaDoc.protocollo = protoE;
            }
            else if (schedaDoc.tipoProto.ToUpper().Equals("U"))
            {
                schedaDoc.protocollo = protoU;
            }
            else if (schedaDoc.tipoProto.ToUpper().Equals("I"))
            {
                schedaDoc.protocollo = protoI;
            }
            else
            {
                result = false;
                throw (new Exception("tipo protocollo non valido"));
            }
            result = this.protocollazioneEstesa(token, schedaDoc, out numProto, out AnnoProto, out segnatura, out dataProtocollo);
            return result;
        }
        #endregion

        #region protocollazioneEstesa
        [WebMethod]
        public virtual bool protocollazioneEstesa(string token, DocsPaVO.documento.SchedaDocumento schedaDoc, out int numProto, out int AnnoProto, out string segnatura, out string dataProtocollo)
        {
            bool result;
            numProto = 0;
            AnnoProto = 0;

            #region verifica dati input
            if (token.Equals(""))
            {
                throw (new Exception("Campo Token obbligatorio"));
            }

            if (schedaDoc.oggetto == null || schedaDoc.oggetto.descrizione.Trim().Equals(""))
            {
                throw (new Exception("Campo Oggetto obbligatorio"));
            }

            if (schedaDoc.tipoProto == null || schedaDoc.tipoProto.Trim().Equals(""))
            {
                throw (new Exception("Campo TipoProtocollo obbligatorio"));
            }
            if (!schedaDoc.tipoProto.ToUpper().Equals("A") && !schedaDoc.tipoProto.ToUpper().Equals("P") && !schedaDoc.tipoProto.ToUpper().Equals("I"))
            {
                throw (new Exception("Campo TipoProtocollo non valido"));
            }
            if (schedaDoc.protocollo == null)
            {
                throw (new Exception("Dati Protocollo obbligatori"));
            }
            if (schedaDoc.tipoProto.Equals("A"))
            {
                if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente == null)
                    throw (new Exception("Mittente obbligatorio"));
                if (((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.descrizione == null ||
                    ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente.descrizione.Trim().Equals(""))
                    throw (new Exception("Mittente obbligatorio"));
            }

            if (schedaDoc.tipoProto.Equals("P"))
            {
                if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari == null
                    || ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count < 1)
                    throw (new Exception("Campo destinatari obbligatorio"));
                if (!verificaDatiCorrispondenti(((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari))
                    throw (new Exception("Campo destinatari: le descrizioni sono obbligatorie"));
            }

            if (schedaDoc.tipoProto.Equals("I"))
            {
                if (((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente == null)
                    throw (new Exception("Mittente obbligatorio"));
                if (((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.descrizione == null ||
                   ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente.descrizione.Trim().Equals(""))
                    throw (new Exception("Mittente obbligatorio"));
                if (((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari == null
                    || ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari.Count < 1)
                    throw (new Exception("Campo destinatari obbligatorio"));
                if (!verificaDatiCorrispondenti(((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari))
                    throw (new Exception("Campo destinatari: le descrizioni sono obbligatorie"));
            }
            #endregion verifica dati input

            try
            {
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                #region Protocollazione

                /*  controlla diritti (verifica la micro funzione associata a:
                    protocollazione
                    protocollazione in Giallo 
                    prot in arrivo/prot in partenza/prot interno */

                //if (!WSManager.verificaFunzionalita(Ruolo, "DO_PROT_PROTOCOLLA"))
                //    throw (new Exception("Il ruolo indicato non è abilitato a protocollare"));
                //if (WSManager.getStatoRegistro(Registro).Equals("G"))
                //{
                //    if (!WSManager.verificaFunzionalita(Ruolo, "DO_PROT_PROTOCOLLAG"))
                //        throw (new Exception("Il ruolo indicato non è abilitato a protocollare quando lo stato del registro è GIALLO"));
                //}

                //if (schedaDoc.tipoProto.Equals("A"))
                //{
                //    if (!WSManager.verificaFunzionalita(Ruolo, "PROTO_IN"))
                //        throw (new Exception("Il ruolo indicato non è abilitato alla protocollazione in arrivo"));
                //}else
                //    if (schedaDoc.tipoProto.Equals("P"))
                //    {
                //        if (!WSManager.verificaFunzionalita(Ruolo, "PROTO_OUT"))
                //            throw (new Exception("Il ruolo indicato non è abilitato alla protocollazione in partenza"));
                //    }
                //    else
                //        if (schedaDoc.tipoProto.Equals("I"))
                //        {
                //            if (!WSManager.verificaFunzionalita(Ruolo, "PROTO_OWN"))
                //                throw (new Exception("Il ruolo indicato non è abilitato alla protocollazione interna"));
                //        }

                schedaDoc = this.creaSchedaDocumento(schedaDoc);


                DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione;
                DocsPaVO.documento.SchedaDocumento schedaResult = this.WS.DocumentoProtocolla(schedaDoc, infoUtente, this.Ruolo, out risultatoProtocollazione);

                //inserisci i dettagli dei corrispondneti del protocollo
                if (schedaResult != null)
                    this.putinfoCorr(schedaResult);
                // controllo i risultati 
                if (schedaResult != null && risultatoProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK))
                {
                    #region riempi parametri di output

                    numProto = Int32.Parse(schedaResult.protocollo.numero);
                    AnnoProto = Int32.Parse(schedaResult.protocollo.anno);
                    segnatura = schedaResult.protocollo.segnatura;
                    dataProtocollo = schedaResult.protocollo.dataProtocollazione;
                    dataProtocollo = DateTime.Parse(dataProtocollo, new System.Globalization.CultureInfo("it-IT", false), System.Globalization.DateTimeStyles.None).ToString();
                    dataProtocollo = dataProtocollo.Replace("-", "/");
                    dataProtocollo = dataProtocollo.Substring(0, 10);

                    #endregion riempi parametri di output
                }
                else //	risultatoProtocollazione != DocsPaWR.ResultProtocollazione.OK)
                {
                    #region gestione errore
                    System.Text.StringBuilder descErr = new System.Text.StringBuilder("Errore in fase di protocollazione:", 100);
                    switch (risultatoProtocollazione)
                    {
                        case DocsPaVO.documento.ResultProtocollazione.REGISTRO_CHIUSO:
                            descErr.Append(" Registro chiuso.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.STATO_REGISTRO_ERRATO:
                            descErr.Append(" Stato del Registro errato.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_SUCCESSIVA_ATTUALE:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.DATA_ERRATA:
                            descErr.Append(" Data di protocollazione non valida.");
                            break;
                        case DocsPaVO.documento.ResultProtocollazione.FORMATO_SEGNATURA_MANCANTE:
                            descErr.Append(" Formato della segnatura non impostato. Contattare l'Amministratore");
                            break;
                        default:
                            descErr.AppendFormat(" Nessun dettaglio sul codice errore {0}.", risultatoProtocollazione.ToString());
                            break;
                    }
                    throw (new Exception(descErr.ToString()));

                    #endregion gestione errore
                }
                #endregion

                result = true;
            }
            catch (Exception e)
            {
                throw e;
                result = false;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }

            return result;
        }

        private void putinfoCorr(DocsPaVO.documento.SchedaDocumento sch)
        {
            if (sch != null && sch.protocollo != null)
            {
                if (sch.tipoProto.Equals("A"))
                {
                    DocsPaVO.documento.ProtocolloEntrata pe = (DocsPaVO.documento.ProtocolloEntrata)sch.protocollo;
                    if (pe.mittente != null
                        && pe.mittente.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pe.mittente);
                    if (pe.mittenteIntermedio != null
                        && pe.mittenteIntermedio.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pe.mittenteIntermedio);

                }
                if (sch.tipoProto.Equals("P"))
                {
                    DocsPaVO.documento.ProtocolloUscita pu = (DocsPaVO.documento.ProtocolloUscita)sch.protocollo;
                    if (pu.mittente != null
                        && pu.mittente.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pu.mittente);
                    if (pu.destinatari != null
                        && pu.destinatari.Count > 0)
                        for (int i = 0; i < pu.destinatari.Count; i++)
                        {
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)pu.destinatari[i];
                            if (corr != null && corr.info != null)
                                BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(corr);
                        }

                }
                if (sch.tipoProto.Equals("I"))
                {
                    DocsPaVO.documento.ProtocolloInterno pi = (DocsPaVO.documento.ProtocolloInterno)sch.protocollo;
                    if (pi.mittente != null
                        && pi.mittente.info != null)
                        BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(pi.mittente);
                    if (pi.destinatari != null
                        && pi.destinatari.Count > 0)
                        for (int i = 0; i < pi.destinatari.Count; i++)
                        {
                            DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)pi.destinatari[i];
                            if (corr != null && corr.info != null)
                                BusinessLogic.Utenti.UserManager.addDettagliCorrOcc(corr);
                        }

                }

            }
        }

        private bool verificaDatiCorrispondenti(ArrayList listaCorr)
        {
            bool trovato = false;
            for (int i = 0; i < listaCorr.Count && !trovato; i++)
            {
                DocsPaVO.utente.Corrispondente corr = (DocsPaVO.utente.Corrispondente)listaCorr[i];
                if (corr.descrizione != null &&
                    !corr.descrizione.Trim().Equals(""))
                    trovato = true;
            }
            return trovato;
        }
        private DocsPaVO.utente.Corrispondente verificaCorrispondente(DocsPaVO.utente.Corrispondente corrInput)
        {
            if (corrInput == null)
                return null;
            DocsPaVO.utente.Corrispondente corr = null;
            // verifica preesistenza della descrizione del corrispondente
            if (corrInput.codiceRubrica != null && corrInput.codiceRubrica != "")
            {
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.codiceRubrica = corrInput.codiceRubrica;
                qco.getChildren = false;
                qco.fineValidita = true;
                qco.idAmministrazione = Utente.idAmministrazione;  //verifica meglio
                // secondo me è solo esterno, ma per ora lascio GLOBALE.
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.GLOBALE;
                ArrayList corrls = this.WS.AddressbookGetListaCorrispondenti(qco);
                if (corrls != null && corrls.Count > 0)
                    corr = (DocsPaVO.utente.Corrispondente)corrls[0];
            }
            if (corr != null && corr.systemId != null && corr.systemId != "")
            {//trovato, cotrollo se la descrizione è quella inserita dall'utente:
                if (corrInput.descrizione.ToUpper() == corr.descrizione.ToUpper())
                { //ok, è lo stesso, quindi da specifica si usa quello della rubrica di docspa
                    //uso  corrInput	
                }
                else
                {
                    //creo un occ con la descrizione passata dall'utente
                    corr = new DocsPaVO.utente.Corrispondente();
                    corr.descrizione = corrInput.descrizione;
                    corr.tipoCorrispondente = "O";
                    corr.idAmministrazione = Utente.idAmministrazione;
                    corr.info = corrInput.info;
                }
            }
            else
            //non trovato, CREO OCC.
            {
                corr = new DocsPaVO.utente.Corrispondente();
                corr.descrizione = corrInput.descrizione;
                corr.tipoCorrispondente = "O";
                corr.idAmministrazione = Utente.idAmministrazione;
                corr.info = corrInput.info;
            }

            return corr;
        }

        private DocsPaVO.utente.Corrispondente verificaCorrispondenteInterno(DocsPaVO.utente.Corrispondente corrInput)
        {
            if (corrInput == null)
                return null;
            DocsPaVO.utente.Corrispondente corr = null;
            // verifica preesistenza della descrizione del corrispondente
            if (corrInput.codiceRubrica != null && corrInput.codiceRubrica != "")
            {
                DocsPaVO.addressbook.QueryCorrispondente qco = new DocsPaVO.addressbook.QueryCorrispondente();
                qco.codiceRubrica = corrInput.codiceRubrica;
                qco.getChildren = false;
                qco.fineValidita = true;
                qco.idAmministrazione = Utente.idAmministrazione;  //verifica meglio
                qco.tipoUtente = DocsPaVO.addressbook.TipoUtente.INTERNO;
                ArrayList corrls = this.WS.AddressbookGetListaCorrispondenti(qco);
                if (corrls != null && corrls.Count > 0)
                    corr = (DocsPaVO.utente.Corrispondente)corrls[0];
                else
                    throw (new Exception("Il corrispondente indicato non esiste o non è interno"));
            }
            else
            //non trovato, CREO OCC.
            {
                corr = null;
                throw (new Exception("Il corrispondente indicato non esiste o non è interno"));
            }

            return corr;
        }

        #endregion

        #region Annullamento
        /// <summary>
        /// annulla protocollo
        /// </summary>
        /// <param name="User_ID"></param>
        /// <param name="Pwd"></param>
        /// <param name="numProto"></param>
        /// <param name="annoProto"></param>
        /// <param name="noteAnnullamento"></param>
        /// <returns></returns>
        [WebMethod]
        public virtual bool annullaProtocollo(string token, string segnatura, string noteAnnullamento)
        {
            bool annullato = false;
            try
            {
                if (token.Equals(""))
                {
                    throw (new Exception("Campo Token obbligatorio"));
                }
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }

                //controlla diritti (verifica la micro funzione associata all'annullamento di un documento).
                //if (!WSManager.verificaFunzionalita(Ruolo, "DO_PROT_ANNULLA"))
                //    throw (new Exception("Il ruolo indicato non è abilitato ad annullare documenti"));

                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                string idReg = this.Registro.systemId;

                DocsPaVO.documento.SchedaDocumento sch = BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infoUtente);
                if (sch == null || sch.systemId == null)
                {
                    string ErrorMsg = "Documento con segnatura " + segnatura + " non trovato";
                    throw new Exception(ErrorMsg);
                }
                DocsPaVO.documento.ProtocolloAnnullato protoAnn = null;
                //TODO: controlla diritti. Verificare la micro funzione associata all'anullamento di un documento
                if (sch != null && sch.accessRights != null && sch.accessRights != "")
                {
                    if (WSManager.HMDiritti(sch.accessRights, Ruolo, null))
                    {
                        string ErrorMsg = "Attenzione, l'utente " + infoUtente.userId + " non possiede i diritti necessari per annullare il protocollo.";
                        throw new Exception(ErrorMsg);
                    }
                }
                else
                {
                    string ErrorMsg = "Non ci sono sufficienti dati per stabilire i diritti di accesso sul documento.";
                    throw new Exception(ErrorMsg);
                }


                if (sch != null)
                {
                    protoAnn = new DocsPaVO.documento.ProtocolloAnnullato();
                    protoAnn.autorizzazione = noteAnnullamento;
                    annullato = this.WS.DocumentoExecAnnullaProt(infoUtente, ref sch, protoAnn);
                }

                return annullato;
            }
            catch (Exception e)
            {
                throw e;
                return false;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();

            }
        }

        #endregion


        #region ricercaScheda

        [WebMethod]
        public DocsPaVO.documento.SchedaDocumento ricercaSchedabySegnatura(string segnatura, string token)
        {
            DocsPaVO.documento.SchedaDocumento sch = null;
            try
            {

                if (token.Equals(""))
                {
                    throw (new Exception("Campo Token obbligatorio"));
                }
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                sch = BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infoUtente);
                return sch;
            }
            catch (Exception e)
            { throw e; }

            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }


        }
        #endregion

        #region modificaScheda
        [WebMethod]
        public DocsPaVO.documento.SchedaDocumento modificaProtocollo(DocsPaVO.documento.SchedaDocumento sch, string token, out string ErrorMsg)
        {
            DocsPaVO.documento.SchedaDocumento result = null;
            ErrorMsg = "";
            try
            {
                if (token.Equals(""))
                {
                    throw (new Exception("Campo Token obbligatorio"));
                }
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                //controlla diritti (verifica la micro funzione associata alla modifica di un documento).
                //if (!WSManager.verificaFunzionalita(Ruolo, "DO_PROT_SALVA"))
                //{
                //    throw (new Exception("Il ruolo indicato non è abilitato a modificare documenti"));
                //}
                if (sch != null && sch.accessRights != null && sch.accessRights != "")
                {
                    if (WSManager.HMDiritti(sch.accessRights, Ruolo, null))
                    {
                        ErrorMsg = "Attenzione, l'utente " + infoUtente.userId + " non possiede i diritti necessari per la modifica del protocollo.";
                        throw new Exception(ErrorMsg);
                    }
                }
                else
                {
                    ErrorMsg = "Non ci sono sufficienti dati per stabilire i diritti di accesso sul documento.";
                    throw new Exception(ErrorMsg);
                }
                //controllo se il documento è annullato
                bool isAnnullato = BusinessLogic.Documenti.ProtoManager.isDocAnnullato(sch.docNumber);
                if (isAnnullato)
                    throw (new Exception("Il documento risulta annullato. Non può essere modificato"));

                bool uffref = false;
                result = this.WS.DocumentoSaveDocumento(null, infoUtente, sch, false, out uffref);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }

            return result;
        }
        #endregion


        #region Upload / DownLoad file

        /// <summary>
        /// permette l'upload, della versione corrente di un protocollo.
        /// </summary>
        /// <example> Questo Esempio mostra come utilizzare il webMethod
        /// <code>
        ///   //login; --> infoutente
        ///		DocspaVO.SchedaDocumento sch=ricercaScheda(segnatura);
        ///     FileRequest fileReq =sch.documento[0]; //ultima versione;
        ///		FileDocumento fileDoc = new FileDocumento();
        ///		file=HTTPPostedFile; //filestream ottenuto da wweb o da file system.					
        ///		fileDoc.name = System.IO.Path.GetFileName(file.FileName); //nome+path del file
        ///		fileDoc.fullName = fileDoc.name;
        ///		fileDoc.contentType = file.ContentType;
        ///		fileDoc.length = file.ContentLength;
        ///		fileDoc.content = new Byte[fileDoc.length];
        ///		file.InputStream.Read(fileDoc.content, 0, fileDoc.length); //copia dello stream nell'oggetto FileDoc
        ///		fileReq.fileName=System.IO.Path.GetFileName(file.FileName);
        ///		fileReq = docsPaWS.DocumentoPutFile(fileReq, fileDoc, infoUtente);
        /// </code>
        /// </example>
        [WebMethod]
        public DocsPaVO.documento.FileRequest putFile(DocsPaVO.documento.FileRequest fileReq, DocsPaVO.documento.FileDocumento fileDoc, string token)
        {
            DocsPaVO.documento.FileRequest fileReqOut = null;
            try
            {
                if (token.Equals(""))
                {
                    throw (new Exception("Campo Token obbligatorio"));
                }
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                //controllo se il documento è annullato
                bool isAnnullato = BusinessLogic.Documenti.ProtoManager.isDocAnnullato(fileReq.docNumber);
                if (isAnnullato)
                    throw (new Exception("Il documento risulta annullato. Non può essere modificato"));
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);
                return fileReqOut = this.WS.DocumentoPutFile(fileReq, fileDoc, infoUtente);
            }
            catch (Exception ex)
            { throw ex; }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }
        }
        /// <summary>
        /// permette il donwload del file associato alla versione corrente del protocollo.
        /// </summary>
        /// /// <example> Questo Esempio mostra come utilizzare il webMethod
        /// <code>
        ///   //login; --> infoutente
        ///   DocsPaWR.FileDocumento fileDoc=getfile(segnatura,infoutente);
        ///		fileDoc.name //nome+path del file
        ///		fileDoc.fullName //nome file nel sistema docspa;
        ///		fileDoc.contentType ;
        ///		fileDoc.length ;
        ///		fileDoc.content ; //content per richeare uno stream da cui ottenere il file.
        /// </code>
        /// </example>
        /// <param name="segnatura"></param>
        /// <param name="infoUt"></param>
        /// <returns></returns>
        [WebMethod]
        public DocsPaVO.documento.FileDocumento getFile(string segnatura, string token)
        {
            DocsPaVO.documento.FileDocumento fileDoc = null;

            try
            {
                if (token.Equals(""))
                {
                    throw (new Exception("Campo Token obbligatorio"));
                }
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                DocsPaVO.documento.SchedaDocumento sch = BusinessLogic.Documenti.ProtoManager.ricercaScheda(segnatura, infoUtente);

                if (sch != null && sch.documenti != null && sch.documenti.Count > 0)
                {
                    fileDoc = this.WS.DocumentoGetFile((DocsPaVO.documento.FileRequest)sch.documenti[0], infoUtente);
                }
                return fileDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }
        }
        #endregion


        /// <summary>
        /// permette il donwload del file associato alla versione corrente del protocollo.
        /// </summary>
        /// <param name="fileRequest"></param>
        /// <param name="fileDocumento"></param>
        /// <param name="token"></param>
        /// <param name="firmato">booleano che indica se true si deve restituire il content del file firmato, se false il content del file originale</param>
        /// <returns></returns>
        [WebMethod]
        public DocsPaVO.documento.FileDocumento getFileToken(DocsPaVO.documento.FileRequest fileRequest, string token, bool firmato, out bool result)
        {

            result = false;
            try
            {
                if (token.Equals(""))
                {
                    throw (new Exception("Campo Token obbligatorio"));
                }
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                DocsPaVO.documento.FileDocumento fileDocumento = null;
                if (fileRequest != null
                    && !string.IsNullOrEmpty(fileRequest.fileName)
                    && fileRequest.fileName.ToUpper().Contains("P7M")
                    && firmato)
                    fileDocumento = BusinessLogic.Documenti.FileManager.getFileFirmato(fileRequest, infoUtente, false);
                else
                    fileDocumento = BusinessLogic.Documenti.FileManager.getFile(fileRequest, infoUtente);

                if (fileDocumento != null)
                {
                    result = true;
                    return fileDocumento;
                }
            }
            catch (Exception e)
            {
                logger.Debug("errore nel metodo getFileToken - errore: " + e.Message + "; StackTrace: " + e.StackTrace);
            }
            return null;
        }
        [WebMethod]
        public virtual bool InsertCorrispondenteEsterno(string token, string codice, string codrubrica,
            string descrizione, string tipo_urp, string email, string indirizzo, string cap, string citta, string prov,
            string cognome, string nome)
        {
            #region Controllo dati in input

            if (token == null || token.Length == 0)
                throw (new Exception("Campo token mancante"));

            if (codice == null || codice.Length == 0)
                throw (new Exception("Campo codice mancante"));

            if (codrubrica == null || codrubrica.Length == 0)
                throw (new Exception("Campo codice rubrica mancante"));

            if (tipo_urp == null || tipo_urp.Length == 0 || (tipo_urp.ToUpper() != "U" && tipo_urp.ToUpper() != "P"))
                throw (new Exception("Tipo corrispondente (U/P) errato o mancante"));

            if (tipo_urp == "P")
            {
                if (cognome == null || cognome.Length == 0)
                    throw (new Exception("Campo cognome mancante"));
                if (nome == null || nome.Length == 0)
                    throw (new Exception("Campo nome mancante"));
            }

            if (tipo_urp == "U")
            {
                if (descrizione == null || descrizione.Length == 0)
                    throw (new Exception("Campo descrizione mancante"));
            }

            if (email != null && email.Length > 0)
            {
                if (!WSManager.ValidateEmail(email))
                    throw (new Exception("Indirizzo email non conforme"));
            }

            #endregion

            try
            {
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                DocsPaVO.utente.InfoUtente infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);
                string idRegistro = Registro.systemId;
                return WSManager.insertCorrEsterno(codice, codrubrica,
                                            descrizione, tipo_urp.ToUpper(),
                                            email, indirizzo, cap, citta, prov,
                                            cognome, nome, infoUtente, idRegistro, WS);

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }

        }


        private bool Login(string UserId, string CodRuolo, string CodReg)
        {
            //			true  - Login effettuato
            //			false - errore
            bool retValue = false;
            this.Ruolo = null;
            try
            {
                #region login
                userLogin = new DocsPaVO.utente.UserLogin();
                userLogin.UserName = UserID;
                userLogin.Token = this.Token;
                userLogin.Update = false;

                //prova a loggarsi come un utente
                DocsPaVO.utente.UserLogin.LoginResult Lr = this.loginOnDocspa(userLogin, false);
                System.Text.StringBuilder descErr = new System.Text.StringBuilder("Errore in fase di Autenticazione a DocsPa: ", 150);

                if (Lr.Equals(DocsPaVO.utente.UserLogin.LoginResult.USER_ALREADY_LOGGED_IN))
                {
                    descErr.Append(" Utente già connesso. ");
                    //riprova forzando l'autenticazione
                    Lr = this.loginOnDocspa(userLogin, true);
                }
                retValue = WSManager.verificaRisposta(Lr, out descErr);

                if (retValue == false)
                {
                    throw (new Exception(descErr.ToString()));
                }

                #endregion login

                if (this.Utente == null)
                    throw (new Exception("Utente non valido"));

                // verifica ruolo e carica ruolo
                getRuolo(Utente, CodRuolo);
                if (this.Ruolo == null)
                {
                    throw (new Exception("codice Ruolo non valido"));
                }
                getRegistro(Ruolo, CodReg);
                if (this.Registro == null)
                {
                    throw (new Exception("codice Registro non valido"));
                }

                retValue = true;

            }
            catch (System.SystemException ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                retValue = false;
            }
            return retValue;
        }

        #region DATI DAL TOKEN
        internal string getInfoFromToken(string tipoInfo, string token)
        {
            string result = "";
            char[] sep = { '&' };
            string[] dati = token.Split(sep);
            if (dati.Length != 4)
            {
                return result;
            }
            switch (tipoInfo)
            {
                case "USER_ID":
                    return dati[0];
                case "COD_RUOLO":
                    return dati[1];
                case "COD_REG":
                    return dati[2];
                default:
                    return result;
            }

        }
        internal bool getRuolo(DocsPaVO.utente.Utente utente, string codRuolo)
        {
            bool result = false;
            bool trovato = false;
            try
            {
                //provo prima tra i ruoli dell'utente
                if (utente.ruoli != null && utente.ruoli.Count > 0)
                {
                    for (int i = 0; i < utente.ruoli.Count && !trovato; i++)
                    {
                        if (((DocsPaVO.utente.Ruolo)utente.ruoli[i]).codiceRubrica.ToUpper().Equals(codRuolo.ToUpper()))
                        {
                            this.Ruolo = (DocsPaVO.utente.Ruolo)utente.ruoli[i];
                            trovato = true;
                        }
                    }
                }
                //se l'utente non ha quel ruolo lo ricerco e lo associo in automatico 
                if (!trovato)
                {
                    this.Ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getCorrispondenteByCodRubrica(codRuolo, DocsPaVO.addressbook.TipoUtente.INTERNO, utente.idAmministrazione);
                    this.Ruolo = (DocsPaVO.utente.Ruolo)BusinessLogic.Utenti.UserManager.getRuolo(Ruolo.systemId);
                    {
                        //associo il ruolo all'utente
                        DocsPaVO.utente.Ruolo[] ruoli = new DocsPaVO.utente.Ruolo[1];
                        ruoli[0] = Ruolo;
                        try
                        {
                            BusinessLogic.Amministrazione.UtenteManager.SetRuoliUtente(ruoli, null, utente);
                            return true;
                        }
                        catch (Exception e)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                this.Ruolo = null;
                return result;
            }
            return result;
        }
        internal bool getRegistro(DocsPaVO.utente.Ruolo ruolo, string codReg)
        {
            bool trovato = false;
            if (ruolo != null && ruolo.registri != null && ruolo.registri.Count > 0)
            {
                for (int i = 0; i < ruolo.registri.Count && !trovato; i++)
                {
                    if (((DocsPaVO.utente.Registro)(ruolo.registri[i])).codRegistro.ToUpper().Equals(codReg.ToUpper()))
                    {
                        this.Registro = (DocsPaVO.utente.Registro)ruolo.registri[i];
                        trovato = true;
                    }
                }
            }
            return trovato;
        }
        #endregion

        private DocsPaVO.documento.SchedaDocumento creaSchedaDocumento(DocsPaVO.documento.SchedaDocumento schedaDoc)
        {

            if (schedaDoc == null)
                throw (new Exception("Errore nella fase di inizializzazione del nuovo documento"));

            #region campi scheda costanti
            //schedaDoc.systemId = null;
            schedaDoc.privato = "0";  //doc non privato

            // campi obbligatori per DocsFusion
            schedaDoc.idPeople = this.Utente.idPeople;
            schedaDoc.userId = this.Utente.userId;
            schedaDoc.typeId = "LETTERA";
            schedaDoc.appId = "ACROBAT";
            #endregion campi scheda costanti

            #region carica registro
            schedaDoc.registro = this.Registro;

            #region verifica che la protocollazione avvenga in data odierna
            switch (WSManager.getStatoRegistro(schedaDoc.registro))
            {
                case "V": break;// = Verde -  APERTO
                case "R": // = Rosso -  CHIUSO
                    {
                        throw (new Exception("Stato del registro non valido"));
                    }
                case "G":// = Giallo - APERTO IN GIALLO 
                    {
                            throw (new Exception("Data apertura del registro non valida"));
                        break;
                    }
                default:
                    {
                        throw (new Exception("Stato del registro non valido"));
                    }
            }
            #endregion verifica che la protocollazione avvenga in data odierna
            #endregion carica registro

            #region crea oggetto scheda
            //schedaDoc.oggetto.descrizione = schedaDoc.oggetto.descrizione.Replace("'", "''");
            #endregion crea oggetto scheda

            #region crea note scheda

            #endregion crea note scheda

            #region crea data protocollo
            if (schedaDoc.registro.dataApertura != null)
                schedaDoc.protocollo.dataProtocollazione = schedaDoc.registro.dataApertura;
            else
                throw (new Exception("data ultimo protocollo del registro selezionato non è settata"));
            #endregion crea data protocollo

            #region Tipologia Protocollo
            //protocollo in partenza
            if (schedaDoc.tipoProto == "P")
            {
                DocsPaVO.utente.Corrispondente corr;
                //verifica mittente
                corr = ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente;
                ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).mittente = verificaCorrispondente(corr);
                //verifica destinatari
                for (int i = 0; i < ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari.Count; i++)
                {
                    corr = (DocsPaVO.utente.Corrispondente)((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari[i];
                    ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatari[i] = verificaCorrispondente(corr);
                }
                //verifica destinatari CC
                if (((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza != null)
                {
                    for (int i = 0; i < ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza.Count; i++)
                    {
                        corr = (DocsPaVO.utente.Corrispondente)((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza[i];
                        ((DocsPaVO.documento.ProtocolloUscita)schedaDoc.protocollo).destinatariConoscenza[i] = verificaCorrispondente(corr);
                    }
                }

            }
            // protocollo in arrivo
            else if (schedaDoc.tipoProto.Equals("A"))
            {
                DocsPaVO.utente.Corrispondente corr;
                //verifica mittente
                corr = ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente;
                ((DocsPaVO.documento.ProtocolloEntrata)schedaDoc.protocollo).mittente = verificaCorrispondente(corr);

            }
            else if (schedaDoc.tipoProto.Equals("I"))
            {
                DocsPaVO.utente.Corrispondente corr;
                //verifica mittente
                corr = ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente;
                ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).mittente = verificaCorrispondenteInterno(corr);
                //verifica destinatari
                for (int i = 0; i < ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari.Count; i++)
                {
                    corr = (DocsPaVO.utente.Corrispondente)((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari[i];
                    ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatari[i] = verificaCorrispondenteInterno(corr);
                }
                //verifica destinatari CC
                if (((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza != null)
                {
                    for (int i = 0; i < ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza.Count; i++)
                    {
                        corr = (DocsPaVO.utente.Corrispondente)((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza[i];
                        ((DocsPaVO.documento.ProtocolloInterno)schedaDoc.protocollo).destinatariConoscenza[i] = verificaCorrispondente(corr);
                    }
                }
            }

            #endregion


            return schedaDoc;

        }

        #region METODI DUPLICATI //duplicati


        #region utilità

        private bool canaleWSAperto()
        {
            if (this.WS == null)
                apriCanaleWS();
            return (this.WS != null);
        }
        private void apriCanaleWS()
        {
            this.WS = new DocsPaWS.DocsPaWebService();
        }
        private void chiudiCanaleWS()
        {
            this.WS.Dispose();
        }
        #endregion


        private DocsPaVO.utente.UserLogin.LoginResult loginOnDocspa(DocsPaVO.utente.UserLogin objLogin, bool forzaLogin)
        {
            this.Utente = null;
            this.IdAddress = null;
            return this.WS.Login(userLogin, out Utente, forzaLogin, this.SessionIdWSTokenAuth, out this.IdAddress);
        }

        #endregion

        #region nuovi Metodi
        /// <summary>
        /// metodo che aggiunge un documento grigio
        /// </summary>
        /// <param name="tokenDiAutenticazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="result"></param>
        /// <returns>ritorna scheda documento e result</returns>
        [WebMethod]
        public virtual bool creaDocumentoGrigio(string tokenDiAutenticazione, ref DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            DocsPaVO.utente.InfoUtente infoUtente = null;
            try
            {

                if (schedaDocumento != null)
                {
                    this.canaleWSAperto();
                    this.UserID = this.getInfoFromToken("USER_ID", tokenDiAutenticazione);
                    this.CodRuolo = this.getInfoFromToken("COD_RUOLO", tokenDiAutenticazione);
                    this.CodReg = this.getInfoFromToken("COD_REG", tokenDiAutenticazione);
                    this.Token = tokenDiAutenticazione;
                    if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                    {
                        throw (new Exception("Errore durante l'autenticazione dell'utente"));
                    }
                    infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                    if (infoUtente != null)
                    {
                        //valorizzo campi schedaDoc
                        #region campi scheda costanti
                        schedaDocumento.systemId = null;
                        //schedaDoc.privato = "0";  //doc non privato

                        // campi obbligatori per DocsFusion
                        schedaDocumento.idPeople = this.Utente.idPeople;
                        schedaDocumento.userId = this.Utente.userId;
                        schedaDocumento.typeId = "LETTERA";
                        schedaDocumento.appId = "ACROBAT";
                        #endregion campi scheda costanti
                        schedaDocumento = BusinessLogic.Documenti.DocSave.addDocGrigia(schedaDocumento, infoUtente, Ruolo);
                        if (schedaDocumento != null)
                        {
                            BusinessLogic.UserLog.UserLog.WriteLog(infoUtente, "CREADOCUMENTOGRIGIO", schedaDocumento.systemId, string.Format("{0} {1}", "N.ro Doc.: ", schedaDocumento.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
                            return true;
                        }

                    }
                }


            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaWSToken.asmx  - metodo: creaDocumentoGrigio", e);
            }

            return false;
        }
        /// <summary>
        /// metodo che protocolla un documento grigio
        /// </summary>
        /// <param name="tokenDiAutenticazione"></param>
        /// <param name="schedaDocumento"></param>
        /// <param name="result"></param>
        /// <returns>scheda documento e result</returns>
        [WebMethod]
        public virtual bool protocollaDocumento(string tokenDiAutenticazione, ref DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {

            if (schedaDocumento == null)
                return false;
            
            //recupero di infoutente
            DocsPaVO.utente.InfoUtente infoUtente = null;
            this.canaleWSAperto();
            this.UserID = this.getInfoFromToken("USER_ID", tokenDiAutenticazione);
            this.CodRuolo = this.getInfoFromToken("COD_RUOLO", tokenDiAutenticazione);
            this.CodReg = this.getInfoFromToken("COD_REG", tokenDiAutenticazione);
            this.Token = tokenDiAutenticazione;
            if (!Login(this.UserID, this.CodRuolo, this.CodReg))
            {
                throw (new Exception("Errore durante l'autenticazione dell'utente"));
            }

            infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);


            string VarDescOggetto = string.Empty;

            DocsPaVO.documento.ResultProtocollazione risultatoProtocollazione = new DocsPaVO.documento.ResultProtocollazione();
            //protocollazione
            try
            {
                schedaDocumento = this.creaSchedaDocumento(schedaDocumento);
                //schedaDocumento.predisponiProtocollazione = true;
                schedaDocumento = BusinessLogic.Documenti.ProtoManager.protocolla(schedaDocumento, Ruolo, infoUtente, out risultatoProtocollazione);
                if (schedaDocumento != null && risultatoProtocollazione.Equals(DocsPaVO.documento.ResultProtocollazione.OK))
                {
                    if (schedaDocumento.protocollo != null)
                        VarDescOggetto = string.Format("{0}{1} / {2}{3}", "N.ro Doc.: ", schedaDocumento.docNumber, "Segnatura: ", schedaDocumento.protocollo.segnatura);
                    else
                        VarDescOggetto = string.Format("{0}{1}", "N.ro Doc.: ", schedaDocumento.docNumber);

                    logger.Debug("esito ok: " + VarDescOggetto);
                    return true;
                    
                }
            }
            catch (Exception e)
            {
                logger.Debug("Errore in DocsPaServices.asmx  - metodo: DocumentoProtocolla", e);
                if (risultatoProtocollazione != DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO)
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.APPLICATION_ERROR;
                    schedaDocumento = null;
                }
                else
                {
                    risultatoProtocollazione = DocsPaVO.documento.ResultProtocollazione.DOCUMENTO_GIA_PROTOCOLLATO;
                    schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, schedaDocumento.systemId, schedaDocumento.docNumber);
                }

            }

            return false;
        }

        /// <summary>
        /// Cestina i documenti grigi e predisposti alla protocollazione
        /// </summary>
        /// <param name="infoutente"></param>
        /// <param name="schedaDoc"></param>
        /// <returns></returns>
        [WebMethod]
        public virtual bool cestinaDocumento(string token, string idDocumento, string note)
        {
            DocsPaVO.utente.InfoUtente infoUtente;
            DocsPaVO.documento.SchedaDocumento schedaDoc;
            string tipoDoc = "P";  //non so perchè viene passato P (forse sta per PROFILO)
            if (note == null || note.Equals(""))
                note = "Documento rimosso da wsToken";
            string errorMsg;

            errorMsg = string.Empty;
            bool result = false;
            try
            {
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                //Bisogna cercare la scheda del documento dall'idDocumento

                schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, null, idDocumento);
                if (schedaDoc == null)
                {
                    throw (new Exception("Errore nel reperimento del documento"));
                }

                //verificare il campo note, la dimensione 256 (è lo stesso controllo che c'è nel frontend)
                if (note.Length > 256)
                    note = note.Substring(0, 255);


                //Bisogna verificare se il documento può essere cestinato  !!!
                string verificaMsg = "";
                verificaMsg = BusinessLogic.Documenti.DocManager.VerificaDiritti(infoUtente, schedaDoc);
                if (!verificaMsg.Equals("Del"))
                {
                    throw (new Exception("il documento non può essere cestinato"));
                }

                result = BusinessLogic.Documenti.DocManager.CestinaDocumento(infoUtente, schedaDoc, tipoDoc, note, out errorMsg);

                //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "DOCUMENTOEXECCESTINA", schedaDoc.systemId, string.Format("{0} {1}", "Spostamento in cestino del Doc. N.ro:", schedaDoc.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "DOCUMENTOEXECCESTINA", schedaDoc.systemId, string.Format("{0} {1}", "Spostamento in cestino del Doc. N.ro:", schedaDoc.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                result = false;
                throw e;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }

            return result;
        }

        /// <summary>
        /// ripristina un documento recuperandolo dal cestino
        /// </summary>
        /// <param name="token"></param>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        [WebMethod]
        public virtual bool ripristinaDocumentoDalCestino(string token, string idDocumento)
        {
            bool result = false;
            DocsPaVO.utente.InfoUtente infoUtente;
            DocsPaVO.documento.InfoDocumento infoDocumento;

            try
            {
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                //Bisogna cercare la scheda del documento dall'idDocumento
                DocsPaVO.documento.SchedaDocumento schedaDoc = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, null, idDocumento);
                if (schedaDoc == null)
                {
                    throw (new Exception("Errore nel reperimento del documento"));
                }

                infoDocumento = BusinessLogic.Documenti.DocManager.GetInfoDocumento(infoUtente, schedaDoc.systemId, idDocumento);
                if (infoDocumento == null)
                {
                    throw (new Exception("Errore nel reperimento del documento"));
                }

                result = BusinessLogic.Documenti.DocManager.RiattivaDocumento(infoUtente, infoDocumento);
                //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "DOCUMENTORIATTIVADOC", infoDocumento.docNumber, string.Format("{0} {1}", "Riattivazione del Doc. N.ro:", infoDocumento.docNumber), DocsPaVO.Logger.CodAzione.Esito.OK);
            }
            catch (Exception e)
            {
                logger.Debug(e.ToString());
                //BusinessLogic.UserLog.UserLog.getInstance().WriteLog(infoUtente, "DOCUMENTORIATTIVADOC", infoDocumento.docNumber, string.Format("{0} {1}", "Riattivazione del Doc. N.ro:", infoDocumento.docNumber), DocsPaVO.Logger.CodAzione.Esito.KO);
                result = false;
                throw e;
            }
            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }
            return result;
        }

        /// <summary>
        /// ricerca documento tramite l'id del documento
        /// </summary>
        /// <param name="token"></param>
        /// <param name="idDocumento"></param>
        /// <param name="result"></param>
        /// <param name="schedaDocumento"></param>
        /// <returns>scheda documento e un bool</returns>
        [WebMethod]
        public virtual DocsPaVO.documento.SchedaDocumento ricercaDocumentoById(string token, string idDocumento)
        {
            
            DocsPaVO.utente.InfoUtente infoUtente;
            DocsPaVO.documento.SchedaDocumento schedaDocumento = null;

            try
            {
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", token);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", token);
                this.CodReg = this.getInfoFromToken("COD_REG", token);
                this.Token = token;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

                //riceca del documento
                schedaDocumento = BusinessLogic.Documenti.DocManager.getDettaglio(infoUtente, null, idDocumento);
                if (schedaDocumento == null)
                {
                    throw (new Exception("Errore nel reperimento del documento"));
                }
                else
                    return schedaDocumento;
            }
            catch (Exception e)
            {
                logger.Debug("errore ricercaDocumentoById: "+ e.Message);
            }

            finally
            {
                WSManager.Logoff(Utente, WS);
                this.chiudiCanaleWS();
            }
            return schedaDocumento;
        }

        /// <summary>
        /// web method che aggiunge un allegato ad un documento
        /// </summary>
        /// <param name="tokenDiAutenticazione"></param>
        /// <param name="scheda"></param>
        /// <param name="NumPagine"></param>
        /// <param name="Descrizione"></param>
        /// <param name="fd"></param>
        /// <param name="allegato"></param>
        /// <returns> un bool con l'esito e l'allegato</returns>
        [WebMethod]
        public virtual bool aggiungiAllegato(string tokenDiAutenticazione, DocsPaVO.documento.SchedaDocumento scheda, string NumPagine, string Descrizione, DocsPaVO.documento.FileDocumento fd,ref DocsPaVO.documento.Allegato allegato)
        {
            bool esito = true;
            string errorMessage = String.Empty;
            DocsPaVO.utente.InfoUtente infoUtente;
            try
            {
                this.canaleWSAperto();
                this.UserID = this.getInfoFromToken("USER_ID", tokenDiAutenticazione);
                this.CodRuolo = this.getInfoFromToken("COD_RUOLO", tokenDiAutenticazione);
                this.CodReg = this.getInfoFromToken("COD_REG", tokenDiAutenticazione);
                this.Token = tokenDiAutenticazione;
                if (!Login(this.UserID, this.CodRuolo, this.CodReg))
                {
                    throw (new Exception("Errore durante l'autenticazione dell'utente"));
                }
                infoUtente = WSManager.getInfoUtenteAttuale(Utente, Ruolo);

       
                allegato.descrizione = Descrizione;
                allegato.numeroPagine = Convert.ToInt32(NumPagine);
                allegato.docNumber = scheda.docNumber;
                allegato.version = "0";
                allegato = this.WS.DocumentoAggiungiAllegato(infoUtente, allegato);
                if (allegato == null)
                    esito = false;
                DocsPaVO.documento.FileRequest fr = (DocsPaVO.documento.FileRequest)allegato;
                this.WS.DocumentoPutFileNoException(ref fr, fd, infoUtente, out errorMessage);
            }
            catch (Exception ex)
            {
                esito = false;
                throw ex;
            }
            finally
            {
                this.chiudiCanaleWS();
            }

            return esito;
        }
        #endregion
    }
}
