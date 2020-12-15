using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocsPaVO.documento;
using DocsPaVO.utente;
using BusinessLogic.Documenti;
using BusinessLogic.Utenti;
using BusinessLogic.ProfilazioneDinamica;
using DocsPaVO.ProfilazioneDinamica;
using System.IO;
using System.Collections;
using DocsPaWS.Sanita.VO;
using DocsPaWS.Sanita.VO.Responses;
using DocsPaWS.Sanita.VO.Requests;
using DocsPaVO.CheckInOut;
using DocsPaVO.trasmissione;
using BusinessLogic.Trasmissioni;
using DocsPaVO.addressbook;
using DocsPaWS.Sanita.Builders;
using BusinessLogic.Interoperabilità;
using DocsPaVO.Interoperabilita;
using System.Xml;
using DocsPaWS.Sanita.Interoperabilita;
using log4net;

namespace DocsPaWS.Sanita
{
    public class SanitaManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(SanitaManager));

        public static ImportAttestatiResponse ImportAttestati(ImportAttestatiRequest request)
        {
            ImportAttestatiResponse resp = new ImportAttestatiResponse();
            InfoUtente iu=null;
            Ruolo role=null;
            List<SchedaDocumento> docsTrasmissione = new List<SchedaDocumento>();
            using (DocsPaDB.TransactionContext transactionContext = new DocsPaDB.TransactionContext())
            {
                try
                {
                    iu = request.InfoUtente.InfoUtente;
                    role = UserManager.getRuolo(request.Ruolo.Id);
                    iu.idGruppo = role.idGruppo;
                    iu.idCorrGlobali = role.systemId;
                    AttestatiListVO attestati = request.Attestati;
                    //lock del documento principale
                    logger.Debug("lock del documento principale, doc number : " + attestati.DocNumber);
                    //creazione schede
                    foreach (AttestatoVO att in attestati.Attestati)
                    {
                        SchedaDocumento sd = CreateSchedaDocumentoFromAttestato(att, iu, role, request.IdRegistro);
                        AcquireFileAndAllegato(att, sd, iu);
                        docsTrasmissione.Add(sd);
                    }
                    //set dello stato del doc principale
                    SchedaDocumento attList = DocManager.getDettaglio(iu, null, request.Attestati.DocNumber);
                    OggettoCustom oc = attList.template.ELENCO_OGGETTI.Cast<OggettoCustom>().Single(e => request.Attestati.StatusTemplate.StatusTemplateFieldName.Equals(e.DESCRIZIONE));
                    oc.VALORE_DATABASE = request.Attestati.StatusTemplate.StatusProcessedValue;
                    bool daAggiornareUffRef;
                    DocSave.save(iu, attList, false, out daAggiornareUffRef, role);
                    //unlock del documento principale
                    transactionContext.Complete();
                }
                catch (Exception e)
                {
                    resp.Code = ImportAttestatiResponseCode.KO;
                    resp.ErrorMessage = e.Message;
                    return resp;
                }
            }
            if (request.Trasmissione == null)
            {
                logger.Debug("Nessuna trasmissione da eseguire");
                resp.Code = ImportAttestatiResponseCode.OK;
                return resp;
            }
            else
            {
                try
                {
                    logger.Debug("Esegui trasmissioni");
                    EseguiTrasmissioni(request.Trasmissione,docsTrasmissione, iu, role);
                    resp.Code = ImportAttestatiResponseCode.OK;
                    return resp;
                }
                catch (Exception e)
                {
                    resp.ErrorMessage = e.Message;
                    resp.Code = ImportAttestatiResponseCode.WARNING;
                    return resp;
                }
            }
        }

        public static InteropCreazioneDocResponse InteropCreazioneDoc(InteropCreazioneDocRequest request)
        {
            Registro reg = RegistriManager.getRegistro(request.IdRegistro);
            Utente ut = UserManager.getUtente(request.InfoUtente.IdPeople);
            Ruolo ruolo = UserManager.getRuolo(request.Ruolo.Id);
            InteropSchedaDocHandler handler = new InteropSchedaDocHandler(request);
            MailAccountCheckResponse mailCheckResponse = null;
            bool result=InteroperabilitaRicezione.interopRiceviMethodProtocollazione(request.ServerName, reg, ut, ruolo, handler,out mailCheckResponse);
            InteropCreazioneDocResponse response = new InteropCreazioneDocResponse();
            response.Code = (result) ? InteropCreazioneDocResponseCode.OK : InteropCreazioneDocResponseCode.KO;
            response.ErrorMessage = mailCheckResponse.ErrorMessage;
            return response;
        }

        private static SchedaDocumento CreateSchedaDocumentoFromAttestato(AttestatoVO att,InfoUtente infoUtente,Ruolo ruolo,string idRegistro)
        {
                SchedaDocumento sd = DocManager.NewSchedaDocumento(infoUtente);
                Oggetto ogg = new Oggetto();
                ogg.idRegistro = idRegistro;
                ogg.descrizione = att.Oggetto;
                sd.oggetto = ogg;
                // Impostazione del registro
                sd.registro = RegistriManager.getRegistro(idRegistro);
                // Impostazione del tipo atto
                sd.tipoProto = "";
                sd.template = ProfilazioneDocumenti.getTemplateById(att.IdTemplate);
                sd.tipologiaAtto = new TipologiaAtto();
                sd.tipologiaAtto.descrizione = sd.template.DESCRIZIONE;
                sd.tipologiaAtto.systemId = sd.template.SYSTEM_ID.ToString();
                sd.daAggiornareTipoAtto = true;
                sd=DocSave.addDocGrigia(sd,infoUtente,ruolo);
                Allegato all = new Allegato();
                all.descrizione = "ALL1";
                all.docNumber = sd.docNumber;
                all.version = "0";
                string idPeopleDelegato = "0";
                if (infoUtente.delegato != null)
                    idPeopleDelegato = infoUtente.delegato.idPeople;
                all.idPeopleDelegato = idPeopleDelegato;

                // Impostazione del repositoryContext associato al documento
                all.repositoryContext = sd.repositoryContext;
                all.position = 1;
                Allegato resAll=AllegatiManager.aggiungiAllegato(infoUtente, all);
                sd.allegati = new ArrayList();
                sd.allegati.Add(resAll);
                return sd;
        }

        private static void AcquireFileAndAllegato(AttestatoVO att,SchedaDocumento sd, InfoUtente userInfo)
        {
            Templates temp = ProfilazioneDocumenti.getTemplateById(att.IdTemplate);
            string path = temp.PATH_MODELLO_1;
            byte[] content = GetFileContent(path);
            // Creazione dell'oggetto fileDocumento
            FileDocumento fileDocumento = new FileDocumento();
            fileDocumento.name = Path.GetFileName(sd.systemId + ".rtf");
            fileDocumento.fullName = sd.systemId + ".rtf";
            fileDocumento.estensioneFile = "rtf";
            fileDocumento.length = content.Length;
            fileDocumento.content = content;
            FileRequest fileRequest = (FileRequest)sd.documenti[0];
            try
            {
                FileManager.putFile(fileRequest, fileDocumento, userInfo);
            }
            catch (Exception e)
            {
                throw new Exception("Errore durante l'upload del file.");
            }
            FileDocumento fileAll = new FileDocumento();
            fileAll.name = Path.GetFileName(att.FileName);
            fileAll.fullName = att.FileName;
            fileAll.estensioneFile = att.FileName.Substring(att.FileName.LastIndexOf(".")+1);
            fileAll.length = content.Length;
            fileAll.content = att.Content;
            FileRequest allRequest = (FileRequest)sd.allegati[0];
            FileManager.putFile(allRequest, fileAll, userInfo);
        }

        private static void EseguiTrasmissioni(TrasmissioneVO trasmInfo, List<SchedaDocumento> docs,InfoUtente infoUtente, Ruolo ruolo)
        {
            TrasmissioneBuilder trasmBuilder = new TrasmissioneBuilder(infoUtente, ruolo, trasmInfo);
            foreach (SchedaDocumento doc in docs)
            {
                Trasmissione trasm = trasmBuilder.BuildTrasmissione(doc.InfoDocumento);
                logger.Debug("Salvataggio trasmissione...");
                trasm = TrasmManager.saveTrasmMethod(trasm);
                logger.Debug("Esecuzione trasmissione con id " + trasm.systemId);
                ExecTrasmManager.executeTrasmMethod("", trasm);
            }
        }

        private static byte[] GetFileContent(string filePath)
        {
            byte[] buff = null;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(filePath).Length;
            buff = br.ReadBytes((int)numBytes);
            return buff;
        }
    }
}
